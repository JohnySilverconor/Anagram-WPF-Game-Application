using System;
using System.Windows.Data;
using System.Globalization;


namespace AnagramWPF.Converters
{
	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Adds a double-asterisk to the word displayed in the listbox on the main form if 
	/// that word is the original word (the word that was scrambled).
	/// </summary>
	[ValueConversion(typeof(bool), typeof(string))]
	public class OriginalWordConverter : IValueConverter
	{
		//--------------------------------------------------------------------------------
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ViewModel.DisplayWord word = value as ViewModel.DisplayWord;
			string result = word.Text;
			if (word.IsOriginalWord)
			{
				result += "**";
			}
			return result;
		}
 
		//--------------------------------------------------------------------------------
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
