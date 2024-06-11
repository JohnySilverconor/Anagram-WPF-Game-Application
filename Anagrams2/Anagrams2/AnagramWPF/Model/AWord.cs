namespace AnagramWPF.Model
{
	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represents a word in the dictionary file.
	/// </summary>
	public class AWord
	{
		#region Properties
		//................................................................................
		/// <summary>
		/// Get/set - specifies the text of the word
		/// </summary>
		public string Text   { get; private set; }

		//................................................................................
		/// <summary>
		/// Get - specifies the points this word is worth
		/// </summary>
		public int    Points { get; private set; }

		//................................................................................
		/// <summary>
		/// Get/set - specifies whether this word has been used as a game word in the 
		/// current application session.
		/// </summary>
		public bool   Used   { get; set; }

		public bool IsClassName { get; private set; }

		#endregion Properties

		#region Constructors
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="text">The word text</param>
		public AWord(string text, bool isClassName = false)
		{
			this.Text        = text;
			this.Used        = false;
			this.IsClassName = isClassName;
			this.Points      = Globals.CalcWordScore(this.Text);
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="word">the word being copied</param>
		public AWord(AWord word, bool isClassName = false)
		{
			this.Text        = word.Text;
			this.Points      = word.Points;
			this.Used        = word.Used;
			this.IsClassName = word.IsClassName;
		}
		#endregion Constructors
	}

}
