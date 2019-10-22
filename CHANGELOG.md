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