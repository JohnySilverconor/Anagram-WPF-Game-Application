using System.ComponentModel;
using System.Text;

using AnagramWPF.Model;

namespace AnagramWPF.ViewModel
{
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represnts the words displayed in the listbox.
	/// </summary>
	public class DisplayWord : AWord, INotifyPropertyChanged
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

		#region Color constants
        private const string NORMAL_COLOR       = "#AAAAAA";
        private const string FOUND_COLOR        = "#0000FF";
        private const string ORIGINALWORD_COLOR = "#FF0000";
		#endregion Color constants

		#region Data members
		private string m_scrambled;
		private bool   m_found;
		private bool   m_isOriginalWord;
		private string m_foreground;
		private bool   m_statisfiesFilter = true;
		private bool   m_isLastWordFound    = false;
		#endregion Data members

		#region DataMember properties
		//................................................................................
		/// <summary>
		/// Get/set - specifies the color used to display the word in the listbox. This 
		/// color depends on the word's status:
		/// 
		///   Found=false - color is gray
		///   Found=true - color is blue, and 
		///   IsOriginalWord=true - color is red
		/// </summary>
		public string Foreground
		{
			get { return m_foreground; }
			set 
			{ 
				m_foreground = value; 
				RaisePropertyChanged("Foreground"); 
			}
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies the the scrambled verson of the word. This will only have 
		/// a value if it's the current game word.
		/// </summary>
		public string Scramble       
		{ 
			get { return m_scrambled; }
			set 
			{ 
				m_scrambled = value; 
				RaisePropertyChanged("Scramble"); 
			}
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies whether or not this word satisfies the currently specified 
		/// user word text.
		/// </summary>
		public bool SatisfiesFilter 
		{ 
			get { return m_statisfiesFilter; }
			set
			{
				m_statisfiesFilter = value;
				RaisePropertyChanged("Found");
			}
		}

		//................................................................................
		/// <summary>
		/// Get/set - specifies whether or not this word has been found during the 
		/// current game (set when a valid word is provided by the user).
		/// </summary>
		public bool   Found          
		{ 
			get { return (m_found && SatisfiesFilter);  }
            set 
			{ 
				m_found = value; 
				RaisePropertyChanged("Found"); 
			}
		}
		//................................................................................
		/// <summary>
		/// Get/set - indicates whether or not this word is the original game word for 
		/// the current game (set when the word is scrambled).
		/// </summary>
		public bool   IsOriginalWord 
		{ 
			get { return m_isOriginalWord; }
			set 
			{ 
				m_isOriginalWord = value; 
				RaisePropertyChanged("IsOriginalWord"); 
			}
		}

		//................................................................................
		/// <summary>
		/// Get/set - represents an indicator used to indicate whether or not this is the 
		/// last word found by the user in a game.
		/// </summary>
		public bool IsLastWordFound
		{
			get { return m_isLastWordFound; }
			set
			{
				m_isLastWordFound = value;
				RaisePropertyChanged("IsLastWordFound");
			}
		}
		#endregion DataMember properties

		#region Constructor
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="word"></param>
		/// <param name="scramble"></param>
		public DisplayWord(AWord word, bool scramble=false):base(word)
		{
			Reset();
			if (scramble)
			{
				ScrambleIt();
			}
		}
		#endregion Constructor

		#region Methods
		//--------------------------------------------------------------------------------
		/// <summary>
		///  Overriden to return the value of the Text property.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.Text;
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether or not the word contains the spceified text
		/// </summary>
		/// <param name="inText"></param>
		/// <returns></returns>
		public bool Contains(string inText)
		{
			return Globals.Contains(this.Text, inText);
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Sets the found color. 
		/// </summary>
		public void SetFoundColor()
		{
			Foreground = (IsOriginalWord) ? ORIGINALWORD_COLOR : FOUND_COLOR;
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Scarmbles the word (until the scrambled version is not equal to the original 
		/// word).
		/// </summary>
		public void ScrambleIt()
		{
			StringBuilder scramble = new StringBuilder();
			string temp = "";
			do // do this until the scrambled text isn't the same as the word itself
			{

				scramble.Clear();
				// reset the temporary string
				temp = this.Text;
	
				do // do this until we run out of letters
				{
					if (temp.Length > 1)
					{
						int index = (temp.Length > 1) ? Globals.RandomNumber(0, temp.Length-1) : 0;
						scramble.Append(temp[index]);
						temp = temp.Remove(index, 1);
					}
					else
					{
						scramble.Append(temp);
						temp = "";
					}
				} while (!string.IsNullOrEmpty(temp));

			} while (scramble.ToString() == this.Text);

			this.Scramble       = scramble.ToString();
			this.IsOriginalWord = true;
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Resets the word properties.
		/// </summary>
		public void Reset()
		{
			this.Found           = false;
			this.SatisfiesFilter = true;
			this.IsOriginalWord  = false;
			this.IsLastWordFound = false;
			this.Scramble        = "";
			this.Foreground      = NORMAL_COLOR;
		}
		#endregion Methods

	}
}
