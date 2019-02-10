//********************************************************************************************************************************
// Filename:    CSCodeGenSettingsPanel.cs
// Owner:       Richard Dunkley
// Description: Contains a panel to configure the various global settings in CSCodeGen library. This panel is provided so it can
//              be incorporated into other applications without having to use the complete form.
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2019
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//********************************************************************************************************************************
using CSCodeGen;
using CSCodeGen.Parse;
using System;
using System.IO;
using System.Security;
using System.Windows.Forms;

namespace CSCodeGenSettingsGui
{
	/// <summary>
	///   Panel UserControl class containing the various GUI controls to configure the CSCodeGen global library settings.
	/// </summary>
	/// <remarks>
	///   The properties in this class match the properties in <see cref="DefaultValues"/> so that the same settings file can be
	///   imported/exported into each.
	/// </remarks>
	public partial class CSCodeGenSettingsPanel : UserControl 
	{
		#region Properties

		/// <summary>
		///   Gets or sets the Name of the Company.
		/// </summary>
		public string CompanyName
		{
			get
			{
				return companyTextBox.Text;
			}
			set
			{
				companyTextBox.Text = value;
			}
		}

		/// <summary>
		///   Gets or sets the template for the copyright statement.
		/// </summary>
		/// <remarks>This line is wrapped if needed.</remarks>
		public string CopyrightTemplate
		{
			get
			{
				return copyrightTextBox.Text;
			}
			set
			{
				copyrightTextBox.Text = value;
			}
		}

		/// <summary>
		///   Gets or sets the name of the developer that will be in the generated code.
		/// </summary>
		public string Developer
		{
			get
			{
				return developerTextBox.Text;
			}
			set
			{
				developerTextBox.Text = value;
			}
		}

		/// <summary>
		///   Gets or sets the template for the file information header.
		/// </summary>
		/// <remarks>
		///   These lines should not exceed <see cref="NumCharactersPerLine"/> since the lines will not be wrapped.
		/// </remarks>
		public string[] FileInfoTemplate
		{
			get
			{
				return fileRichTextBox.Lines;
			}
			set
			{
				fileRichTextBox.Lines = value;
			}
		}

		/// <summary>
		///   Gets or sets the flower box character to use for documentation. Can be null.
		/// </summary>
		/// <remarks><see cref="ValidateSettings"/> should be called prior to attempting to get this value.</remarks>
		public char? FlowerBoxCharacter
		{
			get
			{
				if (string.IsNullOrWhiteSpace(flowerTextBox.Text))
					return null;
				return flowerTextBox.Text[0];
			}
			set
			{
				if (value.HasValue)
					flowerTextBox.Text = value.Value.ToString();
				else
					flowerTextBox.Text = "";
			}
		}

		/// <summary>
		///   Gets or sets the path to the HTML Help 1.x version compiler.
		/// </summary>
		/// <remarks>The path is required to generate Sandcastle Help File Builder projects.</remarks>
		public string HtmlHelp1xCompilerPath
		{
			get
			{
				return html1TextBox.Text;
			}
			set
			{
				html1TextBox.Text = value;
			}
		}

		/// <summary>
		///   Gets or sets the path to the HTML Help 2.x version compiler.
		/// </summary>
		/// <remarks>The path is required to generate Sandcastle Help File Builder projects.</remarks>
		public string HtmlHelp2xCompilerPath
		{
			get
			{
				return html2TextBox.Text;
			}
			set
			{
				html2TextBox.Text = value;
			}
		}

		/// <summary>
		///   Gets or sets whether the sub-header should be added to the file header.
		/// </summary>
		/// <remarks>The sub-header breaks down all the components of the class.</remarks>
		public bool IncludeSubHeader
		{
			get
			{
				return subHeaderCheckBox.Checked;
			}
			set
			{
				subHeaderCheckBox.Checked = value;
			}
		}

		/// <summary>
		///   Gets or sets the template for the auto-generated code license.
		/// </summary>
		/// <remarks>This line is wrapped if needed.</remarks>
		public string[] LicenseTemplate
		{
			get
			{
				return licenseRichTextBox.Lines;
			}
			set
			{
				licenseRichTextBox.Lines = value;
			}
		}

		/// <summary>
		///   Gets or sets the number of characters allowed per line. This only restricts documentation lines.
		/// </summary>
		/// <remarks><see cref="ValidateSettings"/> should be called prior to attempting to get this value.</remarks>
		public int NumCharactersPerLine
		{
			get
			{
				return int.Parse(numPerLineTextBox.Text);
			}
			set
			{
				numPerLineTextBox.Text = value.ToString();
			}
		}

		/// <summary>
		///   Gets or sets the path to the Sandcastle Help File Builder.
		/// </summary>
		/// <remarks>The path is required to generate Sandcastle Help File Builder projects.</remarks>
		public string SandcastlePath
		{
			get
			{
				return sandcastleTextBox.Text;
			}
			set
			{
				sandcastleTextBox.Text = value;
			}
		}

		/// <summary>
		///   Gets or sets the number of spaces per tab.
		/// </summary>
		/// <remarks><see cref="ValidateSettings"/> should be called prior to attempting to get this value.</remarks>
		public int TabSize
		{
			get
			{
				return int.Parse(indentTextBox.Text);
			}
			set
			{
				indentTextBox.Text = value.ToString();
			}
		}

		/// <summary>
		///   Gets or sets whether tabs or spaces should be used for indentation.
		/// </summary>
		/// <remarks>True if tabs should be used for indentation, false if spaces should be used.</remarks>
		public bool UseTabs
		{
			get
			{
				return tabCheckBox.Checked;
			}
			set
			{
				tabCheckBox.Checked = value;
			}
		}

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="CSCodeGenSettingsPanel"/> object.
		/// </summary>
		/// <remarks>
		///   This panel pulls the initial values in the panel from the <see cref="DefaultValues"/> class in CSCodeGen library.
		/// </remarks>
		public CSCodeGenSettingsPanel()
		{
			InitializeComponent();

			applicationNameLabel.Text = DefaultValues.ApplicationName;
			applicationVersionLabel.Text = DefaultValues.ApplicationVersion;
			companyTextBox.Text = DefaultValues.CompanyName;
			copyrightTextBox.Text = DefaultValues.CopyrightTemplate;
			developerTextBox.Text = DefaultValues.Developer;
			fileRichTextBox.Lines = DefaultValues.FileInfoTemplate;
			if (DefaultValues.FlowerBoxCharacter.HasValue)
				flowerTextBox.Text = DefaultValues.FlowerBoxCharacter.Value.ToString();
			else
				flowerTextBox.Text = string.Empty;
			html1TextBox.Text = DefaultValues.HtmlHelp1xCompilerPath;
			html2TextBox.Text = DefaultValues.HtmlHelp2xCompilerPath;
			subHeaderCheckBox.Checked = DefaultValues.IncludeSubHeader;
			libraryNameLabel.Text = DefaultValues.LibraryName;
			libraryVersionLabel.Text = DefaultValues.LibraryVersion;
			licenseRichTextBox.Lines = DefaultValues.LicenseTemplate;
			numPerLineTextBox.Text = DefaultValues.NumCharactersPerLine.ToString();
			sandcastleTextBox.Text = DefaultValues.SandcastlePath;
			indentTextBox.Text = DefaultValues.TabSize.ToString();
			tabCheckBox.Checked = DefaultValues.UseTabs;
		}

		/// <summary>
		///   Allows a caller to export the settings from the panel.
		/// </summary>
		/// <returns>
		///   Array of <see cref="SettingInfo"/> objects containing the various settings or null if the values do not pass validation.
		/// </returns>
		/// <remarks>
		///   The settings exported here are the current GUI values. <see cref="ValidateSettings"/> is called in this method prior to
		///   exporting the settings.
		/// </remarks>
		public SettingInfo[] ExportSettings()
		{
			if (!ValidateSettings())
				return null;

			return Settings.GetSettingInfos(typeof(CSCodeGenSettingsPanel), this);
		}

		/// <summary>
		///   Event handler called when the HTML v1.x Compiler 'Browse' button is clicked.
		/// </summary>
		/// <param name="sender">This object.</param>
		/// <param name="e">Arguments for the event.</param>
		private void html1BrowseButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.CheckPathExists = true;
			dialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
			dialog.Title = "Select the HTML 1.x Compiler Executable";
			dialog.Multiselect = false;
			if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.HTML1BrowsePath))
				dialog.InitialDirectory = Properties.Settings.Default.HTML1BrowsePath;

			if (dialog.ShowDialog() != DialogResult.OK)
				return;

			html1TextBox.Text = dialog.FileName;
			Properties.Settings.Default.HTML1BrowsePath = Path.GetDirectoryName(dialog.FileName);
			Properties.Settings.Default.Save();
		}

		/// <summary>
		///   Event handler called when the HTML v2.x Compiler 'Browse' button is clicked.
		/// </summary>
		/// <param name="sender">This object.</param>
		/// <param name="e">Arguments for the event.</param>
		private void html2BrowseButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.CheckPathExists = true;
			dialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
			dialog.Title = "Select the HTML 2.x Compiler Executable";
			dialog.Multiselect = false;
			if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.HTML2BrowsePath))
				dialog.InitialDirectory = Properties.Settings.Default.HTML2BrowsePath;

			if (dialog.ShowDialog() != DialogResult.OK)
				return;

			html2TextBox.Text = dialog.FileName;
			Properties.Settings.Default.HTML2BrowsePath = Path.GetDirectoryName(dialog.FileName);
			Properties.Settings.Default.Save();
		}

		/// <summary>
		///   Allows a caller to import settings into the panel.
		/// </summary>
		/// <param name="settings"><see cref="Settings"/> object containing the settings to import.</param>
		/// <remarks>
		///   The settings imported here will only appear in the panel. It won't update the <see cref="DefaultValues"/> class
		///   until <see cref="UpdateDefaultValues"/> is called.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="settings"/> is a null reference.</exception>
		public void ImportSettings(Settings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			settings.SetProperties(typeof(CSCodeGenSettingsPanel), this);
		}


		/// <summary>
		///   Event handler called when the sandcastle folder 'Browse' button is clicked.
		/// </summary>
		/// <param name="sender">This object.</param>
		/// <param name="e">Arguments for the event.</param>
		private void sandcastleBrowseButton_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = "Select the folder where Sandcastle is installed in";
			dialog.ShowNewFolderButton = true;
			if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.SandcastleBrowsePath))
				dialog.SelectedPath = Properties.Settings.Default.SandcastleBrowsePath;

			if (dialog.ShowDialog() != DialogResult.OK)
				return;

			sandcastleTextBox.Text = dialog.SelectedPath;
			Properties.Settings.Default.SandcastleBrowsePath = dialog.SelectedPath;
			Properties.Settings.Default.Save();
		}

		/// <summary>
		///   Updates the <see cref="DefaultValues"/> class in CSCodeGen library with the panel's values.
		/// </summary>
		/// <returns>True if the values passed validation and the class was updated, false otherwise.</returns>
		/// <remarks>
		///   This method calls <see cref="ValidateSettings"/> prior to setting the values. If the validation fails, then no
		///   values will be updated.
		/// </remarks>
		public bool UpdateDefaultValues()
		{
			if (!ValidateSettings())
				return false;

			DefaultValues.CompanyName = companyTextBox.Text;
			DefaultValues.CopyrightTemplate = copyrightTextBox.Text;
			DefaultValues.Developer = developerTextBox.Text;
			DefaultValues.FileInfoTemplate = fileRichTextBox.Lines;
			if (string.IsNullOrWhiteSpace(flowerTextBox.Text))
				DefaultValues.FlowerBoxCharacter = null;
			else
				DefaultValues.FlowerBoxCharacter = flowerTextBox.Text[0];
			DefaultValues.HtmlHelp1xCompilerPath = html1TextBox.Text;
			DefaultValues.HtmlHelp2xCompilerPath = html2TextBox.Text;
			DefaultValues.IncludeSubHeader = subHeaderCheckBox.Checked;
			DefaultValues.LicenseTemplate = licenseRichTextBox.Lines;
			DefaultValues.NumCharactersPerLine = int.Parse(numPerLineTextBox.Text);
			DefaultValues.SandcastlePath = sandcastleTextBox.Text;
			DefaultValues.TabSize = int.Parse(indentTextBox.Text);
			DefaultValues.UseTabs = tabCheckBox.Checked;
			return true;
		}

		/// <summary>
		///   Validates that the values in the form are valid.
		/// </summary>
		/// <returns>True if all values pass validation, false if one or more fail.</returns>
		/// <remarks>This method will display a message box to the user if one of the values fails.</remarks>
		public bool ValidateSettings()
		{
			// Validate the fields.
			if (!string.IsNullOrWhiteSpace(flowerTextBox.Text))
			{
				if (flowerTextBox.Text.Length > 1)
				{
					MessageBox.Show
					(
						string.Format
						(
							"The Flower Box Character specified is more than one character ({0}), only one character should be entered. It will be repeated to form the flower box.",
							flowerTextBox.Text
						),
						"Error With Input Values",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
					return false;
				}
			}
			if (!string.IsNullOrWhiteSpace(html1TextBox.Text))
			{
				try
				{
					string filePath = Path.GetFullPath(html1TextBox.Text);
				}
				catch (Exception e)
				{
					if (e is ArgumentException || e is SecurityException || e is NotSupportedException || e is PathTooLongException)
					{
						MessageBox.Show
						(
							string.Format
							(
								"A problem occurred with the Sandcastle HTML Version 1.x Compiler path ({0}): {1}.",
								html1TextBox.Text,
								e.Message
							),
							"Error With Input Values",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error
						);
						return false;
					}
					throw;
				}
			}
			if (!string.IsNullOrWhiteSpace(html2TextBox.Text))
			{
				try
				{
					string filePath = Path.GetFullPath(html2TextBox.Text);
				}
				catch (Exception e)
				{
					if (e is ArgumentException || e is SecurityException || e is NotSupportedException || e is PathTooLongException)
					{
						MessageBox.Show
						(
							string.Format
							(
								"A problem occurred with the Sandcastle HTML Version 2.x Compiler path ({0}): {1}.",
								html2TextBox.Text,
								e.Message
							),
							"Error With Input Values",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error
						);
						return false;
					}
					throw;
				}
			}
			if (!string.IsNullOrWhiteSpace(sandcastleTextBox.Text))
			{
				try
				{
					string folderPath = Path.GetFullPath(sandcastleTextBox.Text);
				}
				catch (Exception e)
				{
					if (e is ArgumentException || e is SecurityException || e is NotSupportedException || e is PathTooLongException)
					{
						MessageBox.Show
						(
							string.Format
							(
								"A problem occurred with the Sandcastle folder path ({0}): {1}.",
								sandcastleTextBox.Text,
								e.Message
							),
							"Error With Input Values",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error
						);
						return false;
					}
					throw;
				}
			}
			if (!int.TryParse(numPerLineTextBox.Text, out int numPerLine))
			{
				MessageBox.Show
				(
					"The value provided for the Number of Characters Per Line is not a valid integer.",
					"Error With Input Values",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				return false;
			}
			if (!int.TryParse(indentTextBox.Text, out int indent))
			{
				MessageBox.Show
				(
					"The value provided for the Indent Size is not a valid integer.",
					"Error With Input Values",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				return false;
			}
			return true;
		}

		#endregion Methods
	}
}
