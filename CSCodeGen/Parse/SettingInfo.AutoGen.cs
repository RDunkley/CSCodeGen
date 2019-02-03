// ******************************************************************************************************************************
// Filename:    SettingInfo.AutoGen.cs
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
// CSCodeGen.Parse.SettingInfo (class) (public, partial)
//   Properties:
//               Name                  (public)
//               Ordinal               (public)
//               Value                 (public)
//
//   Methods:
//               SettingInfo(2)        (public)
//               CreateElement         (public)
//               GetNameString         (public)
//               GetValueString        (public)
//               SetNameFromString     (public)
//               SetValueFromString    (public)
//*******************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CSCodeGen.Parse
{
	//***************************************************************************************************************************
	/// <summary>Represents a setting from the file.</summary>
	//***************************************************************************************************************************
	public partial class SettingInfo
	{
		#region Properties

		//***********************************************************************************************************************
		/// <summary>Gets the name of the setting.</summary>
		//***********************************************************************************************************************
		public string Name { get; set; }

		//***********************************************************************************************************************
		/// <summary>Gets the index of this object in relation to the other child element of this object's parent.</summary>
		///
		/// <remarks>
		///   If the value is -1, then this object was not created from an XML node and the property has not been set.
		/// </remarks>
		//***********************************************************************************************************************
		public int Ordinal { get; set; }

		//***********************************************************************************************************************
		/// <summary>Gets the value of the setting. Can be null. Can be empty.</summary>
		//***********************************************************************************************************************
		public string Value { get; set; }

		#endregion Properties

		#region Methods

		//***********************************************************************************************************************
		/// <overloads><summary>Instantiates a new <see cref="SettingInfo"/> object.</summary></overloads>
		///
		/// <summary>Instantiates a new <see cref="SettingInfo"/> object using the provided information.</summary>
		///
		/// <param name="name">'name' String attribute contained in the XML element.</param>
		/// <param name="valueValue">'value' String attribute contained in the XML element. Can be null. Can be empty.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="name"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference.</exception>
		//***********************************************************************************************************************
		public SettingInfo(string name, string valueValue)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentException("name is empty");
			Name = name;
			Value = valueValue;
			Ordinal = -1;
		}

		//***********************************************************************************************************************
		/// <summary>Instantiates a new <see cref="SettingInfo"/> object from an <see cref="XmlNode"/> object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="node"/> does not correspond to a setting node or is not an 'Element' type node or <paramref
		///   name="ordinal"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//***********************************************************************************************************************
		public SettingInfo(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(ordinal < 0)
				throw new ArgumentException("the ordinal specified is negative.");
			if(node.NodeType != XmlNodeType.Element)
				throw new ArgumentException("node is not of type 'Element'.");
			if(string.Compare(node.Name, "setting", false) != 0)
				throw new ArgumentException("node does not correspond to a setting node.");

			XmlAttribute attrib;

			// name
			attrib = node.Attributes["name"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (name) is not optional, but was not found in the XML"
					+ " element (setting).");
			SetNameFromString(attrib.Value);

			// value
			attrib = node.Attributes["value"];
			if(attrib == null)
				Value = null;
			else
				SetValueFromString(attrib.Value);
			Ordinal = ordinal;
		}

		//***********************************************************************************************************************
		/// <summary>Creates an XML element for this object using the provided <see cref="XmlDocument"/> object.</summary>
		///
		/// <param name="doc"><see cref="XmlDocument"/> object to generate the element from.</param>
		///
		/// <returns><see cref="XmlElement"/> object containing this classes data.</returns>
		///
		/// <exception cref="ArgumentNullException"><paramref name="doc"/> is a null reference.</exception>
		//***********************************************************************************************************************
		public XmlElement CreateElement(XmlDocument doc)
		{
			if(doc == null)
				throw new ArgumentNullException("doc");
			XmlElement returnElement = doc.CreateElement("setting");

			string valueString;

			// name
			valueString = GetNameString();
			returnElement.SetAttribute("name", valueString);

			// value
			valueString = GetValueString();
			if(valueString != null)
				returnElement.SetAttribute("value", valueString);
			return returnElement;
		}

		//***********************************************************************************************************************
		/// <summary>Gets a string representation of Name.</summary>
		///
		/// <returns>String representing the value.</returns>
		//***********************************************************************************************************************
		public string GetNameString()
		{
			return Name;
		}

		//***********************************************************************************************************************
		/// <summary>Gets a string representation of Value.</summary>
		///
		/// <returns>String representing the value. Can be null. Can be empty.</returns>
		//***********************************************************************************************************************
		public string GetValueString()
		{
			return Value;
		}

		//***********************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Name.</summary>
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
		//***********************************************************************************************************************
		public void SetNameFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'name' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'name' is an empty string.");
			Name = value;
		}

		//***********************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Value.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">The string value could not be parsed.</exception>
		//***********************************************************************************************************************
		public void SetValueFromString(string value)
		{
			if(value == null)
			{
				Value = null;
				return;
			}
			Value = value;
		}

		#endregion Methods
	}
}
