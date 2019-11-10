# Airdrop Panel

## About
Change the color of the Airplane icon when an Airdrop is active.
It will not toggle for PlaneCrash or Airstrike plugins

## Configuration
 
 ```json
{
  "Active Color": "#00FF00FF",
  "Inactive Color": "#FFFFFF1A",
  "Update Rate (Seconds)": 5.0,
  "Panel Settings": {
    "Dock": "center",
    "Width": 0.02,
    "Order": 0,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "http://i.imgur.com/dble6vf.png",
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
a cargo plane and don't want the airdrop panel 
to toggle

```c#
//Name - name of the panel (ex. AirdropPanel)
//plane - plane that caused the show
//return true to allow the airdrop panel to go active for this CargoPlane
//return false to not allow the airdrop panel to go active for this CargoPlane
object MagicPanelCanShow(string name, CargoPlane plane)
```