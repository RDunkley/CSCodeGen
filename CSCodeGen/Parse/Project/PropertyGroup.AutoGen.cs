// ******************************************************************************************************************************
// Filename:    PropertyGroup.AutoGen.cs
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
// CSCodeGen.Parse.Project.PropertyGroup (class) (public, partial)
//   Properties:
//               Condition                       (public)
//               Ordinal                         (public)
//               TargetFrameworks                (public)
//               WarningLevels                   (public)
//
//   Methods:
//               PropertyGroup(2)                (public)
//               CreateElement                   (public)
//               GetConditionString              (public)
//               SetConditionFromString          (public)
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
	/// <summary>In memory representation of the XML element "PropertyGroup".</summary>
	//***************************************************************************************************************************
	public partial class PropertyGroup
	{
		#region Properties

		//***********************************************************************************************************************
		/// <summary>Gets or sets the value of the child Condition component. Can be null.</summary>
		//***********************************************************************************************************************
		public string Condition { get; set; }

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
		public TargetFramework[] TargetFrameworks { get; private set; }

		//***********************************************************************************************************************
		/// <summary>Gets or sets the child XML elements.</summary>
		//***********************************************************************************************************************
		public WarningLevel[] WarningLevels { get; private set; }

		#endregion Properties

		#region Methods

		//***********************************************************************************************************************
		/// <overloads><summary>Instantiates a new <see cref="PropertyGroup"/> object.</summary></overloads>
		///
		/// <summary>Instantiates a new <see cref="PropertyGroup"/> object using the provided information.</summary>
		///
		/// <param name="condition">'Condition' String attribute contained in the XML element. Can be null.</param>
		/// <param name="targetFrameworks">
		///   Array of TargetFramework elements which are child elements of this node. Can be empty.
		/// </param>
		/// <param name="warningLevels">
		///   Array of WarningLevel elements which are child elements of this node. Can be empty.
		/// </param>
		///
		/// <exception cref="ArgumentException"><paramref name="condition"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="targetFrameworks"/>, or <paramref name="warningLevels"/> is a null reference.
		/// </exception>
		//***********************************************************************************************************************
		public PropertyGroup(string condition, TargetFramework[] targetFrameworks, WarningLevel[] warningLevels)
		{
			if(condition != null && condition.Length == 0)
				throw new ArgumentException("condition is empty");
			if(targetFrameworks == null)
				throw new ArgumentNullException("targetFrameworks");
			if(warningLevels == null)
				throw new ArgumentNullException("warningLevels");
			Condition = condition;
			TargetFrameworks = targetFrameworks;
			WarningLevels = warningLevels;
			Ordinal = -1;

			// Compute the maximum index used on any child items.
			int maxIndex = 0;
			foreach(TargetFramework item in TargetFrameworks)
			{
				if(item.Ordinal >= maxIndex)
					maxIndex = item.Ordinal + 1; // Set to first index after this index.
			}
			foreach(WarningLevel item in WarningLevels)
			{
				if(item.Ordinal >= maxIndex)
					maxIndex = item.Ordinal + 1; // Set to first index after this index.
			}

			// Assign ordinal for any child items that don't have it set (-1).
			foreach(TargetFramework item in TargetFrameworks)
			{
				if(item.Ordinal == -1)
					item.Ordinal = maxIndex++;
			}
			foreach(WarningLevel item in WarningLevels)
			{
				if(item.Ordinal == -1)
					item.Ordinal = maxIndex++;
			}
		}

		//***********************************************************************************************************************
		/// <summary>Instantiates a new <see cref="PropertyGroup"/> object from an <see cref="XmlNode"/> object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="node"/> does not correspond to a PropertyGroup node or is not an 'Element' type node or <paramref
		///   name="ordinal"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//***********************************************************************************************************************
		public PropertyGroup(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(ordinal < 0)
				throw new ArgumentException("the ordinal specified is negative.");
			if(node.NodeType != XmlNodeType.Element)
				throw new ArgumentException("node is not of type 'Element'.");
			if(string.Compare(node.Name, "PropertyGroup", false) != 0)
				throw new ArgumentException("node does not correspond to a PropertyGroup node.");

			XmlAttribute attrib;

			// Condition
			attrib = node.Attributes["Condition"];
			if(attrib == null)
				Condition = null;
			else
				SetConditionFromString(attrib.Value);

			// Read the child objects.
			List<TargetFramework> targetFrameworksList = new List<TargetFramework>();
			List<WarningLevel> warningLevelsList = new List<WarningLevel>();
			int index = 0;
			foreach(XmlNode child in node.ChildNodes)
			{
				if(child.NodeType == XmlNodeType.Element && child.Name == "TargetFramework")
					targetFrameworksList.Add(new TargetFramework(child, index++));
				if(child.NodeType == XmlNodeType.Element && child.Name == "WarningLevel")
					warningLevelsList.Add(new WarningLevel(child, index++));
			}
			TargetFrameworks = targetFrameworksList.ToArray();
			WarningLevels = warningLevelsList.ToArray();

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
			XmlElement returnElement = doc.CreateElement("PropertyGroup");

			string valueString;

			// Condition
			valueString = GetConditionString();
			if(valueString != null)
				returnElement.SetAttribute("Condition", valueString);
			// Build up dictionary of indexes and corresponding items.
			Dictionary<int, object> lookup = new Dictionary<int, object>();

			foreach(TargetFramework child in TargetFrameworks)
			{
				if(lookup.ContainsKey(child.Ordinal))
					throw new InvalidOperationException("An attempt was made to generate the XML element with two child elements"
						+ " with the same ordinal.Ordinals must be unique across all child objects.");
				lookup.Add(child.Ordinal, child);
			}

			foreach(WarningLevel child in WarningLevels)
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
				if(lookup[key] is TargetFramework)
					returnElement.AppendChild(((TargetFramework)lookup[key]).CreateElement(doc));
				if(lookup[key] is WarningLevel)
					returnElement.AppendChild(((WarningLevel)lookup[key]).CreateElement(doc));
			}
			return returnElement;
		}

		//***********************************************************************************************************************
		/// <summary>Gets a string representation of Condition.</summary>
		///
		/// <returns>String representing the value. Can be null.</returns>
		//***********************************************************************************************************************
		public string GetConditionString()
		{
			return Condition;
		}

		//***********************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Condition.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item>The string value is an empty string.</item>
		///     <item>The string value could not be parsed.</item>
		///   </list>
		/// </exception>
		//***********************************************************************************************************************
		public void SetConditionFromString(string value)
		{
			if(value == null)
			{
				Condition = null;
				return;
			}
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'Condition' is an empty string.");
			Condition = value;
		}

		#endregion Methods
	}
}
