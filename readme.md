# MakeFwl
A simple standalone cli program to create Valheim world metadata files with specific seeds. Intended for people running dedicated servers and want to use specific world seeds but do not wish to use the game client to generate the files.

## About World Files
A Valheim world consists of two files. There is the metadata (.fwl) and the save data (.db). The metadata is a very small file containing the name of the world, the world generator seed, and a randomly generated unique id. This file never changes once created. The save data contains the state of the world and changes as players play the game.

Only a metadata file is required to start a world. The save data will be created by the game once the world loads. MakeFwl only creates the metadata file.

## Installation
You can either build from source or grab a [prebuilt release](https://github.com/CrystalFerrai/MakeFwl/releases). Releases are only built for Windows. Other platforms are untested.

MakeFwl is a standalone CLI application that does not come with an installer. Simply extract the executable somewhere and run it as desired.

## Usage
Run the program from a command line without any parameters to see the usage.

```
MakeFwl [world_name] [[seed]] [[-o path]]

    world_name  The name of the world to generate. 5-20 characters.

    seed        (optional) The random seed from which to generate the world.
                1-10 characters. If ommitted, will use random value.

    -o path     (optional) Output file path. If omitted, will use name of world
                as file name and place in current directory.
```