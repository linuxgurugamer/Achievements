Originally written by  @blizzy78, continued by @malkuth , and finally by myself.

Original thread here: https://forum.kerbalspaceprogram.com/index.php?/topic/48225-0242-achievements-163-earn-136-achievements-while-playing/

Previous thread here: https://forum.kerbalspaceprogram.com/index.php?/topic/91806-ksp-achievements-1942-ksp-141-3142018/


https://i.imgur.com/Li3Y3P4.png


The new version for KSP 1.8.1/1.9.1 has new dependencies

Dependencies Updated

Click Through Blocker
ToolbarController
SpaceTuxLibrary


This plugin brings Kerbal Space Program a Steam Like Achievement System. Currently as of this release there are about 145 Achievements in the following categories.

 

Crew Operations
General Flight
Ground Operations
Landing
Research and Development
SpaceFlight
Contracts
Funds
Reputation
 

https://i.imgur.com/Gx6sE4C.png

Availablility

Source code:https://github.com/linuxgurugamer/Achievements
Download: https://spacedock.info/mod/2422/Achievements
License: GPLv3
Change and Updates from previous Maintainer

Adoption by Linuxgurugamer
Added support for ClickThroughBlocker
Added version file
Added AssemblyVersion.tt
Added InstallChecker
Added support for Toolbarcontroller, using old button-normal/button-highlight for the blizzy toolbar buttons
Replaced stock button with new image, normal and highlight now working
Moved all images into PluginData
Replaced all GetTexture calls with ToolbarControl.LoadImageFromFile to avoid compression
Removed all the SENTAR specific code
Replaced all hard-coded planet specific code with code which gets the available planets from the game itself.
Added new achievement to number of launches (500)
Removed hard-coded techs, now gets list of techs from the game
Removed hard-coded altitude records, gets list from the game
Added toggle to only show earned achievements
Defined CFG file for user-defined achievements
Surface Sample
Orbit Achievement
Landing
Added access to button in flight scene
Removed the installScenaro() method 
Removed old code to convert old storage to new method
Removed updatechecker
Added settings page for:
KSP Skin flag
Achievement timeout value
