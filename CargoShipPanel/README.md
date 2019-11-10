# Cargo Ship Panel

## About
Change the color of the Cargo Ship icon when the Cargo Ship is active

## Configuration
 
 ```json
{
  "Active Color": "#DE8732FF",
  "Inactive Color": "#FFFFFF1A",
  "Update Rate (Seconds)": 5.0,
  "Panel Settings": {
    "Dock": "center",
    "Width": 0.02,
    "Order": 5,
    "BackgroundColor": "#FFF2DF08"
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/LhZndt9.png",
      "Enabled": true,
      "Color": "#FFFFFFFF",
      "Order": 0,
      "Width": 1.0,
      "Padding": {
        "Left": 0.05,
        "Right": 0.05,
        "Top": 0.0,
        "Bottom": 0.0
      }
    }
  }
}
 ```

## API Hook

This hooks is to be used by other plugins that modify 
the cargo ship and don't want the cargo ship panel 
to toggle

```c#
//Name - name of the panel (ex. CargoShipPanel)
//cargo - cargo ship that caused the show
//return true to allow the cargo ship panel to go active for this CargoShip
//return false to not allow the cargo ship panel to go active for this CargoShip
object MagicPanelCanShow(string name, CargoShip cargo)
```