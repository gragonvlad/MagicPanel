# Hostile Panel

## About
Displays how much longer the player is considered hostile

## Configuration
 
 ```json
{
  "Show/Hide panel": false,
  "Panel Settings": {
    "Dock": "centerupper",
    "Width": 0.0725,
    "Order": 14,
    "BackgroundColor": "#fff2df08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/v5sdNHg.png",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 0,
      "Width": 0.3,
      "Padding": {
        "Left": 0.05,
        "Right": 0.05,
        "Top": 0.15,
        "Bottom": 0.15
      }
    },
    "Text": {
      "Text": "{0}m: {1:00}s",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 1,
      "Width": 0.7,
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
`Show hide panel` if false the panel will always be shown 
if true the panel will only be shown when the player is hostile
