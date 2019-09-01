//********************************************************************************************************************************
// Filename:    SettingsForm.cs
// Owner:       Richard Dunkley
// Description: Contains a form to configure the various global settings in CSCodeGen library.
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2019
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//********************************************************************************************************************************
using CSCodeGen.Parse.SettingsFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSCodeGenSettingsGui
{
	/// <summary>
	///   Contains a form that allows you to configure the various settings for CSCodeGen library.
	/// </summary>
	public partial class SettingsForm : Form
	{
		#region Fields

		/// <summary>
		///   <see cref="CSCodeGenSettingsPanel"/> object used to configure the various settings in the form.
		/// </summary>
		private CSCodeGenSettingsPanel mPanel;

		/// <summary>
		///   Stores the various file extensions that should be used when importing and exporting settings.
		/// </summary>
		private Dictionary<string, string> mFileTypes;

		#endregion Fields

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="SettingsForm"/> object.
		/// </summary>
		/// <param name="includeCancel">True if the 'Cancel' button should be included in the form, false otherwise.</param>
		/// <param name="includeImport">True if the 'import' button should be included in the form, false otherwise.</param>
		/// <param name="includeExport">True if the 'export' button should be included in the form, false otherwise.</param>
		/// <remarks>
		///   The cancel button can be used when the form is used as a form within another GUI application to set the CSCodeGen
		///   settings. The cancel button can be ommitted when the form is used as the entire GUI of the application. In the
		///   latter case the 'Ok' button is replaced with an 'Exit' button.
		/// </remarks>
		public SettingsForm(bool includeCancel = true, bool includeImport = true, bool includeExport = true)
		{
			mFileTypes = new Dictionary<string, string>();

			InitializeComponent();

			mPanel = new CSCodeGenSettingsPanel();
			mainTableLayoutPanel.Controls.Add(mPanel, 0, 0);
			mainTableLayoutPanel.SetColumnSpan(mPanel, 5);
			mPanel.Dock = DockStyle.Fill;

			if(!includeCancel)
			{
				mainTableLayoutPanel.Controls.Remove(cancelButton);
				okButton.Text = "Exit";
			}
			if(!includeImport)
				mainTableLayoutPanel.Controls.Remove(importButton);
			if(!includeExport)
				mainTableLayoutPanel.Controls.Remove(exportButton);
		}

		/// <summary>
		///   Adds a file type to the import and export dialogs.
		/// </summary>
		/// <param name="extension">
		///   File extension of the new file type. Should include only alpha-numeric characters. No dots or wildcards.
		/// </param>
		/// <param name="name">Name displayed to the user of the file type.</param>
		/// <exception cref="ArgumentException">
		///   The extension contains characters that are not alpha-numeric, the extension has already been added, or the name
		///   contains a '|' character.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="extension"/> or <paramref name="name"/> is a null reference.
		/// </exception>
		public void AddFileType(string extension, string name)
		{
			if (extension == null)
				throw new ArgumentNullException("extension");
			if (name == null)
				throw new ArgumentNullException("name");

			if (!extension.All(char.IsLetterOrDigit))
				throw new ArgumentException(
					string.Format("The extension provided ({0}) contains characters that are not alpha-numeric.", extension));

			if(mFileTypes.ContainsKey(extension))
				throw new ArgumentException(string.Format("The extension provided ({0}) has already been added.", extension));

			if(name.Contains('|'))
				throw new ArgumentException(string.Format("The name provided ({0}) contains a '|' character.", extension));

			mFileTypes.Add(extension, name);
		}

		/// <summary>
		///   Builds a file filter string from the various added file types.
		/// </summary>
		/// <returns>String containing the filter.</returns>
		/// <remarks>This method adds 'All files' to the other added types.</remarks>
		private string BuildFilterString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (string key in mFileTypes.Keys)
			{
				sb.AppendFormat("{0} (*.{1})|*.{1}|", mFileTypes[key], key);
			}
			sb.Append("All files (*.*)|*.*");
			return sb.ToString();
		}

		/// <summary>
		///   Event handler called when the 'cancel' button is clicked.
		/// </summary>
		/// <param name="sender">This object.</param>
		/// <param name="e">Arguments for the event.</param>
		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		/// <summary>
		///   Event handler called when the 'export' button is clicked.
		/// </summary>
		/// <param name="sender">This object.</param>
		/// <param name="e">Arguments for the event.</param>
		private void ExportButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog
			{
				Filter = BuildFilterString(),
				OverwritePrompt = true,
				Title = "Select the File to Export the Setting Values To"
			};

			if (dialog.ShowDialog() != DialogResult.OK)
				return;

			if (!mPanel.ValidateSettings())
				return;

			Settings root = new Settings(DateTime.Now, new Version(1, 0), mPanel.ExportSettings());
			try
			{
				root.ExportToXML(dialog.FileName);
			}
			catch (Exception error)
			{
				if (error is InvalidOperationException || error is ArgumentException)
				{
					MessageBox.Show(string.Format("Unable to save the settings to the file path specified ({0}): {1}",
						dialog.FileName, error.Message), "Error Exporting Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				throw;
			}
		}

		/// <summary>
		///   Event handler called when the 'import' button is clicked.
		/// </summary>
		/// <param name="sender">This object.</param>
		/// <param name="e">Arguments for the event.</param>
		private void ImportButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog
			{
				CheckFileExists = true,
				CheckPathExists = true,
				Filter = BuildFilterString(),
				Title = "Select the File Containing the Settings Values",
				Multiselect = false
			};

			if (dialog.ShowDialog() != DialogResult.OK)
				return;

			Settings sf;
			try
			{
				sf = new Settings(dialog.FileName);
			}
			catch (Exception error)
			{
				if (error is InvalidOperationException || error is InvalidDataException)
				{
					MessageBox.Show
					(
						string.Format("The file path specified ({0}) could not be loaded: {1}", dialog.FileName, error.Message),
						"Error Importing Settings",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
					return;
				}
				throw;
			}

			mPanel.ImportSettings(sf);
		}

		/// <summary>
		///   Event handler called when the 'ok' button is clicked.
		/// </summary>
		/// <param name="sender">This object.</param>
		/// <param name="e">Arguments for the event.</param>
		private void OkButton_Click(object sender, EventArgs e)
		{
			if (!mPanel.ValidateSettings())
				return;

			mPanel.UpdateDefaultValues();
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		#endregion Methods
	}
}
