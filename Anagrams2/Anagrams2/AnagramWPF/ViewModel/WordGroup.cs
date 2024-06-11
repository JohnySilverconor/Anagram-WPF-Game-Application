using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AnagramWPF.ViewModel
{
	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// This class holds a group of words. 
    /// </summary>
    public class WordGroup
    {
        private int m_letterCount = 0;
        //private int[] m_wordPoints = { 1, 3, 3, 2, 1, 4, 2, 4, 1, 8, 5, 1, 3, 1, 1, 3, 10, 1, 1, 1, 1, 4, 4, 8, 4, 10 };
        private int[] m_1stLetter = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        List<DisplayWord> m_wordList = new List<DisplayWord>();
        private bool m_trackUsedWords = true;
        //private int m_lowEnd = -1;
        private int m_highEnd = -1;
        //private DateTime m_dtCreated = DateTime.Now;
		private string m_currentWord = "";

        #region Read-only properties
        public int    Count       { get { return m_wordList.Count; } }
        public int    CountUsed   { get { return CountUsedWords(); } }
        public int    LetterCount { get { return m_letterCount;    } }
		public string CurrentWord { get { return m_currentWord;    } }
        #endregion

        #region Non-read-only properties
        public bool TrackRepeats
        {
            get { return m_trackUsedWords; }
            set { m_trackUsedWords = value; }
        }
        #endregion

        #region Constructors
        //public WordGroup()
        //{
        //}

		//---------------------------------------------------------------------------------
        public WordGroup(int letterCount)
        {
            m_wordList.Clear();
            m_letterCount = letterCount;
            // we only need to load a file if the number of letters is 3 or more
            if (letterCount >= 3)
            {
				string path = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                string wordsPath = System.IO.Path.Combine(path, string.Format("Words\\{0}.txt", letterCount));
                LoadFile(wordsPath);
            }
            this.m_highEnd = this.Count;
        }
        #endregion

		//---------------------------------------------------------------------------------
		/// <summary>
		/// Calclates a random seed based on the number of milliseconds since 
		/// the top of the current hour.
		/// </summary>
		/// <returns></returns>
		protected static int GetRandomSeed()
		{
			DateTime dtNow = DateTime.Now;
			DateTime dtLastHour = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour,0,0,0);
			TimeSpan span = dtLastHour - dtNow;
			return span.Milliseconds;
		}
			
		//---------------------------------------------------------------------------------
        /// <summary>
        /// Loads the file that contains a list of words. Each list of words is 
        /// constrained by the number of letters in the words. All words are converted to 
		/// lowercase as they're loaded.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        protected bool LoadFile(string fileName)
        {
            bool success = false;
            try
            {
                FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                StreamReader reader = new StreamReader(stream);
                string words;
                while (!reader.EndOfStream)
                {
                    words = reader.ReadLine();
                    if (words.Length > 0)
                    {
                        string[] wordsplit = words.Split(' ');
                        for (int i = 0; i < wordsplit.Length; i++)
                        {
                            DisplayWord item = new DisplayWord(wordsplit[i].ToLower());
                            m_wordList.Add(item);
                        }
                    }
                }
                success = true;
            }
            catch (Exception e)
            {
                if (e != null) { }
            }
            if (success)
            {
                FindLetterIndexes();
            }
            return success;
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Finds the starting position for each word by the first letter in 
        /// that word, and stores the results in a list.  This helps limit 
        /// the search for a correct word by allowing us to search a very 
        /// small subset of each letter-count group.  Some word groups 
        /// contain tens of thousands of words.
        /// </summary>
        protected void FindLetterIndexes()
        {
            int start = 0;
            for (int i = 0; i < 26; i++)
            {
                int intValue = 97 + i;
                string charValue = Convert.ToChar(intValue).ToString();
                if (m_wordList[start].Text.StartsWith(charValue))
                {
                    m_1stLetter[i] = start;
                    for (int j = start; j < this.Count; j++)
                    {
                        if (!m_wordList[j].Text.StartsWith(charValue))
                        {
                            start = j;
                            break;
                        }
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Resets the word list to the specified percentage of original words. This is 
        /// only done when the user has elected to prevent duplicate useage of words, and 
        /// the user has used up all of the words in this word list.  The words that are 
        /// actually reset are randomly selected.
        /// </summary>
        /// <param name="percent"></param>
        private void ResetWordList(int percent)
        {
            Random random = new Random(GetRandomSeed());

            int resetCount = Convert.ToInt32(m_wordList.Count * (percent*0.01));
            while (resetCount > 0)
            {
                bool reset = false;
                while (!reset)
                {
                    int index = random.Next(0, m_wordList.Count);
                    if (m_wordList[index].Used)
                    {
                        reset = true;
                        m_wordList[index].Used = false;
                    }
                }
                resetCount--;
            }
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Retrieves a randomly selected word from the dictionary. The same 
        /// word should rarely (if ever) be selected more than once. To 
        /// reduce the chances of that,we trsack when a word at either of the 
        /// extreme ends of the dictionary are chosen.  This limits what 
        /// random number is generated to a small degeree and given repeated 
        /// play with the same word size, eventually reduces the possible 
        /// returns that can be generated by the random number generator.
        /// </summary>
        /// <param name="scrambled"></param>
        /// <returns></returns>
        public string GetRandomWord(bool scrambled)
        {
            if (m_wordList.Count == 0)
            {
                return "";
            }

            Random random = new Random(GetRandomSeed());

            // get the word we think we want to use
            string randomWord = "";
            while (randomWord.Length == 0)
            {
                int index = random.Next(0, m_wordList.Count);
                if (!m_wordList[index].Used)
                {
                    randomWord = m_wordList[index].Text;
                    if (this.TrackRepeats)
                    {
                        m_wordList[index].Used = true;
                    }
                }
            }

            if (scrambled)
            {
                randomWord = Scramble(randomWord);
            }
            return randomWord;
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Counts the number of words that have been used
        /// </summary>
        /// <returns></returns>
        private int CountUsedWords()
        {
            int used = 0;
            if (this.TrackRepeats)
            {
                for (int i = 0; i < m_wordList.Count; i++)
                {
                    if (m_wordList[i].Used)
                    {
                        used++;
                    }
                }
            }
            return used;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// Scrambles the specified word by randomly selecting letters from the 
        /// original word, and building a new "word" from those characters. In the 
		/// unlikely event that the scrambled word is exactly the same as the original 
		/// word, we do it again (until the two words don't match).
        /// </summary>
        /// <param name="oldWord"></param>
        /// <returns></returns>
        public string Scramble(string oldWord)
        {
			m_currentWord = oldWord;
            Random random = new Random(GetRandomSeed());
            string newWord = "";
			string originalWord = oldWord;
			do
			{
				newWord = "";
				while (originalWord.Length > 0)
				{
					int index    = random.Next(0, originalWord.Length);
					newWord     += originalWord[index];
					originalWord = originalWord.Remove(index, 1);
				}
			} while (newWord == oldWord);
            return newWord;
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Calculates the number of points that a word scores according to Scrabble 
        /// letter values.
        /// </summary>
        /// <param name="scoreWord"></param>
        /// <returns></returns>
        public int CalculateWordPoints(string scoreWord)
        {
            int points = 0;
            int count = scoreWord.Length;
            for (int i = 0; i < count; i++)
            {
                points += Globals.WordPoints[Convert.ToInt16(scoreWord[0]) - 97];
            }
            return points;
        }
        public int CalculateWordPoints(int index)
        {
            return CalculateWordPoints(m_wordList[index].Text);
        }

        #region For the possible words word group (index 0)
        //--------------------------------------------------------------------------------
        public void ClearPossibleWords()
        {
            m_wordList.Clear();
        }

        //--------------------------------------------------------------------------------
        public void AddPossibleWord(string word)
        {
            m_wordList.Add(new DisplayWord(word));
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Finds the word that the user typed.
        /// </summary>
        /// <param name="wordToFind">The word submitted by the user.</param>
        /// <returns>True if the word was found</returns>
        public int FindWord(string wordToFind)
        {
            // limit the search to words that start with the first letter of the word 
            // we're looking for
            int index = -1;
            for (int i = 0; i < m_wordList.Count; i++)
            {
                if (m_wordList[i].Text == wordToFind.ToLower())
                {
                    if (!m_wordList[i].Used)
                    {
                        index = i;
                    }
                }
            }
            return index;
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Determines if the word has already been used
        /// </summary>
        /// <param name="word"></param>
        /// <returns>The index of the word, or -1 if it hasn't been used</returns>
        public bool IsUsed(int index)
        {
            return (m_wordList[index].Used);
        }

        //--------------------------------------------------------------------------------
        public bool IsUsed(string word)
        {
            int index = FindWord(word);
            if (index >= 0)
            {
                return IsUsed(index);
            }
            // if we couldn't find the word, we assume it's used
            return true;
        }

        //--------------------------------------------------------------------------------
        public void MarkWordAsUsed(int index)
        {
            m_wordList[index].Used = true;
        }

        //--------------------------------------------------------------------------------
        public void MarkWordAsUsed(string word)
        {
            int index = FindWord(word);
            if (index >= 0)
            {
                MarkWordAsUsed(index);
            }
        }

        //--------------------------------------------------------------------------------
        public static bool IsPossibleWord(string baseWord, string compareWord)
        {
            bool found = true;
            while (found && compareWord.Length > 0)
            {
                char currChar = compareWord[0];
                compareWord = compareWord.Remove(0, 1);
                int index = baseWord.IndexOf(currChar);
                if (index >= 0)
                {
                    baseWord = baseWord.Remove(index, 1);
                }
                else
                {
                    found = false;
                    break;
                }
            }
            return found;
        }

        //--------------------------------------------------------------------------------
        public List<DisplayWord> FindPossibleWords(string thisWord)
		{
			List<DisplayWord> foundList = new List<DisplayWord>();
            for (int i = 0; i < m_wordList.Count; i++)
            {
                string originalWord = thisWord;
                DisplayWord item = m_wordList[i];
                string compareWord = item.Text;
                if (IsPossibleWord(thisWord, compareWord))
                {
                    foundList.Add(new DisplayWord(compareWord));
                }
            }
			return foundList;
		}

        //--------------------------------------------------------------------------------
		public void AddPossibleWords(List<DisplayWord> foundList)
		{
			m_wordList.AddRange(foundList);
		}

        //--------------------------------------------------------------------------------
		public DisplayWord GetWord(int index)
		{
			DisplayWord item = null;
			if (index >= 0 && index < m_wordList.Count)
			{
				item = m_wordList[index];
			}
			return item;
		}

        #endregion

        //--------------------------------------------------------------------------------
        public DisplayWord GetWord(string word)
        {
            DisplayWord item = null;
            for (int i = 0; i < m_wordList.Count; i++)
            {
                item = m_wordList[i];
                if (item.Text == word)
                {
                    break;
                }
                else
                {
                    item = null;
                }
            }
            return item;
        }

        //--------------------------------------------------------------------------------
        public void Clear()
        {
            m_wordList.Clear();
        }
    }
}
