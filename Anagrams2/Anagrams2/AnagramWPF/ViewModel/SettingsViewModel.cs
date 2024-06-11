using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace AnagramWPF
{

	public class SettingsViewModel : INotifyPropertyChanged
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

		public LetterPoolMode LetterPoolMode
		{
			get { return m_letterPoolMode; }
			set { m_letterPoolMode = value; RaisePropertyChanged("LetterPoolMode"); }
		}
		public TimerMode TimerMode
		{
			get { return m_timerMode; }
			set { m_timerMode = value; RaisePropertyChanged("TimerMode"); }
		}
		public int LetterPoolCount
		{
			get { return m_letterPoolCount; }
			set { m_letterPoolCount = value; RaisePropertyChanged("LetterPoolCount"); }
		}
		public bool TrackUsedWords
		{
			get { return m_trackUsedWords; }
			set { m_trackUsedWords = value; RaisePropertyChanged("TrackUsedWords"); }
		}
		public int BonusTime
		{
			get { return m_bonusTime; }
			set { m_bonusTime = value; RaisePropertyChanged("BonusTime"); }
		}
		public int BonusWords
		{
			get { return m_bonusWords; }
			set { m_bonusWords = value; RaisePropertyChanged("BonusWords"); }
		}
		public bool PlayTickSound
		{
			get { return m_playTickSound; }
			set { m_playTickSound = value; RaisePropertyChanged("PlayTickSound"); }
		}
		public string TickSoundFile
		{
			get { return m_tickSoundFile; }
			set { m_tickSoundFile = value; RaisePropertyChanged("TickSoundFile"); }
		}
		public int StaticTimeValue
		{
			get { return m_staticTimeValue; }
			set { m_staticTimeValue = value; RaisePropertyChanged("StaticTimeValue"); }
		}
		public bool AwardBonusTime
		{
			get { return m_awardBonusTime; }
			set { m_awardBonusTime = value; RaisePropertyChanged("AwardBonusTime"); }
		}
		public int SecondsPerLetter
		{
			get { return m_secondsPerLetter; }
			set { m_secondsPerLetter = value; RaisePropertyChanged("SecondsPerLetter"); }
		}

		public SettingsViewModel()
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
		}

		public void Save()
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
		}
	}

}
