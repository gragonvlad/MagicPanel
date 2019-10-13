# Airdrop Panel

## About
Change the color of the Airplane icon when an Airdrop is active.
It will not toggle for PlaneCrash or Airstrike plugins

## Configuration
 
 ```json
{
  "Active Color": "#00FF00FF", //Color when plane is active
  "Inactive Color": "#FFFFFF1A", //Color when plane is inactive
  "Update Rate (Seconds)": 5.0, //Update rate to check if planes are still active
  "Panel Settings": {
    "Dock": "right", // Which dock the panel is assigned to
    "Width": 0.02, //How wide the panel is
    "Order": 0, //Which order the panel should be placed
    "BackgroundColor": "#FFF2DF08" // The color of the panel
  },
  "Panel Layout": {
    "Image": {
      "Url": "http://i.imgur.com/dble6vf.png", //Url for the icon for the airplane
      "Color": "#FFFFFFFF", //Color for the image
      "Order": 0, //Which order the image and text should be in
      "Width": 1.0, //How wide the image should be compared to the text
      "Padding": {
        "Left": 0.05, //How much padding should be on the left
        "Right": 0.05, //How much padding should be on the right
        "Top": 0.05, //How much padding should be on the top
        "Bottom": 0.05 //How much padding should be on the bottom
      }
    },
    "Text": null //Not Used by Airdrop Panel
  }
}
 ```