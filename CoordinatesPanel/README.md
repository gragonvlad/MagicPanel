# Coordinates Panel

## About
Displays the players current x y z position

## Configuration
 
 ```json
{
  "Update Rate (Seconds)": 5.0,
  "Panel Settings": {
    "Dock": "leftbottom",
    "Width": 0.11,
    "Order": 3,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/jeTMOyo.png",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 0,
      "Width": 0.2,
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
      "Enabled": true,
      "Color": "#FF6600FF",
      "Order": 1,
      "Width": 0.8,
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

### Configuration Options
`Update Rate (Seconds)` How often the panel should update the players position in seconds

You can customize which of the x y z positions are show by changing the Text value.
- For x you need to have `{0}`
- For y you need to have `{1}`
- For z you need to have `{2}`
    - If you wish to format the value add a `:0` after the number like `{0:0}`
