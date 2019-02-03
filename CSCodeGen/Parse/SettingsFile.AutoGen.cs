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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
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
		/// <exception cref="ArgumentException"><paramref name="filePath"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is a null reference.</exception>
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

		#endregion Methods
	}
}
