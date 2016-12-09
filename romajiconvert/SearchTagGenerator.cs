using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace romajiconvert
{
	class Reading
	{
		Word[] words;

		public Reading(string sentence)
		{
			words = sentence.Split(new char[] { ' ' }).Select(s => new Word(s)).ToArray();
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < words.Length - 1; i++)
			{
				builder.Append(words[i].readings[0]);
				//builder.Append(' ');
			}
			//Just take the first for now -- graph searching will be implemented later...
			builder.Append(words[words.Length - 1].readings[0]);
			return builder.ToString();
		}
	}

	class Word
	{
		public string[] readings;

		public Word(string[] readings)
		{
			this.readings = readings;
		}

		public Word(string input)
		{
			if (input.StartsWith("{") && input.EndsWith("}"))
			{
				readings = input.Substring(1, input.Length - 2)
							.Split(new char[] { '|' })
							.ToArray();
				ApplyFixes();
			}
			else
				readings = new string[1] { input };
		}

		private void ApplyFixes()
		{
			if (readings.Length == 2 && readings[0] == "kun" && readings[1] == "kimi")
				readings = new string[] { "kimi" };
		}

		public bool Match(string input)
		{
			for (int i = 0; i < readings.Length; i++)
			{
				if (readings[i] == input)
					return true;
			}
			return false;
		}
	}

	class SearchTagGenerator
	{
		//Process the ^ and {|} cases, and strip out all other characters other than letters and numbers. Also convert to lowercase.
		private static string Postprocess(string input)
		{
			List<char> newString = new List<char>();
			for (int i = 0; i < input.Length; i++)
			{
				bool lastSpace = false;
				if (input[i] == '^' && i > 0)
				{
					switch (input[i - 1])
					{
						case 'a':
							newString.Add('a');
							break;
						case 'e':
							newString.Add('e');
							break;
						case 'o':
							newString.Add('u');
							break;

					}
				}
				else if (input[i] == '{' || input[i] == '}' || input[i] == '|' ||
					(input[i] >= '0' && input[i] <= '9') || (input[i] >= 'a' && input[i] <= 'z'))
				{
					lastSpace = false;
					newString.Add(input[i]);
				}
				else if (input[i] >= 'A' && input[i] <= 'Z')
				{
					lastSpace = false;
					newString.Add((char)(input[i] | 32)); //To lower
				}
				else
				{
					if (!lastSpace)
					{
						lastSpace = true;
						newString.Add(' ');
					}
				}
			}

			return string.Join("", newString);
		}

		private static Reading ConvertToRomaji(string input)
		{
			Process process = new Process();
			process.StartInfo.FileName = @"C:\kakasi\bin\kakasi.exe";
			process.StartInfo.Arguments = "-s -Ja -Ha -Ka -p -c";
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;

			process.Start();

			StreamWriter writer = new StreamWriter(process.StandardInput.BaseStream, Encoding.GetEncoding(932)); //Codepage 932 = Shift-JIS encoding
			writer.WriteLine(input);
			writer.Flush();
			writer.Close();

			string output = process.StandardOutput.ReadToEnd().Trim();

			return new Reading(Postprocess(output));
		}

		public static string ProcessString(string input)
		{
			return ConvertToRomaji(input).ToString();
		}

		private static void ProcessSong(Song song)
		{
			var title = ConvertToRomaji(song.title);
			var artist = ConvertToRomaji(song.artist);
			var anime = ConvertToRomaji(song.anime);
			var tag = title + " " + artist + " " + anime;
			song.tag = tag;
		}

		public static SongList ProcessFile(string inputFile)
		{
			SongList list = JsonHelper.Parse<SongList>(File.ReadAllText(inputFile));

			list.songs.Multithread(16, (song) =>
			{
				ProcessSong(song);
				Console.Write(".");
			});

			return list;
		}
	}
}
