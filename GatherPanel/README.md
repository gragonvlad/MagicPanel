# Gather Panel

## About
Displays the servers gather rate.

## Configuration
 
 ```json
{
  "Default Gather": 1.0,
  "Panel Settings": {
    "Dock": "lefttop",
    "Width": 0.055,
    "Order": 0,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/gV9P0cK.png",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 0,
      "Width": 0.33,
      "Padding": {
        "Left": 0.05,
        "Right": 0.0,
        "Top": 0.2,
        "Bottom": 0.05
      }
    },
    "Text": {
      "Text": "{0:0.00}x",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 1,
      "Width": 0.67,
      "Padding": {
        "Left": 0.05,
        "Right": 0.05,
        "Top": 0.05,
        "Bottom": 0.05
      }
    }
  }
}
 ```

### Gather Rate
The gather rate can be set by a plugin or in the config. 
If no plugin is being used it will use the default gather rate in the config for everyone

### Global Gather Rate

If your plugin sets a global gather rate you can implement the hook below in your plugin to return what the current gather rate is

```c#
//Return the global gather for all player
private float GetGlobalGather()
```

To update the gather panel for every player you can call the following hook on GatherPanel

```c#
//Will update gather panel globally for all players
GatherPanel.Call("OnGlobalGatherUpdated");
```

### Player Gather Rate
If your plugin sets a player gather rate you can implement the hook below in your plugin to return what the players current gather rate is

```c#
//Returns the current gather for the player
private float GetGatherForPlayer(Baseplayer player)
```

To update the gather panel for a specific player you can call the following hook on GatherPanel

```c#
//Will update gather panel globally for all players
GatherPanel.Call("OnPlayerGatherUpdated" player);
```