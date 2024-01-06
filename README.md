# GTFO_VR_Plugin
### A plugin to add full roomscale virtual reality support to your favorite game!

GTFO VR now also has a little corner over on the flatscreen to VR modding discord!
https://discord.gg/ZFSCSDe

Active trello board (upcoming features and ongoing work) - https://trello.com/b/zSk7bBMS/gtfovr

E-mail any suggestions for new features or tweaks over to GTFOVRMod@gmail.com

Grab the newest version from here! https://github.com/DSprtn/GTFO_VR_Plugin/releases

Grab newest beta versions from the Discord. Join the beta testers under gt-getting-started and look for builds under gt-test-builds

Try to get used to the game in Non-VR mode first before using this mod! 

#### Recommendation: VRPerfkit
As GTFO is quite a performance-hog, it is recommended to use the VRPerformanceKit to upscale your game resolution. For the time being, a version of VRPerformanceKit is included with GTFO VR as some GTFO-specific fixes had to be made.

#### Recommendation: Wireless VR Solution
If you are using Wireless PCVR to play, and the weapon handling feels 'floaty' or 'unresponsive', try using ALVR (https://github.com/alvr-org/ALVR). Other users report this provides a smoother experience.

## Installation: 

	If you are upgrading to R7/R1:Ext or have an old Bepinex/GTFO_VR installation, delete it! 
	Removing the bepinex folder should suffice. You'll have to redo your config!

 	1. Download BepInEx 6.0.0b670
	from https://builds.bepinex.dev/projects/bepinex_be/670/BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.670%2B42a6727.zip
	
	2. Download the latest version of the GTFO_VR plugin from https://github.com/DSprtn/GTFO_VR_Plugin/releases
	   OR grab one of the beta versions from the discord mentioned above!

	3. Locate your GTFO folder using the steam library
                Right click GTFO in your Steam library, go into Properties->Local Files->Browse

	4. Extract both archives into this folder (SteamLibrary/SteamApps/common/GTFO)

	At this time your directories and files should look more or less like this:
	
	GTFO/
	├── BepInEx/
	|   ├── plugins/
	│       ├── bhaptics-patterns/
	│       ├── GTFO_VR.dll
	│       ├── Newtonsoft.Json.dll
	│       ├── openvr_api.dll
	│       ├── SteamVR_Standalone_IL2CPP.dll
	│       └── etc.
	|   └── etc.
	├── GTFO_DATA/
	|	├── Plugins/
	|	│	├── openvr_api.dll
	|	│	└── etc.
	|    	└── StreamingAssets/
	|		├── AssetBundles
	|		├── vrshaders
	|		├── vrwatch
	|		├── SteamVR_Standalone/
	|			├── actions.json
	|			├── etc.
	├── GTFO.exe
	├── etc.
	
	
	5. Make sure the "Present non-VR Applications on Threater screen upon Launch" setting is be disabled. 
		It can be found under VR settings -> Dashboard.
	
	6. While SteamVR is running, launch the game from the library or from within SteamVR and you're in!
		Ignore the "GTFO does not support VR" warning, just click OK
	
	7. Tweak or setup your controls under SteamVR -> Settings -> Controllers -> 
	   Manage Controller Bindings -> GTFO VR
	
	8. Tweak all VR config options inside GTFO game under Settings -> VR Settings

	9. Lower/disable all graphical options! They either cause bad performance or visual artifacts.
	This includes anti-aliasing under 'Display'
	
	
	To enable tracking or change the angle at which the weapons are being held you must set an action pose 
	within the binding menu. Tip seems to be the most intuitive. Remember to set all required actions or the 
	binding won't get saved!
	
	
## Usage notes:

#### CONFIGURATION 
	
	There are configurable options for:
	-	Left handed mode 
	-	ProtubeVR haptics support
	-	Bhaptics support
	- 	Snap turn - Amount, smooth turn
	-  	Radial menus - Extra text info, quickswitch weapon to last used 
	-	Rendering tweaks - Resolution
	- 	Floor height offset (for seated mode and height adjustment!)
	-	Movement vignette - Toggle, intensity
	- 	Shooting haptics - Toggle, strength
	- 	IRL Crouching - Toggle, height tweak
	-	Laser pointer - Toggle, color
	-	Melee weapons - Visual charge indicator, 'Old melee' (auto-swing with animations)
	-	Watch - Scale, color, number display for ammo
	- 	Holographic ammo display - Toggle
	-	Two handed aiming - Toggle, always on mode
	-	No motion controller mode (You can play with a gamepad!)
	-	etc. - Have a look in the menu!

	They can be found in-game under Settings -> VR Settings and/or in a config file which is created 
	after starting the game at least once with the VR mod installed. 
	The config file can be found under "GTFO\BepInEx\config\com.Spartan.GTFO_VR_Plugin.cfg"

#### PERFORMANCE

	The game is very performance hungry, even moreso this update, so I recommend lowering your in-game settings greatly!
	Turn off SSAO, bloom, subsurface scattering, depth of field, motion blur etc.
	Lower texture resolution, fog resolution and fog diffusion quality. 
	If you're experiencing crashing lower the texture resolution! 
	Use the VR Performance Toolkit!
	
### MISC IN-GAME ACTIONS

#### RADIAL MENUS

	Radial menus have been added for selecting watch modes/weapons. There are also config options for how much
	text info should be shown and a quickswitch functionality. If you open the menu and close it immediately 
	with it enabled, you will switch to the last weapon you used (if it's available.) 
	
	Remember to bind the menus if they aren't bound in your current input config!

#### Melee weapons

	You have to charge any melee weapon just like in non-VR GTFO. You can only hit with it while it's charging/charged.
	Haptics and sound indicate the charging level. A flash of light indicates the weapon is 50% charged, and a bigger 
	one indicates it's fully charged.
	
	The flash of light and haptics can be turned off.
	There is also a config option to use the 'old' VR melee system which swings by itself.


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
	
	A floating keyboard interface will appear alongside shortcuts whenever you interact with a terminal. You can copypaste text 
	from the terminal by clicking on it in the terminal screen. The keyboard will work just like in flatscreen mode.


![Capture](https://user-images.githubusercontent.com/11588107/171913470-7d269bd2-3006-496c-a99b-b58e7c47be64.PNG)

				
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
	
	Support for built-in controller haptics, BHaptics as well as ProtubeVR haptics
	
	Snap turn

	Customized melee mechanics for VR
	
## Known issues: 
	SteamVR games crash often on v28 of Oculus. There's a workaround in the config that should
	reduce crashing with some small side effects (Oculus Crash workaround.)
	
	WMR HMDs may have issues if you're not using the lkg_release of WMR. (In Steam go into Windows Mixed Reality ->Betas to change.)

	The game is very VRAM hungry! If your texture or render resolution settings are too high and you're using too much VRAM your game will be very unstable!
	Changing resolution in-game or even playing some maps might cause crashes unless you lower your texture resolution/game resolution.

	Having dithering on in in-game and fixed foveated rendering in VRPerfomanceToolKit will cause flickering. Turn dithering off!

	Having upscaling on with VRPerfomanceToolKit will cause crashes with some HMDs (Pimax, Quest 2), if you get crashing try to turn this off first.
	
	If a crash occurs send me the output log from the path given below (before starting another game!):
	\Users\$USER\AppData\LocalLow\10 Chambers Collective\GTFO\player.log OR player-prev.log if you already played another game of GTFO.
	

## Want to contribute?

Join the discord and send some feedback my way!
Alternatively, e-mail any suggestions for new features or tweaks over to GTFOVRMod@gmail.com
Open up tickets for any issues you find over here on github, or mention them on the discord.
