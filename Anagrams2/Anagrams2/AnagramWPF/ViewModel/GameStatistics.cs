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
	/// Represents the game statistics object. This is where game stats are calculated.
	/// </summary>
	public class GameStatistics : INotifyPropertyChanged
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
		private int        m_possibleWords = 0;
		private WordCounts m_wordCount     = null;
		private decimal    m_pctWordsFound = 0M;
		private decimal    m_pointsPerWord = 0M;
		private int        m_gamePoints    = 0;
		#endregion Data members

		#region Data member properties
		//................................................................................
		/// <summary>
		/// Get/set - specifies the collection of possible word sizes that can be found 
		/// during a game.
		/// </summary>
		public WordCounts WordCount
		{
			get { return m_wordCount; }
			set
			{
				m_wordCount = value;
				RaisePropertyChanged("WordCount");
			}
		}
		//................................................................................
		/// <summary>
		/// Get/set - specifies the percentage of possible words that have been found by 
		/// the user.
		/// </summary>
		public decimal PctWordsFound
		{
			get { return m_pctWordsFound; }
			set
			{
				m_pctWordsFound = value;
				RaisePropertyChanged("PctWordsFound");
			}
		}
		//................................................................................
		/// <summary>
		/// Get/set - specifies how many points per word have been awarded (includes 
		/// bonus points).
		/// </summary>
		public decimal PointsPerWord
		{
			get { return m_pointsPerWord; }
			set
			{
				m_pointsPerWord = value;
				RaisePropertyChanged("PointsPerWord");
			}
		}
		//................................................................................
		/// <summary>
		/// Get/set - specifies the number of points earned in the current game.
		/// </summary>
		public int GamePoints
		{
			get { return m_gamePoints; }
			set
			{
				m_gamePoints = value;
				RaisePropertyChanged("GamePoints");
			}
		}

		private GameDictionary m_gameDictionary = null;

		#endregion Data member properties

		#region Constructor
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="possibleWords"></param>
		public GameStatistics(GameDictionary parentDictionary)
		{
			WordCount        = new WordCounts(parentDictionary);
			m_possibleWords  = parentDictionary.Count;
			m_gameDictionary = parentDictionary;
			Reset();
		}
		#endregion Constructor

		#region Methods
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Updates the word count (if necessary), and the points value
		/// </summary>
		/// <param name="letterCount">If zero, wordcount is not incremented</param>
		/// <param name="wordPoints">Points to add (or subtract)</param>
		public void Update(int letterCount, int wordPoints)
		{
			if (letterCount > 0)
			{
				this.WordCount.Update(letterCount);
				this.PctWordsFound  = ((decimal)this.WordCount.WordsFound / (decimal)this.m_possibleWords) * 100M;
				RaisePropertyChanged("WordCount");
			}
			this.GamePoints += wordPoints;
			this.PointsPerWord  = ((decimal)this.GamePoints / (decimal)Math.Max(this.WordCount.WordsFound, 1));
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Sets all statistic properties to inital values.
		/// </summary>
		public void Reset()
		{
			PctWordsFound   = 0M;
			PointsPerWord   = 0M;
			GamePoints      = 0;
			WordCount.Reset();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Updates and saves the cumulative statistics when a game ends.
		/// </summary>
		/// <param name="possibleWords">The total number of words that are possible</param>
		/// <param name="foundOriginalWord">Indicates whether or not the original game word was found</param>
		public void UpdateSolvedStats(int possibleWords, bool foundOriginalWord)
		{
			int foundWords = this.WordCount.WordsFound;
			Settings settings = m_gameDictionary.Settings;
			if (settings.SaveStats)
			{
				settings.TotalPossibleWords += m_gameDictionary.WordCount;
				settings.TotalFoundWords    += foundWords;
				settings.GamesWon           += (m_gameDictionary.IsWinner) ? 1 : 0;
				settings.BestPercent         = Math.Max(settings.BestPercent, this.PctWordsFound);
				settings.WorstPercent        = (settings.WorstPercent == 0M) ? this.PctWordsFound : Math.Min(settings.WorstPercent, this.PctWordsFound);
				settings.AveragePercent      = ((decimal)settings.TotalFoundWords / (decimal)settings.TotalPossibleWords) * 100M;
				settings.MostFoundWords      = Math.Max(settings.MostFoundWords, foundWords);
				settings.TotalFoundGameWord += (m_gameDictionary.GameWord.Found) ? 1 : 0;
				settings.TotalPoints        += m_gameDictionary.Statistics.GamePoints;
				settings.GamesPlayed++;
			}
			else
			{
				settings.TotalPossibleWords = 0;
				settings.TotalFoundWords    = 0;
				settings.GamesPlayed        = 0;
				settings.GamesWon           = 0;
				settings.BestPercent        = 0M;
				settings.WorstPercent       = 0M;
				settings.AveragePercent     = 0M;
				settings.MostFoundWords     = 0;
				settings.TotalFoundGameWord = 0;
				settings.TotalPoints        = 0;
			}
			settings.SaveStatistics();
		}
		#endregion Methods

	}
}

