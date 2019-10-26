# Wipe Panel

## About
Displays how much longer until the server wipes

## Required Plugin Dependency
The LastWipe plugin is required in order to use this panel.

## Configuration
 
 ```json
{
  "Panel Settings": {
    "Dock": "centerupper",
    "Width": 0.08,
    "Order": 10,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "",
      "Enabled": false,
      "Color": "#FFFFFFFF",
      "Order": 0,
      "Width": 0.4,
      "Padding": {
        "Left": 0.05,
        "Right": 0.0,
        "Top": 0.1,
        "Bottom": 0.1
      }
    },
    "Text": {
      "Text": "Wipe: {0}",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Enabled": true,
      "Color": "#08C717FF",
      "Order": 1,
      "Width": 1.0,
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

### Date Format
To learn how you can configure the displayed date in the panel please checkout the link below:

[Formatting the display date](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings)