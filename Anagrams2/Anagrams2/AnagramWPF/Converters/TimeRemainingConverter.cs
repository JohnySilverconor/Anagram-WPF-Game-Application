using System;
using System.Windows.Data;
using System.Globalization;

namespace AnagramWPF.Converters
{

	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Converts the specified value to a formatted string that represents the number 
	/// of minutes and seconds remaining on the timer.
	/// </summary>
	[ValueConversion(typeof(int), typeof(string))]
	public class TimeRemainingConverter : IValueConverter
	{

		//--------------------------------------------------------------------------------
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			TimeSpan span   = new TimeSpan(0, 0, (int)value);
			string   result = string.Format("{0:00}:{1:00}", span.Minutes, span.Seconds);
			return result;
		}
 
		//--------------------------------------------------------------------------------
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

}
