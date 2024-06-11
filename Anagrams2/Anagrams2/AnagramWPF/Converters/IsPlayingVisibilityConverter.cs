using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace AnagramWPF.Converters
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	class IsPlayingVisibilityConverter : IValueConverter
	{
		//--------------------------------------------------------------------------------
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Visibility result = ((bool)value == true) ? Visibility.Collapsed : Visibility.Visible;
			return result;
		}
 
		//--------------------------------------------------------------------------------
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}


