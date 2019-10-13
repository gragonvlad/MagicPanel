﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Magic Panel", "MJSU", "0.0.1")]
    [Description("Displays information to the players on their hud.")]
    internal class MagicPanel : RustPlugin
    {
        #region Class Fields
        private StoredData _storedData; //Plugin Data
        private PluginConfig _pluginConfig; //Plugin Config

        private readonly Hash<string, Hash<string, float>> _panelPositions = new Hash<string, Hash<string, float>>();
        private readonly Hash<string, PanelRegistration> _registeredPanels = new Hash<string, PanelRegistration>();
        private readonly Hash<string, HiddenPanelInfo> _hiddenPanels = new Hash<string, HiddenPanelInfo>();

        private const string AccentColor = "#de8732";

        private bool _init;

        private enum UpdateEnum { All, Panel, Image, Text }
        private enum PanelAlignEnum { Left, Center, Right }
        private enum PanelTypeEnum { None = 0, Global = 1, Player = 2 }
        
        #endregion

        #region Setup & Loading
        private void Init()
        {
            ConfigLoad();

            _storedData = Interface.Oxide.DataFileSystem.ReadObject<StoredData>(Name);
        }
        
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                [LangKeys.Chat] = $"<color=#bebebe>[<color={AccentColor}>{Title}</color>] {{0}}</color>",
                [LangKeys.On] = "on",
                [LangKeys.Off] = "off",
                [LangKeys.SettingsChanged] = "All your panels are now {0}",
                [LangKeys.Help] = "Controls the visibility of the magic panels:\n" +
                                  $"<color={AccentColor}>/mp on</color> shows all the magic panels\n" +
                                  $"<color={AccentColor}>/mp off</color> hides all the magic panels"
            }, this);
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
            config.Docks = config.Docks ?? new Hash<string, Dock>
            {
                ["lefttop"] = new Dock
                {
                    BackgroundColor = "#00000000",
                    Enabled = true,
                    Alignment = PanelAlignEnum.Left,
                    Position = new DockPosition
                    {
                        XPos = .07f,
                        StartYPos = 0.072f,
                        Height = 0.035f
                    },
                    DockPadding = 0.001f,
                    PanelPadding = 0.004f
                },
                ["leftmiddle"] = new Dock
                {
                    BackgroundColor = "#00000000",
                    Enabled = true,
                    Alignment = PanelAlignEnum.Left,
                    Position = new DockPosition
                    {
                        XPos = .07f,
                        StartYPos = 0.036f,
                        Height = 0.035f
                    },
                    DockPadding = 0.001f,
                    PanelPadding = 0.004f
                },
                ["leftbottom"] = new Dock
                {
                    BackgroundColor = "#00000000",
                    Enabled = true,
                    Alignment = PanelAlignEnum.Left,
                    Position = new DockPosition
                    {
                        XPos = .07f,
                        StartYPos = 0.0f,
                        Height = 0.035f
                    },
                    DockPadding = 0.001f,
                    PanelPadding = 0.004f
                },
                ["image"] = new Dock
                {
                    BackgroundColor = "#00000000",
                    Enabled = true,
                    Alignment = PanelAlignEnum.Left,
                    Position = new DockPosition
                    {
                        XPos = 0.65f,
                        StartYPos = 0.0f,
                        Height = 0.1f
                    },
                    DockPadding = 0.001f,
                    PanelPadding = 0.004f
                },
                ["right"] = new Dock
                {
                    BackgroundColor = "#00000000",
                    Enabled = true,
                    Alignment = PanelAlignEnum.Right,
                    Position = new DockPosition
                    {
                        XPos = 0.644f,
                        StartYPos = 0.109f,// .01f,
                        Height = 0.035f
                    },
                    DockPadding = 0.001f,
                    PanelPadding = 0.004f
                },
                ["bottom"] = new Dock
                {
                    BackgroundColor = "#00000000",
                    Enabled = true,
                    Alignment = PanelAlignEnum.Center,
                    Position = new DockPosition
                    {
                        XPos = .4966f,
                        StartYPos = 0.0f,
                        Height = 0.0235f
                    },
                    DockPadding = 0.001f,
                    PanelPadding = 0.004f
                }
            };
            return config;
        }

        private void OnServerInitialized()
        {
            NextTick(() =>
            {
                Interface.Call("RegisterPanels");
                _init = true;

                DrawDock(BasePlayer.activePlayerList);

                foreach (PanelRegistration panel in _registeredPanels.Values)
                {
                    DrawPanel(BasePlayer.activePlayerList, panel, UpdateEnum.All);
                }
            });
        }

        private void OnPluginUnloaded(Plugin plugin)
        {
            UnregisterPluginPanels(plugin);
        }

        private void OnPlayerInit(BasePlayer player)
        {
            if (player.HasPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot))
            {
                timer.In(1f, () => { OnPlayerInit(player); });
                return;
            }

            DrawDock(new List<BasePlayer> { player });

            NextTick(() =>
            {
                List<BasePlayer> playerList = new List<BasePlayer> { player };
                foreach (PanelRegistration panel in _registeredPanels.Values)
                {
                    DrawPanel(playerList, panel, UpdateEnum.All);
                }
            });
        }

        private void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            DestroyAllUi(player);
        }

        private void Unload()
        {
            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                DestroyAllUi(player);
            }
        }
        #endregion

        #region Chat Commands

        [ChatCommand("mp")]
        private void MagicPanelChatCommand(BasePlayer player, string cmd, string[] args)
        {
            PlayerSettings settings = _storedData.Settings[player.userID];
            if (settings == null)
            {
                settings = new PlayerSettings
                {
                    Enabled = true
                };
                _storedData.Settings[player.userID] = settings;
            }

            if (args.Length == 0)
            {
                DisplayHelp(player);
                return;
            }

            switch (args[0].ToLower())
            {
                case "on":
                    OnPlayerInit(player);
                    Chat(player, Lang(LangKeys.SettingsChanged, player, Lang(LangKeys.On, player)));
                    settings.Enabled = true;
                    break;

                case "off":
                    DestroyAllUi(player);
                    Chat(player, Lang(LangKeys.SettingsChanged, player, Lang(LangKeys.Off, player)));
                    settings.Enabled = false;
                    break;

                default:
                    DisplayHelp(player);
                    break;
            }

            SaveData();
        }

        private void DisplayHelp(BasePlayer player)
        {
            Chat(player, Lang(LangKeys.Help, player));
        }
        #endregion

        #region Panel Register
        private void RegisterPlayerPanel(Plugin plugin, string name, string panelData, string getMethodName)
        {
            RegisterPanel(plugin, name, panelData, getMethodName, PanelTypeEnum.Player);
        }

        private void RegisterGlobalPanel(Plugin plugin, string name, string panelData, string getMethodName)
        {
            RegisterPanel(plugin, name, panelData, getMethodName, PanelTypeEnum.Global);
        }

        private void RegisterPanel(Plugin plugin, string name, string panelData, string getMethodName, PanelTypeEnum type)
        {
            PanelRegistration panel = JsonConvert.DeserializeObject<PanelRegistration>(panelData);
            panel.Plugin = plugin;
            panel.PanelType = type;
            panel.GetPanelMethod = getMethodName;
            panel.Name = name;
            
            if (string.IsNullOrEmpty(panel.Name))
            {
                PrintError("A registered panel must have a name");
                return;
            }

            if (!_pluginConfig.Docks.ContainsKey(panel.Dock))
            {
                PrintError($"Dock '{panel.Dock}' does not exist for panel '{panel.Name}'");
                return;
            }

            if (!_pluginConfig.Docks[panel.Dock].Enabled)
            {
                PrintError($"Dock '{panel.Dock}' is not enabled and can't have panels assigned to it");
                return;
            }

            _registeredPanels[panel.Name] = panel;
            _hiddenPanels[panel.Name] = new HiddenPanelInfo();
            RecalculatePositions(panel.Dock);
            DrawDock(BasePlayer.activePlayerList);
        }

        private void UnregisterPanel(PanelRegistration panel)
        {
            if (!_registeredPanels.ContainsKey(panel.Name))
            {
                return;
            }

            _registeredPanels.Remove(panel.Name);

            string panelUiName = GetPanelUiName(panel.Name);
            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                CuiHelper.DestroyUi(player, panelUiName);
            }

            _hiddenPanels[panel.Name] = null;
            RecalculatePositions(panel.Dock);
            DrawDock(BasePlayer.activePlayerList);
        }

        private void UnregisterPluginPanels(Plugin plugin)
        {
            foreach (PanelRegistration panel in _registeredPanels.Values.Where(rp => rp.Plugin.Name == plugin.Name).ToList())
            {
                UnregisterPanel(panel);
            }
        }

        private void RecalculatePositions(string dockName)
        {
            Hash<string, float> panelPositions = _panelPositions[dockName];
            if (panelPositions == null)
            {
                panelPositions = new Hash<string, float>();
                _panelPositions[dockName] = panelPositions;
            }
            else
            {
                panelPositions.Clear();
            }

            PanelAlignEnum align = _pluginConfig.Docks[dockName].Alignment;
            Dock dock = _pluginConfig.Docks[dockName];
            DockPosition pos = dock.Position;
            float offset = dock.DockPadding;

            List<PanelRegistration> dockPanels = _registeredPanels.Values
                .Where(rp => rp.Dock == dockName)
                .OrderBy(rp => rp.Order)
                .ToList();

            float startX = pos.XPos;
            if (align == PanelAlignEnum.Right)
            {
                foreach (PanelRegistration panel in dockPanels)
                {
                    startX -= panel.Width + dock.PanelPadding;
                }
                
                startX -= dock.DockPadding;
            }
            else if (align == PanelAlignEnum.Center)
            {
                foreach (PanelRegistration panel in dockPanels)
                {
                    startX -= panel.Width / 2 + dock.PanelPadding;
                }

                startX -= dock.DockPadding;
            } 
            
            if (align == PanelAlignEnum.Right)
            {
                dockPanels = dockPanels.OrderByDescending(p => p.Order).ToList();
            }

            foreach (PanelRegistration panel in dockPanels)
            {
                panelPositions[panel.Name] = startX + offset;
                offset += panel.Width + dock.PanelPadding;
                DrawPanel(BasePlayer.activePlayerList, panel, UpdateEnum.All);
            }
        }
        #endregion

        #region Panel Update API
        private void UpdatePanel(string panelName, int update)
        {
            if (!_registeredPanels.ContainsKey(panelName))
            {
                return;
            }

            NextTick(() =>
            {
                DrawPanel(BasePlayer.activePlayerList, _registeredPanels[panelName], (UpdateEnum)update);
            });
        }

        private void UpdatePanel(BasePlayer player, string panelName, int update)
        {
            if (!_registeredPanels.ContainsKey(panelName))
            {
                return;
            }

            NextTick(() =>
            {
                DrawPanel(new List<BasePlayer> { player }, _registeredPanels[panelName], (UpdateEnum)update);
            });
        }
        #endregion

        #region Panel Show / Hide
        private void ShowPanel(string name)
        {
            if (!_hiddenPanels[name].All)
            {
                return;
            }

            _hiddenPanels[name].All = false;
            DrawPanel(BasePlayer.activePlayerList, _registeredPanels[name], UpdateEnum.All);
        }

        private void ShowPanel(string name, BasePlayer player)
        {
            if (!_hiddenPanels[name].PlayerHidden.Contains(player.userID))
            {
                return;
            }

            _hiddenPanels[name].PlayerHidden.RemoveAll(p => p == player.userID);
            DrawPanel(new List<BasePlayer> { player }, _registeredPanels[name], UpdateEnum.All);
        }

        private void HidePanel(string name)
        {
            if (_hiddenPanels[name].All)
            {
                return;
            }

            _hiddenPanels[name].All = true;
            DrawPanel(BasePlayer.activePlayerList, _registeredPanels[name], UpdateEnum.All);
        }

        private void HidePanel(string name, BasePlayer player)
        {
            if (_hiddenPanels[name].PlayerHidden.Contains(player.userID))
            {
                return;
            }

            _hiddenPanels[name].PlayerHidden.Add(player.userID);
            DrawPanel(new List<BasePlayer> { player }, _registeredPanels[name], UpdateEnum.All);
        }
        #endregion

        #region Panel Draw

        private void DrawDock(List<BasePlayer> players)
        {
            foreach (BasePlayer player in players)
            {
                PlayerSettings settings = _storedData.Settings[player.userID];
                if (settings != null && !settings.Enabled)
                {
                    continue;
                }

                foreach (KeyValuePair<string, Dock> dock in _pluginConfig.Docks.Where(d => d.Value.Enabled))
                {
                    string dockName = GetDockUiName(dock.Key);
                    CuiElementContainer container = Ui.Container(Ui.Color(dock.Value.BackgroundColor), GetDockUiPosition(dock.Value.Position, dock.Key), false, dockName);
                    CuiHelper.DestroyUi(player, dockName);
                    CuiHelper.AddUi(player, container);
                }
            }
        }

        private void DrawPanel(List<BasePlayer> players, PanelRegistration registeredPanel, UpdateEnum updateEnum)
        {
            if (!_init)
            {
                return;
            }

            PanelCreator creator = new PanelCreator
            {
                Pos = _pluginConfig.Docks[registeredPanel.Dock].Position,
                PanelColor = Ui.Color(registeredPanel.BackgroundColor),
                StartPos = _panelPositions[registeredPanel.Dock][registeredPanel.Name],
                UiPanelName = GetPanelUiName(registeredPanel.Name),
                PanelReg = registeredPanel,
            };
            
            if (registeredPanel.PanelType == PanelTypeEnum.Global)
            {
                DrawGlobalPanel(players, creator, updateEnum);
            }
            else if (registeredPanel.PanelType == PanelTypeEnum.Player)
            {
                DrawPlayersPanel(players, creator, updateEnum);
            }
        }

        private void DrawGlobalPanel(List<BasePlayer> players, PanelCreator creator, UpdateEnum updateEnum)
        {
            PanelRegistration reg = creator.PanelReg;

            string panelData = reg.Plugin.Call<string>(reg.GetPanelMethod, reg.Name);
            if (string.IsNullOrEmpty(panelData))
            {
                PrintError($"DrawGlobalPanel: {reg.Plugin.Name} returned no data from {reg.GetPanelMethod} method");
                return;
            }
            
            Panel panel = JsonConvert.DeserializeObject<Panel>(panelData);
            
            
            List<PanelUpdate> containers = CreatePanel(panel, creator, updateEnum);
            foreach (BasePlayer player in players)
            {
                PlayerSettings settings = _storedData.Settings[player.userID];
                if (settings != null && !settings.Enabled)
                {
                    continue;
                }

                foreach (PanelUpdate update in containers)
                {
                    CuiHelper.DestroyUi(player, update.PanelName);

                    HiddenPanelInfo info = _hiddenPanels[creator.PanelReg.Name];

                    if (!info.All && !info.PlayerHidden.Contains(player.userID))
                    {
                        CuiHelper.AddUi(player, update.Container);
                    }
                }
            }
        }

        private void DrawPlayersPanel(List<BasePlayer> players, PanelCreator creator, UpdateEnum updateEnum)
        {
            foreach (BasePlayer player in players)
            {
                PlayerSettings settings = _storedData.Settings[player.userID];
                if (settings != null && !settings.Enabled)
                {
                    continue;
                }

                PanelRegistration reg = creator.PanelReg;
                string panelData = reg.Plugin.Call<string>(reg.GetPanelMethod, player, reg.Name);
                if (string.IsNullOrEmpty(panelData))
                {
                    PrintError($"DrawPlayersPanel: {reg.Plugin.Name} returned no data from {reg.GetPanelMethod} method");
                    return;
                }
                
                Panel panel = JsonConvert.DeserializeObject<Panel>(panelData);
                foreach (PanelUpdate update in CreatePanel(panel, creator, updateEnum))
                {
                    CuiHelper.DestroyUi(player, update.PanelName);
                    HiddenPanelInfo info = _hiddenPanels[creator.PanelReg.Name];

                    if (!info.All && !info.PlayerHidden.Contains(player.userID))
                    {
                        CuiHelper.AddUi(player, update.Container);
                    }
                }
            }
        }

        private List<PanelUpdate> CreatePanel(Panel panel, PanelCreator creator, UpdateEnum update)
        {
            List<PanelUpdate> containers = new List<PanelUpdate>();

            if (update == UpdateEnum.All || update == UpdateEnum.Panel)
            {
                UiPosition pos = new UiPosition(creator.StartPos, creator.Pos.StartYPos, creator.PanelReg.Width, creator.Pos.Height);
                CuiElementContainer container = Ui.Container(creator.PanelColor, pos, false, creator.UiPanelName);
                containers.Add(new PanelUpdate
                {
                    Container = container,
                    PanelName = creator.UiPanelName
                });
            }

            List<PanelType> panelTypes = new List<PanelType>();
            if (panel.Text != null)
            {
                panelTypes.Add(panel.Text);
            }

            if (panel.Image != null)
            {
                panelTypes.Add(panel.Image);
            }

            panelTypes = panelTypes.OrderBy(pt => pt.Order).ToList();

            float offset = 0;
            foreach (PanelType type in panelTypes)
            {
                if (type is PanelText)
                {
                    if (update == UpdateEnum.All || update == UpdateEnum.Text)
                    {
                        containers.Add(CreateText(panel, creator.UiPanelName, offset));
                    }
                }
                else if (type is PanelImage)
                {
                    if (update == UpdateEnum.All || update == UpdateEnum.Image)
                    {
                        containers.Add(CreateImage(panel, creator.UiPanelName, offset));
                    }
                }
                
                offset += type.Width;
            }

            return containers;
        }

        private PanelUpdate CreateImage(Panel panel, string panelName, float offset)
        {
            string imageName = GetPanelUiImageName(panelName);
            UiPosition pos = GetPaddedPosition(offset, panel.Image.Width, panel.Image.Padding);
            CuiElementContainer container = Ui.Container(_clearColor, pos, false, imageName, panelName);
            Ui.LoadImage(ref container, panel.Image.Url, Ui.Color(panel.Image.Color), _fullSize);
            return new PanelUpdate
            {
                Container = container,
                PanelName = imageName
            };
        }

        private PanelUpdate CreateText(Panel panel, string panelName, float offset)
        {
            string textName = GetPanelUiTextName(panelName);
            UiPosition pos = GetPaddedPosition(offset, panel.Text.Width, panel.Text.Padding);
            CuiElementContainer container = Ui.Container(_clearColor, pos, false, textName, panelName);
            Ui.Label(ref container, panel.Text.Text, panel.Text.FontSize, Ui.Color(panel.Text.Color), _fullSize, panel.Text.TextAnchor);
            return new PanelUpdate
            {
                Container = container,
                PanelName = textName
            };
        }
        #endregion

        #region Helper Methods

        private string GetPanelUiName(string panelName)
        {
            return $"{UiPanel}{panelName}";
        }

        private string GetPanelUiImageName(string panelName)
        {
            return $"{panelName}_Image";
        }

        private string GetPanelUiTextName(string panelName)
        {
            return $"{panelName}_Text";
        }

        private string GetDockUiName(string dockName)
        {
            return $"{UiPanel}_dock_{dockName}";
        }

        private UiPosition GetPaddedPosition(float startPos, float widthPercentage, TypePadding padding)
        {
            UiPosition pos = new UiPosition(startPos + padding.Left,
                padding.Bottom,
                widthPercentage - (padding.Left + padding.Right),
                1 - (padding.Top + padding.Bottom));
            return pos;
        }

        private UiPosition GetDockUiPosition(DockPosition pos, string dockName)
        {
            float startPos = 0f;
            float endPos = 0f;
            if (_panelPositions.ContainsKey(dockName))
            {
                Dock dock = _pluginConfig.Docks[dockName];
                ICollection<float> panels = _panelPositions[dockName].Values;
                if (panels.Count != 0)
                {
                    startPos = panels.Min(pp => pp) - dock.DockPadding;
                    endPos = panels.Max(pp => pp) + dock.DockPadding;
                    endPos += _registeredPanels.Values.Where(p => p.Dock == dockName).OrderBy(p => p.Order).Last()?.Width ?? 0;
                }
            }
            
            return new UiPosition(startPos, pos.StartYPos, endPos - startPos , pos.Height);
        }

        private void SaveData() => Interface.Oxide.DataFileSystem.WriteObject(Name, _storedData);

        private void Chat(BasePlayer player, string format, params object[] args) => PrintToChat(player, Lang(LangKeys.Chat, player, format), args);
        
        private string Lang(string key, BasePlayer player = null, params object[] args) => string.Format(lang.GetMessage(key, this, player?.UserIDString), args);
        #endregion

        #region Classes
        private class PluginConfig
        {
            [JsonProperty(PropertyName = "Docks")]
            public Hash<string, Dock> Docks { get; set; }
        }

        private class Dock
        {
            [JsonProperty(PropertyName = "Position")]
            public DockPosition Position { get; set; }

            [JsonProperty(PropertyName = "Enabled")]
            public bool Enabled { get; set; }

            [JsonProperty(PropertyName = "Background Color")]
            public string BackgroundColor { get; set; }
            
            [JsonConverter(typeof(StringEnumConverter))]
            [JsonProperty(PropertyName = "Panel Alignment (Left, Center, Right)")]
            public PanelAlignEnum Alignment { get; set; }

            [JsonProperty(PropertyName = "Panel Padding")]
            public float PanelPadding { get; set; }
            
            [JsonProperty(PropertyName = "Dock Padding")]
            public float DockPadding { get; set; }
        }

        private class DockPosition
        {
            [JsonProperty(PropertyName = "X Position")]
            public float XPos { get; set; }

            [JsonProperty(PropertyName = "Y Start Position")]
            public float StartYPos { get; set; }

            [JsonProperty(PropertyName = "Height")]
            public float Height { get; set; }
        }

        private class StoredData
        {
            public Hash<ulong, PlayerSettings> Settings = new Hash<ulong, PlayerSettings>();
        }
        
        private static class LangKeys
        {
            public const string On = "On";
            public const string Off = "Off";
            public const string SettingsChanged = "SettingsChanged";
            public const string Chat = "Chat";
            public const string Help = "Help";
        }

        private class PlayerSettings
        {
            public bool Enabled { get; set; }
        }

        private class PanelRegistration
        {
            public string Name { get; set; }
            public string Dock { get; set; }
            public float Width { get; set; }
            public int Order { get; set; }
            public string BackgroundColor { get; set; }
            public string GetPanelMethod { get; set; }
            public PanelTypeEnum PanelType { get; set; }
            public Plugin Plugin { get; set; }
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

        private class PanelCreator
        {
            public DockPosition Pos { get; set; }
            public float StartPos { get; set; }
            public string UiPanelName { get; set; }
            public string PanelColor { get; set; }
            public PanelRegistration PanelReg { get; set; }
        }

        private class PanelUpdate
        {
            public CuiElementContainer Container { get; set; }
            public string PanelName { get; set; }
        }

        private class HiddenPanelInfo
        {
            public List<ulong> PlayerHidden { get; set; }
            public bool All { get; set; }

            public HiddenPanelInfo()
            {
                All = false;
                PlayerHidden = new List<ulong>();
            }
        }
        #endregion

        #region UI
        private const string UiPanel = "MagicPanel_";
        private readonly string _clearColor = Ui.Color("#00000000");
        private readonly UiPosition _fullSize = new UiPosition(0, 0, 1, 1);

        private static class Ui
        {
            private static string UiPanel { get; set; }

            public static CuiElementContainer Container(string color, UiPosition pos, bool useCursor, string panel, string parent = "Hud")
            {
                UiPanel = panel;
                return new CuiElementContainer
                {
                    {
                        new CuiPanel
                        {
                            Image = {Color = color},
                            RectTransform = {AnchorMin = pos.GetMin(), AnchorMax = pos.GetMax()},
                            CursorEnabled = useCursor
                        },
                        new CuiElement().Parent = parent,
                        panel
                    }
                };
            }

            public static void Label(ref CuiElementContainer container, string text, int size, string color, UiPosition pos, TextAnchor align = TextAnchor.MiddleCenter)
            {
                container.Add(new CuiLabel
                {
                    Text = { FontSize = size, Align = align, Text = text, Color = color },
                    RectTransform = { AnchorMin = pos.GetMin(), AnchorMax = pos.GetMax() }

                },
                UiPanel);
            }

            public static void LoadImage(ref CuiElementContainer container, string png, string color, UiPosition pos)
            {
                container.Add(new CuiElement
                {
                    Name = CuiHelper.GetGuid(),
                    Parent = UiPanel,
                    Components =
                    {
                        new CuiRawImageComponent {Color = color, Png = !png?.StartsWith("http") ?? false ? png : null, Url = png?.StartsWith("http") ?? false ? png : null},
                        new CuiRectTransformComponent {AnchorMin = pos.GetMin(), AnchorMax = pos.GetMax() }
                    }
                });
            }

            public static string Color(string hexColor)
            {
                hexColor = hexColor.TrimStart('#');
                int red = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                int green = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                int blue = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
                int alpha = 255;
                if (hexColor.Length == 8)
                {
                    alpha = int.Parse(hexColor.Substring(6, 2), NumberStyles.AllowHexSpecifier);
                }

                return $"{red / 255.0} {green / 255.0} {blue / 255.0} {alpha / 255.0}";
            }
        }

        private class UiPosition
        {
            public float XPos { get; set; }
            public float YPos { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }

            public UiPosition(float xPos, float yPos, float width, float height)
            {
                XPos = xPos;
                YPos = yPos;
                Width = width;
                Height = height;
            }

            public string GetMin() => $"{XPos} {YPos}";
            public string GetMax() => $"{XPos + Width} {YPos + Height}";

            public override string ToString()
            {
                return $"{XPos} {YPos} {Width} {Height}";
            }
        }

        private void DestroyAllUi(BasePlayer player)
        {
            foreach (PanelRegistration panel in _registeredPanels.Values)
            {
                string panelName = GetPanelUiName(panel.Name);
                CuiHelper.DestroyUi(player, panelName);
                CuiHelper.DestroyUi(player, GetPanelUiImageName(panelName));
                CuiHelper.DestroyUi(player, GetPanelUiTextName(panelName));
            }

            foreach (string dock in _pluginConfig.Docks.Keys)
            {
                CuiHelper.DestroyUi(player, GetDockUiName(dock));
            }
        }
        #endregion
    }
}