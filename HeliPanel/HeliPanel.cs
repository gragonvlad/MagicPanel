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
    [Info("Heli Panel", "MJSU", "0.0.6")]
    [Description("Displays if the helicopter event is active")]
    internal class HeliPanel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private readonly Plugin MagicPanel;

        private PluginConfig _pluginConfig; //Plugin Config
        private List<BaseHelicopter> _activeHelis = new List<BaseHelicopter>();
        private bool _isHeliActive;

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
            config.Panel = new Panel
            {
                Image = new PanelImage
                {
                    Enabled = config.Panel?.Image?.Enabled ?? true,
                    Color = config.Panel?.Image?.Color ?? "#FFFFFFFF",
                    Order = config.Panel?.Image?.Order ?? 0,
                    Width = config.Panel?.Image?.Width ?? 1f,
                    Url = config.Panel?.Image?.Url ?? "http://i.imgur.com/hTTyTTx.png",
                    Padding = config.Panel?.Image?.Padding ?? new TypePadding(0.1f, 0.1f, 0.1f, 0.1f)
                }
            };
            config.PanelSettings = new PanelRegistration
            {
                BackgroundColor = config.PanelSettings?.BackgroundColor ?? "#FFF2DF08",
                Dock = config.PanelSettings?.Dock ?? "center",
                Order = config.PanelSettings?.Order ?? 3,
                Width = config.PanelSettings?.Width ?? 0.02f
            };
            return config;
        }

        private void OnServerInitialized()
        {
            RegisterPanels();

            NextTick(() =>
            {
                _activeHelis = UnityEngine.Object.FindObjectsOfType<BaseHelicopter>().Where(CanShowPanel).ToList();
                CheckHelicopter();
            });

            timer.Every(_pluginConfig.UpdateRate, CheckHelicopter);
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

        private void CheckHelicopter()
        {
            _activeHelis.RemoveAll(p => !p.IsValid() || !p.gameObject.activeInHierarchy);

            bool areHelisActive = _activeHelis.Count > 0;

            if (areHelisActive != _isHeliActive)
            {
                _isHeliActive = areHelisActive;
                MagicPanel?.Call("UpdatePanel", Name, (int)UpdateEnum.Image);
            }
        }
        #endregion

        #region uMod Hooks

        private void OnEntitySpawned(BaseHelicopter heli)
        {
            if (!CanShowPanel(heli))
            {
                return;
            }

            _activeHelis.Add(heli);
            CheckHelicopter();
        }
        #endregion

        #region MagicPanel Hook
        private Hash<string, object> GetPanel()
        {
            Panel panel = _pluginConfig.Panel;
            PanelImage image = panel.Image;
            if (image != null)
            {
                image.Color = _isHeliActive ? _pluginConfig.ActiveColor : _pluginConfig.InactiveColor;
            }

            return panel.ToHash();
        }
        #endregion

        #region Helper Methods

        private bool CanShowPanel(BaseHelicopter heli)
        {
            object result = Interface.Call("MagicPanelCanShow", Name, heli);
            if (result is bool)
            {
                return (bool) result;
            }

            return true;
        }
        #endregion

        #region Classes

        private class PluginConfig
        {
            [DefaultValue("#B33333FF")]
            [JsonProperty(PropertyName = "Active Color")]
            public string ActiveColor { get; set; }

            [DefaultValue("#FFFFFF1A")]
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
            
            public Hash<string, object> ToHash()
            {
                return new Hash<string, object>
                {
                    [nameof(Image)] = Image.ToHash(),
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
