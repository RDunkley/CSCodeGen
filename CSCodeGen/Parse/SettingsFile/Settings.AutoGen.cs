// Filename:    Settings.AutoGen.cs
// Owner:       Richard Dunkley
// Description:
// Generated using XMLToDataClass version 1.1.0 with CSCodeGen.dll version 1.0.0.
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2019
// CSCodeGen.Parse.SettingsFile.Settings (class)                   (public, partial)
//   Enumerations:
//                 VersionVersionType                              (public)
//
//   Fields:
//                 mDefaultXMLEncoding                             (private, const)
//                 mDefaultXMLVersion                              (private, const)
//
//   Properties:
//                 CreationDate                                    (public)
//                 CreationDateParsedFormat                        (public)
//                 Ordinal                                         (public)
//                 SettingInfos                                    (public)
//                 Version                                         (public)
//                 VersionParsedType                               (public)
//                 XmlFileEncoding                                 (public)
//                 XmlFileVersion                                  (public)
//
//   Methods:
//                 Settings(6)                                     (public)
//                 CreateElement                                   (public)
//                 ExportToXML(4)                                  (public)
//                 GetCreationDateString                           (public)
//                 GetVersionString                                (public)
//                 ImportFromXML(4)                                (public)
//                 ParseXmlNode                                    (public)
//                 SetCreationDateFromString                       (public)
//                 SetVersionFromString                            (public)
//********************************************************************************************************************************
// CSCodeGen.Parse.SettingsFile.Settings.VersionVersionType (enum) (public)
//   Names:
//                 MajorMinor
//                 MajorMinorBuild
//                 MajorMinorBuildRevision
//********************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CSCodeGen.Parse.SettingsFile
{
	//****************************************************************************************************************************
	/// <summary>Represents a collection of settings for the CS Code Generation Library.</summary>
	//****************************************************************************************************************************
	public partial class Settings
	{
		#region Enumerations

		//************************************************************************************************************************
		/// <summary>Enumerates the various version types that can be parsed into a version</summary>
		//************************************************************************************************************************
		public enum VersionVersionType
		{
			#region Names

			//********************************************************************************************************************
			/// <summary>Only the major and minor part were provided.</summary>
			//********************************************************************************************************************
			MajorMinor,

			//********************************************************************************************************************
			/// <summary>The major, minor, and build were provided.</summary>
			//********************************************************************************************************************
			MajorMinorBuild,

			//********************************************************************************************************************
			/// <summary>The major, minor, build, and revision were provided.</summary>
			//********************************************************************************************************************
			MajorMinorBuildRevision,

			#endregion Names
		}

		#endregion Enumerations

		#region Fields

		//************************************************************************************************************************
		/// <summary>Default encoding of the XML file generated from this object.</summary>
		//************************************************************************************************************************
		private const string mDefaultXMLEncoding = "UTF-8";

		//************************************************************************************************************************
		/// <summary>Default version of the XML file generated from this object.</summary>
		//************************************************************************************************************************
		private const string mDefaultXMLVersion = "1.0";

		#endregion Fields

		#region Properties

		//************************************************************************************************************************
		/// <summary>Gets or sets the creation date of the settings file.</summary>
		//************************************************************************************************************************
		public DateTime CreationDate { get; set; }

		//************************************************************************************************************************
		/// <summary>Stores how the CreationDate date/time is converted to XML string</summary>
		//************************************************************************************************************************
		public string CreationDateParsedFormat { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets the index of this object in relation to the other child element of this object's parent.</summary>
		///
		/// <remarks>
		///   If the value is -1, then this object was not created from an XML node and the property has not been set.
		/// </remarks>
		//************************************************************************************************************************
		public int Ordinal { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets the child XML elements.</summary>
		//************************************************************************************************************************
		public SettingInfo[] SettingInfos { get; private set; }

		//************************************************************************************************************************
		/// <summary>Gets the version of the application that generated the file.</summary>
		//************************************************************************************************************************
		public Version Version { get; set; }

		//************************************************************************************************************************
		/// <summary>Stores how the Version version is converted to an XML string</summary>
		//************************************************************************************************************************
		public VersionVersionType VersionParsedType { get; set; }

		//************************************************************************************************************************
		/// <summary>Encoding of the XML file this root node will be contained in.</summary>
		///
		/// <remarks>Defaults to 'UTF-8'</remarks>
		//************************************************************************************************************************
		public string XmlFileEncoding { get; set; }

		//************************************************************************************************************************
		/// <summary>Version of the XML file this root node will be contained in.</summary>
		///
		/// <remarks>Defaults to '1.0'</remarks>
		//************************************************************************************************************************
		public string XmlFileVersion { get; set; }

		#endregion Properties

		#region Methods

		//************************************************************************************************************************
		/// <overloads><summary>Instantiates a new <see cref="Settings"/> object.</summary></overloads>
		///
		/// <summary>Instantiates a new <see cref="Settings"/> object using the provided information.</summary>
		///
		/// <param name="creationDate">'date' Date/Time attribute contained in the XML element.</param>
		/// <param name="version">'version' Version attribute contained in the XML element.</param>
		/// <param name="settingInfos">Array of setting elements which are child elements of this node. Can be empty.</param>
		///
		/// <exception cref="ArgumentNullException">
		///   <paramref name="version"/>, or <paramref name="settingInfos"/> is a null reference.
		/// </exception>
		//************************************************************************************************************************
		public Settings(DateTime creationDate, Version version, SettingInfo[] settingInfos)
		{
			if(version == null)
				throw new ArgumentNullException("version");
			if(settingInfos == null)
				throw new ArgumentNullException("settingInfos");
			CreationDate = creationDate;
			Version = version;
			SettingInfos = settingInfos;
			Ordinal = -1;

			// Compute the maximum index used on any child items.
			int maxIndex = 0;
			foreach(SettingInfo item in SettingInfos)
			{
				if(item.Ordinal >= maxIndex)
					maxIndex = item.Ordinal + 1; // Set to first index after this index.
			}

			// Assign ordinal for any child items that don't have it set (-1).
			foreach(SettingInfo item in SettingInfos)
			{
				if(item.Ordinal == -1)
					item.Ordinal = maxIndex++;
			}
			XmlFileVersion = mDefaultXMLVersion;
			XmlFileEncoding = mDefaultXMLEncoding;
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Settings"/> object from an XML file.</summary>
		///
		/// <param name="stream">Stream containing the XML file data.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="stream"/> did not contain valid XML.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">An error occurred while parsing the XML data.</exception>
		//************************************************************************************************************************
		public Settings(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			ImportFromXML(stream);
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Settings"/> object from an XML file.</summary>
		///
		/// <param name="filePath">Path to the XML file containing the data to be imported.</param>
		///
		/// <exception cref="ArgumentException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item><paramref name="filePath"/> is invalid or an error occurred while accessing it.</item>
		///     <item><paramref name="filePath"/> is an empty array.</item>
		///   </list>
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">An error occurred while parsing the XML data.</exception>
		//************************************************************************************************************************
		public Settings(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentException("filePath is empty");

			ImportFromXML(filePath);
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Settings"/> object from an XML file.</summary>
		///
		/// <param name="reader">TextReader object containing the XML file data.</param>
		///
		/// <exception cref="ArgumentException">
		///   A parsing error occurred while attempting to load the XML from <paramref name="reader"/>.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">An error occurred while parsing the XML data.</exception>
		//************************************************************************************************************************
		public Settings(TextReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			ImportFromXML(reader);
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Settings"/> object from an <see cref="XmlNode"/> object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="node"/> does not correspond to a settings node or is not an 'Element' type node or <paramref
		///   name="ordinal"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public Settings(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(ordinal < 0)
				throw new ArgumentException("the ordinal specified is negative.");
			if(node.NodeType != XmlNodeType.Element)
				throw new ArgumentException("node is not of type 'Element'.");

			ParseXmlNode(node, ordinal);
			XmlFileVersion = mDefaultXMLVersion;
			XmlFileEncoding = mDefaultXMLEncoding;
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Settings"/> object from an XML file.</summary>
		///
		/// <param name="reader">XmlReader object containing the XML file data.</param>
		///
		/// <exception cref="ArgumentException">
		///   A parsing error occurred while attempting to load the XML from <paramref name="reader"/>.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">An error occurred while parsing the XML data.</exception>
		//************************************************************************************************************************
		public Settings(XmlReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			ImportFromXML(reader);
		}

		//************************************************************************************************************************
		/// <summary>Creates an XML element for this object using the provided <see cref="XmlDocument"/> object.</summary>
		///
		/// <param name="doc"><see cref="XmlDocument"/> object to generate the element from.</param>
		///
		/// <returns><see cref="XmlElement"/> object containing this classes data.</returns>
		///
		/// <exception cref="ArgumentNullException"><paramref name="doc"/> is a null reference.</exception>
		//************************************************************************************************************************
		public XmlElement CreateElement(XmlDocument doc)
		{
			if(doc == null)
				throw new ArgumentNullException("doc");
			XmlElement returnElement = doc.CreateElement("settings");

			string valueString;

			// date
			valueString = GetCreationDateString();
			returnElement.SetAttribute("date", valueString);

			// version
			valueString = GetVersionString();
			returnElement.SetAttribute("version", valueString);
			// Build up dictionary of indexes and corresponding items.
			Dictionary<int, object> lookup = new Dictionary<int, object>();

			foreach(SettingInfo child in SettingInfos)
			{
				if(lookup.ContainsKey(child.Ordinal))
					throw new InvalidOperationException("An attempt was made to generate the XML element with two child elements"
						+ " with the same ordinal.Ordinals must be unique across all child objects.");
				lookup.Add(child.Ordinal, child);
			}

			// Sort the keys.
			List<int> keys = lookup.Keys.ToList();
			keys.Sort();

			foreach (int key in keys)
			{
				if(lookup[key] is SettingInfo)
					returnElement.AppendChild(((SettingInfo)lookup[key]).CreateElement(doc));
			}
			return returnElement;
		}

		//************************************************************************************************************************
		/// <summary>Exports data to an XML file.</summary>
		///
		/// <param name="stream">Stream to write the XML to.</param>
		///
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is a null reference.</exception>
		//************************************************************************************************************************
		public void ExportToXML(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			XmlDocument doc = new XmlDocument();
			XmlDeclaration dec = doc.CreateXmlDeclaration(XmlFileVersion, XmlFileEncoding, null);
			doc.InsertBefore(dec, doc.DocumentElement);

			XmlElement root = CreateElement(doc);
			doc.AppendChild(root);
			doc.Save(stream);
		}

		//************************************************************************************************************************
		/// <summary>Exports data to an XML file.</summary>
		///
		/// <param name="filePath">Path to the XML file to be written to. If file exists all contents will be destroyed.</param>
		///
		/// <exception cref="ArgumentException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item><paramref name="filePath"/> is invalid or an error occurred while accessing it.</item>
		///     <item><paramref name="filePath"/> is an empty array.</item>
		///   </list>
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is a null reference.</exception>
		//************************************************************************************************************************
		public void ExportToXML(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentException("filePath is empty");
			XmlDocument doc = new XmlDocument();
			XmlDeclaration dec = doc.CreateXmlDeclaration(XmlFileVersion, XmlFileEncoding, null);
			doc.InsertBefore(dec, doc.DocumentElement);

			XmlElement root = CreateElement(doc);
			doc.AppendChild(root);
			doc.Save(filePath);
		}

		//************************************************************************************************************************
		/// <summary>Exports data to an XML file.</summary>
		///
		/// <param name="writer">TextWriter object to write the XML to.</param>
		///
		/// <exception cref="ArgumentNullException"><paramref name="writer"/> is a null reference.</exception>
		//************************************************************************************************************************
		public void ExportToXML(TextWriter writer)
		{
			if(writer == null)
				throw new ArgumentNullException("writer");
			XmlDocument doc = new XmlDocument();
			XmlDeclaration dec = doc.CreateXmlDeclaration(XmlFileVersion, XmlFileEncoding, null);
			doc.InsertBefore(dec, doc.DocumentElement);

			XmlElement root = CreateElement(doc);
			doc.AppendChild(root);
			doc.Save(writer);
		}

		//************************************************************************************************************************
		/// <summary>Exports data to an XML file.</summary>
		///
		/// <param name="writer">XmlWriter object to write the XML to.</param>
		///
		/// <exception cref="ArgumentNullException"><paramref name="writer"/> is a null reference.</exception>
		//************************************************************************************************************************
		public void ExportToXML(XmlWriter writer)
		{
			if(writer == null)
				throw new ArgumentNullException("writer");
			XmlDocument doc = new XmlDocument();
			XmlDeclaration dec = doc.CreateXmlDeclaration(XmlFileVersion, XmlFileEncoding, null);
			doc.InsertBefore(dec, doc.DocumentElement);

			XmlElement root = CreateElement(doc);
			doc.AppendChild(root);
			doc.Save(writer);
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of CreationDate.</summary>
		///
		/// <returns>String representing the value.</returns>
		//************************************************************************************************************************
		public string GetCreationDateString()
		{

			CultureInfo culture = new CultureInfo("en-US");
			return CreationDate.ToString(CreationDateParsedFormat, culture);
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Version.</summary>
		///
		/// <returns>String representing the value.</returns>
		//************************************************************************************************************************
		public string GetVersionString()
		{
			if(VersionParsedType == VersionVersionType.MajorMinorBuild)
				return Version.ToString(3);
			else if(VersionParsedType == VersionVersionType.MajorMinorBuildRevision)
				return Version.ToString(4);
			return Version.ToString(2);
		}

		//************************************************************************************************************************
		/// <summary>Imports data from an XML stream.</summary>
		///
		/// <param name="stream">Stream containing the XML file data.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="stream"/> did not contain valid XML.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   The XML was valid, but an error occurred while extracting the data from it.
		/// </exception>
		//************************************************************************************************************************
		public void ImportFromXML(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(stream);
			}
			catch(XmlException e)
			{
				throw new ArgumentException(string.Format("Unable to parse the XML from the stream. Error message: {0}.",
					e.Message), nameof(stream), e);
			}

			// Pull the version and encoding
			XmlDeclaration dec = doc.FirstChild as XmlDeclaration;
			if(dec != null)
			{
				XmlFileVersion = dec.Version;
				XmlFileEncoding = dec.Encoding;
			}
			else
			{
				XmlFileVersion = mDefaultXMLVersion;
				XmlFileEncoding = mDefaultXMLEncoding;
			}

			XmlElement root = doc.DocumentElement;
			if(root.NodeType != XmlNodeType.Element)
				throw new InvalidDataException("The root node is not an element node.");
			if(string.Compare(root.Name, "settings", false) != 0)
				throw new InvalidDataException(string.Format("The root element name is not the one expected (Actual: '{0}',"
					+ " Expected: 'settings').", root.Name));

			ParseXmlNode(root, 0);
		}

		//************************************************************************************************************************
		/// <summary>Imports data from an XML file.</summary>
		///
		/// <param name="filePath">Path to the XML file containing the data to be imported.</param>
		///
		/// <exception cref="ArgumentException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item><paramref name="filePath"/> is invalid or an error occurred while accessing it.</item>
		///     <item><paramref name="filePath"/> is an empty array.</item>
		///   </list>
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   The XML was valid, but an error occurred while extracting the data from it.
		/// </exception>
		//************************************************************************************************************************
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
			catch(PathTooLongException e)
			{
				throw new ArgumentException(string.Format("The file path specified ({0}) is not valid ({1}).", filePath,
					e.Message), nameof(filePath), e);
			}
			catch(DirectoryNotFoundException e)
			{
				throw new ArgumentException(string.Format("The file path specified ({0}) is not valid ({1}).", filePath,
					e.Message), nameof(filePath), e);
			}
			catch(NotSupportedException e)
			{
				throw new ArgumentException(string.Format("The file path specified ({0}) is not valid ({1}).", filePath,
					e.Message), nameof(filePath), e);
			}
			catch(FileNotFoundException e)
			{
				throw new ArgumentException(string.Format("The file could not be located at the path specified ({0}).", filePath),
					nameof(filePath), e);
			}
			catch(IOException e)
			{
				throw new ArgumentException(string.Format("An I/O error occurred ({0}) while opening the file specified ({1}).",
					e.Message, filePath), nameof(filePath), e);
			}
			catch(UnauthorizedAccessException e)
			{
				throw new ArgumentException(string.Format("Unable to access the file path specified ({0}).", filePath),
					nameof(filePath), e);
			}
			catch(SecurityException e)
			{
				throw new ArgumentException(string.Format("The caller doesn't have the required permissions to access the file"
					+ " path specified ({0}).", filePath), nameof(filePath), e);
			}
			catch(XmlException e)
			{
				throw new ArgumentException(string.Format("Unable to parse the XML from the file specified ({0}). Error message:"
					+ " {1}.", filePath, e.Message), nameof(filePath), e);
			}

			// Pull the version and encoding
			XmlDeclaration dec = doc.FirstChild as XmlDeclaration;
			if(dec != null)
			{
				XmlFileVersion = dec.Version;
				XmlFileEncoding = dec.Encoding;
			}
			else
			{
				XmlFileVersion = mDefaultXMLVersion;
				XmlFileEncoding = mDefaultXMLEncoding;
			}

			XmlElement root = doc.DocumentElement;
			if(root.NodeType != XmlNodeType.Element)
				throw new InvalidDataException("The root node is not an element node.");
			if(string.Compare(root.Name, "settings", false) != 0)
				throw new InvalidDataException(string.Format("The root element name is not the one expected (Actual: '{0}',"
					+ " Expected: 'settings').", root.Name));

			ParseXmlNode(root, 0);
		}

		//************************************************************************************************************************
		/// <summary>Imports data from an XML text reader.</summary>
		///
		/// <param name="reader">TextReader object containing the XML file data.</param>
		///
		/// <exception cref="ArgumentException">
		///   A parsing error occurred while attempting to load the XML from <paramref name="reader"/>.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   The XML was valid, but an error occurred while extracting the data from it.
		/// </exception>
		//************************************************************************************************************************
		public void ImportFromXML(TextReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(reader);
			}
			catch(XmlException e)
			{
				throw new ArgumentException(string.Format("Unable to parse the XML from the reader. Error message: {0}.",
					e.Message), nameof(reader), e);
			}

			// Pull the version and encoding
			XmlDeclaration dec = doc.FirstChild as XmlDeclaration;
			if(dec != null)
			{
				XmlFileVersion = dec.Version;
				XmlFileEncoding = dec.Encoding;
			}
			else
			{
				XmlFileVersion = mDefaultXMLVersion;
				XmlFileEncoding = mDefaultXMLEncoding;
			}

			XmlElement root = doc.DocumentElement;
			if(root.NodeType != XmlNodeType.Element)
				throw new InvalidDataException("The root node is not an element node.");
			if(string.Compare(root.Name, "settings", false) != 0)
				throw new InvalidDataException(string.Format("The root element name is not the one expected (Actual: '{0}',"
					+ " Expected: 'settings').", root.Name));

			ParseXmlNode(root, 0);
		}

		//************************************************************************************************************************
		/// <summary>Imports data from an XML reader.</summary>
		///
		/// <param name="reader">XmlReader object containing the XML file data.</param>
		///
		/// <exception cref="ArgumentException">
		///   A parsing error occurred while attempting to load the XML from <paramref name="reader"/>.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   The XML was valid, but an error occurred while extracting the data from it.
		/// </exception>
		//************************************************************************************************************************
		public void ImportFromXML(XmlReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(reader);
			}
			catch(XmlException e)
			{
				throw new ArgumentException(string.Format("Unable to parse the XML from the reader. Error message: {0}.",
					e.Message), nameof(reader), e);
			}

			// Pull the version and encoding
			XmlDeclaration dec = doc.FirstChild as XmlDeclaration;
			if(dec != null)
			{
				XmlFileVersion = dec.Version;
				XmlFileEncoding = dec.Encoding;
			}
			else
			{
				XmlFileVersion = mDefaultXMLVersion;
				XmlFileEncoding = mDefaultXMLEncoding;
			}

			XmlElement root = doc.DocumentElement;
			if(root.NodeType != XmlNodeType.Element)
				throw new InvalidDataException("The root node is not an element node.");
			if(string.Compare(root.Name, "settings", false) != 0)
				throw new InvalidDataException(string.Format("The root element name is not the one expected (Actual: '{0}',"
					+ " Expected: 'settings').", root.Name));

			ParseXmlNode(root, 0);
		}

		//************************************************************************************************************************
		/// <summary>Parses an XML node and populates the data into this object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="node"/> does not correspond to a settings node.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public void ParseXmlNode(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(string.Compare(node.Name, "settings", false) != 0)
				throw new ArgumentException("node does not correspond to a settings node.");

			XmlAttribute attrib;

			// date
			attrib = node.Attributes["date"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (date) is not optional, but was not found in the XML"
					+ " element (settings).");
			SetCreationDateFromString(attrib.Value);

			// version
			attrib = node.Attributes["version"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (version) is not optional, but was not found in the XML"
					+ " element (settings).");
			SetVersionFromString(attrib.Value);

			// Read the child objects.
			List<SettingInfo> settingInfosList = new List<SettingInfo>();
			int index = 0;
			foreach(XmlNode child in node.ChildNodes)
			{
				if(child.NodeType == XmlNodeType.Element && child.Name == "setting")
					settingInfosList.Add(new SettingInfo(child, index++));
			}
			SettingInfos = settingInfosList.ToArray();

			Ordinal = ordinal;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in CreationDate.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item>The string value is a null reference or an empty string.</item>
		///     <item>The string value could not be parsed.</item>
		///   </list>
		/// </exception>
		//************************************************************************************************************************
		public void SetCreationDateFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'date' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'date' is an empty string.");
			CultureInfo info = CultureInfo.GetCultureInfo("en-US");
			DateTimeStyles styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;
			DateTime returnValue;

			if(DateTime.TryParseExact(value, "d", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "d";
				return;
			}

			if(DateTime.TryParseExact(value, "D", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "D";
				return;
			}

			if(DateTime.TryParseExact(value, "t", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "t";
				return;
			}

			if(DateTime.TryParseExact(value, "T", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "T";
				return;
			}

			if(DateTime.TryParseExact(value, "m", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "m";
				return;
			}

			if(DateTime.TryParseExact(value, "y", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "y";
				return;
			}

			if(DateTime.TryParseExact(value, "f", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "f";
				return;
			}

			if(DateTime.TryParseExact(value, "F", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "F";
				return;
			}

			if(DateTime.TryParseExact(value, "g", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "g";
				return;
			}

			if(DateTime.TryParseExact(value, "G", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "G";
				return;
			}

			if(DateTime.TryParseExact(value, "o", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "o";
				return;
			}

			if(DateTime.TryParseExact(value, "R", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "R";
				return;
			}

			if(DateTime.TryParseExact(value, "s", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "s";
				return;
			}

			if(DateTime.TryParseExact(value, "u", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "u";
				return;
			}

			if(DateTime.TryParseExact(value, "U", info, styles, out returnValue))
			{
				CreationDate = returnValue;
				CreationDateParsedFormat = "U";
				return;
			}

			throw new InvalidDataException(string.Format("The Date/Time value specified ({0}) is not a valid DateTime standard"
				+ " string representation", value));
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Version.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item>The string value is a null reference or an empty string.</item>
		///     <item>The string value could not be parsed.</item>
		///   </list>
		/// </exception>
		//************************************************************************************************************************
		public void SetVersionFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'version' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'version' is an empty string.");
			string[] splits = value.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
			if(splits.Length < 2)
				throw new InvalidDataException(string.Format("The Version value ({0}) for 'version' does not contain at least two"
					+ " components separated by a period (Ex: <major>.<minor>)", value));
			if(splits.Length > 4)
				throw new InvalidDataException(string.Format("The Version value ({0}) for 'version' has more than 4 components"
					+ " separated by a period. A version is limited to four at most (Ex: <major>.<minor>.<build>.<revision>)",
					value));

			if(splits.Length == 3)
				VersionParsedType = VersionVersionType.MajorMinorBuild;
			else if(splits.Length == 4)
				VersionParsedType = VersionVersionType.MajorMinorBuildRevision;
			else
			VersionParsedType = VersionVersionType.MajorMinor;

			try
			{
				int major = int.Parse(splits[0]);
				int minor = int.Parse(splits[1]);
				if(splits.Length == 2)
				{
					Version = new Version(major, minor);
					return;
				}
				int build = int.Parse(splits[2]);
				if(splits.Length == 3)
				{
					Version = new Version(major, minor, build);
					return;
				}
				Version = new Version(major, minor, build, int.Parse(splits[3]));
				return;
			}
			catch(Exception e)
			{
				throw new InvalidDataException(string.Format("The Version value ({0}) for 'version' is not valid. See Inner"
					+ " Exception.", value), e);
			}
		}

		#endregion Methods
	}
}
