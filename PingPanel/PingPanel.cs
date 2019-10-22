using System.ComponentModel;
using Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Ping Panel", "MJSU", "0.0.4")]
    [Description("Displays players ping in magic panel")]
    internal class PingPanel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private Plugin MagicPanel;

        private PluginConfig _pluginConfig; //Plugin Config

        private static PingPanel _ins;

        private enum UpdateEnum { All = 1, Panel = 2, Image = 3, Text = 4 }
        
        private string _text;
        #endregion

        #region Setup & Loading
        private void Init()
        {
            _ins = this;
            _text = _pluginConfig.Panel.Text.Text;
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
                    Enabled = config.Panel?.Image?.Enabled ?? false,
                    Color = config.Panel?.Image?.Color ?? "#FFFFFFFF",
                    Order = config.Panel?.Image?.Order ?? 0,
                    Width = config.Panel?.Image?.Width ?? 0.33f,
                    Url = config.Panel?.Image?.Url ?? "",
                    Padding = config.Panel?.Image?.Padding ?? new TypePadding(0.05f, 0.0f, 0.05f, 0.05f)
                },
                Text = new PanelText
                {
                    Enabled = config.Panel?.Text?.Enabled ?? true,
                    Color = config.Panel?.Text?.Color ?? "#32CD32FF",
                    Order = config.Panel?.Text?.Order ?? 1,
                    Width = config.Panel?.Text?.Width ?? 1,
                    FontSize = config.Panel?.Text?.FontSize ?? 14,
                    Padding = config.Panel?.Text?.Padding ?? new TypePadding(0.05f, 0.05f, 0.05f, 0.05f),
                    TextAnchor = config.Panel?.Text?.TextAnchor ?? TextAnchor.MiddleCenter,
                    Text = config?.Panel?.Text?.Text ?? "Ping: {0}ms",
                }
            };
            config.PanelSettings = new PanelRegistration
            {
                BackgroundColor = config.PanelSettings?.BackgroundColor ?? "#FFF2DF08",
                Dock = config.PanelSettings?.Dock ?? "lefttop",
                Order = config.PanelSettings?.Order ?? 5,
                Width = config.PanelSettings?.Width ?? 0.055f
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
            player.GetComponent<PingBehavior>()?.DoDestroy();
        }

        private void Unload()
        {
            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                player.GetComponent<PingBehavior>()?.DoDestroy();
            }
            _ins = null;
        }
        #endregion

        #region Helper Methods

        private void AddBehavior(BasePlayer player)
        {
            if (player.GetComponent<PingBehavior>() == null)
            {
                player.gameObject.AddComponent<PingBehavior>();
            }
        }

        private string GetPanel(BasePlayer player)
        {
            Panel panel = _pluginConfig.Panel;
            PanelText text = panel.Text;
            if (text != null)
            {
                if (player != null && player.IsValid())
                {
                    text.Text = string.Format(_text, player.GetComponent<PingBehavior>()?.GetPing() ?? 0);
                }
                else
                {
                    text.Text = string.Format(_text, 0);
                }
            }

            return JsonConvert.SerializeObject(panel);
        }
        #endregion

        #region Classes

        private class PingBehavior : FacepunchBehaviour
        {
            private BasePlayer Player { get; set; }
            private int LastPing { get;set; }

            private void Awake()
            {
                enabled = false;
                Player = GetComponent<BasePlayer>();
                LastPing = CheckPing();
                InvokeRepeating(UpdatePing, _ins._pluginConfig.UpdateRate, _ins._pluginConfig.UpdateRate);
            }

            private void UpdatePing()
            {
                int ping = CheckPing();
                if (ping != LastPing)
                {
                    LastPing = ping;
                    _ins.MagicPanel?.Call("UpdatePanel", Player, _ins.Name, (int) UpdateEnum.Text);
                }
            }

            private int CheckPing()
            {
                return Net.sv.GetAveragePing(Player.net.connection);
            }

            public int GetPing()
            {
                return LastPing;
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
