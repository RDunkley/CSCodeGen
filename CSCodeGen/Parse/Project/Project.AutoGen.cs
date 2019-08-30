// ******************************************************************************************************************************
// Filename:    Project.AutoGen.cs
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
// CSCodeGen.Parse.Project.Project (class) (public, partial)
//   Properties:
//               Ordinal                   (public)
//               PropertyGroups            (public)
//               Sdk                       (public)
//
//   Methods:
//               Project(2)                (public)
//               CreateElement             (public)
//               GetSdkString              (public)
//               SetSdkFromString          (public)
//*******************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CSCodeGen.Parse.Project
{
	//***************************************************************************************************************************
	/// <summary>In memory representation of the XML element "Project".</summary>
	//***************************************************************************************************************************
	public partial class Project
	{
		#region Properties

		//***********************************************************************************************************************
		/// <summary>Gets the index of this object in relation to the other child element of this object's parent.</summary>
		///
		/// <remarks>
		///   If the value is -1, then this object was not created from an XML node and the property has not been set.
		/// </remarks>
		//***********************************************************************************************************************
		public int Ordinal { get; set; }

		//***********************************************************************************************************************
		/// <summary>Gets or sets the child XML elements.</summary>
		//***********************************************************************************************************************
		public PropertyGroup[] PropertyGroups { get; private set; }

		//***********************************************************************************************************************
		/// <summary>Gets or sets the value of the child Sdk component.</summary>
		//***********************************************************************************************************************
		public string Sdk { get; set; }

		#endregion Properties

		#region Methods

		//***********************************************************************************************************************
		/// <overloads><summary>Instantiates a new <see cref="Project"/> object.</summary></overloads>
		///
		/// <summary>Instantiates a new <see cref="Project"/> object using the provided information.</summary>
		///
		/// <param name="sdk">'Sdk' String attribute contained in the XML element.</param>
		/// <param name="propertyGroups">
		///   Array of PropertyGroup elements which are child elements of this node. Can be empty.
		/// </param>
		///
		/// <exception cref="ArgumentException"><paramref name="sdk"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="sdk"/>, or <paramref name="propertyGroups"/> is a null reference.
		/// </exception>
		//***********************************************************************************************************************
		public Project(string sdk, PropertyGroup[] propertyGroups)
		{
			if(sdk == null)
				throw new ArgumentNullException("sdk");
			if(sdk.Length == 0)
				throw new ArgumentException("sdk is empty");
			if(propertyGroups == null)
				throw new ArgumentNullException("propertyGroups");
			Sdk = sdk;
			PropertyGroups = propertyGroups;
			Ordinal = -1;

			// Compute the maximum index used on any child items.
			int maxIndex = 0;
			foreach(PropertyGroup item in PropertyGroups)
			{
				if(item.Ordinal >= maxIndex)
					maxIndex = item.Ordinal + 1; // Set to first index after this index.
			}

			// Assign ordinal for any child items that don't have it set (-1).
			foreach(PropertyGroup item in PropertyGroups)
			{
				if(item.Ordinal == -1)
					item.Ordinal = maxIndex++;
			}
		}

		//***********************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Project"/> object from an <see cref="XmlNode"/> object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="node"/> does not correspond to a Project node or is not an 'Element' type node or <paramref
		///   name="ordinal"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//***********************************************************************************************************************
		public Project(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(ordinal < 0)
				throw new ArgumentException("the ordinal specified is negative.");
			if(node.NodeType != XmlNodeType.Element)
				throw new ArgumentException("node is not of type 'Element'.");
			if(string.Compare(node.Name, "Project", false) != 0)
				throw new ArgumentException("node does not correspond to a Project node.");

			XmlAttribute attrib;

			// Sdk
			attrib = node.Attributes["Sdk"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (Sdk) is not optional, but was not found in the XML"
					+ " element (Project).");
			SetSdkFromString(attrib.Value);

			// Read the child objects.
			List<PropertyGroup> propertyGroupsList = new List<PropertyGroup>();
			int index = 0;
			foreach(XmlNode child in node.ChildNodes)
			{
				if(child.NodeType == XmlNodeType.Element && child.Name == "PropertyGroup")
					propertyGroupsList.Add(new PropertyGroup(child, index++));
			}
			PropertyGroups = propertyGroupsList.ToArray();

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
			XmlElement returnElement = doc.CreateElement("Project");

			string valueString;

			// Sdk
			valueString = GetSdkString();
			returnElement.SetAttribute("Sdk", valueString);
			// Build up dictionary of indexes and corresponding items.
			Dictionary<int, object> lookup = new Dictionary<int, object>();

			foreach(PropertyGroup child in PropertyGroups)
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
				if(lookup[key] is PropertyGroup)
					returnElement.AppendChild(((PropertyGroup)lookup[key]).CreateElement(doc));
			}
			return returnElement;
		}

		//***********************************************************************************************************************
		/// <summary>Gets a string representation of Sdk.</summary>
		///
		/// <returns>String representing the value.</returns>
		//***********************************************************************************************************************
		public string GetSdkString()
		{
			return Sdk;
		}

		//***********************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Sdk.</summary>
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
		public void SetSdkFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'Sdk' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'Sdk' is an empty string.");
			Sdk = value;
		}

		#endregion Methods
	}
}
