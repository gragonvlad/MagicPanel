using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Air Strike Panel", "MJSU", "0.0.1")]
    [Description("Displays if the air strike event is active")]
    internal class AirStrikePanel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private readonly Plugin MagicPanel, Airstrike;

        private PluginConfig _pluginConfig; //Plugin Config
        private List<CargoPlane> _activeStrikePlanes = new List<CargoPlane>();
        private bool _isStrikePlane;

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
                    Order = config.Panel?.Image?.Order ?? 2,
                    Width = config.Panel?.Image?.Width ?? 1f,
                    Url = config.Panel?.Image?.Url ?? "https://i.imgur.com/h3ayf2x.png",
                    Padding = config.Panel?.Image?.Padding ?? new TypePadding(0.1f, 0.1f, 0.05f, 0.05f)
                }
            };
            config.PanelSettings = new PanelRegistration
            {
                BackgroundColor = config.PanelSettings?.BackgroundColor ?? "#FFF2DF08",
                Dock = config.PanelSettings?.Dock ?? "right",
                Order = config.PanelSettings?.Order ?? 1,
                Width = config.PanelSettings?.Width ?? 0.02f
            };
            return config;
        }

        private void OnServerInitialized()
        {
            RegisterPanels();

            NextTick(() =>
            {
                _activeStrikePlanes = UnityEngine.Object.FindObjectsOfType<CargoPlane>().Where(CanShowPanel).ToList();
                CheckStrikePlanes();
            });

            timer.Every(_pluginConfig.UpdateRate, CheckStrikePlanes);
        }

        private void RegisterPanels()
        {
            MagicPanel?.Call("RegisterGlobalPanel", this, Name, JsonConvert.SerializeObject(_pluginConfig.PanelSettings), nameof(GetPanel));
        }

        private void CheckStrikePlanes()
        {
            _activeStrikePlanes.RemoveAll(p => !p.IsValid() || !p.gameObject.activeInHierarchy);

            bool areStrikePlanesActive = _activeStrikePlanes.Count > 0;

            if (areStrikePlanesActive != _isStrikePlane)
            {
                _isStrikePlane = areStrikePlanesActive;
                MagicPanel?.Call("UpdatePanel", Name, (int)UpdateEnum.Image);
            }
        }
        #endregion

        #region uMod Hooks

        private void OnEntitySpawned(CargoPlane plane)
        {
            NextTick(() =>
            {
                if (!CanShowPanel(plane))
                {
                    return;
                }

                _activeStrikePlanes.Add(plane);
                CheckStrikePlanes();
            });
        }
        #endregion

        #region Helper Methods
        private bool CanShowPanel(CargoPlane plane)
        {
            if (!IsStrikePlane(plane))
            {
                return false;
            }
            
            object result = Interface.Call("MagicPanelCanShow", Name, plane);
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
                image.Color = _isStrikePlane ? _pluginConfig.ActiveColor : _pluginConfig.InactiveColor;
            }

            return JsonConvert.SerializeObject(panel);
        }

        private bool IsStrikePlane(CargoPlane plane)
        {
            return Airstrike?.Call<bool>("isStrikePlane", plane) ?? false;
        }
        #endregion

        #region Classes

        private class PluginConfig
        {
            [DefaultValue("#00FF00FF")]
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
