using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Server Rewards Panel", "MJSU", "0.0.4")]
    [Description("Displays player server rewards data in MagicPanel")]
    internal class ServerRewardsPanel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private readonly Plugin MagicPanel, ServerRewards;

        private PluginConfig _pluginConfig; //Plugin Config
        private string _textFormat;

        private enum UpdateEnum { All = 1, Panel = 2, Image = 3, Text = 4 }

        private static ServerRewardsPanel _ins;
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
                    Width = config.Panel?.Image?.Width ?? 0.33f,
                    Url = config.Panel?.Image?.Url ?? "https://i.imgur.com/2kGm5dH.png",
                    Padding = config.Panel?.Image?.Padding ?? new TypePadding(0.05f, 0.0f, 0.2f, 0.05f)
                },
                Text = new PanelText
                {
                    Enabled = config.Panel?.Text?.Enabled ?? true,
                    Color = config.Panel?.Text?.Color ?? "#85BB65FF",
                    Order = config.Panel?.Text?.Order ?? 1,
                    Width = config.Panel?.Text?.Width ?? 0.67f,
                    FontSize = config.Panel?.Text?.FontSize ?? 14,
                    Padding = config.Panel?.Text?.Padding ?? new TypePadding(0.05f, 0.05f, 0.05f, 0.05f),
                    TextAnchor = config.Panel?.Text?.TextAnchor ?? TextAnchor.MiddleCenter,
                    Text = config.Panel?.Text?.Text ?? "{0:0.00}",
                }
            };
            config.PanelSettings = new PanelRegistration
            {
                BackgroundColor = config.PanelSettings?.BackgroundColor ?? "#FFF2DF08",
                Dock = config.PanelSettings?.Dock ?? "leftmiddle",
                Order = config.PanelSettings?.Order ?? 1,
                Width = config.PanelSettings?.Width ?? 0.0625f
            };
            return config;
        }

        private void OnServerInitialized()
        {
            RegisterPanels();

            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                OnPlayerInit(player);
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
            if (player.GetComponent<PlayerRewardsBehavior>() == null)
            {
                player.gameObject.AddComponent<PlayerRewardsBehavior>();
            }
        }
        
        private void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            player.GetComponent<PlayerRewardsBehavior>()?.DoDestroy();
        }

        private void Unload()
        {
            foreach (PlayerRewardsBehavior behavior in GameObject.FindObjectsOfType<PlayerRewardsBehavior>())
            {
                behavior.DoDestroy();   
            }
            _ins = null;
        }
        #endregion

        #region Helper Methods

        private int GetServerRewards(ulong userId)
        {
            int points = 0;
            object checkedPoints = ServerRewards?.Call<int>("CheckPoints", userId);
            if (checkedPoints is int)
            {
                points = (int) checkedPoints;
            }

            return points;
        }

        private string GetPanel(BasePlayer player)
        {
            Panel panel = _pluginConfig.Panel;
            PanelText text = panel.Text;
            if (text != null)
            {
                PlayerRewardsBehavior points = player.GetComponent<PlayerRewardsBehavior>();
                if (points == null)
                {
                    OnPlayerInit(player);
                    points = player.GetComponent<PlayerRewardsBehavior>();
                }
                
                text.Text = string.Format(_textFormat, points.GetPoints());
            }

            return JsonConvert.SerializeObject(panel);
        }
        #endregion

        #region Behavior

        private class PlayerRewardsBehavior : FacepunchBehaviour
        {
            private BasePlayer Player { get; set; }
            private int Points { get; set; }

            private void Awake()
            {
                Player = GetComponent<BasePlayer>();
                Points = _ins.GetServerRewards(Player.userID);
                InvokeRepeating(UpdatePoints, _ins._pluginConfig.UpdateRate, _ins._pluginConfig.UpdateRate);
            }

            private void UpdatePoints()
            {
                int newPoints = _ins.GetServerRewards(Player.userID);
                if (newPoints != Points)
                {
                    Points = newPoints;
                    _ins.MagicPanel?.Call("UpdatePanel", Player, _ins.Name, (int)UpdateEnum.Text);   
                }
            }
            
            public int GetPoints()
            {
                return Points;
            }

            public void DoDestroy()
            {
                Destroy(this);
            }
        }

        #endregion

        #region Classes
        private class PluginConfig
        {
            [DefaultValue(5f)]
            [JsonProperty(PropertyName = "Panel Update Rate (Seconds)")]
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
