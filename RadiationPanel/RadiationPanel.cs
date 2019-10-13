﻿using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Radiation Panel", "MJSU", "0.0.1")]
    [Description("Displays if players are in a radiation zone")]
    internal class RadiationPanel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private readonly Plugin MagicPanel;

        private PluginConfig _pluginConfig; //Plugin Config

        private readonly Hash<ulong, int> _playersRadiation = new Hash<ulong, int>();

        private enum UpdateEnum { All, Panel, Image, Text }
        #endregion

        #region Setup & Loading
        private void Loaded()
        {
            ConfigLoad();
        }

        protected override void LoadDefaultConfig()
        {
            PrintWarning("Loading Default Config");
        }

        private void ConfigLoad()
        {
            Config.Settings.DefaultValueHandling = DefaultValueHandling.Populate;
            _pluginConfig = AdditionalConfig(Config.ReadObject<PluginConfig>());
            Config.WriteObject(_pluginConfig);
        }

        private PluginConfig AdditionalConfig(PluginConfig config)
        {
            config.Panel = new Panel
            {
                Image = new PanelImage
                {
                    Color = config.Panel?.Image?.Color ?? "#FFFFFFFF",
                    Order = config.Panel?.Image?.Order ?? 0,
                    Width = config.Panel?.Image?.Width ?? 1f,
                    Url = config.Panel?.Image?.Url ?? "http://i.imgur.com/owVdFsK.png",
                    Padding = config.Panel?.Image?.Padding ?? new TypePadding(0.05f, 0.05f, 0.05f, 0.05f)
                }
            };
            config.PanelSettings = new PanelRegistration
            {
                BackgroundColor = config.PanelSettings?.BackgroundColor ?? "#FFF2DF08",
                Dock = config.PanelSettings?.Dock ?? "right",
                Order = config.PanelSettings?.Order ?? 7,
                Width = config.PanelSettings?.Width ?? 0.02f
            };
            return config;
        }

        private void OnServerInitialized()
        {
            RegisterPanels();
        }

        private void RegisterPanels()
        {
            MagicPanel?.Call("RegisterPlayerPanel", this, Name, JsonConvert.SerializeObject(_pluginConfig.PanelSettings), nameof(GetPanel));
        }
        #endregion

        #region Radiation Zone Hooks
        private void OnEntityEnter(TriggerBase trigger, BaseEntity entity)
        {
            TriggerRadiation rad = trigger as TriggerRadiation;
            BasePlayer player = entity as BasePlayer;

            if (rad == null || player == null || entity.IsNpc)
            {
                return;
            }

            if (_playersRadiation.ContainsKey(player.userID))
            {
                _playersRadiation[player.userID]++;
                if (_playersRadiation[player.userID] > 1)
                {
                    return;
                }
            }
            else
            {
                _playersRadiation[player.userID] = 1;
            }

            NextTick(() =>
            {
                MagicPanel?.Call("UpdatePanel", player, Name, (int)UpdateEnum.Image);
            });
        }

        private void OnEntityLeave(TriggerBase trigger, BaseEntity entity)
        {
            TriggerRadiation rad = trigger as TriggerRadiation;
            BasePlayer player = entity as BasePlayer;

            if (rad == null || player == null || player.IsDead() || entity.IsNpc)
            {
                return;
            }

            if (_playersRadiation.ContainsKey(player.userID))
            {
                _playersRadiation[player.userID]--;
                if (_playersRadiation[player.userID] != 0)
                {
                    return;
                }
            }

            NextTick(() =>
            {
                MagicPanel?.Call("UpdatePanel", player, Name, (int)UpdateEnum.Image);
            });
        }
        #endregion

        #region Helper Methods

        private string GetPanel(BasePlayer player)
        {
            Panel panel = _pluginConfig.Panel;
            PanelImage image = panel.Image;
            if (image != null)
            {
                image.Color = player.FindTrigger<TriggerRadiation>() != null ? _pluginConfig.ActiveColor : _pluginConfig.InactiveColor;
            }

            return JsonConvert.SerializeObject(panel);
        }
        #endregion

        #region Classes

        private class PluginConfig
        {
            [DefaultValue("#FFFF00FF")]
            [JsonProperty(PropertyName = "Active Color")]
            public string ActiveColor { get; set; }

            [DefaultValue("#FFFFFF1A")]
            [JsonProperty(PropertyName = "Inactive Color")]
            public string InactiveColor { get; set; }

            [JsonProperty(PropertyName = "Panel Settings")]
            public PanelRegistration PanelSettings { get; set; }

            [JsonProperty(PropertyName = "Panel Layout")]
            public Panel Panel { get; set; }
        }

        private class PanelRegistration
        {
            public string Dock { get; set; }
            public float Width { get; set; }
            public int Order { get; set; }
            public string BackgroundColor { get; set; }
        }

        private class Panel
        {
            public PanelImage Image { get; set; }
            public PanelText Text { get; set; }
        }

        private abstract class PanelType
        {
            public string Color { get; set; }
            public int Order { get; set; }
            public float Width { get; set; }
            public TypePadding Padding { get; set; } = new TypePadding();
        }

        private class PanelImage : PanelType
        {
            public string Url { get; set; }
        }

        private class PanelText : PanelType
        {
            public string Text { get; set; }
            public int FontSize { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public TextAnchor TextAnchor { get; set; }
        }

        private class TypePadding
        {
            public float Left { get; set; }
            public float Right { get; set; }
            public float Top { get; set; }
            public float Bottom { get; set; }

            public TypePadding()
            {
                Left = 0;
                Right = 0;
                Top = 0;
                Bottom = 0;
            }

            public TypePadding(float left, float right, float top, float bottom)
            {
                Left = left;
                Right = right;
                Top = top;
                Bottom = bottom;
            }
        }
        #endregion
    }
}