using System;
using System.Windows.Data;
using System.Globalization;

namespace AnagramWPF.Converters
{
	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Takes an wordcount list in the statistics and returns the number of words found 
	/// as indicated by the letter count in the parameter.
	/// </summary>
	[ValueConversion(typeof(ViewModel.WordCounts), typeof(string))]
	public class WordCountStatToDisplay : IValueConverter
	{
		//--------------------------------------------------------------------------------
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ViewModel.WordCounts wordCounts  = value as ViewModel.WordCounts;
			ViewModel.WordCountItem wordCountItem = wordCounts.GetItemForCount(System.Convert.ToInt32(parameter));
			string result = string.Format("{0} / {1}", wordCountItem.WordCount, wordCountItem.PossibleWords);
			return result;
		}
 
		//--------------------------------------------------------------------------------
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}