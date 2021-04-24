# GTFO_VR_Plugin
### A plugin to add full roomscale virtual reality support to your favorite game!

GTFO VR now also has a little corner over on the flatscreen to VR modding discord!
https://discord.gg/ZFSCSDe

Active trello board (upcoming features and ongoing work) - https://trello.com/b/zSk7bBMS/gtfovr

E-mail any suggestions for new features or tweaks over to GTFOVRMod@gmail.com

Grab the newest version from here! https://github.com/DSprtn/GTFO_VR_Plugin/releases

Try to get used to the game in Non-VR mode first before using! 

## Installation: 

	Consult the installation video here if you're having trouble: 
	https://www.youtube.com/watch?v=EH7CsLs3GlQ (Thanks Alabaster!)

	If you have an old Bepinex/GTFO_VR installation, delete it! Removing the bepinex folder should suffice.
	
 	1. Download and extract BepInEx 6.0.0 into your GTFO game folder (SteamLibrary\steamapps\common\GTFO\)
	from https://builds.bepis.io/projects/bepinex_be/350/BepInEx_UnityIL2CPP_x64_07a69cf_6.0.0-be.350.zip
	
	You can also find the GTFO game path by doing the following:
	Right click GTFO in your Steam library, go into Properties->Local Files->Browse
	
	2. Download the latest version of the GTFO_VR plugin from https://github.com/DSprtn/GTFO_VR_Plugin/releases 
	(0.8.1 or later) and extract it into the same folder (SteamLibrary\steamapps\common\GTFO\)
	
	3. Download and extract the following archive into Steamapps\Common\GTFO\Bepinex\unity-libs\ 
	https://github.com/LavaGang/MelonLoader/raw/master/BaseLibs/UnityDependencies/2019.4.1.zip
	
	The order of extracting the archives is important! It should be done in order of the steps.
	
	At this time your directories and files should look more or less like this:
	
	GTFO/
	├── BepInEx/
	│   ├── core/
	│       ├── AssemblyUnhollower.dll
	|       ├── etc.
	│   ├── plugins/
	│       ├── GTFO_VR.dll
	│       ├── Newtonsoft.Json.dll
	│       ├── openvr_api.dll
	│       └── SteamVR_Standalone_IL2CPP.dll
	│   ├── unhollowed/ - Delete this folder if you're having issues!
	│   └── unity-libs/
	│       ├── UnityEngine.AccessbilityModule.dll
	│       ├── etc.
	└── GTFO_DATA/
	    └── StreamingAssets/
		├── AssetBundles
		├── vrshaders
		├── vrwatch
		├── SteamVR_Standalone/
		    ├── actions.json
		    ├── etc.
	
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
	the VR mod installed. It can be found under "GTFO\BepInEx\config\com.Spartan.GTFO_VR_Plugin.cfg"

#### PERFORMANCE

	Lower your render resolution in SteamVR and also the quality of the in-game settings. 
	GTFO VR is a BIG resource hog!
	
	If you don't mind small artifacting on lights in exchange for a bit of extra performance
	set light rendering mode to '3' in GTFO\BepInEx\config\com.github.dsprtn.gtfovr.cfg
	
	There is now also a setting to alternate rendering of lights and shadows per eye, per frame. This WILL
	look janky as all hell if you move your head around fast but it will boost performance greatly. I would
	only recommend using this if nothing else works to make the game run in VR for you.
	
#### MISC IN-GAME ACTIONS

	Double handed aiming is triggered by proximity. Bring your hands together on a double handed weapon 
	and you will enable it. Bring them far apart and it will switch back. 
	
	The watch can be toggled between inventory and objectives with the ToggleWatchMode action.
	
	The watch UI is as follows ---
	
![image](https://user-images.githubusercontent.com/11588107/115968696-661fa180-a539-11eb-9c2b-9c988bc89a60.png)

	
	The top 5 bars represent the inventory slots, with ammo percentages (1 bar = 20%.)
	Each bar is a mag of ammo, 20% of your tool ammo or a single use of a consumable item. 
	On the bottom left there are bars for HP, infection and oxygen, they are each 20% a bar and are not visible 
	if they don't have to be at the moment. 
	On the bottom right you can see the ammo in your current mag. Each block is a bullet.
	There is a config setting to change the ammo to a number display.
	
	Remember to bind openmenu and openmap too! They work in-game correctly as you'd expect.
	
	Menu and map UI is controlled by the 'movement' action or pointing the controller. 
	'Crouch' and 'aim' re-orient the overlay and the 'fire' action interacts with the buttons.
	
#### STEAM_VR DESKTOP 
	
	If you'd like to use SteamVR desktop set the game to windowed mode (unless you have multiple monitors) 
	If you 'tab out' with SteamVR desktop remember to tab back in (by clicking on the game icon in the taskbar) 
	to get game sound.
	
	
#### TERMINAL
	
	Shortcuts exist as uppercase letters on the virtual keyboard. To use, simply set the keyboard to uppercase 
	and it will autofill the text based on the uppercase letters you type in.
	
	The shortcuts return the following:
	
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
	for all the actions in GTFO. Most in-game actions are supported, if you're missing any let me know!
	
	Full VR controller based aiming (including fancy laserpointer)
		
	Customized user interface for VR
	
	Main menu, map UI and terminal working correctly in VR
	
	Support for single or double handed aiming, gun stocks
	
	Snap turn
	
	Loads of configurable options (more coming soon and if you're missing any let me know!)
	
## Known issues: 

	The IL2CPP build hasn't been tested thoroughly yet so crashes may occur. If one does occur,
	send me the most recent output logs from the paths given below:
	AppData\LocalLow\10 Chambers Collective\GTFO\$...Netstat
	AND/OR
	GTFO/Bepinex/LogOutput

## Want to contribute?

E-mail any suggestions for new features or tweaks over to GTFOVRMod@gmail.com
Open up tickets for any issues you find over here on github, 
or if you're unfamiliar with Github just send me an e-mail.
