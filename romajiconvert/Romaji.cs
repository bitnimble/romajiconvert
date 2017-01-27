using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomajiConvert
{
	public static class StringSplit
	{
		public static string[] Split(this string s, char c)
		{
			return s.Split(new char[] { c });
		}

		public static bool Contains(this string s, string[] values)
		{
			foreach (string v in values)
				if (s.Contains(v))
					return true;
			return false;
		}
		
		public static string Replace(this string input, Dictionary<string, string> replacements)
		{
			foreach (var kvp in replacements)
			{
				input = input.Replace(kvp.Key, kvp.Value);
			}
			return input;
		}
	}

	public class Romaji
	{
		#region Romaji Mappings
		//Kana to romaji conversion is a port of github:lovell/hepburn from nodejs to C#, with macrons removed
		static Dictionary<string, string> hiraganaMonographs = new Dictionary<string, string> {
			{ "あ", "A"}, { "い", "I"}, { "う", "U"}, { "え", "E"}, { "お", "O"},
			{ "か", "KA"}, { "き", "KI"}, { "く", "KU"}, { "け", "KE"}, { "こ", "KO"},
			{ "さ", "SA"}, { "し", "SHI"}, { "す", "SU"}, { "せ", "SE"}, { "そ", "SO"},
			{ "た", "TA"}, { "ち", "CHI"}, { "つ", "TSU"}, { "て", "TE"}, { "と", "TO"},
			{ "な", "NA"}, { "に", "NI"}, { "ぬ", "NU"}, { "ね", "NE"}, { "の", "NO"},
			{ "は", "HA"}, { "ひ", "HI"}, { "ふ", "FU"}, { "へ", "HE"}, { "ほ", "HO"},
			{ "ま", "MA"}, { "み", "MI"}, { "む", "MU"}, { "め", "ME"}, { "も", "MO"},
			{ "や", "YA"}, { "ゆ", "YU"}, { "よ", "YO"},
			{ "ら", "RA"}, { "り", "RI"}, { "る", "RU"}, { "れ", "RE"}, { "ろ", "RO"},
			{ "わ", "WA"}, { "ゐ", "WI"}, { "ゑ", "WE"}, { "を", "WO"}, { "ん", "N'"},
			{ "が", "GA"}, { "ぎ", "GI"}, { "ぐ", "GU"}, { "げ", "GE"}, { "ご", "GO"},
			{ "ざ", "ZA"}, { "じ", "JI"}, { "ず", "ZU"}, { "ぜ", "ZE"}, { "ぞ", "ZO"},
			{ "だ", "DA"}, { "ぢ", "DJI"}, { "づ", "DZU"}, { "で", "DE"}, { "ど", "DO"},
			{ "ば", "BA"}, { "び", "BI"}, { "ぶ", "BU"}, { "べ", "BE"}, { "ぼ", "BO"},
			{ "ぱ", "PA"}, { "ぴ", "PI"}, { "ぷ", "PU"}, { "ぺ", "PE"}, { "ぽ", "PO" }
		};

		static Dictionary<string, string> hiraganaDigraphs = new Dictionary<string, string> {
			{ "きゃ", "KYA"}, { "きゅ", "KYU"}, { "きょ", "KYO"},
			{ "しゃ", "SHA"}, { "しゅ", "SHU"}, { "しょ", "SHO"},
			{ "ちゃ", "CHA"}, { "ちゅ", "CHU"}, { "ちょ", "CHO"},
			{ "にゃ", "NYA"}, { "にゅ", "NYU"}, { "にょ", "NYO"},
			{ "ひゃ", "HYA"}, { "ひゅ", "HYU"}, { "ひょ", "HYO"},
			{ "みゃ", "MYA"}, { "みゅ", "MYU"}, { "みょ", "MYO"},
			{ "りゃ", "RYA"}, { "りゅ", "RYU"}, { "りょ", "RYO"},
			{ "ぎゃ", "GYA"}, { "ぎゅ", "GYU"}, { "ぎょ", "GYO"},
			{ "じゃ", "JA"}, { "じゅ", "JU"}, { "じょ", "JO"},
			{ "びゃ", "BYA"}, { "びゅ", "BYU"}, { "びょ", "BYO"},
			{ "ぴゃ", "PYA"}, { "ぴゅ", "PYU"}, { "ぴょ", "PYO" }
		};

		static Dictionary<string, string> katakanaMonographs = new Dictionary<string, string> {
			{ "ア", "A"}, { "イ", "I"}, { "ウ", "U"}, { "エ", "E"}, { "オ", "O"},
			{ "カ", "KA"}, { "キ", "KI"}, { "ク", "KU"}, { "ケ", "KE"}, { "コ", "KO"},
			{ "サ", "SA"}, { "シ", "SHI"}, { "ス", "SU"}, { "セ", "SE"}, { "ソ", "SO"},
			{ "タ", "TA"}, { "チ", "CHI"}, { "ツ", "TSU"}, { "テ", "TE"}, { "ト", "TO"},
			{ "ナ", "NA"}, { "ニ", "NI"}, { "ヌ", "NU"}, { "ネ", "NE"}, { "ノ", "NO"},
			{ "ハ", "HA"}, { "ヒ", "HI"}, { "フ", "FU"}, { "ヘ", "HE"}, { "ホ", "HO"},
			{ "マ", "MA"}, { "ミ", "MI"}, { "ム", "MU"}, { "メ", "ME"}, { "モ", "MO"},
			{ "ヤ", "YA"}, { "ユ", "YU"}, { "ヨ", "YO"},
			{ "ラ", "RA"}, { "リ", "RI"}, { "ル", "RU"}, { "レ", "RE"}, { "ロ", "RO"},
			{ "ワ", "WA"}, { "ヰ", "WI"}, { "ヱ", "WE"}, {  "ヲ", "WO"}, { "ン", "N"},
			{ "ガ", "GA"}, { "ギ", "GI"}, { "グ", "GU"}, { "ゲ", "GE"}, { "ゴ", "GO"},
			{ "ザ", "ZA"}, { "ジ", "JI"}, { "ズ", "ZU"}, { "ゼ", "ZE"}, { "ゾ", "ZO"},
			{ "ダ", "DA"}, { "ヂ", "DJI"}, { "ヅ", "DZU"}, { "デ", "DE"}, { "ド", "DO"},
			{ "バ", "BA"}, { "ビ", "BI"}, { "ブ", "BU"}, { "ベ", "BE"}, { "ボ", "BO"},
			{ "パ", "PA"}, { "ピ", "PI"}, { "プ", "PU"}, { "ペ", "PE"}, { "ポ", "PO" }
		};

		static Dictionary<string, string> katakanaDigraphs = new Dictionary<string, string> {
			{ "アー", "AA"}, { "イー", "II"}, { "ウー", "UU"}, { "エー", "EI"}, { "オー", "O"},
			{ "カー", "KAA"}, { "キー", "KII"}, { "クー", "KUU"}, { "ケー", "KEI"}, { "コー", "KO"},
			{ "サー", "SAA"}, { "シー", "SHII"}, { "スー", "SUU"}, { "セー", "SEI"}, { "ソー", "SO"},
			{ "ター", "TAA"}, { "チー", "CHII"}, { "ツー", "TSUU"}, { "テー", "TEI"}, { "トー", "TO"},
			{ "ナー", "NAA"}, { "ニー", "NII"}, { "ヌー", "NUU"}, { "ネー", "NEI"}, { "ノー", "NO"},
			{ "ハー", "HAA"}, { "ヒー", "HII"}, { "フー", "FUU"}, { "ヘー", "HEI"}, { "ホー", "HO"},
			{ "マー", "MAA"}, { "ミー", "MII"}, { "ムー", "MUU"}, { "メー", "MEI"}, { "モー", "MO"},
			{ "ヤー", "YAA"}, { "ユー", "YUU"}, { "ヨー", "YO"},
			{ "ラー", "RAA"}, { "リー", "RII"}, { "ルー", "RUU"}, { "レー", "REI"}, { "ロー", "RO"},
			{ "ワー", "WAA"}, { "ヰー", "WII"}, { "ヱー", "WEI"}, {  "ヲー", "WŌ"}, { "ンー", "N"},
			{ "ガー", "GAA"}, { "ギー", "GII"}, { "グー", "GUU"}, { "ゲー", "GEI"}, { "ゴー", "GO"},
			{ "ザー", "ZAA"}, { "ジー", "JII"}, { "ズー", "ZUU"}, { "ゼー", "ZEI"}, { "ゾー", "ZO"},
			{ "ダー", "DAA"}, { "ヂー", "DJII"}, { "ヅー", "DZUU"}, { "デー", "DEI"}, { "ドー", "DO"},
			{ "バー", "BAA"}, { "ビー", "BII"}, { "ブー", "BUU"}, { "ベー", "BEI"}, { "ボー", "BO"},
			{ "パー", "PAA"}, { "ピー", "PII"}, { "プー", "PUU"}, { "ペー", "PEI"}, { "ポー", "PO"},
			{ "キャ", "KYA"}, { "キュ", "KYU"}, { "キョ", "KYO"},
			{ "シャ", "SHA"}, { "シュ", "SHU"}, { "ショ", "SHO"},
			{ "チャ", "CHA"}, { "チュ", "CHU"}, { "チョ", "CHO"},
			{ "ニャ", "NYA"}, { "ニュ", "NYU"}, { "ニョ", "NYO"},
			{ "ヒャ", "HYA"}, { "ヒュ", "HYU"}, { "ヒョ", "HYO"},
			{ "ミャ", "MYA"}, { "ミュ", "MYU"}, { "ミョ", "MYO"},
			{ "リャ", "RYA"}, { "リュ", "RYU"}, { "リョ", "RYO"},
			{ "ギャ", "GYA"}, { "ギュ", "GYU"}, { "ギョ", "GYO"},
			{ "ジャ", "JA"}, { "ジュ", "JU"}, { "ジョ", "JO"},
			{ "ビャ", "BYA"}, { "ビュ", "BYU"}, { "ビョ", "BYO"},
			{ "ピャ", "PYA"}, { "ピュ", "PYU"}, { "ピョ", "PYO" }
		};

		static Dictionary<string, string> DefaultCustomTags = new Dictionary<string, string>()
		{
			{ "アフターサービス", "after service" },
			{ "アイドル", "idol" },
			{ "アイスクリーム", "ice cream" },
			{ "アンサー", "answer" },
			{ "バーサーカー", "berserker" },
			{ "バーゲン", "bargain" },
			{ "バイク", "bike" },
			{ "バター", "butter" },
			{ "ビジネスホテル", "business hotel" },
			{ "ブレザー", "blazer" },
			{ "チケット", "ticket" },
			{ "コンピューター", "computer" },
			{ "コンピュータ", "computer" },
			{ "ドライバー", "driver" },
			{ "ドライブイン", "drivein" },
			{ "ドラマ", "drama" },
			{ "ドリフト", "drift" },
			{ "エレベーター", "elevator" },
			{ "エスカレーター", "escalator" },
			{ "ファイナル", "final" },
			{ "ファイト", "fight" },
			{ "ファンファーレ", "fanfare" },
			{ "ファッション", "fashion" },
			{ "フライドポテト", "fried potato" },
			{ "フロント", "front" },
			{ "ゲームセンタ", "game centre" },
			{ "ゴールデンアワー", "golden hour" },
			{ "ゴールデンタイム", "golden time" },
			{ "ゴールデンウィーク", "golden week" },
			{ "ゴールデンウイーク", "golden week" },
			{ "グラス", "glass" },
			{ "ギャラリー", "gallery" },
			{ "ハッカー", "hacker" },
			{ "ハッピーエンド", "happy end" },
			{ "ハンバーガー", "hamburger" },
			{ "ホットケーキ", "hotcake" },
			{ "ホワイトデー", "white day" },
			{ "クリスタル", "crystal" },
			{ "キーボード", "keyboard" },
			{ "モバイル", "mobile" },
			{ "パーソナルコンピューター", "personal computer" },
			{ "ライバル", "rival" },
			{ "ライブアクション", "live action" },
			{ "ライブハウス", "live house" },
			{ "ロリコン", "lolicon" },
			{ "サービス", "service" },
			{ "スキンシップ", "skinship" },
			{ "タバコ", "tobacco" },
			{ "タイムオーバー", "time over" },
			{ "ティーンエージャー", "teenager" },
			{ "トラブル", "trouble" },
			{ "ワンピース", "one piece" },
			{ "れをる", "REOL" }
		};
		#endregion

		Dictionary<string, string> customTags;

		public Romaji()
		{
			customTags = new Dictionary<string, string>(DefaultCustomTags);
		}

		public Romaji(Dictionary<string, string> customTags)
		{
			this.customTags = new Dictionary<string, string>(customTags);
		}

		private string toRomaji(string kana)
		{
			kana = kana.Trim();
			if (customTags.ContainsKey(kana))
				return customTags[kana];

			kana = kana.Replace(hiraganaDigraphs);
			kana = kana.Replace(katakanaDigraphs);

			kana = kana.Replace(hiraganaMonographs);
			kana = kana.Replace(katakanaMonographs);

			kana = kana.Replace("っC", "TC").Replace("ッC", "TC");

			var kanaArr = kana.ToCharArray();
			for (int i = kanaArr.Length - 2; i >= 0; i--)
			{
				if (kanaArr[i] == 'っ' || kanaArr[i] == 'ッ')
				{
					kanaArr[i] = kanaArr[i + 1];
				}
			}
			kana = new string(kanaArr);
			kana.Replace("っ", "").Replace("ッ", "");

			kana = kana.Replace("Aー", "AA");
			kana = kana.Replace("Iー", "II");
			kana = kana.Replace("Uー", "UU");
			kana = kana.Replace("Eー", "EI");
			kana = kana.Replace("Oー", "O");

			kana = kana.Replace("、", ",");
			kana = kana.Replace("。", ".");

			return kana;
		}

		enum LineState
		{
			Hiragana,
			Katakana,
			Other,
			Default
		}

		string postprocess(string input)
		{
			input = input.ToLowerInvariant();
			List<char> result = new List<char>();
			foreach (int c in input)
			{
				if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z'))
					result.Add((char)c);
			}
			return string.Join("", result);
		}

		string[] replaceReading(string[] lines, int start, int count, string replacement)
		{
			string toAdd = postprocess(replacement) + "\t,*,*,*,*,*,*,*,*";

			//The poor man's splice
			List<string> result = new List<string>();
			for (int i = 0; i < start; i++)
				result.Add(lines[i]);
			result.Add(toAdd);
			for (int i = start + count; i < lines.Length; i++)
				result.Add(lines[i]);

			return result.ToArray();
		}

		string[] processInlineCustomTags(string[] lines)
		{
			//Clone array
			string[] newLines = new string[lines.Length];
			Array.Copy(lines, newLines, lines.Length);

			int start = 0;
			int count = 0;
			string currentSequence = "";
			LineState lastState = LineState.Default;
			bool first = true;

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				string[] components = line.Trim().Split(',');
				string token = components[0].Split('\t')[0];

				//What type is this line?
				bool hira = token.Contains(hiraganaMonographs.Keys.ToArray());
				bool kata = token.Contains(katakanaMonographs.Keys.ToArray());
				LineState thisState = hira ? LineState.Hiragana : (kata ? LineState.Katakana : LineState.Other);

				if (first)
				{
					//Reset start index and count if it's the first token
					first = false;
					currentSequence += token;
					count++;
					start = Math.Max(i - 1, 0);
				}
				else if (thisState != lastState)
				{
					//Changed from hiragana to katakana, vice versa, or to kanji/english
					if (currentSequence.Trim() != "" && customTags.ContainsKey(currentSequence))
						newLines = replaceReading(newLines, start, count, customTags[currentSequence]);

					//Reset the sequence
					first = true;
					currentSequence = token;
					start = i;
					count = 1;
				}
				else
				{
					currentSequence += token;
					count++;
				}

				lastState = thisState;
			}

			//Process the final token
			if (currentSequence.Trim() != "" && customTags.ContainsKey(currentSequence))
				newLines = replaceReading(newLines, start, count, customTags[currentSequence]);

			return newLines;
		}

		private string ParseMecabOutput(string[] lines)
		{
			lines = processInlineCustomTags(lines);

			var results = new List<string>();

			for (int i = 0; i < lines.Length - 1; i++)
			{
				string line = lines[i];
				var components = line.Trim().Split(',');
				if (components[components.Length - 1] == "*")
				{
					//Push the english if MeCab didn't parse it
					results.Add(toRomaji(components[0].Split('\t')[0]));
				}
				else
				{
					results.Add(toRomaji(components[components.Length - 2]));
				}
			}

			return postprocess(string.Join("", results));
		}

		public string Convert(string input)
		{
			Process process = new Process();
			process.StartInfo.FileName = @"mecab";
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;

			process.Start();

			StreamWriter writer = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8); //Codepage 932 = Shift-JIS encoding
			writer.WriteLine(input);
			writer.Flush();
			writer.Close();

			string output = process.StandardOutput.ReadToEnd().Trim();

			string[] lines = output.Split('\n');
			//Strip empty lines
			lines = lines.Where(s => s.Trim() != "").ToArray(); 

			return ParseMecabOutput(lines);
		}
	}
}
