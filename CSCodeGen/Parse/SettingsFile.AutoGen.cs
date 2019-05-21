// ******************************************************************************************************************************
// Filename:    SettingsFile.AutoGen.cs
// Description:
// ******************************************************************************************************************************
// Copyright © Richard Dunkley 2019
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
// ******************************************************************************************************************************
// CSCodeGen.Parse.SettingsFile (class) (public, partial)
//   Properties:
//               Encoding               (public)
//               Root                   (public)
//               Version                (public)
//
//   Methods:
//               SettingsFile(2)        (public)
//               ExportToXML            (public)
//               ImportFromXML          (public)
//*******************************************************************************************************************************
using System;
using System.IO;
using System.Security;
using System.Xml;

namespace CSCodeGen.Parse
{
	//***************************************************************************************************************************
	/// <summary>
	///   Provides the methods to import and export data to/from an XML file. The schema was taken from the settings.xml file.
	/// </summary>
	//***************************************************************************************************************************
	public partial class SettingsFile
	{
		#region Properties

		//***********************************************************************************************************************
		/// <summary>Encoding of the XML file.</summary>
		//***********************************************************************************************************************
		public string Encoding { get; set; }

		//***********************************************************************************************************************
		/// <summary>Contains the root settings element in the XML file.</summary>
		//***********************************************************************************************************************
		public Settings Root { get; private set; }

		//***********************************************************************************************************************
		/// <summary>XML specification version of the XML file.</summary>
		//***********************************************************************************************************************
		public string Version { get; set; }

		#endregion Properties

		#region Methods

		//***********************************************************************************************************************
		/// <overloads><summary>Instantiates a new SettingsFile object.</summary></overloads>
		///
		/// <summary>Instantiates a new SettingsFile object using the provided root object and XML parameters.</summary>
		///
		/// <param name="root">Root object of the XML file.</param>
		/// <param name="xmlEncoding">Encoding of the XML file. Can be null.</param>
		/// <param name="xmlVersion">XML specification version of the XML file. Can be null.</param>
		///
		/// <exception cref="ArgumentNullException"><paramref name="root"/> is a null reference.</exception>
		//***********************************************************************************************************************
		public SettingsFile(Settings root, string xmlEncoding, string xmlVersion)
		{
			if(root == null)
				throw new ArgumentNullException("root");

			if(string.IsNullOrEmpty(xmlEncoding))
				Encoding = "UTF-8";
			else
				Encoding = xmlEncoding;
			if(string.IsNullOrEmpty(xmlVersion))
				Version = "1.0";
			else
				Version = xmlVersion;
			Root = root;
			Root.Ordinal = 0;
		}

		//***********************************************************************************************************************
		/// <overloads><summary>Instantiates a new SettingsFile object.</summary></overloads>
		///
		/// <summary>Instantiates a new SettingsFile object using the provided XML file.</summary>
		///
		/// <param name="filePath">Path to the XML file to be parsed.</param>
		///
		/// <exception cref="ArgumentException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item><paramref name="filePath"/> is an invalid file path.</item>
		///     <item><paramref name="filePath"/> is an empty array.</item>
		///   </list>
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">An error occurred while parsing the XML data.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="filePath"/> could not be opened.</exception>
		//***********************************************************************************************************************
		public SettingsFile(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentException("filePath is empty");

			ImportFromXML(filePath);
		}

		//***********************************************************************************************************************
		/// <overloads><summary>Instantiates a new SettingsFile object.</summary></overloads>
		///
		/// <summary>Instantiates a new SettingsFile object using the provided <see cref="XmlReader"/>.</summary>
		///
		/// <param name="reader"><see cref="XmlReader"/> object containing the XML to parse.</param>
		///
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">An error occurred while parsing the XML data.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="reader"/> does not contain valid XML.</exception>
		//***********************************************************************************************************************
		public SettingsFile(XmlReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			ImportFromXML(reader);
		}

		//***********************************************************************************************************************
		/// <summary>Exports data to an XML file.</summary>
		///
		/// <param name="filePath">Path to the XML file to be written to. If file exists all contents will be destroyed.</param>
		///
		/// <exception cref="ArgumentException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item><paramref name="filePath"/> is an invalid file path.</item>
		///     <item><paramref name="filePath"/> is an empty array.</item>
		///   </list>
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="filePath"/> could not be opened.</exception>
		//***********************************************************************************************************************
		public void ExportToXML(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentException("filePath is empty");
			XmlDocument doc = new XmlDocument();
			XmlDeclaration dec = doc.CreateXmlDeclaration(Version, Encoding, null);
			doc.InsertBefore(dec, doc.DocumentElement);

			XmlElement root = Root.CreateElement(doc);
			doc.AppendChild(root);
			doc.Save(filePath);
		}

		//***********************************************************************************************************************
		/// <summary>Exports data to an <see cref="XmlWriter"/> object.</summary>
		///
		/// <param name="writer"><see cref="XmlWriter"/> object to write the XML to.</param>
		///
		/// <exception cref="ArgumentNullException"><paramref name="writer"/> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">
		///   An error occurred when writing the XML to the <paramref name="writer"/>.
		/// </exception>
		//***********************************************************************************************************************
		public void ExportToXML(XmlWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			XmlDocument doc = new XmlDocument();
			XmlDeclaration dec;
			try
			{
				dec = doc.CreateXmlDeclaration(Version, Encoding, null);
			}
			catch(ArgumentException e)
			{
				throw new InvalidOperationException(string.Format(
					"The Version ({0}) or Encoding ({1}) properties were not valid. See Inner Exception.", Version, Encoding), e);
			}
			doc.InsertBefore(dec, doc.DocumentElement);

			XmlElement root = Root.CreateElement(doc);
			doc.AppendChild(root);

			try
			{
				doc.Save(writer);
			}
			catch(XmlException e)
			{
				throw new InvalidOperationException(
					"The XML would not result in a well formed XML document. See Inner Exception.", e);
			}
		}

		//***********************************************************************************************************************
		/// <summary>Imports data from an XML file.</summary>
		///
		/// <param name="filePath">Path to the XML file containing the data to be imported.</param>
		///
		/// <exception cref="ArgumentException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item><paramref name="filePath"/> is an invalid file path.</item>
		///     <item><paramref name="filePath"/> is an empty array.</item>
		///   </list>
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">An error occurred while parsing the XML data.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="filePath"/> could not be opened.</exception>
		//***********************************************************************************************************************
		public void ImportFromXML(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentException("filePath is empty");

			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(filePath);
			}
			catch(ArgumentException e)
			{
				throw new ArgumentException("filePath was not a valid XML file path.", e);
			}
			catch(PathTooLongException e)
			{
				throw new ArgumentException("filePath was not a valid XML file path.", e);
			}
			catch(DirectoryNotFoundException e)
			{
				throw new ArgumentException("filePath was not a valid XML file path.", e);
			}
			catch(NotSupportedException e)
			{
				throw new InvalidOperationException("filePath referenced a file that is in an invalid format.", e);
			}
			catch(FileNotFoundException e)
			{
				throw new InvalidOperationException("filePath referenced a file that could not be found.", e);
			}
			catch(IOException e)
			{
				throw new InvalidOperationException("filePath referenced a file that could not be opened.", e);
			}
			catch(UnauthorizedAccessException e)
			{
				throw new InvalidOperationException("filePath referenced a file that could not be opened.", e);
			}
			catch(SecurityException e)
			{
				throw new InvalidOperationException("filePath referenced a file that could not be opened.", e);
			}
			catch(XmlException e)
			{
				throw new InvalidOperationException("filePath referenced a file that does not contain valid XML.", e);
			}

			// Pull the version and encoding
			XmlDeclaration dec = doc.FirstChild as XmlDeclaration;
			if(dec != null)
			{
				Version = dec.Version;
				Encoding = dec.Encoding;
			}
			else
			{
				Version = "1.0";
				Encoding = "UTF-8";
			}

			XmlElement root = doc.DocumentElement;
			if(root.NodeType != XmlNodeType.Element)
				throw new InvalidDataException("The root node is not an element node.");
			if(string.Compare(root.Name, "settings", false) != 0)
				throw new InvalidDataException("The root node is not a 'settings' named node.");
			Root = new Settings(root, 0);
		}

		//***********************************************************************************************************************
		/// <summary>Imports data from an XML file.</summary>
		///
		/// <param name="reader"><see cref="XmlReader"/> to import the data from.</param>
		///
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">An error occurred while parsing the XML data.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="reader"/> does not contain valid XML.</exception>
		//***********************************************************************************************************************
		public void ImportFromXML(XmlReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(reader);
			}
			catch (XmlException e)
			{
				throw new InvalidOperationException("reader does not contain valid XML.", e);
			}

			// Pull the version and encoding
			XmlDeclaration dec = doc.FirstChild as XmlDeclaration;
			if (dec != null)
			{
				Version = dec.Version;
				Encoding = dec.Encoding;
			}
			else
			{
				Version = "1.0";
				Encoding = "UTF-8";
			}

			XmlElement root = doc.DocumentElement;
			if (root.NodeType != XmlNodeType.Element)
				throw new InvalidDataException("The root node is not an element node.");
			if (string.Compare(root.Name, "settings", false) != 0)
				throw new InvalidDataException("The root node is not a 'settings' named node.");
			Root = new Settings(root, 0);
		}

		#endregion Methods
	}
}
