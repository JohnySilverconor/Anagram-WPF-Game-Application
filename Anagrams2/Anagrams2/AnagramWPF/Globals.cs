using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Linq;

using AnagramWPF.Model;

namespace AnagramWPF
{
	public enum LetterPoolMode { Random=0, Static=1 };
	public enum TimerMode      { NoTimer=0, Static=1, PerLetter=2, StaticPerLetter=3 };

	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represents a static global variable class.
	/// </summary>
	public static class Globals
	{
		private static int[] m_wordPoints = { 1, 3, 3, 2, 1, 4, 2, 4, 1, 8, 5, 1, 3, 1, 1, 3, 10, 1, 1, 1, 1, 4, 4, 8, 4, 10 };
		//................................................................................
		/// <summary>
		/// Get/set the length of the shortest word in the main dictionary
		/// </summary>
		public static int ShortestWord { get; set; }
		//................................................................................
		/// <summary>
		/// Get/set the length of the longest word in the main dictionary
		/// </summary>
		public static int LongestWord { get; set; }
		//................................................................................
		/// <summary>
		/// Gets the array of points assigned to each letter
		/// </summary>
		public static int[]  WordPoints     { get { return m_wordPoints; } private set { m_wordPoints = value; } }
		//................................................................................
		/// <summary>
		/// Get/set the main dictonary (the app model)
		/// </summary>
		public static MainDictionary MainDictionary { get; set; }


		//--------------------------------------------------------------------------------
		static Globals()
		{
			MainDictionary = new MainDictionary();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Returns a random number
		/// </summary>
		/// <param name="min">Minimum possible random value</param>
		/// <param name="max">Maximum possible random value</param>
		/// <returns></returns>
		public static int RandomNumber(int min, int max)
		{
			return new Random().Next(min, max);
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Calculates the points for the specified word 
		/// </summary>
		/// <param name="text">The word to score</param>
		/// <returns>The points</returns>
		public static int CalcWordScore(string text)
		{
			text = text.ToUpper();
			int points = 0;
			foreach(char character in text)
			{
				int charInt = Convert.ToInt16(character);
				points += m_wordPoints[charInt - ((charInt >= 97) ? 97 : 65)];
			}
			return points;
		}


		//--------------------------------------------------------------------------------
		/// <summary>
		/// Checks to see if inText is contained in wordText. Due to the nature of the 
		/// application, this must be done one character at a time.
		/// </summary>
		/// <param name="wordText">The container string</param>
		/// <param name="inText">The text that the container might contain</param>
		/// <returns></returns>
		public static bool Contains(string container, string inText)
		{
			// The idea is to simply run through the two strings and delete matching 
			// characters as they're found.  If the inText string ends up as an empty 
			// string, the word did in fact contain the specified text.
			bool   contains = false;
			List<char> containerList = new List<char>();
			containerList.AddRange(container);
			containerList.Sort();

			List<char> testList = new List<char>();
			testList.AddRange(inText);
			testList.Sort();

			bool found = false;
			do
			{
				found = false;
				for (int i = 0; i < containerList.Count; i++)
				{
					if (testList[0] == containerList[i])
					{
						testList.RemoveAt(0);
						containerList.RemoveAt(i);
						found = true;
						break;
					}
				}
			} while (found && testList.Count > 0);
		
			contains = (testList.Count == 0);
			return contains;
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Sets an enum from an ordinal value (if possible).
		/// </summary>
		/// <typeparam name="T">The enum type the method must return</typeparam>
		/// <param name="value">The integer value that represents the enum ordinal</param>
		/// <param name="defaultValue">The value to return if the ordinal isn't valid</param>
		/// <returns></returns>
		public static T IntToEnum<T>(int value, T defaultValue)
		{
			T enumValue = (Enum.IsDefined(typeof(T), value)) ? (T)(object)value : defaultValue;
			return enumValue;
		}
	}

}
