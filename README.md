# Magic Panel

## Testing
This repository is for testing Magic Panel and the associated panels before going live on uMod.

## About
Magic Panel is a 100% API driven panel display plugin for rust. 
Each panel is a separate plugin that will register how that panel will function.
Panels dynamically position themselves within a dock based on the dock assigned and the order specified.

## Configuration

### Default Config
```json
{
  "Chat Command": "mp",
  "Use Image Library": true,
  "Docks": {
    "lefttop": {
      "Position": {
        "X Position": 0.07,
        "Y Start Position": 0.072,
        "Height": 0.035
      },
      "Enabled": true,
      "Background Color": "#00000000",
      "Panel Alignment (Left, Center, Right)": "Left",
      "Panel Padding": 0.004,
      "Dock Padding": {
        "Left": 0.001,
        "Right": 0.001,
        "Top": 0.0,
        "Bottom": 0.0
      }
    },
    "leftmiddle": {
      "Position": {
        "X Position": 0.07,
        "Y Start Position": 0.036,
        "Height": 0.035
      },
      "Enabled": true,
      "Background Color": "#00000000",
      "Panel Alignment (Left, Center, Right)": "Left",
      "Panel Padding": 0.004,
      "Dock Padding": {
        "Left": 0.001,
        "Right": 0.001,
        "Top": 0.0,
        "Bottom": 0.0
      }
    },
    "leftbottom": {
      "Position": {
        "X Position": 0.07,
        "Y Start Position": 0.0,
        "Height": 0.035
      },
      "Enabled": true,
      "Background Color": "#00000000",
      "Panel Alignment (Left, Center, Right)": "Left",
      "Panel Padding": 0.004,
      "Dock Padding": {
        "Left": 0.001,
        "Right": 0.001,
        "Top": 0.0,
        "Bottom": 0.0
      }
    },
    "image": {
      "Position": {
        "X Position": 0.65,
        "Y Start Position": 0.0,
        "Height": 0.1
      },
      "Enabled": true,
      "Background Color": "#00000000",
      "Panel Alignment (Left, Center, Right)": "Left",
      "Panel Padding": 0.004,
      "Dock Padding": {
        "Left": 0.001,
        "Right": 0.001,
        "Top": 0.0,
        "Bottom": 0.0
      }
    },
    "center": {
      "Position": {
        "X Position": 0.644,
        "Y Start Position": 0.109,
        "Height": 0.035
      },
      "Enabled": true,
      "Background Color": "#00000000",
      "Panel Alignment (Left, Center, Right)": "Right",
      "Panel Padding": 0.004,
      "Dock Padding": {
        "Left": 0.001,
        "Right": 0.001,
        "Top": 0.0,
        "Bottom": 0.0
      }
    },
    "centerupper": {
      "Position": {
        "X Position": 0.4966,
        "Y Start Position": 0.145,
        "Height": 0.035
      },
      "Enabled": true,
      "Background Color": "#00000000",
      "Panel Alignment (Left, Center, Right)": "Center",
      "Panel Padding": 0.004,
      "Dock Padding": {
        "Left": 0.001,
        "Right": 0.001,
        "Top": 0.0,
        "Bottom": 0.0
      }
    },
    "bottom": {
      "Position": {
        "X Position": 0.4966,
        "Y Start Position": 0.0,
        "Height": 0.0235
      },
      "Enabled": true,
      "Background Color": "#00000000",
      "Panel Alignment (Left, Center, Right)": "Center",
      "Panel Padding": 0.004,
      "Dock Padding": {
        "Left": 0.001,
        "Right": 0.001,
        "Top": 0.0,
        "Bottom": 0.0
      }
    },
    "undercompass": {
      "Position": {
        "X Position": 0.4966,
        "Y Start Position": 0.92,
        "Height": 0.035
      },
      "Enabled": true,
      "Background Color": "#00000000",
      "Panel Alignment (Left, Center, Right)": "Center",
      "Panel Padding": 0.004,
      "Dock Padding": {
        "Left": 0.001,
        "Right": 0.001,
        "Top": 0.0,
        "Bottom": 0.0
      }
    }
  }
}
``` 

### Configuration Options

```
"lefttop": { // Name of the Dock
  "Position": {
    "X Position": 0.07, //X start position of the Dock
    "Y Start Position": 0.072, //Y Start position of the Dock
    "Height": 0.035 //Heigh of the Dock
  },
  "Enabled": true, //If this dock is enabled
  "Background Color": "#FF000000", //Background color of the dock using 6 or 8 digit hex code
  "Panel Alignment (Left, Center, Right)": "Left", //Alighment of the dock. Determines which direction from the x start positon the dock will build itself from
  "Panel Padding": 0.004, //How much padding is between each panel
  "Dock Padding": 0.001 //How much padding is on the left of the first and and right of the last panel
}
```

#### Panel Alignment
- Left: The x Position will be the left position for the dock  
- Center: The x Position will be the the middle of the dock  
- Right: The x Position will be the right position for the dock
    - The order of the panel will be reversed with the right most panel being the lowest order value

## Chat Commands
* `/mp` - shows the magic panel help text  
* `/mp off`- hides all panels for the player  
* `/mp on` - shows all panels to the player

## Localization
```json
{
  "Chat": "<color=#bebebe>[<color=#de8732>Magic Panel</color>] {0}</color>",
  "On": "on",
  "Off": "off",
  "SettingsChanged": "All your panels are now {0}",
  "Help": "Controls the visibility of the magic panels:\n<color=#de8732>/mp on</color> shows all the magic panels\n<color=#de8732>/mp off</color> hides all the magic panels"
}
```

## API

There are two types of panels for Magic Panel. 
A global panel which displays the same data to every player and a player panel which displays data only to a specific player

### Classes needed for the API
These classes are used when sending information to Magic Panel and should be added to your Panel Plugin

#### Registration for the API
JSON serialized and sent to Magic Panel.
This tells Magic Panel information about the panel itself
```c#
private class PanelRegistration
{
    public string Dock { get; set; }
    public float Width { get; set; }
    public int Order { get; set; }
    public string BackgroundColor { get; set; }
}
```

#### Panel Class
This contains all the information how to build out the specific panel and contains the image and text information
```c#
private class Panel
{
    public PanelImage Image { get; set; }
    public PanelText Text { get; set; }
    
    public Hash<string, object> ToHash()
    {
        return new Hash<string, object>
        {
            [nameof(Image)] = Image.ToHash(),
            [nameof(Text)] = Text.ToHash()
        };
    }
}
```

#### Base Panel Class
This contains the basic information for each image and text
```c#
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
```

#### Image Panel Class
Tells Magic Panel how to display the image
```c#
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
```

#### Text Panel Class
Tells magic panel how to display the text
```c#
private class PanelText : PanelType
{
    public string Text { get; set; }
    public int FontSize { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TextAnchor TextAnchor { get; set; }
    
    public override Hash<string, object> ToHash()
    {
        Hash<string, object> hash = base.ToHash();
        hash[nameof(Text)] = Text;
        hash[nameof(FontSize)] = FontSize;
        hash[nameof(TextAnchor)] = TextAnchor;
        return hash;
    }
}
```

#### Type Padding
Applies padding to different parts of the panel
```c#
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
```

#### Update Enum
Passed into Magic Panel when updating telling it which panels should be updated
```c#
enum UpdateEnum { All = 1, Panel = 2, Image = 3, Text = 4 }
```

### Hooks

#### Register Player Panel
Registers a Player panel in Magic Panel
```c#
//plugin - plugin registering the panel
//name - name of the panel (typically he name of the plugin)
//panelData - JSON serialized PanelRegistration class
//getMethodName- The Hook to call in the plugin to get the panel updates
void RegisterPlayerPanel(Plugin plugin, string name, string panelData, string getMethodName)
```

#### Register Global Panel
Registers a Global Panel in Magic Panel
```c#
//plugin - plugin registering the panel
//name - name of the panel (typically he name of the plugin)
//panelData - JSON serialized PanelRegistration class
//getMethodName- The Hook to call in the plugin to get the panel updates
void RegisterGlobalPanel(Plugin plugin, string name, string panelData, string getMethodName)
```

#### Update Global Panel
Tells Magic Panel to update the specified global panel
```c#
//panelName - Panel to update
//update - int of the UpdateEnum to update
void UpdatePanel(string panelName, int update)
```

#### Update Player Panel
Tells Magic Panel to update the specified global panel
```c#
//player - Player to update the panel for
//panelName - Name of the panel to update
//update - int of the UpdateEnum to update
void UpdatePanel(BasePlayer player, string panelName, int update)
```

#### Show Panel
Show a hidden global panel
```c#
//name - name of the hidden panel to show
void ShowPanel(string name)
```

#### Show Panel
Show a hidden player panel
```c#
//name - name of the hidden panel to show
//player - player to unhide the panel for)
void ShowPanel(string name, BasePlayer player)
```

#### Hide Panel
Hides a global panel
```c#
//name - name of the panel to hide
void HidePanel(string name)
```

#### Hide Panel
Hides a player panel
```c#
//name - name of the panel to hide
//player - layer to hide the panel for
void HidePanel(string name, BasePlayer player )
```

## Testers
I want to thank all the testers who assisted with testing this plugin and associated panels
* Bull
* Supreme
* Trey
