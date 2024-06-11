using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using AnagramWPF.ViewModel;

namespace AnagramWPF
{
	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
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
		private GameDictionary m_gameDictionary = null;
		private Settings       m_settings       = null;
		#endregion Data members

		#region Properties
		//................................................................................
		/// <summary>
		/// Get/set - specifies the dictionary in use for the current game.
		/// </summary>
		public GameDictionary CurrentGameDictionary
		{
			get { return m_gameDictionary; }
			set
			{
				m_gameDictionary = value;
				RaisePropertyChanged("GameDictionary");
			}
		}
		#endregion Properties

		#region Constructor
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		public MainWindow()
		{
			if (Globals.MainDictionary.CanPlay)
			{
				MessageBox.Show(string.Format("{0} words found in dictionary. Gird your loins.", Globals.MainDictionary.Count), "Word Up!");
			}
			else
			{
				MessageBox.Show(string.Format("No words found in dictionary (is the file missing?).\r\nTerminating application.", Globals.MainDictionary.Count), "OH NO!!!");
			}
			m_settings = new Settings();
			InitializeComponent();
			this.pageCumulativeStats.DataContext = m_settings;
			UpdateButtons();
			FocusUserWord();
			
		}
		#endregion Constructor

		#region Methods
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Updates the buttons depending on the current game's status
		/// </summary>
		private void UpdateButtons()
		{
			this.buttonNewGame.IsEnabled = ((CurrentGameDictionary == null) || (CurrentGameDictionary != null && !CurrentGameDictionary.IsPlaying));
			this.buttonSetup.IsEnabled   = ((CurrentGameDictionary == null) || (CurrentGameDictionary != null && !CurrentGameDictionary.IsPlaying));
			this.buttonSolve.IsEnabled   = (CurrentGameDictionary != null && CurrentGameDictionary.IsPlaying);
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Solves the game and updates the buttons
		/// </summary>
		private void Solve()
		{
			CurrentGameDictionary.Solve();
			UpdateButtons();
			if (this.wordList.Items.Count > 0)
			{
				this.wordList.ScrollIntoView(this.wordList.Items[this.wordList.Items.Count-1]);
			}
		}
		#endregion Methods

		#region Events
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Fired when the user clicks the New Game button. Creates a new game dictionary, 
		/// initializes the form, and starts the game.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonNewGame_Click(object sender, RoutedEventArgs e)
		{
			if (this.CurrentGameDictionary != null && this.CurrentGameDictionary.IsPlaying)
			{
				wordList.ItemsSource = null;
				this.DataContext     = null;
			}
			this.CurrentGameDictionary = new GameDictionary(m_settings);
			this.DataContext           = this.CurrentGameDictionary;
			wordList.ItemsSource       = CurrentGameDictionary;
			this.pageCurrentGameStats.Focus();
			this.textboxUserWord.Focus();
			UpdateButtons();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Fired when the user clicks the Done button. Stops the game in progress and 
		/// closes the application.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonDone_Click(object sender, RoutedEventArgs e)
		{
			if (this.CurrentGameDictionary != null)
			{
				this.CurrentGameDictionary.StopTimer();
			}
			Close();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Fired when the user clicks the Solve button. Marks all words as found, and 
		/// updates the buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonSolve_Click(object sender, RoutedEventArgs e)
		{
			this.textboxUserWord.Text = "";
			Solve();
			this.textboxUserWord.Focus();
			this.pageCumulativeStats.Focus();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Fired when user presses RETURN. Submits the user word for scoring and updates 
		/// the game statsitics as required.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonSubmit_Click(object sender, RoutedEventArgs e)
		{
			CurrentGameDictionary.ValidAndScoreWord(textboxUserWord.Text);
			textboxUserWord.Text = "";
			textboxUserWord.Focus();
			if (CurrentGameDictionary.IsWinner)
			{
				WindowWinner form = new WindowWinner();
				form.ShowDialog();
			}
			this.textboxUserWord.Focus();
			CurrentGameDictionary.SetFilterText(textboxUserWord.Text);
			UpdateButtons();
			this.wordList.ScrollIntoView(CurrentGameDictionary.LastWordFound);
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Fired when the user clicks the Setup button. Displays the setup form, which 
		/// allows the user to modify game parameters.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonSetup_Click(object sender, RoutedEventArgs e)
		{
			WindowSetup form = new WindowSetup(m_settings);
			form.ShowDialog();
			this.textboxUserWord.Focus();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Fired when the progressbar value changes. Updates the form, and if time has 
		/// run out, the game is solved and stopped.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.progressBar.Value == 0d)
			{
				buttonSolve_Click(null, null);
			}
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Fired when the user types a letter into the user word textbox. This allows 
		/// the listbox to filter the found words as the user is typing in an attempt to 
		/// help him/her not enter a word that's already been found (thus loosing a 
		/// point).
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textboxUserWord_TextChanged(object sender, TextChangedEventArgs e)
		{
			CurrentGameDictionary.SetFilterText(textboxUserWord.Text);
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Fired when the Reset Stats button on the cumulkative tab age is clicked. It 
		/// causes the cumulative statistics to reset to zero.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ResetStats_Click(object sender, RoutedEventArgs e)
		{
			m_settings.ResetStatistics();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Fired when either of the stats tab pages gets focus. Focus is reset to the 
		/// user word text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabpage_GotFocus(object sender, RoutedEventArgs e)
		{
			FocusUserWord();
		}
		#endregion Events

		private void Window_GotFocus(object sender, RoutedEventArgs e)
		{
			FocusUserWord();
		}

		private void FocusUserWord()
		{
			this.textboxUserWord.Focus();
			this.textboxUserWord.Text = this.textboxUserWord.Text.Trim();
			this.textboxUserWord.CaretIndex = this.textboxUserWord.Text.Length;
		}

	}
}
