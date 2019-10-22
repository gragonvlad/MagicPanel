﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Message Panel", "MJSU", "0.0.4")]
    [Description("Displays messages to the player")]
    internal class MessagePanel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private readonly Plugin MagicPanel;

        private PluginConfig _pluginConfig; //Plugin Config

        private enum UpdateEnum { All = 1, Panel = 2, Image = 3, Text = 4 }
        #endregion

        #region Setup & Loading
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
            config.Panels = config.Panels ?? new Hash<string, PanelData>
            {
                [$"{Name}_1"] = new PanelData
                {
                    Panel = new Panel
                    {
                        Text = new PanelText
                        {
                            Enabled = true,
                            Color = "#FFFFFFFF",
                            Order = 1,
                            Width = 1f,
                            FontSize = 14,
                            Padding = new TypePadding(0.05f, 0.05f, 0.1f, 0.00f),
                            TextAnchor = TextAnchor.MiddleCenter,
                            Text = ""
                        }
                    },
                    PanelSettings = new PanelRegistration
                    {
                        BackgroundColor = "#FFF2DF08",
                        Dock = "bottom",
                        Order = 0,
                        Width = 0.2954f
                    },
                    Messages = new List<string>
                    {
                        "Message 1",
                        "Message 2",
                        "<color=#FF0000>This message is red</color>"
                    },
                    UpdateRate = 15f
                }
            };
            
            return config;
        }

        private void OnServerInitialized()
        {
            RegisterPanels();

            foreach (IGrouping<float, KeyValuePair<string, PanelData>> panelUpdates in _pluginConfig.Panels.GroupBy(p => p.Value.UpdateRate))
            {
                timer.Every(panelUpdates.Key, () =>
                {
                    foreach (KeyValuePair<string, PanelData> data in panelUpdates)
                    {
                        MagicPanel?.Call("UpdatePanel", data.Key, (int)UpdateEnum.Text);
                    }
                });
            }
        }

        private void RegisterPanels()
        {
            if (MagicPanel == null)
            {
                PrintError("Missing plugin dependency MagicPanel: https://github.com/dassjosh/MagicPanel");
                return;
            }
        
            foreach (KeyValuePair<string, PanelData> panel in _pluginConfig.Panels)
            {
                MagicPanel?.Call("RegisterGlobalPanel", this, panel.Key, JsonConvert.SerializeObject(panel.Value.PanelSettings), nameof(GetPanel));
            }
        }
        #endregion

        #region Helper Methods

        private string GetPanel(string panelName)
        {
            PanelData panelData = _pluginConfig.Panels[panelName];
            Panel panel = panelData.Panel;
            PanelText text = panel.Text;
            if (text != null)
            {
                if (panelData.Messages.Count == 0)
                {
                    text.Text = string.Empty;
                }
                if (panelData.Messages.Count == 1)
                {
                    text.Text = panelData.Messages[0];
                }
                else
                {
                    text.Text = panelData.Messages.Where(m => text.Text != m).ToList().GetRandom();
                }
            }

            return JsonConvert.SerializeObject(panel);
        }
        #endregion

        #region Classes
        private class PluginConfig
        {
            [JsonProperty(PropertyName = "Message Panels")]
            public Hash<string, PanelData> Panels { get; set; }
        }

        private class PanelData
        {
            [JsonProperty(PropertyName = "Panel Settings")]
            public PanelRegistration PanelSettings { get; set; }
            
            [JsonProperty(PropertyName = "Panel Layout")]
            public Panel Panel { get; set; }
            
            [JsonProperty(PropertyName = "Messages")]
            public List<string> Messages { get; set; }
            [JsonProperty(PropertyName = "Update Rate (Seconds)")]
            public float UpdateRate { get; set; }
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
            public PanelText Text { get; set; }
        }

        private abstract class PanelType
        {
            public bool Enabled { get; set; }
            public string Color { get; set; }
            public int Order { get; set; }
            public float Width { get; set; }
            public TypePadding Padding { get; set; }
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
