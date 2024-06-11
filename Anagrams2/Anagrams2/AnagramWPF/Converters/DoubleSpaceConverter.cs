using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Text;

namespace AnagramWPF.Converters
{
	[ValueConversion(typeof(ViewModel.DisplayWord), typeof(string))]
	class DoubleSpaceConverter : IValueConverter
	{
		//--------------------------------------------------------------------------------
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ViewModel.DisplayWord word = (ViewModel.DisplayWord)value;
			string result = "";
			if (word != null)
			{
				string originalString = word.Scramble;
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < originalString.Length; i++)
				{
					builder.AppendFormat("{0} ", originalString[i]);
				}
				result = builder.ToString().Trim();
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
