# Death Notes Panel

## About
Displays the death note message in a panel.

## Configuration
 
 ```json
{
  "Display Duration (Seconds)": 5.0,
  "Panel Settings": {
    "Dock": "undercompass",
    "Width": 0.4,
    "Order": 1,
    "BackgroundColor": "#fff2df08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "",
      "Enabled": false,
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
      "Text": "{0}",
      "FontSize": 14,
      "TextAnchor": "MiddleCenter",
      "Enabled": true,
      "Color": "#33B5E6FF",
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

### Configuration Options
`Display Duration (Seconds)` Controls how long each message is displayed for
