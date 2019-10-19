﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Clock Panel", "MJSU", "0.0.3")]
    [Description("Displays the in game time in magic panel")]
    internal class ClockPanel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private readonly Plugin MagicPanel;

        private PluginConfig _pluginConfig; //Plugin Config

        private TOD_Sky _sky;
        private TOD_Time _time;

        private enum UpdateEnum { All = 1, Panel = 2, Image = 3, Text = 4 }

        private string _textFormat;
        #endregion

        #region Setup & Loading
        private void Init()
        {
            ConfigLoad();
            _textFormat = _pluginConfig.Panel.Text.Text;
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
                    Enabled = config.Panel?.Image?.Enabled ?? true,
                    Color = config.Panel?.Image?.Color     ?? "#FFFFFFFF",
                    Order = config.Panel?.Image?.Order     ?? 0,
                    Width = config.Panel?.Image?.Width     ?? 0.28f,
                    Url = config.Panel?.Image?.Url         ?? "https://i.imgur.com/cSykHxd.png",
                    Padding = config.Panel?.Image?.Padding ?? new TypePadding(0.05f, 0.0f, 0.1f, 0.1f)
                },
                Text = new PanelText
                {
                    Enabled = config.Panel?.Text?.Enabled       ?? true,
                    Color = config.Panel?.Text?.Color           ?? "#FF804FFF",
                    Order = config.Panel?.Text?.Order           ?? 1,
                    Width = config.Panel?.Text?.Width           ?? 0.72f,
                    FontSize = config.Panel?.Text?.FontSize     ?? 14,
                    Padding = config.Panel?.Text?.Padding       ?? new TypePadding(0.05f, 0.05f, 0.05f, 0.05f),
                    TextAnchor = config.Panel?.Text?.TextAnchor ?? TextAnchor.MiddleCenter,
                    Text = config.Panel?.Text?.Text             ?? "{0:hh:mm tt}"
                }
            };
            config.PanelSettings = new PanelRegistration
            {
                BackgroundColor = config.PanelSettings?.BackgroundColor ?? "#FFF2DF08",
                Dock = config.PanelSettings?.Dock ?? "leftbottom",
                Order = config.PanelSettings?.Order ?? 1,
                Width = config.PanelSettings?.Width ?? 0.075f
            };
            return config;
        }

        private void OnServerInitialized()
        {
            _sky = TOD_Sky.Instance;
            _time = _sky.Components.Time;
            _time.OnMinute += UpdateTime;
            RegisterPanels();
        }

        private void RegisterPanels()
        {
            if (MagicPanel == null)
            {
                PrintError("Missing plugin dependency MagicPanel: https://github.com/dassjosh/MagicPanel");
                return;
            }
        
            MagicPanel?.Call("RegisterGlobalPanel", this, Name, JsonConvert.SerializeObject(_pluginConfig.PanelSettings), nameof(GetPanel));
        }

        private void UpdateTime()
        {
            MagicPanel?.Call("UpdatePanel", Name, (int)UpdateEnum.Text);
        }

        private void Unload()
        {
            _time.OnMinute -= UpdateTime;
        }
        #endregion

        #region Helper Methods

        private string GetPanel()
        {
            Panel panel = _pluginConfig.Panel;
            PanelText text = panel.Text;
            if (text != null)
            {
                text.Text = string.Format(_textFormat, _sky.Cycle.DateTime);
            }

            return JsonConvert.SerializeObject(panel);
        }
        #endregion

        #region Classes
        private class PluginConfig
        {
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
            public bool Enabled { get; set; }
            public string Color { get; set; }
            public int Order { get; set; }
            public float Width { get; set; }
            public TypePadding Padding { get; set; }
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
