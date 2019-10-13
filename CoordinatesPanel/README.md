# Coordinates Panel

## About
Displays the players current position

## Configuration
 
 ```json
{
  "Update Rate (Seconds)": 5.0,
  "Panel Settings": {
    "Dock": "leftbottom",
    "Width": 0.1,
    "Order": 3,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/jeTMOyo.png",
      "Color": "#FFFFFFFF",
      "Order": 0,
      "Width": 0.24,
      "Padding": {
        "Left": 0.01,
        "Right": 0.0,
        "Top": 0.1,
        "Bottom": 0.1
      }
    },
    "Text": {
      "Text": "X: {0:0} | Z: {2:0}",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Color": "#FF6600FF",
      "Order": 1,
      "Width": 0.76,
      "Padding": {
        "Left": 0.01,
        "Right": 0.01,
        "Top": 0.05,
        "Bottom": 0.05
      }
    }
  }
}
 ```