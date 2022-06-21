# SupercowTexturesTool
STT is WinForms application based on .Net Framework 3.5 created for importing/exporting sprites in game Supercow.
# Converter
This is not the full source code of STT because it is a very big piece of shit and unfunny jokes. This code is also a piece of shit, but it is the most important part of the program.

## How to use converter?
Download repo, drag to your project... Actually idc how you use this code, but you know, download [TGASharpLib.cs](https://github.com/ALEXGREENALEX/TGASharpLib/blob/a6a881351408f075909c813ac74cedad8e613726/TGASharpLib/TGASharpLib.cs) from the [TGASharpLib repo](https://github.com/ALEXGREENALEX/TGASharpLib/) and [JpegEncoderCore](https://github.com/b9chris/ArpanJpegEncoder/tree/master/JpegEncoderCore) from the [ArpanJpegEncoder repo](https://github.com/b9chris/ArpanJpegEncoder), you need it for a couple of functions.
> The last library I modified in my program as it was a bit broken so good luck with the bugs and missing namespaces lol. 

## So how to use it?
In code write smth like
```
string outputfilepath = Converter.Convert(inputfilepath, outputfolderpath, fromto);
```
"fromto" is a kludge, which was formed because I use two ComboBoxes and it's written as
```
string fromto = combobox1.Text + combobox2.Text;
```
Idc, it works great for me.
