# Futatabi Downhill

This is a small C# program to add the "downhill" (inbound) layout to the
[90's Golden Drift Spot Project](https://90sgdsp.gumroad.com/) track
[Mt. Futatabi](https://www.racedepartment.com/downloads/90s-golden-drift-spot-project-7-mountain-futatabi-%E5%86%8D%E5%BA%A6%E5%B1%B1.22990/)
for [Assetto Corsa](https://store.steampowered.com/app/244210/Assetto_Corsa/).

## How to Use

First, download the original track from RaceDepartment [here](https://www.racedepartment.com/downloads/90s-golden-drift-spot-project-7-mountain-futatabi-%E5%86%8D%E5%BA%A6%E5%B1%B1.22990/).
Install it as usual, either via [Content Manager](https://assettocorsa.club/content-manager.html) or manually.

Once that's done, download the program from the [releases](https://github.com/davidgarland/FutatabiDownhill/releases) page. Run the .exe.

If your Assetto Corsa install is at `C:\Program Files (x86)\Steam\steamapps\common\assettocorsa`, the program will automatically find futatabi and patch it.
Otherwise, you will be prompted to give the path to your assettocorsa install.

## How It Works

For the most part, this program just moves around files and injects some custom ones-- if it only had to do that, I would have
just gone ahead and written this as a powershell script instead.

But because we don't want to distribute the `.kn5` for Futatabi Downhill, instead we use an [Octodiff](https://github.com/OctopusDeploy/Octodiff)
delta file. This delta file basically says what changes need to be made to the normal `futatabi.kn5` to generate `futatabi downhill.kn5`.

## But Why?

We aren't allowed to redistribute the track (because it takes away traffic from the original download), and 90sGDSP has no interest in adding the downhill
layout to their upload of the track. This program solves that problem, and as a bonus takes even less network usage to download assuming you already have the original.

To give you a sense of how efficient this solution is:
- `FutatabiDownhill.exe` is 104MB.
- `futatabi downhill.kn5` is 270MB.
- `futatabi.zip` (the original one) is 215MB.

The program is made open source for those who are curious how it works. Everything fits snugly into `Program.cs`, and can be
understood without much difficulty.
