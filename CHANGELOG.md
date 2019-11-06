## Version 0.0.7
HostilePanel -> Update Panel immediately when becoming hostile  
LastWipePanel -> Fixed Plugin Name and Description  
MagicPanel -> Fixed Left and Right padding not working on Text and Images

## Version 0.0.6
HostilePanel -> Text formatting changed  
PlayersPanel -> Configuration to hide admins from count  
All Panels -> Improved GetPanel performance by 5x-8x by sending a dictionary instead of serialized json  
MagicPanel -> Fixed issues with more padding being added to panels and the text and image components than should be  
MagicPanel -> Don't update panels if no players to update to  
MagicPanel -> Don't attempt to send updates to players with MagicPanel disabled  
MagicPanel -> Remove player hidden panels on disconnect  

### Note: All panels need to be on version 0.0.6 to work

## Version 0.0.5
DeathNotesPanel -> New Panel  
WipePanel -> Uses renamed WipeInfo -> WipeInfoApi  
WipeInfo -> Renamed to WipeInfoApi  
MagicPanel -> Protections to prevent invalid images from being attempted to be used  
MagicPanel -> Added new dock for DeathNotesPanel  

## Version 0.0.4
MagicPanel -> Added ImageLibrary support  
MagicPanel -> Fixed Right align calculation not including right dock padding  
MagicPanel -> If hex color is invalid default to black  
MagicPanel -> Made chat command configurable  
ServerRewardsPanel -> Fixed default Update Rate being 0  
RadiationPanel -> Fixed panel not displaying inactive when leaving a radiation zone  
JoiningPanel -> Fixed displaying queue count and not joining count  
WipePanel -> Use renamed WipeInfo plugin  
WipePanel -> Display error if WipeInfo is null  
WipeInfo -> Renamed from LastWipe  
All Panels -> Use new config loading  
All Panels -> Changed some default dock names and panels default docks  


## Version 0.0.3

MagicPanel -> Updated Dock Padding to use TypePadding. Padding can now be applied to Left, Right, Top, Bottom of the dock  
AllPanels -> Cleanup, Added option to enable / disable the Text or Image components
  
Added HostilePanel, RadiationInfoPanel, LastWipePanel, WipePanel  
Added LastWipe which is used by WipePanel

### Note:
If upgrading from a previous version it is recommended that you wipe your configs. There should not be another need to wipe configs after this.

## Version 0.0.2
MagicPanel -> Fixed RPC Error if Text is null  
MessagePanel -> Don't send null text if only 1 message

## Version 0.0.1
Initial Test Version