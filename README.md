# GTFO_VR_Plugin
## A plugin to add full roomscale virtual reality support to your favorite game!


### WARNING: MOD NOT YET WORKING AS OF R4, Il2CPP UPDATE SHOULD BE COMING UP SOON

Active trello board (upcoming features and ongoing work) - https://trello.com/b/zSk7bBMS/gtfovr

E-mail any suggestions for new features or tweaks over to GTFOVRMod@gmail.com

Grab the newest version from here! https://github.com/DSprtn/GTFO_VR_Plugin/releases

Tutorial video for the installation: https://www.youtube.com/watch?v=sZPK1GfqxBE&feature=youtu.be (Thanks F4cepa1m!)

Try to get used to the game in Non-VR mode first before using! 

## Installation: 

 	1. Download and extract BepInEx into your GTFO game folder (SteamLibrary\steamapps\common\GTFO\) 
	from https://github.com/BepInEx/BepInEx/releases
	
	2. Launch the game once and close it
	
	3. Download the latest version of the plugin from https://github.com/DSprtn/GTFO_VR_Plugin/releases and 
	extract it into the same folder (SteamLibrary\steamapps\common\GTFO\)
	
	4. Make sure 'use desktop game theatre' is off in the properties of GTFO in the steam library 
	(or in general steam settings)
	
	5. Start SteamVR
	
	6. Launch the game from within the library or from within SteamVR and you're in!
	(Ignore the "GTFO does not support VR" warning, just click OK)
	
	7. Tweak or setup your controls under SteamVR -> Settings -> Controllers 
	-> Manage Controller Bindings -> GTFO VR
	
	When in doubt, consult the video here https://www.youtube.com/watch?v=sZPK1GfqxBE&feature=youtu.be&t=4m14s
	It is still mostly up to date.
	
	To enable tracking or change the angle at which the weapons are being held you must set an action pose 
	within the binding menu. Tip seems to be the most intuitive. Remember to set all required actions or the 
	binding won't get saved!
	

	
## Usage notes:

#### CONFIGURATION 
	
	There are configurable options for:
	-	Experimental performance tweaks 
	-	Left handed mode 
	-	No VR controller mode 
	- 	IRL Crouching detection 
	-	Two handed aiming (toggleable in-game based on the distance of your hands)
	-	Always aiming two handed, ignoring hand distance
	- 	Snap turn amount
	-	Smooth snap turning
	-	UI Watch scale
	-	Number display for ammo
	- 	Toggle for SteamVR check (so you can play the game without VR without removing anything!) 
	-	VR mod on/off switch (I recommend just using the SteamVR check)

	They can be found in a config file which is created after starting the game at least once with 
	the VR mod installed. It can be found under "GTFO\BepInEx\config\com.github.dsprtn.gtfovr.cfg"

#### PERFORMANCE

	Lower your render resolution in SteamVR and the quality of the in-game settings, GTFO VR is a BIG resource hog!
	
	If you don't mind small artifacting on lights in exchange for a bit of extra performance
	set light rendering mode to '2' in GTFO\BepInEx\config\com.github.dsprtn.gtfovr.cfg
	
#### MISC IN-GAME ACTIONS

	Double handed aiming is triggered by proximity. Bring your hands together on a double handed weapon 
	and you will enable it. Bring them far apart and it will switch back. 
	
	The watch can be toggled between inventory and objectives with the ToggleWatchMode action.
	
	The watch UI is as follows ---
	The top 5 bars represent the inventory slots, with ammo percentages.
	Each bar is a mag of ammo, 20% of your tool ammo or a single use of a consumable item. 
	On the bottom left there are bars for HP, infection and oxygen, they are each 20% and are not visible 
	if they don't have to be at the moment. 
	On the bottom right you can see the ammo in your current mag. Each block is a bullet.
	
	Remember to bind openmenu and openmap too! They work in-game correctly as you'd expect.
	
	Menu and map UI are controlled by aiming and the 'movement', 'crouch' (for re-orienting) and the 'fire' actions.
	
#### STEAM_VR DESKTOP 
	
	
	If you'd like to use SteamVR desktop set the game to windowed mode (unless you have multiple monitors) 
	If you 'tab out' with SteamVR desktop remember to tab back in (by clicking on the game icon in the taskbar) 
	to get game sound.
	
	
#### TERMINAL
	
	Shortcuts exist as uppercase letters on the virtual keyboard. To use, simply set the keyboard to uppercase 
	and it will autofill the text based on the uppercase letters you type in.
	
	The shortcuts return the following:
	
		case ("-"):
			returns "_";	// More accessible, dash is less needed and tilde is hidden away
		case ("~"):
			returns "-";
		case ("L"):
			returns "LIST ";
		case ("Q"):
			returns "QUERY ";
		case ("R"):
			returns "REACTOR";
		case ("H"):
			returns "HELP";
		case ("C"):
			returns "COMMANDS";
		case ("V"):
			returns "REACTOR_VERIFY ";
		case ("P"):
			returns "PING ";
		case ("A"):
			returns "AMMOPACK_";
		case ("T"):
			returns "TOOL_REFILL_";
		case ("M"):
			returns "MEDIPACK_";
		case ("Z"):
			returns "ZONE_";
		case ("U"):
			returns "UPLINK_VERIFY ";
		

	If you'd like to have custom bindable shortcuts let me know! 
				
## Features:
	Works in multiplayer, with others not having to have the mod!
	
	Full SteamVR_Input binding support, you can play with all VR controllers, provided they have enough buttons 
	for all the actions in GTFO. All in-game actions are supported, except for 'undo' in the terminal!
	
	Full VR controller based aiming (including fancy laserpointer)
		
	Fully customized VR UI
	
	Main menu, map UI and terminal working correctly in VR
	
	Support for single or double handed aiming
	
	Snap turn
	
	Loads of configurable options (more coming soon!)
	
## Known issues: 
	
	Nothing left over here! If you do encounter a bug please send me the output log that can be found under 
	AppData\LocalLow\10 Chambers Collective\GTFO\output_log.txt

## Want to contribute?

E-mail any suggestions for new features or tweaks over to GTFOVRMod@gmail.com
Open up tickets for any issues you find over here on github, 
or if you're unfamiliar with it send them to the same e-mail.
