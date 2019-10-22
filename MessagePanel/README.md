# Images Panel

## About
Displays Messages

## Configuration
 
 ```json
{
  "Message Panels": {
    "MessagePanel_1": {
      "Panel Settings": {
        "Dock": "bottom",
        "Width": 0.2954,
        "Order": 0,
        "BackgroundColor": "#FFF2DF08"
      },
      "Panel Layout": {
        "Text": {
          "Text": "",
          "FontSize": 14,
          "TextAnchor": "MiddleCenter",
          "Enabled": true,
          "Color": "#FFFFFFFF",
          "Order": 1,
          "Width": 1.0,
          "Padding": {
            "Left": 0.05,
            "Right": 0.05,
            "Top": 0.1,
            "Bottom": 0.0
          }
        }
      },
      "Messages": [
        "Message 1",
        "Message 2",
        "<color=#FF0000>This message is red</color>"
      ],
      "Update Rate (Seconds)": 15.0
    }
  }
}
 ```

### Configuration Options
`Update Rate (Seconds)` How often the panel should change the message in seconds

`MessagePanel_1` is the name used for the panel. 
Adding more will allow you to add more message panels