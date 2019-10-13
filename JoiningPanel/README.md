# Joining Panel

## About
Displays how many players a currently joining

## Configuration
 
 ```json
{
  "Panel Settings": {
    "Dock": "lefttop",
    "Width": 0.045,
    "Order": 4,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/l0kKN4c.png",
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
      "Text": "{0}",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Color": "#FD5F00FF",
      "Order": 1,
      "Width": 0.67,
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