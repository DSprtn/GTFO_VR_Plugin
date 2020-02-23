# GTFO_VR_Plugin
A BepInEx GTFO plugin to make GTFO in Virtual Reality work almost as well as Subnautica VR!

Active trello board - https://trello.com/b/zSk7bBMS/gtfovr

Requires a BepInEx installation - https://github.com/BepInEx/BepInEx/releases 

Grab the newest version from here! https://github.com/DSprtn/GTFO_VR_Plugin/releases

Try to get used to the game in Non-VR mode first before using! 



## Installation: 

 	Download and extract BepInEx into your GTFO game folder (SteamLibrary\steamapps\common\GTFO\) 
	from https://github.com/BepInEx/BepInEx/releases
	Follow the instruction on BepInEx's github, launch the game once
	Download the latest version of the plugin from https://github.com/DSprtn/GTFO_VR_Plugin/releases and 
	extract it into the same folder (SteamLibrary\steamapps\common\GTFO\)
	Make sure 'use desktop game theatre' is off in the properties of GTFO in the steam library 
	(or in general in settings) 
	Start SteamVR
	Launch the game from within the library or however you like, and you're in!
	Menu and map UI is controlled by the 'movement', 'fire' and 'interact' actions
	For now the in-game UI is a bit janky so you can toggle it by using the 'aim' action

	If your controllers don't do anything in-game you might have to download an input scheme 
	from the workshop if anyone made one, or create one yourself (use the 'old binding UI' menu 
	and set action pose to 'tip'! (or whichever setting you prefer))

	Note: When you save your binding SteamVR might not grab the latest action poses, 
	in that cases just create another binding, 
	switch to that one and switch back and it will update correctly.
	
	There are configurable options for left handed mode, no VR controller mode and IRL crouching detection in 
	GTFO\BepInEx\config\com.github.dsprtn.gtfovr.cfg
	This config is made when you start up the VR mod for the first time

### Usage notes:

	Lower your render resolution and in-game settings, GTFO VR is a BIG resource hog!
	Remember to bind openmenu and openmap too! They work in-game correctly as you'd expect.
	You can reorient the menu and map UI with the crouch input action
	Using the 'aim' action toggles UI, it doesn't do anything else! --- I'll change this after the in-game UI update!
		
	If you'd like to use SteamVR desktop set the game to windowed mode (unless you have multiple monitors) 
	If you 'tab out' with SteamVR desktop remember to tab back in (by clicking on the game icon in the taskbar) 
	to get game sound

	Terminals only work with VR controllers, so not with gamepads
	Shortcuts exist under uppercase letters on the keyboard. 
	If you'd like to have custom bindable shortcuts let me know! 
	
	The shortcuts return the following:
	
		case ("L"):
				return "LIST ";
		case ("Q"):
				return "QUERY ";
		case ("P"):
				return "PING ";
		case ("A"):
				return "AMMOPACK";
		case ("T"):
				return "TOOL_REFILL";
		case ("M"):
				return "MEDIPACK";
		case ("R"):
				return "REACTOR";
		case ("V"):
				return "REACTOR_VERIFY ";
				
### Features:
	Mostly correctly working VR UI and gameplay (also works in multiplayer, with others not having the mod!)
	Full SteamVR_Input binding support, you can play with all VR controllers, 
	provided they have enough buttons for all the actions in GTFO!
	Full VR controller based aiming (including fancy laserpointer)
	Main menu, map UI and terminal working correctly in VR
	
### Known issues: 

	Oculus integration isn't working well yet (disable controllers in config to play anyway)! 
	--- in GTFO\BepInEx\config\com.github.dsprtn.gtfovr.cfg
	In-game HUD is crappy looking --- intel, objectives, ammo info (except in map) or not positioned correctly  
	--- enemy markings, teammate name/stats (except in map)
	Hacking tool minigame isn't rendered correctly for VR

## Want to contribute?

Give me a holler on Discord and I'll bring you up to speed - Spartan#8541 
