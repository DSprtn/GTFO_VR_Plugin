# GTFO_VR_Plugin
### A plugin to add full roomscale virtual reality support to your favorite game!
## Not working for Rundown 6 yet!
GTFO VR now also has a little corner over on the flatscreen to VR modding discord!
https://discord.gg/ZFSCSDe

Active trello board (upcoming features and ongoing work) - https://trello.com/b/zSk7bBMS/gtfovr

E-mail any suggestions for new features or tweaks over to GTFOVRMod@gmail.com

Grab the newest version from here! https://github.com/DSprtn/GTFO_VR_Plugin/releases

Try to get used to the game in Non-VR mode first before using! 

## Install Instructions: 

	1. Download the first-install zip archive from https://github.com/DSprtn/GTFO_VR_Plugin/releases
		This contains the all the files needed to install the plugin.
		
	2. Locate your GTFO folder using the steam library 
		Right click GTFO in your Steam library, go into Properties->Local Files->Browse
		
	3. Copy all the files in the first-install zip archive to the root directory of GTFO, 
	not inside the GTFO_data folder.
	
	5. Ensure the Steam option "Use Desktop Game Theatre while SteamVR is active" is unchecked.
		Right click GTFO in your Steam library, go into Properties->General
	
	4. While SteamVR is running, launch the game from the library or from within SteamVR and you're in!
		Ignore the "GTFO does not support VR" warning, just click OK
	
	5. Tweak or setup your controls under SteamVR -> Settings -> Controllers -> 
	Manage Controller Bindings -> GTFO VR
	
	6. Tweak all VR config options inside GTFO game under Settings -> VR Settings
	

## Upgrade Instructions: 
<details>
  <summary>Expand</summary>
	
	If you are upgrading to R5 or have an old Bepinex/GTFO_VR installation, delete it! 
		Removing the bepinex folder should suffice. You'll have to redo your config!
	
	1. Follow the Install Instructions above, replacing any conflicting files 
	with what is in the first-install zip archive.
</details>


## DIY Install Instructions:
<details>
  <summary>Expand</summary>
	
	Consult the installation video here if you're having trouble: 
	https://www.youtube.com/watch?v=EH7CsLs3GlQ (Thanks Alabaster!)
		Video may be out of date, follow at your own risk!
	
 	1. Download and extract BepInEx 6.0.0b363 into your GTFO game folder (SteamLibrary\steamapps\common\GTFO\)
	from https://builds.bepis.io/projects/bepinex_be/363/BepInEx_UnityIL2CPP_x64_086d2f8_6.0.0-be.363.zip
	
		You can also find the GTFO game path by doing the following:
		Right click GTFO in your Steam library, go into Properties->Local Files->Browse
	
	2. Download the latest version of the GTFO_VR plugin from https://github.com/DSprtn/GTFO_VR_Plugin/releases 
	and extract it into the same folder (SteamLibrary\steamapps\common\GTFO\)
	
	3. Download and extract the following archive into Steamapps\Common\GTFO\Bepinex\unity-libs\ 
	https://github.com/DSprtn/MelonLoader/raw/master/BaseLibs/UnityDependencies/_2019.4.21.zip
	
	The order of extracting the archives is important! It should be done in order of the steps.
	
	At this time your directories and files should look more or less like this:
	
	GTFO/
	├── BepInEx/
	|   ├── core/
	│       ├── AssemblyUnhollower.dll
	│       ├── etc.
	|   ├── plugins/
	│       ├── GTFO_VR.dll
	│       ├── Newtonsoft.Json.dll
	│       ├── openvr_api.dll
	│       └── SteamVR_Standalone_IL2CPP.dll
	|   ├── unhollowed/ - Delete this folder if you're having issues!
	|   └── unity-libs/
	│       ├── UnityEngine.AccessbilityModule.dll
	│       ├── etc.
	└── GTFO_DATA/
		├── Plugins/
		│	├── openvr_api.dll
		│	└── etc.
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
	
	8. Tweak all VR config options under Settings -> VR Settings
	
	
	To enable tracking or change the angle at which the weapons are being held you must set an action pose 
	within the binding menu. Tip seems to be the most intuitive. Remember to set all required actions or the 
	binding won't get saved!
</details>
	
## Usage notes:

#### CONFIGURATION 
	
	There are configurable options for:
	-	Left handed mode 
	- 	Snap turn - Amount, smooth turn
	-  	Radial menus - Extra text info, quickswitch weapon to last used 
	-	Rendering tweaks - Post-processing, resolution and other performance tweaks/hacks
	- 	Floor height offset (for seated mode and height adjustment!)
	-	Movement vignette - Toggle, intensity
	- 	Shooting haptics - Toggle, strength
	- 	IRL Crouching - Toggle, height tweak
	-	Laser pointer - Toggle, color
	-	Hammer - Visual charge indicator, 'Old hammer' (auto-swing with animations)
	-	Watch - Scale, color, number display for ammo
	- 	Holographic ammo display - Toggle
	-	Two handed aiming - Toggle, always on mode
	-	No motion controller mode (You can play with a gamepad!)
	-	etc. - Have a look in the menu!

	They can be found in-game under Settings -> VR Settings and/or in a config file which is created 
	after starting the game at least once with the VR mod installed. 
	The config file can be found under "GTFO\BepInEx\config\com.Spartan.GTFO_VR_Plugin.cfg"

#### PERFORMANCE

	Lower your render resolution (under Settings->VR Settings) and the quality of the in-game settings! 
	GTFO VR is a BIG resource hog!
	
	You can disable post processing effects like bloom or eye adaptation in the config to 
	gain a good bit of performance.
	
	If you don't mind small artifacting on lights and fog in exchange for a bit of extra performance
	set light rendering mode to '2' or '3' in GTFO\BepInEx\config\com.Spartan.GTFO_VR_Plugin.cfg
	
	There is now also a setting to alternate rendering of lights and shadows per eye, per frame. This WILL
	look janky as all hell if you move your head around fast but it will boost performance greatly. I would
	only recommend using this if nothing else works to make the game run in VR for you.
	
### MISC IN-GAME ACTIONS

#### RADIAL MENUS

	Radial menus have been added for selecting watch modes/weapons. There are also config options for how much
	text info should be shown and a quickswitch functionality. If you open the menu and close it immediately 
	with it enabled, you will switch to the last weapon you used (if it's available.) 
	
	Remember to bind the menus if they aren't bound in your current input config!

#### HAMMER

	You have to charge the hammer just like in non-VR GTFO. You can only hit with it while it's charging/charged.
	Haptics and sound indicate the charging level. A flash of light indicates the hammer is 50% charged, and a bigger 
	one indicates it's fully charged.
	
	The flash of light and haptics can be turned off.
	There is also a config option to use the 'old' VR hammer which swings by itself.


#### AIMING

	Double handed aiming is triggered by the proximity of your offhand (the one without a weapon) to
	the weapon grip. Bring your offhand to the gun's grip as you would IRL and you will enable it. 
	Bring your offhand far enough away and you will disable it. 
	This can be tweaked/disabled in the config.
	
#### WATCH
	
	The watch can be toggled between inventory, chat and objectives with the ToggleWatchMode action.
	There is also a radial menu bind that allows you switch between all the modes freely and
	also allows you to type in chat.
	
	The inventory watch UI is as follows ---
	
![image](https://user-images.githubusercontent.com/11588107/115968678-47210f80-a539-11eb-9551-e7c274c28f34.png)

	
	The top 5 bars represent each of the inventory slots.
	Each bar is a mag of ammo, 20% of your tool ammo or a single use of a consumable item. 
	
	On the bottom left there are bars for HP, and infection. Infection is invisible until you're
	actually infected.
	
	On the bottom right you can see the ammo in your current mag. Each block is a bullet.
	There is a config setting to change the ammo to a number display.
	
	Menu and map UI is controlled by the 'movement' action or pointing the controller. 
	'Crouch' and 'ToggleWatchMode' re-orient the overlay and the 'fire' action interacts with the buttons.
	
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
	Works in multiplayer, with others not needing any mods!

	Everything is fully configurable in-game and in an external config. 
	(If you're missing any options, let me know!)
	
	Full SteamVR input binding support, you can play with all VR controllers, provided they have enough buttons 
	for all the actions in GTFO. Most in-game actions are supported, if you're missing any let me know!
	
	Full motion controller based aiming (including fancy laserpointer)
	
	Customized user interface for VR - 3D UI, watch, radial menus, etc.
	
	Main menu, map UI and terminal working correctly in VR
	
	Support for single or double handed aiming, gun stocks
	
	Haptics
	
	Snap turn

	Customized melee mechanics for VR
	
## Known issues: 
	Joining a game in progress can result in a black screen while still being able to move/shoot:
		Open/close your map a dozen times or so, this can bring the game back.

	SteamVR games crash often on v28 of Oculus. There's a workaround in the config that should
	reduce crashing with some small side effects (Oculus Crash workaround.)

	If a crash occurs send me the output log from the path given below (before starting another game!):
	SteamApps/GTFO/Bepinex/LogOutput.log
	

## Want to contribute?

Join the discord and send some feedback my way!
Alternatively, e-mail any suggestions for new features or tweaks over to GTFOVRMod@gmail.com
Open up tickets for any issues you find over here on github, or mention them on the discord.
