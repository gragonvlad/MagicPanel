using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Radiation Info Panel", "MJSU", "0.0.4")]
    [Description("Displays how much radiation protection the player has verse how much they need")]
    internal class RadiationInfoPanel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private Plugin MagicPanel;

        private PluginConfig _pluginConfig; //Plugin Config

        private string _textFormat;

        private static RadiationInfoPanel _ins;

        private enum UpdateEnum { All = 1, Panel = 2, Image = 3, Text = 4 }
        #endregion

        #region Setup & Loading
        private void Init()
        {
            _ins = this;
            _textFormat = _pluginConfig.Panel.Text.Text;
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
            config.Panel = new Panel
            {
                Image = new PanelImage
                {
                    Enabled = config.Panel?.Image?.Enabled ?? true,
                    Color = config.Panel?.Image?.Color ?? "#FFFFFFFF",
                    Order = config.Panel?.Image?.Order ?? 0,
                    Width = config.Panel?.Image?.Width ?? 0.24f,
                    Url = config.Panel?.Image?.Url ?? "https://i.imgur.com/hnNhgFj.png",
                    Padding = config.Panel?.Image?.Padding ?? new TypePadding(0.01f, 0.00f, 0.1f, 0.1f)
                },
                Text = new PanelText
                {
                    Enabled = config.Panel?.Text?.Enabled ?? true,
                    Color = config.Panel?.Text?.Color ?? "#FF6600FF",
                    Order = config.Panel?.Text?.Order ?? 1,
                    Width = config.Panel?.Text?.Width ?? .76f,
                    FontSize = config.Panel?.Text?.FontSize ?? 14,
                    Padding = config.Panel?.Text?.Padding ?? new TypePadding(0.01f, 0.01f, 0.05f, 0.05f),
                    TextAnchor = config.Panel?.Text?.TextAnchor ?? TextAnchor.MiddleCenter,
                    Text = config.Panel?.Text?.Text ?? "{0:0}/{1:0}"
                }
            };
            config.PanelSettings = new PanelRegistration
            {
                BackgroundColor = config.PanelSettings?.BackgroundColor ?? "#FFF2DF08",
                Dock = config.PanelSettings?.Dock ?? "centerupper",
                Order = config.PanelSettings?.Order ?? 12,
                Width = config.PanelSettings?.Width ?? 0.06f
            };
            return config;
        }

        private void OnServerInitialized()
        {
            RegisterPanels();
            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                AddBehavior(player);
            }
        }

        private void RegisterPanels()
        {
            if (MagicPanel == null)
            {
                PrintError("Missing plugin dependency MagicPanel: https://github.com/dassjosh/MagicPanel");
                return;
            }
        
            MagicPanel?.Call("RegisterPlayerPanel", this, Name, JsonConvert.SerializeObject(_pluginConfig.PanelSettings), nameof(GetPanel));
        }

        private void OnPlayerInit(BasePlayer player)
        {
            AddBehavior(player);
        }

        private void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            DestroyBehavior(player);
        }

        private void Unload()
        {
            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                DestroyBehavior(player);
            }
            _ins = null;
        }
        #endregion

        #region Helper Methods

        private void AddBehavior(BasePlayer player)
        {
            if (player.GetComponent<RadiationBehavior>() == null)
            {
                player.gameObject.AddComponent<RadiationBehavior>();
            }
        }

        private void DestroyBehavior(BasePlayer player)
        {
            RadiationBehavior radiation = player.GetComponent<RadiationBehavior>();
            radiation?.DoDestroy();
        }

        private string GetPanel(BasePlayer player)
        {
            Panel panel = _pluginConfig.Panel;
            PanelText text = panel.Text;
            if (text != null)
            {
                RadiationBehavior rad = player.GetComponent<RadiationBehavior>();
                float radAmount = rad?.LastRadAmount ?? 0;
                float radProtection =  rad?.LastProtectionAmount ?? 0;

                text.Text = string.Format(_textFormat, radProtection, radAmount);
            }

            return JsonConvert.SerializeObject(panel);
        }
        #endregion

        #region Classes

        private class RadiationBehavior : FacepunchBehaviour
        {
            private BasePlayer Player { get; set; }
            public float LastRadAmount { get; private set; }
            public float LastProtectionAmount { get; private set; }

            private void Awake()
            {
                enabled = false;
                Player = GetComponent<BasePlayer>();
                InvokeRepeating(UpdateRadiation, _ins._pluginConfig.UpdateRate, _ins._pluginConfig.UpdateRate);
            }

            private void UpdateRadiation()
            {
                float radAmount = 0f;
                if (Player.triggers != null)
                {
                    foreach (TriggerBase trigger in Player.triggers)
                    {
                        TriggerRadiation radiation = trigger as TriggerRadiation;
                        if (radiation != null)
                        {
                            radAmount = Mathf.Max(radAmount, radiation.GetRadiation(Player.transform.position, 0));
                        }
                    }
                }

                float radProtection = Player.RadiationProtection();

                if (radAmount != LastRadAmount || radProtection != LastProtectionAmount)
                {
                    _ins.MagicPanel?.Call("UpdatePanel", Player, _ins.Name, (int)UpdateEnum.Text);
                    LastRadAmount = radAmount;
                    LastProtectionAmount = radProtection;
                }
            }

            public void DoDestroy()
            {
                Destroy(this);
            }
        }

        private class PluginConfig
        {
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
