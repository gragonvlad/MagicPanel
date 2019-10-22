# Hostile Panel

## About
Displays how much longer the player is considered hostile

## Configuration
 
 ```json
{
  "Show hide panel": true,
  "Panel Settings": {
    "Dock": "right",
    "Width": 0.08,
    "Order": 14,
    "BackgroundColor": "#fff2df08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/v5sdNHg.png",
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
      "Text": "{0}",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Enabled": true,
      "Color": "#08C717FF",
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
`Show hide panel` if false the panel will always be shown 
if true the panel will only be shown when the player is hostile
