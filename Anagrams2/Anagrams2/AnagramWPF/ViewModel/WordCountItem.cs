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
	/// Represents a word size and the number of words of that size. 
	/// </summary>
	public class WordCountItem : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged code
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		#endregion INotifyPropertyChanged code

		#region Data members
		private int m_letterCount   = 0;
		private int m_wordCount     = 0;
		private int m_possibleWords = 0;
		#endregion Data members

		#region Data member properties
		//................................................................................
		/// <summary>
		/// Get/set - specifies the word size
		/// </summary>
		public int LetterCount 
		{
			get { return m_letterCount; }
			private set
			{
				m_letterCount = value;
				RaisePropertyChanged("LetterCount");
			}
		}
		//................................................................................
		/// <summary>
		/// Get/set - specifies the number of words found durng the current game.
		/// </summary>
		public int WordCount 
		{
			get { return m_wordCount; }
			set
			{
				m_wordCount = value;
				RaisePropertyChanged("WordCount");
			}
		}

		public int PossibleWords
		{
			get { return m_possibleWords; }
			set
			{
				m_possibleWords = value;
				RaisePropertyChanged("PossibleWords");
			}
		}
		#endregion Data member properties


		//--------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="letterCount"></param>
		public WordCountItem(int letterCount, int  possibleWords)
		{
			this.LetterCount   = letterCount;
			this.PossibleWords = possibleWords;
			this.WordCount     = 0;
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Sets the word count to 0;
		/// </summary>
		public void Reset()
		{
			this.WordCount = 0;
		}
	}

}
