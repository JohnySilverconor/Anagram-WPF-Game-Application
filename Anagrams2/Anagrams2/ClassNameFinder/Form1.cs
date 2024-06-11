using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace ClassNameFinder
{
	public partial class Form1 : Form
	{
		#region Data members
		List<string> classnames = new List<string>();
		#endregion Data members

		#region Constructor
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Constuctor
		/// </summary>
		public Form1()
		{
			InitializeComponent();
		}
		#endregion Constructor

		#region Methods
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Verifies that the class name fits within our evil hard coded values (3 and 10).
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		private bool CheckWordlength(int length)
		{
			return (length >= 3 && length <= 10);
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Called from within the LINQ statement, and removes anything that isn't an 
		/// alpha character. The strings are already capitalized when they get here 
		/// (also done in the LINQ statement).
		/// </summary>
		/// <param name="name">The string to be massaged</param>
		/// <returns>The massaged string</returns>
		private string MassageName(string name)
		{
			string newName = "";
			foreach(char character in name)
			{
				if ((int)character >= 65 && (int)character <= 90)
				{
					newName += character;
				}
			}
			return newName.Trim();
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Finds the class names we can use.
		/// </summary>
		private void BuildWordListFromClassNames()
		{
			Assembly[] assemblies = null;
			try
			{
				// I tried to do this in a single Linq statement, but the first from was 
				// throwing exceptions about not being able to access some of the 
				// assemblies, so I broke it out into separate statements
				assemblies = (from assembly in AppDomain.CurrentDomain.GetAssemblies().AsParallel() 
							  select assembly).ToArray<Assembly>();
			}
			catch (Exception)
			{
				// we don't care about exceptions (we take what reflect deigns to give us) 
			}
			try
			{
				if (assemblies != null)
				{
					foreach (Assembly assembly in assemblies)
					{
						var types = (from type in assembly.GetTypes().AsParallel()
									 where CheckWordlength(MassageName(type.Name.ToUpper()).Length)
									 select MassageName(type.Name.ToUpper())).ToArray<string>();

						if (types.Length > 0)
						{
							foreach(string word in types)
							{
								if (!classnames.Contains(word))
								{
									classnames.Add(word);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Oops!");
			}
			classnames.Sort();
			this.label2.Text = classnames.Count.ToString();
			this.listBox1.Items.AddRange(classnames.ToArray());
		}

		//--------------------------------------------------------------------------------
		/// <summary>
		/// Writes our text file.
		/// </summary>
		private void WriteTextFile()
		{
			StringBuilder text = new StringBuilder();
			// create the string
			foreach(string word in listBox1.Items)
			{
				text.AppendFormat((text.Length > 0)?" {0}":"{0}", word);
			}
			// if we have string
			if (text.Length > 0)
			{
				try
				{
					// build the file path
					string filePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Anagrams2ClassNames.txt");
					// delete the file if ti already exists
					if (File.Exists(filePath))
					{
						File.Delete(filePath);
					}
					// write the file
					using (TextWriter writer = new StreamWriter(filePath))
					{
						writer.Write(text);
					}
					MessageBox.Show("Done!", "Writing Anagrams2ClassNames.txt");
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Oops!");
				}
			}
		}
		#endregion Methods

		#region Events
		//--------------------------------------------------------------------------------
		/// <summary>
		/// Fired when the form is shown. Gathers class names into a list of strings 
		/// via reflection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Shown(object sender, EventArgs e)
		{
			BuildWordListFromClassNames();
		}


		//--------------------------------------------------------------------------------
		/// <summary>
		/// Fired when the user clicks the Make Word List butgton. Creates the word list 
		/// string, and saves it to a text file in the folder where this application was 
		/// executed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonMakeWordList_Click(object sender, EventArgs e)
		{
			WriteTextFile();
		}
		#endregion Events

	}
}