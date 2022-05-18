# Hunt MMR Reader

## What is this?

I made this tool for the game **Hunt: Showdown**. In this game you have some kind of rank that is represented by stars. The ranking goes from 1 to 6 stars, where 1 is the lowest and 6 is the best. These stars are bound to a number, the MMR.\
The specific rank ranges are as follows:
| MMR range | Stars |
|:---------:|:-----:|
|   0-1999  |   ★   |
| 2000-2299 |   ★★   |
| 2300-2599 |   ★★★   |
| 2600-2749 |   ★★★★   |
| 2750-2999 |   ★★★★★   |
| >= 3000   |   ★★★★★★   |

This tool let's you read out your exact MMR as a number.

## How do I use this program?

1. Download the [newest release version](https://github.com/slimDebug/HuntMmrReader/releases/latest) of the unt MMR Reader. Refer to [Downloading](#Downloading) if you are unsure what version to download.
2. Extract the zip whereever you like. I recommend using [7-zip](https://www.7-zip.org/), but other utilities should work without problems as well.
3. Open the **_HuntMmrReader.exe_** file.
4. Click _Settings_ in the menu tab and click _Edit Path..._.![grafik](https://user-images.githubusercontent.com/66317138/169058102-f069f162-1813-4978-b266-dab1a82ea01c.png)

5. Select the _attributes.xml_ file from your **Hunt: Showdown** installation folder. The file should be located in **_SteamLibrary\steamapps\common\Hunt Showdown\user\profiles\default\attributes.xml_**
6. Hit the open button.
7. You should see something like this:![example_screenshot](https://user-images.githubusercontent.com/66317138/169058676-93fed089-2124-45ba-8d7f-d0b265660542.PNG)
8. Congratulations. You can see the MMR of all teams/players of you
r most recent match (_luckily for us this information is saved locally_).

## Reporting errors

If an error occurs and you would like to report it, please oben a GitHub issue or on the [steam guide](https://steamcommunity.com/sharedfiles/filedetails/?id=2806779825) page.\
Errors can be seen in the marked box here:\
![grafik](https://user-images.githubusercontent.com/66317138/169061726-cc95afd3-db82-4931-8a2d-347f9fbffdca.png)
It's important that you also provide a Stack trace with the error you're reporting. You can obtain a Stack trace by right clicking the error and pressing the _Copy Stack trace to clipboard_ option.

## Downloading

Hunt MMR Reader is available in two variants, but you're interested in the package that matches your architecture. If you're using `64`-bit `Win`dows, then you want `win-x64` package. If you're using `32-bit Windows`, then you want `win-x86` package. The packages can be found [here](https://github.com/slimDebug/HuntMmrReader/releases/latest).

## To be noted

- The program automatically detects if the file was modified and reads it on changes.
- Hitting **F5** forces the program to read the file.

## Can I get VAC ban or game ban for using this?

This program just reads a file. You can achieve the same with any text editor. I would say it's very unlikely to get banned for this, but at the end it is up to the **Hunt: Showdown developers** to decide who should get banned. **I'm not liable for any damage caused by this software and I'm not guaranteeing anything. In the end it is up to you and it's your risk to use this software.**
