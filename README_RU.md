# SupercowTexturesTool
STT - это WinForms программа, основанная на .Net Framework 3.5 и созданная для импорта/экспорта текстур из игры Супер Корова.

## Использование

### Обозначения
- JPGA - jpg изображение с вшитым силуэтом, который имитирует прозрачность. В игре подобные текстуры имеют `_a_` в начале названия. Используется для большинства спрайтов в игре.
- TGA - обычный, но довольно старый формат изображения. Может быть альтернативой JPGA, но чаще используется игрой отдельно.
- BJPG - jpg изображение с чёрным фоном, который должен быть прозрачным. Может быть заменён на JPGA, но игрой это ни разу не используется. Используется крайне редко и только для частиц или типа того.

### Функции
- Может конвертировать jpga в png и обратно
- Может конвертировать tga в png и обратно
- Может конвертировать bjpg в png
- Может конвертировать сразу несколько файлов
- Поддерживает перетаскивание

### Установка
Просто скачайте последнюю версию со [страницы версий](https://github.com/Creepobot/SupercowTexturesTool/releases) и запустите её.

# Исходный код
Это не полный исходный код STT, потому что это очень большая куча дерьма и несмешных шуток. Этот код тоже довольно плох, но является важнейшей частью программы.

## Использование
- Установите [Converter.cs](Converter.cs) и [UsefulFuncs.cs](UsefulFuncs.cs) в ваш проект.
- Также вы должны скачать [TGASharpLib.cs](https://github.com/ALEXGREENALEX/TGASharpLib/blob/a6a881351408f075909c813ac74cedad8e613726/TGASharpLib/TGASharpLib.cs) из [репозитория TGASharpLib](https://github.com/ALEXGREENALEX/TGASharpLib/), она обязательно нужна для функций `PngToTga()` и `TgaToPng()`.
- Ещё вы можете скачать [JpegEncoderCore](https://github.com/b9chris/ArpanJpegEncoder/tree/master/JpegEncoderCore) из [репозитория ArpanJpegEncoder](https://github.com/b9chris/ArpanJpegEncoder), но она немного сломана и не сильно важна для конвертации. Она используется только в функции `PngToJpga()` и просто потому что встроенный кодировщик jpg из Visual Studio довольно плох.
  > Вы можете не использовать эту библиотеку, просто удалив/закомментировав код в регионе "ArpanJpegEncoder Code" и разкомментировав код в регионе "Unused Code"

В своём коде напишите что-нибудь вроде
```
string outputfilepath = Converter.Convert(inputfilepath, outputfolderpath, fromto);
```
"fromto" - костыль, который образовался потому что я использую два ComboBox'а и это записано как
```
string fromto = combobox1.Text + combobox2.Text;
```
Хз, мне удобно.

### Используйте этот код только под лицензией CC BY-NC-SA, пожалуйста

# Связь

### Для кого эта программа была создана
- Дискорд сервер [Super Cow Cowmoonity](https://discord.com/invite/JzCvwh5) (RUS)

### Если есть вопросы или пожелания насчёт кода
- Дискорд - `Creepobot#9299`


## README на других языках
[ENG](https://github.com/Creepobot/SupercowTexturesTool/blob/main/README.md)
