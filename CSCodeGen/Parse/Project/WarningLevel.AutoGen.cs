// ******************************************************************************************************************************
// Filename:    WarningLevel.AutoGen.cs
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
// CSCodeGen.Parse.Project.WarningLevel (class) (public, partial)
//   Properties:
//               Ordinal                        (public)
//               Text                           (public)
//
//   Methods:
//               WarningLevel(2)                (public)
//               CreateElement                  (public)
//               GetTextString                  (public)
//               SetTextFromString              (public)
//*******************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CSCodeGen.Parse.Project
{
	//***************************************************************************************************************************
	/// <summary>In memory representation of the XML element "WarningLevel".</summary>
	//***************************************************************************************************************************
	public partial class WarningLevel
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
		public int Text { get; set; }

		#endregion Properties

		#region Methods

		//***********************************************************************************************************************
		/// <overloads><summary>Instantiates a new <see cref="WarningLevel"/> object.</summary></overloads>
		///
		/// <summary>Instantiates a new <see cref="WarningLevel"/> object using the provided information.</summary>
		///
		/// <param name="text">Child Text element value.</param>
		//***********************************************************************************************************************
		public WarningLevel(int text)
		{
			Text = text;
			Ordinal = -1;
		}

		//***********************************************************************************************************************
		/// <summary>Instantiates a new <see cref="WarningLevel"/> object from an <see cref="XmlNode"/> object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="node"/> does not correspond to a WarningLevel node or is not an 'Element' type node or <paramref
		///   name="ordinal"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//***********************************************************************************************************************
		public WarningLevel(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(ordinal < 0)
				throw new ArgumentException("the ordinal specified is negative.");
			if(node.NodeType != XmlNodeType.Element)
				throw new ArgumentException("node is not of type 'Element'.");
			if(string.Compare(node.Name, "WarningLevel", false) != 0)
				throw new ArgumentException("node does not correspond to a WarningLevel node.");

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
					+ " (WarningLevel).");

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
			XmlElement returnElement = doc.CreateElement("WarningLevel");

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

			return Text.ToString();
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
			int returnValue = 0;
			bool parsed = false;
			try
			{

				// Attempt to parse the number as just an integer.
				returnValue = int.Parse(value, NumberStyles.Integer | NumberStyles.AllowThousands);
				parsed = true;
			}
			catch(FormatException e)
			{
				throw new InvalidDataException(string.Format("The int value specified ({0}) is not in a valid int string format:"
					+ " {1}.", value, e.Message), e);
			}
			catch(OverflowException e)
			{
				throw new InvalidDataException(string.Format("The int value specified ({0}) was larger or smaller than a int"
					+ " value (Min: {1}, Max: {2}).", value, int.MinValue.ToString(), int.MaxValue.ToString()), e);
			}

			if(!parsed)
				throw new InvalidDataException(string.Format("The int value specified ({0}) is not in a valid int string"
					+ " format.", value));

			// Verify that the int value is not lower than the minimum size.
			if(returnValue < 0)
				throw new InvalidDataException(string.Format("The int value specified ({0}) was less than the minimum value"
					+ " allowed for this type (0).", value));

			Text = returnValue;
		}

		#endregion Methods
	}
}
