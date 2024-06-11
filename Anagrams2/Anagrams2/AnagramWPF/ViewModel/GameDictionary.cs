using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;

using AnagramWPF.Model;

namespace AnagramWPF.ViewModel
{
	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represents the list of words that are to be used by the currnet game. This object 
	/// is re-created each time the user clicks the "New Game" button.
	/// </summary>
	public class GameDictionary : ObservableCollection<DisplayWord>
	{
		#region INotifyPropertyChanged code
		protected override event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		#endregion INotifyPropertyChanged code

		#region Bonus point constants
		private const int ALL_LETTERS_BONUS   = 10;
		private const int ORIGINAL_WORD_BONUS = 25;
		#endregion Bonus point constants

		#region Data members
		private GameStatistics   m_statistics       = null;
		private int              m_wordCount        = 0;
		private bool             m_isPlaying        = false;
		DispatcherTimer          m_timer            = new DispatcherTimer();
		int                      m_secondsRemaining = 0;
		int                      m_secondsAtStart   = 0;
		double                   m_percentRemaining = 0d;
		System.Media.SoundPlayer m_soundPlayer      = new System.Media.SoundPlayer();
		private DisplayWord      m_lastWordFound    = null;
		#endregion Data members

		#region Properties
		//................................................................................
		/// <summary>
		/// Get- represents the flag that indicates that the game is in progress
		/// </summary>
		public bool IsPlaying
		{
			get { return m_isPlaying; }
			private set { m_isPlaying = value; RaisePropertyChanged("IsPlaying"); }
		}

		//................................................................................
		/// <summary>
		/// Get - represents the flag that inbdicates that the user has found all of the 
		/// possible words (the only true "winner" condition).
		/// </summary>
		public bool IsWinner
		{
			get { return (this.Statistics.WordCount.WordsFound == this.Count); }
		}

		//................................................................................
		/// <summary>
		/// Get - specifies the game statistics object
		/// </summary>
		public GameStatistics Statistics
		{
			get { return m_statistics; }
 			private set
			{
				m_statistics = value;
				RaisePropertyChanged("Statistics");
			}
		}

		//................................................................................
		/// <summary>
		/// Get - specifes the number of words in this game
		/// </summary>
		public int WordCount 
		{
			get { return m_wordCount; }
			private set
			{
				m_wordCount = this.Count;
				RaisePropertyChanged("WordCount");
			}
		}

		//................................................................................
		/// <summary>
		/// Get/set - represents the last word found during a game.
		/// </summary>
		public DisplayWord LastWordFound 
		{ 
			get { return m_lastWordFound; }
			set
			{
				if (m_lastWordFound != null)
				{
					m_lastWordFound.IsLastWordFound = false;
				}
				m_lastWordFound                 = value;
				if (m_lastWordFound != null)
				{
					m_lastWordFound.IsLastWordFound = true;
				}
				// we don't need to be notified that the last word  has changed here.
			}
		}

		//................................................................................
		/// <summary>
		/// Gets the flag indicating whether or not the game can proceed
		/// </summary>
		public bool CanPlay { get; private set; }

		//................................................................................
		/// <summary>
		/// Get - respresents the current game word
		/// </summary>
		public DisplayWord GameWord { get; private set; }

		//................................................................................
		/// <summary>
		/// Get - represents the current application settings (stored in the app.config 
		/// file)
		/// </summary>
		public Settings Settings    { get; private set; }

		#region Timer
		//................................................................................
		/// <summary>
		/// Get - specifies the number of seconds remaining in the game
		/// </summary>
		public int SecondsRemaining
		{
			get { return m_secondsRemaining; }
			private set { m_secondsRemaining = value; RaisePropertyChanged("SecondsRemaining"); }
		}

		//................................................................................
		/// <summary>
		/// Get - specifies the number of seconds at the start of the game
		/// </summary>
		public int SecondsAtStart
		{
			get { return m_secondsAtStart; }
			private set { m_secondsAtStart = value; RaisePropertyChanged("SecondsAtStart"); }
		}

		//................................................................................
		/// <summary>
		/// Get - specifies the percentage of time remaining
		/// </summary>
		public double PercentRemaining
		{
			get { return m_percentRemaining; }
			private set { m_percentRemaining = value; RaisePropertyChanged("PercentRemaining"); }
		}
		#endregion Timer
		#endregion Properties

		#region Constructors
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Default constructor
		/// </summary>
		public GameDictionary()
		{
			this.IsPlaying = false;
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">App settings</param>
		public GameDictionary(Settings settings)
		{
			Settings           = settings;

			// determine our actual letter count
			int letterCount    = SetLetterCount();

			// randomly select a word 
			AWord selectedWord = SelectNewWord(letterCount);

			// And finally, we need to construct our list of possible words (words that 
			// are contained in the scrambled word). This method also scrambles the 
			// selected word when it's added to this collection.
			FindPossibleWords(selectedWord);

			// create the statistic object
			//Statistics = new GameStatistics(this.Count);
			Statistics = new GameStatistics(this);

			// Now, we're ready to start the game
			this.CanPlay =  (this.Count > 0);
			ResetGame();
		}
		#endregion Constructors

		#region Methods
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Sets the desired letter count of the game word
		/// </summary>
		/// <returns>The derived letter count</returns>
		private int SetLetterCount()
		{
			int letterCount = 0;
			// First, we have to determine the length of the word we want.
			// To make the game more challenging, we don't allow a word that's 
			// shorter than six charaters to be used as the scrambled word.
			int shortest = Math.Max(Globals.MainDictionary.ShortestWordSize, 6);
			int longest  = Globals.MainDictionary.LongestWordSize;
			switch (this.Settings.LetterPoolMode)
			{
				case LetterPoolMode.Random : 
					letterCount = Globals.RandomNumber(shortest, longest);
					break;

				case LetterPoolMode.Static :
					letterCount = Settings.LetterPoolCount;
					break;
			}
			return letterCount;
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Attempts to select an unused word (a word that hasn't been scrambled in the 
		/// current session) to be scrambled. No matter what happens, a word should be 
		/// returned.  
		/// </summary>
		/// <param name="count">The size of the word being selected</param>
		/// <returns>The word selected from the maindictionary</returns>
		private AWord SelectNewWord(int count)
		{
			AWord selectedWord = null;

			// get a list of words that we can use that are the specified length
			List<AWord> words = Globals.MainDictionary.GetWordsByLength(count);
			if (this.Settings.TrackUsedWords)
			{
				// if we don't have any words left
				if (words.Count == 0)
				{
					// clear the words
					words.Clear();
					// if we're using a random letter count, we can just pick a new letter 
					// count, and try again
					if (this.Settings.LetterPoolMode == LetterPoolMode.Random)
					{
						// clean up memory since we don't need these collections any longer
						// set a new count
						count = SetLetterCount();
						// and recurse this method
						selectedWord = SelectNewWord(count);
					}
					else
					{
						// and regenerate our acceptable word list (without removing anything, 
						// of course)
						words = Globals.MainDictionary.GetWordsByLength(count);
						// select a word
						selectedWord = words.ElementAt(Globals.RandomNumber(0, words.Count-1));
					}
				}
				else // otherwise we have words to pick from that haven't yet been used
				{
					// select a word
					selectedWord = words.ElementAt(Globals.RandomNumber(0, words.Count-1));
				}
			}
			else // we're not tracking used words, so just pick one
			{
				// select a word
				selectedWord = words.ElementAt(Globals.RandomNumber(0, words.Count-1));
			}

			// if we've selected a word
			if (selectedWord != null)
			{
				// update the list of used words
				selectedWord.Used = true;
			}
			return selectedWord;
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Finds all possible words that can be contained in the selected word, and adds 
		/// them to this collection
		/// </summary>
		/// <param name="selectedWord">The word we're looking in</param>
		public void FindPossibleWords(AWord selectedWord)
		{
			this.Clear();
			if (selectedWord != null)
			{
				//var possibleWords = (from item in Globals.MainDictionary
				//					 where Globals.Contains(selectedWord.Text, item.Text)
				//					 select item).ToList<AWord>();
				List<AWord> possibleWords = Globals.MainDictionary.GetPossibleWords(selectedWord);
				foreach(AWord word in possibleWords)
				{
					// while we're here, scramble the selected word
					DisplayWord displayWord = new DisplayWord(word, (word.Text == selectedWord.Text));
					if (displayWord.IsOriginalWord)
					{
						this.GameWord = displayWord;
					}
					this.Add(displayWord);
					Debug.WriteLine("{0} words", this.Count);
				}
			}

			// for testing  (there should only be one item in the resulting list)
			//var originalWord = (from item in this where item.IsOriginalWord select item).ToArray<DisplayWord>();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Initializes game properties and starts the game
		/// </summary>
		public void ResetGame()
		{
			//this.GamePoints = 0;
			//this.WordsFound = 0;
			this.WordCount = this.Count;
			this.Statistics.Reset();
			InitTimer();
			StartTimer();
			this.IsPlaying = true;
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Validates the user word, scores the word, and awards bonus time.
		/// </summary>
		/// <param name="text">The user word</param>
		/// <returns>True if the word is valid</returns>
		public bool ValidAndScoreWord(string text)
		{
			text       = text.ToUpper();
			int points = 0;
			bool valid = (!string.IsNullOrEmpty(text));
			DisplayWord foundWord = null;
			if (valid)
			{
				foundWord = (from item in this 
							 where (item.Text == text && !item.Found) 
							 select item).FirstOrDefault();
				valid = (foundWord != null);
			}
			if (valid)
			{
				foundWord.Found = true;
				foundWord.SetFoundColor();

				// v1.1 - set the last word found
				this.LastWordFound = foundWord;

				points += foundWord.Points;
				// the player can bonus points if he specifies the original word
				if (foundWord.IsOriginalWord)
				{
					// bonus points
					points += ORIGINAL_WORD_BONUS;
				}
				// OR he can get bonus points for specifying a word using all of the 
				// letters that isn't the original word.
				else
				{
					if (foundWord.Text.Length == GameWord.Text.Length)
					{
						// bonus points
						points += ALL_LETTERS_BONUS;
					}
				}
				this.Statistics.Update(foundWord.Text.Length, points);

				// determine bonus time
				if (Settings.TimerMode != TimerMode.NoTimer)
				{
					int wordRemainder;
					Math.DivRem(this.Statistics.WordCount.WordsFound, this.Settings.BonusWords, out wordRemainder);
					if (wordRemainder == 0)
					{
						this.SecondsRemaining += this.Settings.BonusTime;
					}
					if (foundWord.IsOriginalWord)
					{
						this.SecondsRemaining += 60;
					}
					else 
					{
						if (foundWord.Text.Length == GameWord.Text.Length)
						{
							// bonus points
							this.SecondsRemaining += 30;
						}
					}
					this.SecondsAtStart = this.SecondsRemaining;
				}
			}
			else
			{
				points--;
				this.Statistics.Update(0, points);
			}
			if (this.IsWinner)
			{
				StopTimer();
			}
			return (foundWord != null);
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Solves the current game, and stops the timer
		/// </summary>
		public void Solve()
		{
			Statistics.UpdateSolvedStats(this.WordCount,  this.GameWord.Found);
			foreach(DisplayWord word in this)
			{
				word.Found = true;
			}
			this.LastWordFound = null;
			StopTimer();
			//FakeWin();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Used for early testing, and "finds" all words.
		/// </summary>
		public void FakeWin()
		{
			foreach(DisplayWord word in this)
			{
				ValidAndScoreWord(word.Text);
			}
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Sets the property that inidcates that the word starts with the specified text.
		/// </summary>
		/// <param name="text">The text to look for</param>
		public void SetFilterText(string text)
		{
			foreach(DisplayWord word in this)
			{
				word.SatisfiesFilter = (string.IsNullOrEmpty(text) || word.Text.StartsWith(text.ToUpper()));
			}
		}

		#endregion Methods

		#region Game Timer code
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the timer by caluclating the initial time to play available for 
		/// the game.  the timer is initialized whether we need it or not (as specified 
		/// in the setup form).
		/// </summary>
		public void InitTimer()
		{
			m_timer.Interval = TimeSpan.FromMilliseconds(1000); 
			switch (this.Settings.TimerMode)
			{
				case TimerMode.NoTimer         : this.SecondsAtStart = 0; break;
				case TimerMode.Static          : this.SecondsAtStart = this.Settings.StaticTimeValue; break;
				case TimerMode.PerLetter       : this.SecondsAtStart = this.GameWord.Text.Length * this.Settings.SecondsPerLetter; break;
				case TimerMode.StaticPerLetter : this.SecondsAtStart = this.Settings.StaticTimeValue + (this.GameWord.Text.Length * this.Settings.SecondsPerLetter); break;
			}
			this.SecondsRemaining = this.SecondsAtStart;
			if (this.Settings.PlayTickSound && System.IO.File.Exists(this.Settings.TickSoundFile))
			{
				m_soundPlayer.SoundLocation = this.Settings.TickSoundFile;
			}
		} 

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Starts the timer, and contains the code that plays on the Tick event. If the 
		/// game is configured to not use the timer, it isn't started. When time runs out, 
		/// the tick handler code stops the timer.
		/// </summary>
		public void StartTimer()
		{
			if (this.Settings.TimerMode != TimerMode.NoTimer)
			{
				m_timer.Tick += new EventHandler 
				(
					delegate(object s, EventArgs a) 
					{ 
						this.SecondsRemaining--;
						this.PercentRemaining = ((double)this.SecondsRemaining / Math.Max((double)this.SecondsAtStart, 1)) * 100d;
						if (Settings.PlayTickSound)
						{
							m_soundPlayer.Play();
						}
						if (this.SecondsRemaining == 0)
						{
							StopTimer();
						}
					}
				);
				m_timer.Start();
			}
			this.IsPlaying = true;
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Stops the timer (and the player if it's playing)
		/// </summary>
		public void StopTimer()
		{
			this.SecondsRemaining = 0;
			this.PercentRemaining = 0d;
			if (this.Settings.TimerMode != TimerMode.NoTimer)
			{
				m_soundPlayer.Stop();
				m_timer.Stop();
			}
			this.IsPlaying = false;
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Returns the number of words of the specified lenth that are in this game.
		/// </summary>
		/// <param name="length">The size we're looking for</param>
		/// <returns>How many we have</returns>
		public int CountWordsOfLength(int length)
		{
			int count = (from item in this 
						 where item.Text.Length == length 
						 select item).Count();

			return count;
		}
		#endregion Game Timer code
	}
}
