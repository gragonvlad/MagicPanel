using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Last Wipe", "MJSU", "0.0.1")]
    [Description("Tells players when the last and next wipe is")]
    internal class LastWipe : RustPlugin
    {
        #region Class Fields
        [PluginReference] private Plugin DiscordCore;

        private PluginConfig _pluginConfig;

        private DateTime _nextWipe;
        private int _daysTillNextWipe;
        #endregion

        #region Setup & Loading
        private void Init()
        {
            for (int i = 0; i < _pluginConfig.AutoChatTrigger.Count; i++)
            {
                _pluginConfig.AutoChatTrigger[i] = _pluginConfig.AutoChatTrigger[i].ToLower();
            }
            
            cmd.AddChatCommand(_pluginConfig.GameChatCommand, this, WipeChatCommand);
        }
        
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                [LangKeys.Today] = "Today",
                [LangKeys.InDays] = "in {0} days",
                [LangKeys.WipeText] = "The next wipe will occur {0} on {1:dddd MM/dd/yyyy}. " +
                                      "Last wipe was {2:MM/dd/yyyy}.",
                [LangKeys.Chat] = $"<color=#BEBEBE>[<color=#de8732>{Title}</color>] {{0}}</color>",
            }, this);
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

            config.AutoChatTrigger = config.AutoChatTrigger ?? new List<string>
            {
                "wipe",
                "wipe?",
                "wiped",
                "wiped?"
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
            OnDiscordCoreReady();

            if (!_pluginConfig.EnableAutoChatResponse)
            {
                Unsubscribe(nameof(OnPlayerChat));
            }
        }

        private void OnDiscordCoreReady()
        {
            timer.In(1f, () =>
            {
                DiscordCore?.Call("RegisterCommand", _pluginConfig.DiscordChatCommand, this, new Func<IPlayer, string, string, string[], object>(HandleWipe), "To show days till wipe", null, true); 
            });
        }
        
        private void OnPlayerInit(BasePlayer player)
        {
            if (player.HasPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot))
            {
                timer.Once(5f, () => OnPlayerInit(player));
                return;
            }

            timer.Once(10f, () => { WipeChatCommand(player, _pluginConfig.GameChatCommand, new string[0]); });
        }

        private void Unload()
        {
            DiscordCore?.Call("UnregisterCommand", _pluginConfig.DiscordChatCommand, this);
        }
        #endregion

        #region Wipe Calculations
        private void CalculateWipe()
        {
            DateTime wipeDate = SaveRestore.SaveCreatedTime.Date;

            DateTime lastWipe = GetWipe(wipeDate);
            DateTime nextWipe = GetWipe(wipeDate.AddMonths(1));

            if (lastWipe > DateTime.UtcNow)
            {
                nextWipe = lastWipe;
                lastWipe = GetWipe(wipeDate.AddMonths(-1));
            }

            List<WipeSchedule> schedule = null;
            List<DateTime> wipes = new List<DateTime>();
            if ((int) (nextWipe - lastWipe).TotalDays <= 28)
            {
                schedule = _pluginConfig.ScheduleWeek4;
            }
            else if ((int) (nextWipe - lastWipe).TotalDays <= 35)
            {
                schedule = _pluginConfig.ScheduleWeek5;
            }

            if (schedule == null)
            {
                schedule = _pluginConfig.ScheduleWeek5;
            }

            foreach (WipeSchedule wipeSchedule in schedule)
            {
                DateTime wipeNow = GetWipe(wipeDate, wipeSchedule.DayOfWeek, wipeSchedule.Occurrence);
                DateTime wipeNext = GetWipe(wipeDate.AddMonths(1), wipeSchedule.DayOfWeek, wipeSchedule.Occurrence);

                wipes.Add(wipeNow);
                wipes.Add(wipeNext);
            }

            _nextWipe = wipes.OrderBy(w => w).FirstOrDefault(w => w > DateTime.UtcNow);
            _daysTillNextWipe = (_nextWipe - DateTime.Today).Days;
            Puts($"Next Wipe: {_nextWipe} Days Until: {_daysTillNextWipe}\n{GetWipeText()}");
            Interface.Call("OnWipeCalculated");
        }

        private DateTime GetWipe(DateTime date)
        {
            int day = FindDay(date.Year, date.Month, DayOfWeek.Thursday, 1);
            return new DateTime(date.Year, date.Month, day);
        }
        
        private DateTime GetWipe(DateTime date, DayOfWeek dow, int occurence)
        {
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

        private int DaysTillWipe()
        {
            return _daysTillNextWipe;
        }

        private int DaysBetweenWipes()
        {
            return (_nextWipe - SaveRestore.SaveCreatedTime).Days;
        }

        private string GetWipeText(string id = null)
        {
            string nextWipe = _daysTillNextWipe == 0 ? Lang(LangKeys.Today, id) : Lang(LangKeys.InDays, id, _daysTillNextWipe);

            return Lang(LangKeys.WipeText, id, nextWipe, _nextWipe, SaveRestore.SaveCreatedTime.ToLocalTime());
        }
        #endregion

        #region Chat Command
        private void WipeChatCommand(BasePlayer player, string command, string[] args)
        {
            Chat(player, GetWipeText(player.UserIDString));
        }

        private void OnPlayerChat(ConsoleSystem.Arg arg)
        {
            BasePlayer player = arg.Player();
            if (_pluginConfig.AutoChatTrigger.Contains(arg.FullString.ToLower()))
            {
                WipeChatCommand(player, null, null);
            }
        }
        #endregion

        #region Discord Command
        private object HandleWipe(IPlayer player, string channelId, string cmd, string[] args)
        {
            SendMessage(channelId, GetWipeText(player.Id));
            return null;
        }
        #endregion

        #region Helper Methods
        private void SendMessage(string channelId, string message)
        {
            DiscordCore?.Call("SendMessageToChannel", channelId, $"{Title}: {message}");
        }

        private string Lang(string key, string id, params object[] args) => string.Format(lang.GetMessage(key, this, id), args);
        private void Chat(BasePlayer player, string format) => PrintToChat(player, Lang(LangKeys.Chat, player.UserIDString, format));
        #endregion

        #region Classes

        private class PluginConfig
        {
            [DefaultValue(true)]
            [JsonProperty("Enable Auto Chat")]
            public bool EnableAutoChatResponse { get; set; }
            
            [JsonProperty("Auto Chat Triggers")]
            public List<string> AutoChatTrigger { get; set; }
            
            [DefaultValue("wipe")]
            [JsonProperty("Game Chat Command")]
            public string GameChatCommand { get; set; }
            
            [DefaultValue("wipe")]
            [JsonProperty("Discord Chat Command")]
            public string DiscordChatCommand { get; set; }
            
            [JsonProperty("4 week schedule")]
            public List<WipeSchedule> ScheduleWeek4 { get; set; }
            
            [JsonProperty("5 week schedule")]
            public List<WipeSchedule> ScheduleWeek5 { get; set; }
        }

        private class WipeSchedule
        {
            [JsonConverter(typeof(StringEnumConverter))]
            [JsonProperty("Day of the Week")]
            public DayOfWeek DayOfWeek { get; set; }
            
            [JsonProperty("Day of the week occurrence (1-5)")]
            public int Occurrence { get; set; }
        }
        
        private class LangKeys
        {
            public const string Today = "Today";
            public const string InDays = "InDays";
            public const string WipeText = "WipeText";
            public const string Chat = "Chat";
        }
        #endregion
    }
}
