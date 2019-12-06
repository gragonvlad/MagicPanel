using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core;
using Rust;

namespace Oxide.Plugins
{
    [Info("Wipe Info Api", "MJSU", "1.0.1")]
    [Description("Api for when the server is wiping")]
    internal class WipeInfoApi : RustPlugin
    {
        #region Class Fields
        private StoredData _storedData; //Plugin Data
        private PluginConfig _pluginConfig;

        private DateTime _nextWipe;
        private int _daysTillNextWipe;
        private int _daysBetweenWipes;
        private bool _isForcedWipeDay;
        private bool _newSaveVersion;
        #endregion

        #region Setup & Loading

        private void Init()
        {
            _storedData = Interface.Oxide.DataFileSystem.ReadObject<StoredData>(Name);
        }

        private void Loaded()
        {
            if (_storedData.SaveVersion != Protocol.save)
            {
                _storedData.SaveVersion = Protocol.save;
                _newSaveVersion = true;
                Interface.Call("OnSaveVersionChanged");
                NextTick(SaveData);
            }
            
            CalculateWipe();
        }

        protected override void LoadDefaultConfig()
        {
            PrintWarning("Loading Default Config");
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            Config.Settings.DefaultValueHandling = DefaultValueHandling.Populate;
            _pluginConfig = AdditionalConfig(Config.ReadObject<PluginConfig>());
            Config.WriteObject(_pluginConfig);
        }

        private PluginConfig AdditionalConfig(PluginConfig config)
        {
            config.ScheduleWeek4 = config.ScheduleWeek4 ?? new List<WipeSchedule>
            {
                new WipeSchedule
                {
                    DayOfWeek = DayOfWeek.Thursday,
                    Occurrence = 1
                }
            };
            
            config.ScheduleWeek5 = config.ScheduleWeek5 ?? new List<WipeSchedule>
            {
                new WipeSchedule
                {
                    DayOfWeek = DayOfWeek.Thursday,
                    Occurrence = 1
                }
            };
            return config;
        }

        private void OnNewSave(string name)
        {
            CalculateWipe();
        }

        private void OnServerInitialized()
        {
            CalculateWipe();
        }
        #endregion

        #region Wipe Calculations
        private void CalculateWipe()
        {
            if (SaveRestore.SaveCreatedTime > _storedData.PreviousWipe)
            {
                _storedData.PreviousWipe = SaveRestore.SaveCreatedTime.Date;
                NextTick(SaveData);
            }
            
            DateTime lastForcedWipe = GetForcedWipe(_storedData.PreviousWipe);
            DateTime nextForcedWipe = GetForcedWipe(_storedData.PreviousWipe.AddMonths(1));
            if (lastForcedWipe > DateTime.UtcNow)
            {
                nextForcedWipe = lastForcedWipe;
                lastForcedWipe = GetForcedWipe(_storedData.PreviousWipe.AddMonths(-1));
            }

            _isForcedWipeDay = nextForcedWipe.Date == DateTime.Now.Date && !_newSaveVersion;

            _daysBetweenWipes = (int) (nextForcedWipe - lastForcedWipe).TotalDays;
            List<DateTime> wipes = new List<DateTime>();
            List<WipeSchedule> schedule;
            if (_daysBetweenWipes == 28)
            {
                schedule = _pluginConfig.ScheduleWeek4;
            }
            else if (_daysBetweenWipes == 35)
            {
                schedule = _pluginConfig.ScheduleWeek5;
            }
            else
            {
                PrintError($"Invalid Wipe length! Wipe Length: {_daysBetweenWipes}");
                return;
            }

            foreach (WipeSchedule wipeSchedule in schedule)
            {
                DateTime wipeNow = GetScheduledWipe(lastForcedWipe, wipeSchedule.DayOfWeek, wipeSchedule.Occurrence, lastForcedWipe);
                DateTime wipeNext = GetScheduledWipe(lastForcedWipe.AddMonths(1), wipeSchedule.DayOfWeek, wipeSchedule.Occurrence, lastForcedWipe);

                wipes.Add(wipeNow);
                wipes.Add(wipeNext);
            }

            _nextWipe = wipes.OrderBy(w => w).FirstOrDefault(w => w > _storedData.PreviousWipe);
            _daysTillNextWipe = (_nextWipe - DateTime.Today).Days;
            Puts($"Next Wipe: {_nextWipe} Days Until: {_daysTillNextWipe}");
            Interface.Call("OnWipeCalculated", _nextWipe, _daysTillNextWipe);
        }

        private DateTime GetForcedWipe(DateTime date)
        {
            int day = FindDay(date.Year, date.Month, DayOfWeek.Thursday, 1);
            return new DateTime(date.Year, date.Month, day);
        }
        
        private DateTime GetScheduledWipe(DateTime date, DayOfWeek dow, int occurence, DateTime lastWipe)
        {
            int firstOccurence = FindDay(date.Year, date.Month, dow, 1);
            if (firstOccurence < lastWipe.Day && date.Month == lastWipe.Month)
            {
                occurence++;
            }
            
            int day = FindDay(date.Year, date.Month, dow, occurence);
            if (day < 1 || day > DateTime.DaysInMonth(date.Year, date.Month))
            {
                return DateTime.MinValue;
            }
            
            return new DateTime(date.Year, date.Month, day);
        }
        
        private int FindDay(int year, int month, DayOfWeek day, int occurence)
        {
            if (occurence < 1 || occurence > 5)
            {
                PrintError("Occurence cannot be less than 1 or greater than 5");
                return 0;
            }

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            //Substract first day of the month with the required day of the week 
            int daysNeeded = (int)day - (int)firstDayOfMonth.DayOfWeek;
            //if it is less than zero we need to get the next week day (add 7 days)
            if (daysNeeded < 0) daysNeeded = daysNeeded + 7;
            //DayOfWeek is zero index based; multiply by the Occurance to get the day
            int resultedDay = daysNeeded + 1 + 7 * (occurence - 1);

            return resultedDay;
        }
        #endregion

        #region API Hooks

        private int GetDaysTillWipe()
        {
            return _daysTillNextWipe;
        }

        private DateTime GetNextWipe()
        {
            return _nextWipe;
        }

        private int GetDaysBetweenWipe()
        {
            return _daysBetweenWipes;
        }

        private bool IsForcedWipeDay()
        {
            return _isForcedWipeDay;
        }

        private bool IsNewSaveVersion()
        {
            return _newSaveVersion;
        }
        #endregion

        #region Helper Methods
        private void SaveData() => Interface.Oxide.DataFileSystem.WriteObject(Name, _storedData);
        #endregion

        #region Classes

        private class PluginConfig
        {
            [JsonProperty("4 week schedule")]
            public List<WipeSchedule> ScheduleWeek4 { get; set; }
            
            [JsonProperty("5 week schedule")]
            public List<WipeSchedule> ScheduleWeek5 { get; set; }
        }

        private class StoredData
        {
            public int SaveVersion { get; set; }
            public DateTime PreviousWipe { get; set; } = DateTime.MinValue;
        }

        private class WipeSchedule
        {
            [JsonConverter(typeof(StringEnumConverter))]
            [JsonProperty("Day of the Week")]
            public DayOfWeek DayOfWeek { get; set; }
            
            [JsonProperty("Day of the week occurrence (1-5)")]
            public int Occurrence { get; set; }
        }
        #endregion
    }
}
