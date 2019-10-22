# Bradley Panel

## About
Change the color of the Bradley icon when the Bradley is active

## Configuration
 
 ```json
{
  "Active Color": "#00FF00FF",
  "Inactive Color": "#FFFFFF1A",
  "Update Rate (Seconds)": 5.0,
  "Panel Settings": {
    "Dock": "right",
    "Width": 0.02,
    "Order": 0,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/VrYPrKI.png",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 6,
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
the bradley and don't want the bradley panel 
to toggle

```c#
//Name - name of the panel (ex. BradleyPanel)
//bradley - bradley that caused the show
//return true to allow the bradley panel to go active for this BradleyAPC
//return false to not allow the bradley panel to go active for this BradleyAPC
object MagicPanelCanShow(string name, BradleyAPC bradley)
```