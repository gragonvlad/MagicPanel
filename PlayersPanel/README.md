# Players Panel

## About
Displays the current player count

## Configuration
 
 ```json
{
  "Exclude Admins": false,
  "Panel Settings": {
    "Dock": "lefttop",
    "Width": 0.0525,
    "Order": 1,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/TP01GYf.png",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 0,
      "Width": 0.33,
      "Padding": {
        "Left": 0.05,
        "Right": 0.0,
        "Top": 0.1,
        "Bottom": 0.1
      }
    },
    "Text": {
      "Text": "{0}/{1}",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 1,
      "Width": 0.67,
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
