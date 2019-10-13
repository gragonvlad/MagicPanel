﻿using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Joining Panel", "MJSU", "0.0.1")]
    [Description("Displays joining player count in magic panel")]
    internal class JoiningPanel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private readonly Plugin MagicPanel;

        private PluginConfig _pluginConfig; //Plugin Config
        private string _textFormat;

        private enum UpdateEnum { All, Panel, Image, Text }

        private int _joiningCount;
        #endregion

        #region Setup & Loading
        private void Loaded()
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
                    Color = config.Panel?.Image?.Color ?? "#FFFFFFFF",
                    Order = config.Panel?.Image?.Order ?? 0,
                    Width = config.Panel?.Image?.Width ?? 0.33f,
                    Url = config.Panel?.Image?.Url ?? "https://i.imgur.com/l0kKN4c.png",
                    Padding = config.Panel?.Image?.Padding ?? new TypePadding(0.05f, 0.0f, 0.5f, 0.5f)
                },
                Text = new PanelText
                {
                    Color = config.Panel?.Text?.Color ?? "#FD5F00FF",
                    Order = config.Panel?.Text?.Order ?? 1,
                    Width = config.Panel?.Text?.Width ?? .67f,
                    FontSize = config.Panel?.Text?.FontSize ?? 14,
                    Padding = config.Panel?.Text?.Padding ?? new TypePadding(0.05f, 0.05f, 0.05f, 0.05f),
                    TextAnchor = config.Panel?.Text?.TextAnchor ?? TextAnchor.MiddleCenter,
                    Text = config.Panel?.Text?.Text ?? "{0}",
                }
            };
            config.PanelSettings = new PanelRegistration
            {
                BackgroundColor = config.PanelSettings?.BackgroundColor ?? "#FFF2DF08",
                Dock = config.PanelSettings?.Dock ?? "lefttop",
                Order = config.PanelSettings?.Order ?? 4,
                Width = config.PanelSettings?.Width ?? 0.045f
            };
            return config;
        }

        private void OnServerInitialized()
        {
            RegisterPanels();
            _joiningCount = ServerMgr.Instance.connectionQueue.Joining;

            timer.Every(_pluginConfig.UpdateRate, () =>
            {
                int queued = ServerMgr.Instance.connectionQueue.Joining;
                if (queued != _joiningCount)
                {
                    _joiningCount = queued;
                    UpdatePanel();
                }
            });
        }

        private void RegisterPanels()
        {
            MagicPanel?.Call("RegisterGlobalPanel", this, Name, JsonConvert.SerializeObject(_pluginConfig.PanelSettings), nameof(GetPanel));
        }

        private void OnPlayerInit(BasePlayer player)
        {
            UpdatePanel();
        }

        private void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            NextTick(() =>
            {
                MagicPanel?.Call("UpdatePanel", Name, (int)UpdateEnum.Text);
            });
        }
        #endregion

        #region Helper Methods

        private string GetPanel()
        {
            Panel panel = _pluginConfig.Panel;
            PanelText text = panel.Text;
            if (text != null)
            {
                text.Text = string.Format(_textFormat, ServerMgr.Instance.connectionQueue.Queued);
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
            
            [DefaultValue(5f)]
            [JsonProperty(PropertyName = "Update Rates (Seconds)")]
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