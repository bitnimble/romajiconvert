#Information
RomajiConvert is a helper library to generate romaji strings from mixed kana/kanji/english input. It was originally written to generate search tags for a song database that contains titles, artists and anime names with mixed text. The aim is for the user to be able to search for a song in romaji, and receive listings for songs that have kana and kanji titles as well.

![](https://my.mixtape.moe/qgspiq.png)

#Installation

1. Download this project and compile it. 
2. [Download MeCab](http://taku910.github.io/mecab/#download) and install it. The link is next to the (current) "mecab-0.996.exe" text under the "Binary package for MS-Windows" heading.
3. Use the library. API is below. 

# API

The namespace to include is `RomajiConvert`, which will expose the `Romaji` class.

## Romaji class

| Method | Parameters | Returns | Description |
|--------|------------|---------|-------------|
| Constructor | - | `Romaji` | Initialises the converter with the default custom tags (see below for a description of custom tags). The default custom tags is a common set of borrowed English words (e.g. `"ハンバーガー" -> "hamburger"` instead of `"hanbaagaa"`) |
| Constructor | `Dictionary<string, string> customTags` | `Romaji` | Initialises the converter with the specified custom tags. Custom tags is a `Dictionary` mapping kana to English words; they are replaced inline but only if they are isolated words surrounded by non-kana text. For example, with the custom tag `"メリー" -> "Mary"`, "私はメリー" would turn into "watashi wa mary", but "メリーランドに行こう" would _not_ convert it as "メリー" is not the only kana in that kana block, resulting in "meriirando ni ikou". |
| Convert | `string input` | `string romaji` | Converts the specified string to romaji, taking into account the custom tags. |
