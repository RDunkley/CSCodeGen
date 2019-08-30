// ******************************************************************************************************************************
// Filename:    TargetFramework.AutoGen.cs
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
// CSCodeGen.Parse.Project.TargetFramework (class) (public, partial)
//   Properties:
//               Ordinal                           (public)
//               Text                              (public)
//
//   Methods:
//               TargetFramework(2)                (public)
//               CreateElement                     (public)
//               GetTextString                     (public)
//               SetTextFromString                 (public)
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
	/// <summary>In memory representation of the XML element "TargetFramework".</summary>
	//***************************************************************************************************************************
	public partial class TargetFramework
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
		/// <summary>Gets or sets the value of the child Text component.</summary>
		//***********************************************************************************************************************
		public string Text { get; set; }

		#endregion Properties

		#region Methods

		//***********************************************************************************************************************
		/// <overloads><summary>Instantiates a new <see cref="TargetFramework"/> object.</summary></overloads>
		///
		/// <summary>Instantiates a new <see cref="TargetFramework"/> object using the provided information.</summary>
		///
		/// <param name="text">Child Text element value.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="text"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="text"/> is a null reference.</exception>
		//***********************************************************************************************************************
		public TargetFramework(string text)
		{
			if(text == null)
				throw new ArgumentNullException("text");
			if(text.Length == 0)
				throw new ArgumentException("text is empty");
			Text = text;
			Ordinal = -1;
		}

		//***********************************************************************************************************************
		/// <summary>Instantiates a new <see cref="TargetFramework"/> object from an <see cref="XmlNode"/> object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="node"/> does not correspond to a TargetFramework node or is not an 'Element' type node or
		///   <paramref name="ordinal"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//***********************************************************************************************************************
		public TargetFramework(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(ordinal < 0)
				throw new ArgumentException("the ordinal specified is negative.");
			if(node.NodeType != XmlNodeType.Element)
				throw new ArgumentException("node is not of type 'Element'.");
			if(string.Compare(node.Name, "TargetFramework", false) != 0)
				throw new ArgumentException("node does not correspond to a TargetFramework node.");

			// Read the child objects.
			bool textFound = false;
			int index = 0;
			foreach(XmlNode child in node.ChildNodes)
			{
				if(child.NodeType == XmlNodeType.Text)
				{
					SetTextFromString(child.Value);
					textFound = true;
				}
			}

			if(!textFound)
				throw new InvalidDataException("An XML child Text node is not optional, but was not found in the XML element"
					+ " (TargetFramework).");

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
			XmlElement returnElement = doc.CreateElement("TargetFramework");

			string valueString;

			valueString = GetTextString();
			XmlText textNode = doc.CreateTextNode(valueString);
			returnElement.AppendChild(textNode);
			return returnElement;
		}

		//***********************************************************************************************************************
		/// <summary>Gets a string representation of Text.</summary>
		///
		/// <returns>String representing the value.</returns>
		//***********************************************************************************************************************
		public string GetTextString()
		{
			return Text;
		}

		//***********************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Text.</summary>
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
		public void SetTextFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'Text' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'Text' is an empty string.");
			Text = value;
		}

		#endregion Methods
	}
}
