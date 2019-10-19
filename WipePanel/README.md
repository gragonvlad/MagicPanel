# Wipe Panel

## About
Displays how much longer until the server wipes

## Required Plugin Dependency
The LastWipe plugin is required in order to use this panel.

## Configuration
 
 ```json
{
  "Panel Settings": {
    "Dock": "right",
    "Width": 0.1,
    "Order": 9,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "",
      "Enabled": false,
      "Color": "#FFFFFFFF",
      "Order": 0,
      "Width": 0.33,
      "Padding": {
        "Left": 0.05,
        "Right": 0.0,
        "Top": 0.05,
        "Bottom": 0.05
      }
    },
    "Text": {
      "Text": "Last Wipe: {0:MM/dd/yyyy}",
      "FontSize": 12,
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