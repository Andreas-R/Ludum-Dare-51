# LowRezJam2022

Implemented in Godot 3.4.5

## Setup Godot with C#:

* install newest Godot version (3.4.5): https://godotengine.org/download/windows (download Mono version, otherwise no C# support !!!)
* install newest .NET SDK version: https://dotnet.microsoft.com/en-us/download
* In Godot go to `Editor` > `Editor Settings`. In the Settings navigate to `Mono` > `Editor`. Enable `External Editor` and select VS Code or other preferred Editor.
* In VS Code make sure following extensions are installed:
  * C#
  * C# Tools for Godot
  * Mono Debug
* Setup debugger in VS Code:
  * Open Godot project folder in VS Code.
  * Navigate to "Run and Debug" side tab.
  * Click "create a launch.json file" link and select "C# Godot"
  * In launch.json and tasks.json alter all executables to point to your Godot executable.
  * You now have "Play in Editor" and "Launch" start options
