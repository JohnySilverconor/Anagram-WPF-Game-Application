using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AnagramWPF.ViewModel
{
	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represents the individual "found word" counts for the various word sizes.
	/// </summary>
	public class WordCounts : ObservableCollection<WordCountItem>
	{

		#region Properties
		//................................................................................
		/// <summary>
		/// Get the number of words found
		/// </summary>
		public int WordsFound
		{
			get 
			{ 
				int value = 0;
				foreach(WordCountItem item in this)
				{
					value += item.WordCount;
				}
				return value;
			}
		}
		#endregion Properties

		#region Constructor
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parentDictionary"></param>
		public WordCounts(GameDictionary parentDictionary)
		{
			for (int i = Globals.ShortestWord; i <= Globals.LongestWord; i++)
			{
				int count = parentDictionary.CountWordsOfLength(i);
				this.Add(new WordCountItem(i, count));
			}
		}
		#endregion Constructor

		#region Methods
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Updates the wordcount for the specified letterCount
		/// exist.
		/// </summary>
		/// <param name="letterCount">The word size to be updated</param>
		public void Update(int letterCount)
		{
			var counter = (from item in this where item.LetterCount == letterCount select item).FirstOrDefault();
			if (counter != null)
			{
				counter.WordCount += 1;
			}
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Resets the list to have no items.
		/// </summary>
		public void Reset()
		{
			foreach(WordCountItem item in this)
			{
				item.Reset();
			}
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Used by converter to retrieve the word count for words of the specified length
		/// </summary>
		/// <param name="length">word length to return the value for</param>
		/// <returns>The number of words found of the specified length</returns>
		public int GetCountForLength(int length)
		{
			int count = 0;
			var counter = (from item in this where item.LetterCount == length select item).FirstOrDefault();
			if (counter != null)
			{
				count = counter.WordCount;
			}
			return count;
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Used by a converter to retrieve the word count item words of the specified 
		/// length
		/// </summary>
		/// <param name="length">word length to return the value for</param>
		/// <returns>The item found, or null.</returns>
		public WordCountItem GetItemForCount(int length)
		{
			var result = (from item in this where item.LetterCount == length select item).FirstOrDefault();
			return result;
		}

		#endregion Methods

	}


}
