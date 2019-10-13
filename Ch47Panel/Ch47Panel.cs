﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Ch47 Panel", "MJSU", "0.0.1")]
    [Description("Displays is the Ch47 event is active")]
    internal class Ch47Panel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private readonly Plugin MagicPanel;

        private PluginConfig _pluginConfig; //Plugin Config
        private List<CH47Helicopter> _activeCh47 = new List<CH47Helicopter>();
        private bool _isCh47Active;

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
                    Order = config.Panel?.Image?.Order ?? 4,
                    Width = config.Panel?.Image?.Width ?? 1f,
                    Url = config.Panel?.Image?.Url ?? "https://i.imgur.com/89Jm3Lf.png",
                    Padding = config.Panel?.Image?.Padding ?? new TypePadding(0.03f, 0.12f, 0.05f, 0.05f)
                }
            };
            config.PanelSettings = new PanelRegistration
            {
                BackgroundColor = config.PanelSettings?.BackgroundColor ?? "#FFF2DF08",
                Dock = config.PanelSettings?.Dock ?? "right",
                Order = config.PanelSettings?.Order ?? 2,
                Width = config.PanelSettings?.Width ?? 0.02f
            };
            return config;
        }

        private void OnServerInitialized()
        {
            RegisterPanels();

            NextTick(() =>
            {
                _activeCh47 = UnityEngine.Object.FindObjectsOfType<CH47Helicopter>().Where(CanShowPanel).ToList();
                CheckCh47();
            });

            timer.Every(_pluginConfig.UpdateRate, CheckCh47);
        }

        private void RegisterPanels()
        {
            MagicPanel?.Call("RegisterGlobalPanel", this, Name, JsonConvert.SerializeObject(_pluginConfig.PanelSettings), nameof(GetPanel));
        }

        private void CheckCh47()
        {
            _activeCh47.RemoveAll(p => !p.IsValid() || !p.gameObject.activeInHierarchy);

            bool areCh47Active = _activeCh47.Count > 0;

            if (areCh47Active != _isCh47Active)
            {
                _isCh47Active = areCh47Active;
                MagicPanel?.Call("UpdatePanel", Name, (int)UpdateEnum.Image);
            }
        }
        #endregion

        #region uMod Hooks

        private void OnEntitySpawned(CH47Helicopter heli)
        {
            NextTick(() =>
            {
                if (!CanShowPanel(heli))
                {
                    return;
                }
            
                _activeCh47.Add(heli);
                CheckCh47();
            });
        }
        #endregion

        #region Helper Methods
        private bool CanShowPanel(CH47Helicopter cargo)
        {
            object result = Interface.Call("MagicPanelCanShow", Name, cargo);
            if (result is bool)
            {
                return (bool) result;
            }

            return true;
        }

        private string GetPanel()
        {
            Panel panel = _pluginConfig.Panel;
            PanelImage image = panel.Image;
            if (image != null)
            {
                image.Color = _isCh47Active ? _pluginConfig.ActiveColor : _pluginConfig.InactiveColor;
            }

            return JsonConvert.SerializeObject(panel);
        }
        #endregion

        #region Classes

        private class PluginConfig
        {
            [DefaultValue("#008000FF")]
            [JsonProperty(PropertyName = "Active Color")]
            public string ActiveColor { get; set; }

            [DefaultValue("#ffffff1A")]
            [JsonProperty(PropertyName = "Inactive Color")]
            public string InactiveColor { get; set; }

            [DefaultValue(5f)]
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