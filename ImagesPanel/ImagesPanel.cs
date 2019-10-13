using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Images Panel", "MJSU", "0.0.1")]
    [Description("Displays images in Magic Panel")]
    internal class ImagesPanel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private readonly Plugin MagicPanel;

        private PluginConfig _pluginConfig; //Plugin Config
        private List<CargoPlane> _activeAirdrops = new List<CargoPlane>();

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
            config.Panels = config.Panels ?? new Hash<string, PanelData>
            {
                [$"{Name}_1"] = new PanelData
                {
                    Panel = new Panel
                    {
                        Image = new PanelImage
                        {
                            Color = "#FFFFFFFF",
                            Order = 0,
                            Width = 1f,
                            Url = "https://i.imgur.com/FnVe2Fl.png",
                            Padding = new TypePadding(0.05f, 0.05f, 0.05f, 0.05f)
                        }
                    },
                    PanelSettings = new PanelRegistration
                    {
                        BackgroundColor = "#FFF2DF00",
                        Dock = "image",
                        Order = 0,
                        Width = 0.09f
                    }
                },
                [$"{Name}_2"] = new PanelData
                {
                    Panel = new Panel
                    {
                        Image = new PanelImage
                        {
                            Color = "#FFFFFFFF",
                            Order = 0,
                            Width = 1f,
                            Url = "https://i.imgur.com/FnVe2Fl.png",
                            Padding = new TypePadding(0.05f, 0.05f, 0.05f, 0.05f)
                        }
                    },
                    PanelSettings = new PanelRegistration
                    {
                        BackgroundColor = "#FFF2DF00",
                        Dock = "image",
                        Order = 0,
                        Width = 0.09f
                    }
                }
            };
            
            return config;
        }

        private void OnServerInitialized()
        {
            RegisterPanels();
        }

        private void RegisterPanels()
        {
            foreach (KeyValuePair<string,PanelData> panel in _pluginConfig.Panels)
            {
                MagicPanel?.Call("RegisterGlobalPanel", this, panel.Key, JsonConvert.SerializeObject(panel.Value.PanelSettings), nameof(GetPanel));
            }
        }
        #endregion

        #region Helper Methods

        private string GetPanel(string panelName)
        {
            Panel panel = _pluginConfig.Panels[panelName].Panel;
            return JsonConvert.SerializeObject(panel);
        }
        #endregion

        #region Classes

        private class PluginConfig
        {
            [JsonProperty(PropertyName = "Image Panels")]
            public Hash<string, PanelData> Panels { get; set; }
        }

        private class PanelData
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
