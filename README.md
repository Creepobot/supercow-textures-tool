# SupercowTexturesTool
STT is WinForms application based on .Net Framework 3.5 created for importing/exporting sprites from Supercow game.

## Usage

### Designations
- JPGA - jpg image with an embedded silhouette that simulates transparency. In the game, such textures usually have `_a_` at the beginning of the name. Used for most sprites in the game.
- TGA - common but rather old image format. Can be a replacement for JPGA, but is often used by the game separately.
- BJPG - jpg image with a black background that should become transparent. Can be replaced with JPGA, but this is never used in the game. Used extremely rarely and mainly for particles or smth.

### Features
- Can convert jpga to png and vice versa
- Can convert tga to png and vice versa
- Can convert bjpg to png
- Сan convert multiple files at once
- Supports drag-n-drop

### Installation
Just download latest version from [releases page](https://github.com/Creepobot/SupercowTexturesTool/releases) and run it.

# Source code
This is not the full source code of STT because it is a very big piece of shit and unfunny jokes. This code is also very bad, but it is the most important part of the program.

## Usage
- Install [Converter.cs](Converter.cs) and [UsefulFuncs.cs](UsefulFuncs.cs) to your project.
- Also you have to download [TGASharpLib.cs](https://github.com/ALEXGREENALEX/TGASharpLib/blob/a6a881351408f075909c813ac74cedad8e613726/TGASharpLib/TGASharpLib.cs) from the [TGASharpLib repo](https://github.com/ALEXGREENALEX/TGASharpLib/), it is definitely needed for `PngToTga()` and `TgaToPng()` functions.
- You can also download [JpegEncoderCore](https://github.com/b9chris/ArpanJpegEncoder/tree/master/JpegEncoderCore) from the [ArpanJpegEncoder repo](https://github.com/b9chris/ArpanJpegEncoder), but it is a bit broken and not required for conversion. It is used only in `PngToJpga()` function and simply because Visual Studio's jpg encoder is a bit bad.
  > You can choose not to use this library by simply deleting/commenting out the code in the "ArpanJpegEncoder Code" region and uncommenting the code in the "Unused Code" region

In code write smth like
```
string outputfilepath = Converter.Convert(inputfilepath, outputfolderpath, fromto);
```
"fromto" is a kludge, which was formed because I use two ComboBoxes and it's written as
```
string fromto = combobox1.Text + combobox2.Text;
```
Idc, it works great for me.

### Use this code only under CC BY-NC-SA license please

# Communication

### For whom the program was created
- Discord server [Super Cow Cowmoonity](https://discord.com/invite/JzCvwh5)

### If you have any questions or suggestions about the code
- Discord - `Creepobot#9299`
