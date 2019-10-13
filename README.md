# Magic Panel

## Testing
This repository is for testing Magic Panel and the associated panels before going live on uMod.

## About
Magic Panel is a 100% API driven panel display plugin for rust. 
Each panel is a separate plugin that will register how that panel will function.
Panels dynamically position themselves within a dock based on the dock assigned to and the order specified.

## Configuration

### Default Config
```json
{
  "Docks": {
    "lefttop": {
      "Position": {
        "X Position": 0.07,
        "Y Start Position": 0.072,
        "Height": 0.035
      },
      "Enabled": true,
      "Background Color": "#FF000000",
      "Panel Alignment (Left, Center, Right)": "Left",
      "Panel Padding": 0.004,
      "Dock Padding": 0.001
    },
    "leftmiddle": {
      "Position": {
        "X Position": 0.07,
        "Y Start Position": 0.036,
        "Height": 0.035
      },
      "Enabled": true,
      "Background Color": "#00FF0000",
      "Panel Alignment (Left, Center, Right)": "Left",
      "Panel Padding": 0.004,
      "Dock Padding": 0.001
    },
    "leftbottom": {
      "Position": {
        "X Position": 0.07,
        "Y Start Position": 0.0,
        "Height": 0.035
      },
      "Enabled": true,
      "Background Color": "#0000FF00",
      "Panel Alignment (Left, Center, Right)": "Left",
      "Panel Padding": 0.004,
      "Dock Padding": 0.001
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
      "Dock Padding": 0.001
    },
    "right": {
      "Position": {
        "X Position": 0.644,
        "Y Start Position": 0.109,
        "Height": 0.035
      },
      "Enabled": true,
      "Background Color": "#00000000",
      "Panel Alignment (Left, Center, Right)": "Right",
      "Panel Padding": 0.004,
      "Dock Padding": 0.001
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
      "Dock Padding": 0.001
    }
  }
}
``` 

### Dock Configuration Breakdown

```json
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
Left: The x Position will be the left position for the dock
Center: The x Position will be the the middle of the dock
Right: The x Position will be the right position for the dock (The order of the panel will be revesered with the right most panel being the lowest order value)

## API

There are two types of panels for Magic Panel. 
A global panel which displays the same data to every player and a player panel which displays data only to a specific player

### Classes needed for the API
These classes are used when sending information to Magic Panel and should be added to your Panel Plugin

#### Registration for the API
JSON serialized and sent to Magic Panel.
This tells Magic Panel information about the panel itself
```csharp
private class PanelRegistration
{
    public string Dock { get; set; }
    public float Width { get; set; }
    public int Order { get; set; }
    public string BackgroundColor { get; set; }
}
```

#### Panel Data
This contains all the information how to build out the specific panel and contains the image and text information
```csharp
private class Panel
{
    public PanelImage Image { get; set; }
    public PanelText Text { get; set; }
}
```

#### Base Class for Panels data
This contains the basic information for each image and text
```csharp
private abstract class PanelType
{
    public string Color { get; set; }
    public int Order { get; set; }
    public float Width { get; set; }
    public TypePadding Padding { get; set; } = new TypePadding();
}
```

#### Image Panel data
Tells Magic Panel how to display the image
```csharp
private class PanelImage : PanelType
{
    public string Url { get; set; }
}
```

#### Text Panel data
Tells magic panel how to display the text
```csharp
private class PanelText : PanelType
{
    public string Text { get; set; }
    public int FontSize { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TextAnchor TextAnchor { get; set; }
}
```

#### Type Padding
Applies padding to different parts of the panel
```csharp
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
```

#### Update Enum
Passed into Magic Panel when updating telling it which panels should be updated
```csharp
enum UpdateEnum { All, Panel, Image, Text }
```

### Hooks

#### Register Player Panel
Registers a Player panel in Magic Panel
```csharp
RegisterPlayerPanel(Plugin plugin, //Plugin Registering a Panel
string name, //Name of the panel
string panelData,  // JSON serialized PanelRegistration class
string getMethodName) //The Hook to call in the plugin to get the panel updates
```

#### Register Global Panel
Registers a Global Panel in Magic Panel
```csharp
RegisterGlobalPanel(Plugin plugin, //Plugin Registering a Panel
string name, //Name of the panel
string panelData,  // JSON serialized PanelRegistration class
string getMethodName) //The Hook to call in the plugin to get the panel updates
```

#### Update Global Panel
Tells Magic Panel to update the specified global panel
```csharp
UpdatePanel(string panelName, //Panel to update
int update //int of the UpdateEnum to update)
```

#### Update Player Panel
Tells Magic Panel to update the specified global panel
```csharp
UpdatePanel(BasePlayer player, //Player to update the panel for
string panelName, //Panel to update
int update //int of the UpdateEnum to update)
```