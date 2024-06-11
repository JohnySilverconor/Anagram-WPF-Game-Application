using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AnagramWPF.Model
{
	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Respresents the main list of words
	/// </summary>
	public class MainDictionary : List<AWord>
	{

		//................................................................................
		/// <summary>
		/// 
		/// </summary>
		public bool CanPlay         { get; set;         }

		//................................................................................
		/// <summary>
		/// 
		/// </summary>
		public int ShortestWordSize { get; private set; }

		//................................................................................
		/// <summary>
		/// 
		/// </summary>
		public int LongestWordSize  { get; private set; }


		//---------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		public MainDictionary()
		{
			this.ShortestWordSize = 65535;
			this.LongestWordSize  = 0;
			string filename       = Path.Combine(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "Anagrams2Words.txt");
			this.CanPlay          = LoadFile(filename);

			// If the class name dictionary file  exists, load it too. This adds about 3x 
			// time to the initizalization of the app. If you're not interested in this, 
			// comment out the next line of code;
			LoadClassNames();
		}

		//---------------------------------------------------------------------------------
		private void LoadClassNames()
		{
			string filename = Path.Combine(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "Anagrams2ClassNames.txt");
			if (System.IO.File.Exists(filename))
			{
				LoadFile(filename, true);
			}
		}

		//---------------------------------------------------------------------------------
        /// <summary>
        /// Loads the word list file. All words are converted to lowercase as they're 
		/// loaded.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        protected bool LoadFile(string fileName, bool isClassNameFile=false)
        {
            bool success = false;
            try
            {
				using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						string words;
						while (!reader.EndOfStream)
						{
							words = reader.ReadLine();
							if (words.Length > 0)
							{
								int duplicates = 0;
								string[] wordsplit = words.Split(' ');
								for (int i = 0; i < wordsplit.Length; i++)
								{
									string text = wordsplit[i].ToUpper();
									// NEW - in the old version of the app, the min and max 
									// word sizes were hard coded. In this version, min/max 
									// length is determined by the contents of the word 
									// dictionary, allowing the user to longer words than the 
									// default 10-letter maximum.
									ShortestWordSize = Math.Min(ShortestWordSize, text.Length);
									LongestWordSize  = Math.Max(LongestWordSize, text.Length);
									AWord item       = new AWord(text, isClassNameFile);
									bool  canAdd     = true;
									if (isClassNameFile)
									{
										AWord exists = (from existing in this where (existing.Text == item.Text) select existing).FirstOrDefault<AWord>();
										canAdd       = (exists == null);
									}
									if (canAdd)
									{
										Add(item);
									}
									else
									{
										duplicates++;
									}
								}
							}
						}
						Globals.LongestWord  = LongestWordSize;
						Globals.ShortestWord = ShortestWordSize;
						success              = this.Count > 0;

					}
				}
            }
            catch (Exception e)
            {
                if (e != null) { }
            }
			int count = (from word in this where word.IsClassName select word).Count();
            return success;
        }

		//---------------------------------------------------------------------------------
		/// <summary>
		/// Retrieves words of the specified length
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public List<AWord> GetWordsByLength(int length)
		{
			//var list = (from item in this 
			//			where item.Text.Length == length
			//			select item).ToList<AWord>();

			List<AWord> list = null;
			if (Anagram2Settings.Default.UseClassNameDictionary)
			{
				list = (from item in this 
						where item.Text.Length == length
						select item).ToList<AWord>();
			}
			else
			{
				list = (from item in this 
						where (item.Text.Length == length && !item.IsClassName)
						select item).ToList<AWord>();
			}

			return list;
		}

		//---------------------------------------------------------------------------------
		public List<AWord> GetPossibleWords(AWord selectedWord)
		{
			List<AWord> possibleWords = null;
			if (Anagram2Settings.Default.UseClassNameDictionary)
			{
				possibleWords = (from item in this
								 where Globals.Contains(selectedWord.Text, item.Text)
								 select item).ToList<AWord>();
			}
			else
			{
				possibleWords = (from item in this
								 where (Globals.Contains(selectedWord.Text, item.Text) && !item.IsClassName)
								 select item).ToList<AWord>();
			}
			int count = (from item in possibleWords where item.IsClassName select item).Count();

			return possibleWords;
		}
	}
}
