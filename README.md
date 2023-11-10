# RPA

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A universal console app to run batch scripts with TagUI scripts.

## Installation

1. Follow the instruction to download [TagUI](https://tagui.readthedocs.io/en/latest/setup.html) (and [JDK](https://www.oracle.com/java/technologies/downloads/))
2. Clone this console app to your computer
3. Build your first Script base with the instruction below
4. Build your application using "dotnet build"
5. Put "dotnet run /full/path/to/your/script/base" to where you need

## Script Base

Script base is a folder containing at least one main.bat and one config. 
This is the flow of your RPA task. 
You can call TagUI scripts within the bat file.

The location of your script base does not matter.
When called, the content of the script base is copied to a temporary folder "Process".
The console app runs the content in the temporary folder instead of your script base.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
