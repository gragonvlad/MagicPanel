# Radiation Info Panel

## About
Displays the current radiation protection for the player verses how much protection the player needs

## Configuration
 
 ```json
{
  "Update Rate (Seconds)": 5.0,
  "Panel Settings": {
    "Dock": "right",
    "Width": 0.06,
    "Order": 12,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/hnNhgFj.png",
      "Enabled": true,
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
      "Text": "{0:0}/{1:0}",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Enabled": true,
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