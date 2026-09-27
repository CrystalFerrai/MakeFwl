# MakeFwl

A simple standalone cli program to create Valheim world saves with specific seeds. Intended for people running dedicated servers who want to use specific world seeds but do not wish to use the game client to generate the files.

## About World Saves

A Valheim world save consists of a directory containing multiple files. A new world starts with a single metadata file (_main.0.fwl2). This file contains the details needed to generate a new world, including the world name, the world generator seed, a randomly generated unique id and optional world modifiers.

Only a metadata file is required to start a world. Other files will be created by the game when the world loads and when it saves. MakeFwl only creates the folder with the metadata file.

## Installation

First, you will need to install .NET Runtime 9.0 x64 if you do not already have it. You can find a download from Microsoft for your platform [here](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).

You can either build from source or grab a [prebuilt release](https://github.com/CrystalFerrai/MakeFwl/releases). Releases are packaged for Windows and Linux.

MakeFwl is a standalone CLI application that does not come with an installer. Simply extract the files somewhere and run it as desired.

## Usage

Pass in a world name on the command line, and it will output a new world save folder with a metadata file in the current directory. On Windows, run `MakeFwl.exe` followed by the arguments. On Linux, run `dotnet MakeFwl.dll` followed by the arguments. To see additional options, run the program from a command line without any arguments.

```
  MakeFwl [world_name] [[seed]] [[-m path]] [[-o path]]

    world_name  The name of the world to generate. 5-20 characters.

    seed        (optional) The random seed from which to generate the world.
                1-10 characters. If omitted, will use random value.

    -m path     (optional) Modifiers file path. If omitted, will default settings.
                See modifiers.example.txt for an example of a modifiers file.

    -o path     (optional) Output directory. If omitted, will use current directory.
```

### World Modifiers

If you want to include world modifiers in the generated meatadata (fwl) file, you can create a file defining the desired modifiers and pass it into MakeFwl using the `-m` option.

First, make a copy of the file `modifiers.example.txt` that is included with the release. Then edit the new file in a text editor to configure the modifiers you want. The file contains comments explaining how to do the configuration.

Once the file is ready, save it and pass it to MakeFwl along with other desired parameters.
