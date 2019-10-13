# Air Strike Panel

## About
Change the color of the Air Strike icon when an Air Strike is active.

## Configuration
 
 ```json
{
  "Active Color": "#00FF00FF", //Color when the Air Strike is Active
  "Inactive Color": "#FFFFFF1A", //Color when the Air Strike is Inactive
  "Update Rate (Seconds)": 5.0, //Update rate to check if Air Strike are still active
  "Panel Settings": {
    "Dock": "right", // Which dock the panel is assigned to
    "Width": 0.02, //How wide the panel is
    "Order": 1, //Which order the panel should be placed
    "BackgroundColor": "#FFF2DF08" // The color of the panel
  },
  "Panel Layout": {
    "Image": {
      "Url": "https://i.imgur.com/h3ayf2x.png", //Url for the icon for the air strike
      "Color": "#FFFFFFFF", //Color for the image
      "Order": 2, //Which order the image and text should be in
      "Width": 1.0, //How wide the image should be compared to the text
      "Padding": {
        "Left": 0.1, //How much padding should be on the left
        "Right": 0.1, //How much padding should be on the right
        "Top": 0.05, //How much padding should be on the top
        "Bottom": 0.055 //How much padding should be on the bottom
      }
    },
    "Text": null //Not Used by Air Stike Panel
  }
}
 ```