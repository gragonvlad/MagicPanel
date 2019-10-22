# Ch47 Panel

## About
Change the color of the Ch47 icon when the Ch47 Event is active

## Configuration
 
 ```json
{
  "Active Color": "#008000FF",
  "Inactive Color": "#ffffff1A",
  "Update Rate (Seconds)": 5.0,
  "Panel Settings": {
    "Dock": "right",
    "Width": 0.02,
    "Order": 2,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/89Jm3Lf.png",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 4,
      "Width": 1.0,
      "Padding": {
        "Left": 0.03,
        "Right": 0.12,
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