import shutil
import sys
import os
from pathlib import Path
from argparse import ArgumentParser


dependency_targets = ["Newtonsoft.Json.dll", "openvr_api.dll", "Bhaptics.Tact.dll", "SteamVR_Standalone_IL2CPP.dll", "ForceTubeVR_API_x64.dll"]
dependency_dir_targets = ["bhaptics-patterns", "protubeHaptics"]
plugin_targets = ["GTFO_VR.dll"]

gtfo_data_plugin_targets = ["openvr_api.dll"]
gtfo_data_dir_targets = ["StreamingAssets"]

readme_target = ["README.MD"]
changelog_target = ["Changelog.txt"]


def copy_files(list, source: Path, dest: Path, critical=True):
    print("Copying " + str(list) + " from\n " + str(source) + " to \n" + str(dest))
    for item in list:
        if((source/item).exists()):
            shutil.copy(str(source / item), str(dest / item))
        else:
            if(critical):
                sys.exit("Could not find target " + str(item) + ", build failed!")
            print("Warning: Could not find: " + str(item))
    print("Done...!")

def copy_dirs(list, source: Path, dest: Path):
    print("Copying " + str(list) + " from\n " + str(source) + " to \n" + str(dest))
    for dir in list:
        source_dir = source / dir
        dest_dir = dest / dir
        if dest_dir.exists():
            shutil.rmtree(dest_dir)
        shutil.copytree(str(source_dir), str(dest_dir))
        print("Done...!")

def copy_plugins_dir():
    staging_plugins_dir = staging_bepinex_dir / "plugins"
    staging_plugins_dir.mkdir(exist_ok=True)

    copy_files(plugin_targets, built_plugin_dir, staging_plugins_dir)
    copy_files(dependency_targets, release_dependencies_lib_dir, staging_plugins_dir)
    copy_dirs(dependency_dir_targets, release_dependencies_lib_dir, staging_plugins_dir)


def copy_gtfo_data_dir():
    gtfo_data_staging_dir = staging_dir / "GTFO_Data"
    gtfo_data_staging_dir.mkdir(exist_ok=True)

    gtfo_data_plugins_staging_dir = gtfo_data_staging_dir / "Plugins"
    gtfo_data_plugins_staging_dir.mkdir(exist_ok=True)

    copy_files(gtfo_data_plugin_targets, release_dependencies_lib_dir, gtfo_data_plugins_staging_dir)
    copy_dirs(gtfo_data_dir_targets, release_dependencies_dir, gtfo_data_staging_dir)

def copy_text_files():
    readme_path = current_dir / "README.md"
    readme_staging_path = staging_dir / "README.md"

    copy_files(readme_target, current_dir, staging_dir)
    copy_files(changelog_target, release_dependencies_dir, staging_dir, critical=False)


def find_version():
    version_file = current_dir / "GTFO_VR" / "Core" / "GTFO_VR_Plugin.cs"
    print("Trying to get version from: " + str(version_file))
    if not version_file.exists():
        sys.exit("Could not find version file!")
    version_cs = open(str(version_file), 'r', encoding='cp932', errors='ignore')
    lines = version_cs.readlines()
    for line in lines:
        if "VERSION =" in line:
            result = line.split('"')[1]
            return result
    version_cs.close()

print("Running release tool...")

parser = ArgumentParser()

parser.add_argument("-v", "--version", dest="version",
                    help="Create archive with given version")

parser.add_argument('-k', '--keep', dest='keep', action='store_true', default=False,
                    help="Keep staging folder for debugging")

args = parser.parse_args()

current_dir = Path(os.path.dirname(os.path.abspath(__file__)))

built_plugin_dir = current_dir / Path("GTFO_VR/bin/Release/")

if not (built_plugin_dir / "GTFO_VR.dll").exists():
    os.exit("No built GTFO_VR plugin version found in " + str(built_plugin_path) + " ! Build GTFO_VR in release mode"
                                                                                   "before trying to create this package!")

print("Found plugin dir...")

release_dependencies_dir = current_dir / "Release_Dependencies"
release_dependencies_lib_dir = release_dependencies_dir / "libs"

staging_dir = Path(current_dir / "Staging")
print("Setting up stating directory at - " + str(staging_dir))
staging_dir.mkdir(exist_ok=True)

staging_bepinex_dir = (staging_dir / "BepInEx")
staging_bepinex_dir.mkdir(exist_ok=True)

copy_plugins_dir()
copy_gtfo_data_dir()
copy_text_files()

if args.version is None:
    version = find_version()
    version = version.replace('.','_')
else:
    version = args.version
print("Got version " + str(version))

archive_name = "GTFO_VR_Release_" + version
shutil.make_archive(archive_name, 'zip', str(staging_dir))

print("Created archive " + str(archive_name))

if not args.keep:
    if staging_dir.exists():
        shutil.rmtree(staging_dir)