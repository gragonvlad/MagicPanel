# Easter Panel

## About
Change the color of the Easter icon when the Easter Event is active

## Configuration
 
 ```json
{
  "Active Color": "#00FF00FF",
  "Inactive Color": "#FFFFFF1A",
  "Panel Settings": {
    "Dock": "center",
    "Width": 0.02,
    "Order": 12,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/pTpHnl2.png",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 0,
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

## API Hook

This hooks is to be used by other plugins that modify 
the ch47 heli and don't want the ch47 heli panel 
to toggle

```c#
//Name - name of the panel (ex. Ch47Panel)
//heli - ch47 heli that caused the show
//return true to allow the ch47 panel to go active for this CH47Helicopter
//return false to not allow the ch47 ship panel to go active for this CH47Helicopter
object MagicPanelCanShow(string name, CH47Helicopter heli)
```