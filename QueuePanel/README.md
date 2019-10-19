# Queue Panel

## About
Displays the current queue count

## Configuration
 
 ```json
{
  "Panel Settings": {
    "Dock": "lefttop",
    "Width": 0.045,
    "Order": 3,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/vNHFpsm.png",
      "Enabled": true,
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
      "Text": "{0}",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Enabled": true,
      "Color": "#FA8072FF",
      "Order": 1,
      "Width": 0.6,
      "Padding": {
        "Left": 0.05,
        "Right": 0.05,
        "Top": 0.05,
        "Bottom": 0.05
      }
    }
  },
  "Update Rates (Seconds)": 5.0
}
 ```
