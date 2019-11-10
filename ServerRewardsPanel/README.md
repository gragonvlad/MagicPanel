# Server Rewards Panel

## About
Displays the players current server rewards balance

## Configuration
 
 ```json
{
  "Panel Update Rate (Seconds)": 5.0,
  "Panel Settings": {
    "Dock": "leftmiddle",
    "Width": 0.0625,
    "Order": 1,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/2kGm5dH.png",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 0,
      "Width": 0.33,
      "Padding": {
        "Left": 0.05,
        "Right": 0.0,
        "Top": 0.2,
        "Bottom": 0.05
      }
    },
    "Text": {
      "Text": "{0:0.00}",
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

### Configuration Options
`Update Rate (Seconds)` How often the panel should update the players server rewards in seconds