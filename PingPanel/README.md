# Ping Panel

## About
Displays the current ping for the player

## Configuration
 
 ```json
{
  "Update Rate (Seconds)": 5.0,
  "Panel Settings": {
    "Dock": "lefttop",
    "Width": 0.05,
    "Order": 5,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": null,
    "Text": {
      "Text": "Ping: {0}ms",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Color": "#32CD32FF",
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