#Information
romajiconvert is a tool to generate (very loose) search tags for a song database that contains titles, artists and anime names with mixed ASCII/kana/kanji. The aim is for the user to be able to search for a song in romaji, and receive listings for songs that have kana and kanji titles as well.

![](https://my.mixtape.moe/qgspiq.png)

#Installation

Binaries for this project are located in the Github releases page.

1. Download binaries for this. 
2. Download binaries for kakasi and extract it to `C:\kakasi`, so that the executable is at `C:\kakasi\bin\kakasi.exe`. Unfortunately, this location limitation is part of kakasi and not on my end.
3. Download your song library metadata and stick it in the required format, naming it `songs.json` and putting it in the same directory as `romajiconvert.exe`.
4. Run `romajiconvert.exe /generate` to generate the metadata with output tags.
5. You can now use the javascript searcher, or test it directly with `romajiconvert.exe /search [pathToOutput.json]`. `pathToOutput.json` defaults to `output.json`, so running `romajiconvert.exe /search` is fine too. 


#Input song metadata format

The input song metadata file should be in the following json format:

```
{
  "songs": [
    {
      "id": 1,
      "artist": "artistName",
      "title": "titleName",
      "anime": "animeName"      
    },
    {
      "id": 2,
      "artist": "artistName",
      "title": "titleName",
      "anime": "animeName"  
    },
    {
      "id": 3,
      "artist": "artistName",
      "title": "titleName",
      "anime": "animeName"  
    }
  ]
}
```

Id's between songs must be unique. 

#Output metadata format
The output format is the same as the input format, except each song will now also have a `tag` field that is the romaji of the title, artist and anime, consisting of only letters and numbers (no punctuation, unicode or spaces). 
