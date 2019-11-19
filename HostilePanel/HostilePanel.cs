﻿using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core.Configuration;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Hostile Panel", "MJSU", "1.0.0")]
    [Description("Displays how much longer a player is considered hostile")]
    internal class HostilePanel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private readonly Plugin MagicPanel;

        private PluginConfig _pluginConfig; //Plugin Config
        private string _panelText;
        
        private readonly Hash<ulong, Timer> _hostileTimer = new Hash<ulong, Timer>();

        private enum UpdateEnum { All = 1, Panel = 2, Image = 3, Text = 4 }
        #endregion

        #region Setup & Loading
        private void Init()
        {
            _panelText = _pluginConfig.Panel.Text.Text;
        }

        protected override void LoadDefaultConfig()
        {
            PrintWarning("Loading Default Config");
        }

        protected override void LoadConfig()
        {
            string path = $"{Manager.ConfigPath}/MagicPanel/{Name}.json";
            DynamicConfigFile newConfig = new DynamicConfigFile(path);
            if (!newConfig.Exists())
            {
                LoadDefaultConfig();
                newConfig.Save();
            }
            try
            {
                newConfig.Load();
            }
            catch (Exception ex)
            {
                RaiseError("Failed to load config file (is the config file corrupt?) (" + ex.Message + ")");
                return;
            }
            
            newConfig.Settings.DefaultValueHandling = DefaultValueHandling.Populate;
            _pluginConfig = AdditionalConfig(newConfig.ReadObject<PluginConfig>());
            newConfig.WriteObject(_pluginConfig);
        }

        private PluginConfig AdditionalConfig(PluginConfig config)
        {
            config.Panel = new Panel
            {
                Image = new PanelImage
                {
                    Enabled = config.Panel?.Image?.Enabled ?? true,
                    Color = config.Panel?.Image?.Color ?? "#FFFFFFFF",
                    Order = config.Panel?.Image?.Order ?? 0,
                    Width = config.Panel?.Image?.Width ?? 0.3f,
                    Url = config.Panel?.Image?.Url ?? "https://i.imgur.com/v5sdNHg.png",
                    Padding = config.Panel?.Image?.Padding ?? new TypePadding(0.05f, 0.05f, 0.15f, 0.15f)
                },
                Text = new PanelText
                {
                    Enabled = config.Panel?.Text?.Enabled ?? true,
                    Color = config.Panel?.Text?.Color ?? "#FFFFFFFF",  
                    Order = config.Panel?.Text?.Order ?? 1,
                    Width = config.Panel?.Text?.Width ?? 0.7f,
                    FontSize = config.Panel?.Text?.FontSize ?? 14,
                    Padding = config.Panel?.Text?.Padding ?? new TypePadding(0.05f, 0.05f, 0.05f, 0.05f),
                    TextAnchor = config.Panel?.Text?.TextAnchor ?? TextAnchor.MiddleCenter,
                    Text = config.Panel?.Text?.Text ?? "{0}m {1:00}s",
                }
            };
            config.PanelSettings = new PanelRegistration
            {
                BackgroundColor = config.PanelSettings?.BackgroundColor ?? "#fff2df08",
                Dock = config.PanelSettings?.Dock ?? "centerupper",
                Order = config.PanelSettings?.Order ?? 14,
                Width = config.PanelSettings?.Width ?? 0.0725f
            };
            return config;
        }

        private void OnServerInitialized()
        {
            RegisterPanels();
        }

        private void RegisterPanels()
        {
            if (MagicPanel == null)
            {
                PrintError("Missing plugin dependency MagicPanel: https://umod.org/plugins/magic-panel");
                return;
            }
        
            MagicPanel?.Call("RegisterPlayerPanel", this, Name, JsonConvert.SerializeObject(_pluginConfig.PanelSettings), nameof(GetPanel));
            timer.In(1f, () =>
            {
                foreach (BasePlayer player in BasePlayer.activePlayerList)
                {
                    SetupHostile(player);
                }
            });
        }

        private void OnPlayerInit(BasePlayer player)
        {
           HidePanel(player);
        }
        #endregion

        #region uMod Hooks
        private void OnEntityMarkHostile(BasePlayer player)
        {
            if (player == null || player.IsNpc || !player.IsAdmin)
            {
                return;
            }
            
            NextTick(() =>
            {
               SetupHostile(player);
            });
        }

        private void SetupHostile(BasePlayer player)
        {
            if (player.unHostileTime < Time.realtimeSinceStartup)
            {
                HidePanel(player);
                return;
            }

            ShowPanel(player);
            UpdatePanel(player);
            _hostileTimer[player.userID]?.Destroy();
            _hostileTimer[player.userID] = timer.Every(_pluginConfig.UpdateRate, () =>
            {
                if (player.unHostileTime < Time.realtimeSinceStartup)
                {
                    _hostileTimer[player.userID]?.Destroy();
                    HidePanel(player);
                    return;
                }
                
                UpdatePanel(player);
            });
        }
        #endregion

        #region MagicPanel Hook
        private Hash<string, object> GetPanel(BasePlayer player)
        {
            Panel panel = _pluginConfig.Panel;
            PanelText text = panel.Text;
            if (text != null)
            {
                int minutes = 0;
                int seconds = 0;
                if (player.unHostileTime > Time.realtimeSinceStartup)
                {
                    TimeSpan remainingTime = TimeSpan.FromSeconds(player.unHostileTime - Time.realtimeSinceStartup);
                    minutes = remainingTime.Minutes;
                    seconds = remainingTime.Seconds;
                }
                
                text.Text = string.Format(_panelText, minutes, seconds);
            }

            return panel.ToHash();
        }
        #endregion

        #region Helper Methods

        private void HidePanel(BasePlayer player)
        {
            if (!_pluginConfig.ShowHide)
            {
                return;
            }
            
            MagicPanel?.Call("HidePanel", Name, player);
        }
        
        private void ShowPanel(BasePlayer player)
        {
            if (!_pluginConfig.ShowHide)
            {
                return;
            }
            
            MagicPanel?.Call("ShowPanel", Name, player);
        }

        private void UpdatePanel(BasePlayer player)
        {
            MagicPanel?.Call("UpdatePanel", player, Name, (int)UpdateEnum.Text);
        }
        #endregion

        #region Classes
        private class PluginConfig
        {
            [DefaultValue(false)]
            [JsonProperty(PropertyName = "Show/Hide panel")]
            public bool ShowHide { get; set; }
            
            [DefaultValue(1.0f)]
            [JsonProperty(PropertyName = "Update Rate (Seconds)")]
            public float UpdateRate { get; set; }
            
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
            
            public Hash<string, object> ToHash()
            {
                return new Hash<string, object>
                {
                    [nameof(Image)] = Image.ToHash(),
                    [nameof(Text)] = Text.ToHash()
                };
            }
        }

        private abstract class PanelType
        {
            public bool Enabled { get; set; }
            public string Color { get; set; }
            public int Order { get; set; }
            public float Width { get; set; }
            public TypePadding Padding { get; set; }
            
            public virtual Hash<string, object> ToHash()
            {
                return new Hash<string, object>
                {
                    [nameof(Enabled)] = Enabled,
                    [nameof(Color)] = Color,
                    [nameof(Order)] = Order,
                    [nameof(Width)] = Width,
                    [nameof(Padding)] = Padding.ToHash(),
                };
            }
        }

        private class PanelImage : PanelType
        {
            public string Url { get; set; }
            
            public override Hash<string, object> ToHash()
            {
                Hash<string, object> hash = base.ToHash();
                hash[nameof(Url)] = Url;
                return hash;
            }
        }

        private class PanelText : PanelType
        {
            public string Text { get; set; }
            public int FontSize { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public TextAnchor TextAnchor { get; set; }
            
            public override Hash<string, object> ToHash()
            {
                Hash<string, object> hash = base.ToHash();
                hash[nameof(Text)] = Text;
                hash[nameof(FontSize)] = FontSize;
                hash[nameof(TextAnchor)] = TextAnchor;
                return hash;
            }
        }

        private class TypePadding
        {
            public float Left { get; set; }
            public float Right { get; set; }
            public float Top { get; set; }
            public float Bottom { get; set; }

            public TypePadding(float left, float right, float top, float bottom)
            {
                Left = left;
                Right = right;
                Top = top;
                Bottom = bottom;
            }
            
            public Hash<string, object> ToHash()
            {
                return new Hash<string, object>
                {
                    [nameof(Left)] = Left,
                    [nameof(Right)] = Right,
                    [nameof(Top)] = Top,
                    [nameof(Bottom)] = Bottom
                };
            }
        }
        #endregion
    }
}
