# Ping Panel

## About
Displays the current ping for the player

## Configuration
 
 ```json
{
  "Update Rate (Seconds)": 5.0,
  "Panel Settings": {
    "Dock": "leftmiddle",
    "Width": 0.055,
    "Order": 5,
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
      "Text": "Ping: {0}ms",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Enabled": true,
      "Color": "#FFFFFFFF",
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

### Configuration Options
`Update Rate (Seconds)` How often the panel should update the players ping in seconds