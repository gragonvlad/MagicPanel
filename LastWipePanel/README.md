# Last Wipe Panel

## About
Displays when the server last wiped 

## Configuration
 
 ```json
{
  "Panel Settings": {
    "Dock": "centerupper",
    "Width": 0.1,
    "Order": 9,
    "BackgroundColor": "#FFF2DF08"
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
        "Top": 0.05,
        "Bottom": 0.05
      }
    },
    "Text": {
      "Text": "Last Wipe: {0:MM/dd/yyyy}",
      "FontSize": 12,
      "TextAnchor": "MiddleCenter",
      "Enabled": true,
      "Color": "#FFFFFFFF",
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

### Date Format
To learn how you can configure the displayed date in the panel please checkout the link below:

[Formatting the display date](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings)