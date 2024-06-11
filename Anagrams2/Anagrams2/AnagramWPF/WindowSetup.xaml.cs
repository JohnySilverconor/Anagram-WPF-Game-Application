using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using AnagramWPF.ViewModel;

namespace AnagramWPF
{
	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Interaction logic for WindowSetup.xaml
	/// </summary>
	public partial class WindowSetup : Window
	{
		Settings SetupData { get; set; }

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data"></param>
		public WindowSetup(Settings data)
		{
			this.SetupData = data;
			InitializeComponent();
			this.DataContext = this.SetupData;
			InitRadioButtons();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Initilaizes the radio buttons
		/// </summary>
		private void InitRadioButtons()
		{
			radiobuttonRandom.IsChecked           = (SetupData.LetterPoolMode == ViewModel.LetterPoolMode.Random);
			radiobuttonStatic.IsChecked           = (SetupData.LetterPoolMode == ViewModel.LetterPoolMode.Static);
			radiobuttonNoTimer.IsChecked          = (SetupData.TimerMode == TimerMode.NoTimer);
			radiobuttonStaticTime.IsChecked       = (SetupData.TimerMode == TimerMode.Static);
			radiobuttonSecondsPerLetter.IsChecked = (SetupData.TimerMode == TimerMode.PerLetter);
			radiobuttonStaticAndSeconds.IsChecked = (SetupData.TimerMode == TimerMode.StaticPerLetter);
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Saves radio button data to the view model. Everything else is handled in the 
		/// view model.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonOK_Click(object sender, RoutedEventArgs e)
		{
			this.SetupData.LetterPoolMode = (radiobuttonRandom.IsChecked == true) ? ViewModel.LetterPoolMode.Random : ViewModel.LetterPoolMode.Static;
			if (radiobuttonNoTimer.IsChecked == true)
			{
				this.SetupData.TimerMode = TimerMode.NoTimer;
			}
			else if (radiobuttonStaticTime.IsChecked == true)
			{
				this.SetupData.TimerMode = TimerMode.Static;
			}
			else if (radiobuttonSecondsPerLetter.IsChecked == true)
			{
				this.SetupData.TimerMode = TimerMode.PerLetter;
			}
			else if (radiobuttonStaticAndSeconds.IsChecked == true)
			{
				this.SetupData.TimerMode = TimerMode.StaticPerLetter;
			}

			this.SetupData.SaveSettings();
			Close();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Fired when the user clicks the Save Statistics button. All cumulative game 
		/// statistics are zeroed and saved.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonResetStats_Click_1(object sender, RoutedEventArgs e)
		{
			this.SetupData.ResetStatistics();
		}
	}
}
