using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace romajiconvert
{
	class Program
	{
		static void PrintUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("romajiconvert.exe [method] [library path]");
			Console.WriteLine("Available methods:\n\t/generate:\tGenerates a song metadata library with all the romaji tags included.\n\t/search:\tProvides a console to search through a song metadata library through romaji.");
			Console.WriteLine("[library path] must be supplied if using /search.");
		}

		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Search("output.json");
			}
			else if (args.Length > 2)
				PrintUsage();
			else if (args[0] == "/generate")
			{
				Console.WriteLine("Loading songs.json...");
				//This input file is just directly retrieved via the songs API
				Console.WriteLine("Processing songs...");
				SongList list = SearchTagGenerator.ProcessFile("songs.json");

				Console.WriteLine("\n\nFinished processing! Writing output...");
				File.WriteAllText("output.json", JsonHelper.Stringify(list));
				Console.WriteLine("Done. Press enter to exit...");
				Console.ReadLine();
			}
			else if (args[0] == "/search")
			{
				Search(args.Length == 1 ? "output.json" : args[1]);
			}
			else
				PrintUsage();
		}
		
		//Strip out everything other than letters and numbers and change to lowercase
		static string Preprocess(string input)
		{
			List<char> result = new List<char>();
			for (int i = 0; i < input.Length; i++)
			{
				if ((input[i] >= '0' && input[i] <= '9') || (input[i] >= 'a' && input[i] <= 'z'))
				{
					result.Add(input[i]);
				}
				else if (input[i] >= 'A' && input[i] <= 'Z')
				{
					result.Add((char)(input[i] | 32));
				}
			}

			return string.Join("", result);
		}

		static void Search(string inputFile)
		{
			if (!File.Exists(inputFile))
			{
				Console.WriteLine("Could not find input file \"" + inputFile + "\"");
				return;
			}
			Console.OutputEncoding = Encoding.UTF8;
			Console.WriteLine("Type to search: ");
			SongList list = JsonHelper.Parse<SongList>(File.ReadAllText(inputFile));
			Dictionary<int, Song> songs = list.songs.ToDictionary(song => song.id);

			while (true)
			{
				string input = Console.ReadLine();
				input = Preprocess(input);
				Console.WriteLine();

				List<int> indices = new List<int>();
				for (int i = 0; i < list.songs.Length; i++)
				{
					Song song = list.songs[i];
					if (song.tag.Contains(input))
					{
						string animeTitle = song.anime.Trim() == "" ? "" : " (" + song.anime + ")";
						Console.WriteLine(song.id + " - " + song.title + " - " + song.artist + animeTitle);
					}
				}
				Console.WriteLine("\n");
			}
		}
	}
}
