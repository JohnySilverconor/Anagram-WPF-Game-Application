using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace AnagramWPF.ViewModel
{

	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represents the viewmodel for the application settings data.
	/// </summary>
	public class Settings : INotifyPropertyChanged
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
		private LetterPoolMode m_letterPoolMode;
		private int       m_letterPoolCount;
		private bool      m_trackUsedWords;
		private int       m_bonusTime;
		private int       m_bonusWords;
		private TimerMode m_timerMode;
		private bool      m_playTickSound;
		private string    m_tickSoundFile;
		private int       m_staticTimeValue;
		private bool      m_awardBonusTime;
		private int       m_secondsPerLetter;
		private bool      m_useClassNameDictionary;

		// cumulative game stats
		private bool      m_saveStats;
		private decimal   m_worstPercent;
		private decimal   m_bestPercent;
		private decimal   m_averagePercent;
		private int       m_gamesPlayed;
		private int       m_gamesWon;
		private int       m_totalPossibleWords;
		private int       m_totalFoundWords;
		private DateTime  m_statsDate;
		private int       m_mostFoundWords;
		private int       m_totalFoundGameWord;
		private int       m_totalPoints;
		#endregion Data members

		#region Data member properties
		//................................................................................
		/// <summary>
		/// Get/set - specifies the letter pool mode which dictates how a word is 
		/// selected for a game. 
		/// 
		///   Random - words are selected at random with a minimum length of six, and a 
		///   maximum length of the largest word available
		///   
		///   Static - selected word will never be larger than the specified 
		///   LetterPoolCount value.
		/// </summary>
		public LetterPoolMode LetterPoolMode
		{
			get { return m_letterPoolMode; }
			set { m_letterPoolMode = value; RaisePropertyChanged("LetterPoolMode"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies how time is granted with regards to the game timer is 
		/// when a new game is started
		/// 
		///   NoTimer - The timer is not used
		///   
		///   Static - The timer uses the amount of time (in seconds) specified by the 
		///   StaticTimeValue property.
		///   
		///   PerLetter - The timer is assigned a number of seconds per letter in the 
		///   game word, as specified by the SecondsPerLetter property.
		///   
		///   StaticPerletter - The timer uses a combination of static time and seconds 
		///   per letter as specified by the StaticTimeValue and SecondsPerLetter 
		///   properties.
		/// </summary>
		public TimerMode TimerMode
		{
			get { return m_timerMode; }
			set { m_timerMode = value; RaisePropertyChanged("TimerMode"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies the size of possible game words, and is only used when 
		/// the LetterPoolMode is set to Static. Possible value range is 5-size of 
		/// largest word in main dictionary.
		/// </summary>
		public int LetterPoolCount
		{
			get { return m_letterPoolCount; }
			set { m_letterPoolCount = value; RaisePropertyChanged("LetterPoolCount"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies whether or not to track words used during an application 
		/// session. If tracked, the same word won't be used twice in amn application 
		/// setting until all other words have been used.
		/// </summary>
		public bool TrackUsedWords
		{
			get { return m_trackUsedWords; }
			set { m_trackUsedWords = value; RaisePropertyChanged("TrackUsedWords"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies the amount of bonus time to award in the event that the 
		/// specified number of valid words (the BonusWords property) have been entered 
		/// by the user. Value range is 10-60.
		/// </summary>
		public int BonusTime
		{
			get { return m_bonusTime; }
			set { m_bonusTime = value; RaisePropertyChanged("BonusTime"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies the number of words the user must find before being 
		/// awarded bonus time (for every N words, the bonus time is awarded). Value 
		/// range is 5-100.
		/// </summary>
		public int BonusWords
		{
			get { return m_bonusWords; }
			set { m_bonusWords = value; RaisePropertyChanged("BonusWords"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies whether or not to play a tick sound as the game 
		/// progresses. 
		/// </summary>
		public bool PlayTickSound
		{
			get { return m_playTickSound; }
			set { m_playTickSound = value; RaisePropertyChanged("PlayTickSound"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies the tick sound file to play. If PlayTickSound is true, 
		/// and no file has been specified, the application will use the default beep 
		/// sound on the player's system.
		/// </summary>
		public string TickSoundFile
		{
			get { return m_tickSoundFile; }
			set { m_tickSoundFile = value; RaisePropertyChanged("TickSoundFile"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies the number of seconds to allocate when the user selects 
		/// either TimerMode.Static or TimerMode.StaticPerLetter.  Value range is 10-300;
		/// </summary>
		public int StaticTimeValue
		{
			get { return m_staticTimeValue; }
			set { m_staticTimeValue = value; RaisePropertyChanged("StaticTimeValue"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies the toggle that indiacates whether or not bonus time is 
		/// awarded
		/// </summary>
		public bool AwardBonusTime
		{
			get { return m_awardBonusTime; }
			set { m_awardBonusTime = value; RaisePropertyChanged("AwardBonusTime"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies the number of seconds to allocate when the user selects 
		/// either TimerMode.PerLetter or TimerMode.StaticPerLetter.  Value range is 
		/// 10-60;
		/// </summary>
		public int SecondsPerLetter
		{
			get { return m_secondsPerLetter; }
			set { m_secondsPerLetter = value; RaisePropertyChanged("SecondsPerLetter"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies the toggle that indicates whether or not cumulative game 
		/// stats are tracked
		/// </summary>
		public bool      SaveStats 
		{
			get { return m_saveStats; }
			set { m_saveStats = value; RaisePropertyChanged("SaveStats"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - cumulative percentage of words found
		/// </summary>
		public decimal   WorstPercent
		{
			get { return m_worstPercent; }
			set { m_worstPercent = value; RaisePropertyChanged("WorstPercent"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - cumulative best perecentage of words found
		/// </summary>
		public decimal   BestPercent
		{
			get { return m_bestPercent; }
			set { m_bestPercent = value; RaisePropertyChanged("BestPercent"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - cumulative average percentage of words found
		/// </summary>
		public decimal   AveragePercent
		{
			get { return m_averagePercent; }
			set { m_averagePercent = value; RaisePropertyChanged("AveragePercent"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - cumulative number of games played
		/// </summary>
		public int       GamesPlayed
		{
			get { return m_gamesPlayed; }
			set { m_gamesPlayed = value; RaisePropertyChanged("GamesPlayed"); RaisePropertyChanged("PointsPerGame"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - cumulative number of games won
		/// </summary>
		public int       GamesWon
		{
			get { return m_gamesWon; }
			set { m_gamesWon = value; RaisePropertyChanged("GamesWon"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - cumulative total possible words in all games played
		/// </summary>
		public int       TotalPossibleWords
		{
			get { return m_totalPossibleWords; }
			set { m_totalPossibleWords = value; RaisePropertyChanged("TotalPossibleWords"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - cumulative total found words in all games played
		/// </summary>
		public int       TotalFoundWords
		{
			get { return m_totalFoundWords; }
			set { m_totalFoundWords = value; RaisePropertyChanged("TotalFoundWords"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - cumulative datetime at which the saved statistics were reset
		/// </summary>
		public DateTime StatsDate
		{
			get { return m_statsDate; }
			set { m_statsDate = value; RaisePropertyChanged("StatsDate"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - cumulative total times game word was found
		/// </summary>
		public int       TotalFoundGameWord
		{
			get { return m_totalFoundGameWord; }
			set { m_totalFoundGameWord = value; RaisePropertyChanged("TotalFoundGameWord"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - cumulative most found words in all games played
		/// </summary>
		public int       MostFoundWords
		{
			get { return m_mostFoundWords; }
			set { m_mostFoundWords = value; RaisePropertyChanged("MostFoundWords"); }
		}

		//................................................................................
		/// <summary>
		/// Get/set - cumulative most found words in all games played
		/// </summary>
		public int TotalPoints
		{
			get { return m_totalPoints; }
			set { m_totalPoints = value; RaisePropertyChanged("TotalPoints"); RaisePropertyChanged("PointsPerGame"); }
		}

		//................................................................................
		/// <summary>
		/// Get - cumulative points per game (not saved, calculated as needed)
		/// </summary>
		public decimal PointsPerGame
		{
			get { return ((decimal)this.TotalPoints / Math.Max((decimal)this.GamesPlayed, 1)); }
		}

		public bool UseClassNameDictionary 
		{
			get { return m_useClassNameDictionary; }
			set { m_useClassNameDictionary = value; RaisePropertyChanged("UseClassNameDictionary"); }
		}
		#endregion Data member properties

		#region Constructor
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		public Settings()
		{
			this.LetterPoolMode   = Globals.IntToEnum(Anagram2Settings.Default.LetterPoolMode, LetterPoolMode.Random);
			this.TimerMode        = Globals.IntToEnum(Anagram2Settings.Default.LetterPoolMode, TimerMode.Static);
			this.LetterPoolCount  = Anagram2Settings.Default.LetterPoolCount;
			this.TrackUsedWords   = Anagram2Settings.Default.TrackUsedWords;
			this.BonusTime        = Anagram2Settings.Default.BonusTime;
			this.BonusWords       = Anagram2Settings.Default.BonusWords;
			this.PlayTickSound    = Anagram2Settings.Default.PlayTickSound;
			this.TickSoundFile    = Anagram2Settings.Default.TickSoundFile;
			this.StaticTimeValue  = Anagram2Settings.Default.StaticTimeValue;
			this.AwardBonusTime   = Anagram2Settings.Default.AwardBonusTime;
			this.SecondsPerLetter = Anagram2Settings.Default.SecondsPerLetter;
			this.SaveStats        = Anagram2Settings.Default.SaveStats;
			this.UseClassNameDictionary = Anagram2Settings.Default.UseClassNameDictionary;

			this.AveragePercent     = Anagram2Settings.Default.AveragePercent;
			this.BestPercent        = Anagram2Settings.Default.BestPercent;
			this.WorstPercent       = Anagram2Settings.Default.WorstPercent;
			this.GamesPlayed        = Anagram2Settings.Default.GamesPlayed;
			this.GamesWon           = Anagram2Settings.Default.GamesWon;
			this.TotalPossibleWords = Anagram2Settings.Default.TotalPossibleWords;
			this.TotalFoundWords    = Anagram2Settings.Default.TotalFoundWords;
			this.StatsDate          = Anagram2Settings.Default.StatsDate;
			this.TotalFoundGameWord = Anagram2Settings.Default.TotalFoundGameWord;
			this.MostFoundWords     = Anagram2Settings.Default.MostFoundWords;
			this.TotalPoints        = Anagram2Settings.Default.TotalPoints;
			if (this.GamesPlayed == 0)
			{
				this.StatsDate = DateTime.Now;
			}
		}
		#endregion Constructor

		#region Methods
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Saves the current user-configurable settings to the app.config file.
		/// </summary>
		public void SaveSettings()
		{
			Anagram2Settings.Default.LetterPoolMode    = (int)this.LetterPoolMode;
			Anagram2Settings.Default.LetterPoolMode    = (int)this.TimerMode;
			Anagram2Settings.Default.LetterPoolCount   = this.LetterPoolCount; 
			Anagram2Settings.Default.TrackUsedWords    = this.TrackUsedWords;
			Anagram2Settings.Default.BonusTime         = this.BonusTime;
			Anagram2Settings.Default.BonusWords        = this.BonusWords;
			Anagram2Settings.Default.PlayTickSound     = this.PlayTickSound;
			Anagram2Settings.Default.TickSoundFile     = this.TickSoundFile;
			Anagram2Settings.Default.StaticTimeValue   = this.StaticTimeValue;
			Anagram2Settings.Default.AwardBonusTime    = this.AwardBonusTime;
			Anagram2Settings.Default.SecondsPerLetter  = this.SecondsPerLetter;
			Anagram2Settings.Default.SaveStats         = this.SaveStats;
			Anagram2Settings.Default.UseClassNameDictionary = this.UseClassNameDictionary;
			Anagram2Settings.Default.Save();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Saves the cumulative game statistics to the app.config file.
		/// </summary>
		public void SaveStatistics()
		{
			Anagram2Settings.Default.AveragePercent     = this.AveragePercent;
			Anagram2Settings.Default.BestPercent        = this.BestPercent;
			Anagram2Settings.Default.WorstPercent       = this.WorstPercent;
			Anagram2Settings.Default.GamesPlayed        = this.GamesPlayed;
			Anagram2Settings.Default.GamesWon           = this.GamesWon;
			Anagram2Settings.Default.TotalPossibleWords = this.TotalPossibleWords;
			Anagram2Settings.Default.TotalFoundWords    = this.TotalFoundWords;
			Anagram2Settings.Default.TotalFoundGameWord = this.TotalFoundGameWord;
			Anagram2Settings.Default.MostFoundWords     = this.MostFoundWords;
			Anagram2Settings.Default.TotalPoints        = this.TotalPoints;
			Anagram2Settings.Default.StatsDate          = this.StatsDate;

			Anagram2Settings.Default.Save();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Resets the cumulative game statistics
		/// </summary>
		public void ResetStatistics()
		{
			this.AveragePercent     = 0M;
			this.BestPercent        = 0M;
			this.WorstPercent       = 0M;
			this.GamesPlayed        = 0;
			this.GamesWon           = 0;
			this.TotalPossibleWords = 0;
			this.TotalFoundWords    = 0;
			this.TotalFoundGameWord = 0;
			this.MostFoundWords     = 0;
			this.TotalPoints        = 0;
			this.StatsDate          = DateTime.Now;
			SaveStatistics();
		}
		#endregion Methods

	}

}
