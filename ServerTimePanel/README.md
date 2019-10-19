# Server Time Panel

## About
Displays the current in local time for the server

## Configuration
 
 ```json
{
  "Update Rate (Seconds)": 1.0,
  "Panel Settings": {
    "Dock": "leftbottom",
    "Width": 0.075,
    "Order": 2,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/IcYSp9E.png",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 0,
      "Width": 0.28,
      "Padding": {
        "Left": 0.05,
        "Right": 0.0,
        "Top": 0.1,
        "Bottom": 0.1
      }
    },
    "Text": {
      "Text": "{0:hh:mm tt}",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Enabled": true,
      "Color": "#FF804FFF",
      "Order": 1,
      "Width": 0.72,
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