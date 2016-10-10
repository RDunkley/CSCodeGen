//********************************************************************************************************************************
// Filename:    PropertyInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified C# property.
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2016
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//********************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.IO;

namespace CSCodeGen
{
	/// <summary>
	///   Represents the information of a C# Property.
	/// </summary>
	public class PropertyInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   Exceptions that could be thrown when accessing the property.
		/// </summary>
		public List<ExceptionInfo> Exceptions { get; private set; }

		/// <summary>
		/// Determines the accessibility of the get property. If null the property is not included (defaults to overall property accessibility).
		/// </summary>
		/// <value>The get accessibility string.</value>
		public string GetAccess { get; set; }

		/// <summary>
		///   Contains the code lines for the get portion of the property. If null then 'get' is not created. If empty then 'get' is created with default code.
		/// </summary>
		/// <value>The code lines for the property's getter.</value>
		public string[] GetterLines { get; set; }

		/// <summary>
		/// Determines the accessibility of the set property. If null the property is not included (defaults to overall property accessibility).
		/// </summary>
		/// <value>The set accessibility string.</value>
		public string SetAccess { get; set; }

		/// <summary>
		///   Contains the code lines for the set portion of the property. If null then 'set' is not created. If empty then 'set' is created with default code.
		/// </summary>
		/// <value>The code lines for the property's setter.</value>
		public string[] SetterLines { get; set; }

		/// <summary>
		///   Type of the property (Ex: int, string, etc.).
		/// </summary>
		public string Type { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="PropertyInfo"/> object.
		/// </summary>
		/// <param name="access">The access description of the type.</param>
		/// <param name="type">Type of the property.</param>
		/// <param name="name">Name of the type.</param>
		/// <param name="summary">Summary description of the type.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <param name="getAccess">Accessibility of the getter. If null <paramref name="access"/> is used.</param>
		/// <param name="setAccess">Accessibility of the setter. If null <paramref name="access"/> is used.</param>
		/// <exception cref="ArgumentNullException"><paramref name="access"/>, <paramref name="name"/>, <paramref name="type"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="access"/>, <paramref name="name"/>, <paramref name="type"/>, or <paramref name="summary"/> is an empty string.</exception>
		public PropertyInfo(string access, string type, string name, string summary, string remarks = null, string getAccess = null, string setAccess = null) 
			: this(access, type, name, summary, new string[0], new string[0], remarks, getAccess, setAccess)
		{
		}

		/// <summary>
		///   Instantiates a new <see cref="PropertyInfo"/> object.
		/// </summary>
		/// <param name="access">The access description of the type.</param>
		/// <param name="type">Type of the property.</param>
		/// <param name="name">Name of the type.</param>
		/// <param name="summary">Summary description of the type.</param>
		/// <param name="getterLines">Code lines for the getter.</param>
		/// <param name="setterLines">Code lines for the setter.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <param name="getAccess">Accessibility of the getter. If null <paramref name="access"/> is used.</param>
		/// <param name="setAccess">Accessibility of the setter. If null <paramref name="access"/> is used.</param>
		/// <exception cref="ArgumentNullException"><paramref name="access"/>, <paramref name="name"/>, <paramref name="type"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="access"/>, <paramref name="name"/>, <paramref name="type"/>, or <paramref name="summary"/> is an empty string.</exception>
		public PropertyInfo(string access, string type, string name, string summary, string[] getterLines, string[] setterLines, string remarks = null, string getAccess = null, string setAccess = null)
			 : base(access, name, summary, remarks)
		{
			if (type == "null")
				throw new ArgumentNullException("type");
			if (type.Length == 0)
				throw new ArgumentException("type is an empty string");
			Type = type;
			SetAccess = setAccess;
			GetAccess = getAccess;
			SetterLines = setterLines;
			GetterLines = getterLines;
			Exceptions = new List<ExceptionInfo>();
		}

		/// <summary>
		///   Writes the property to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the property to.</param>
		/// <param name="indentOffset">Number of indentations to add before the code.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (GetterLines == null && SetterLines == null)
				throw new InvalidOperationException("GetterLines and SetterLines are both null. Cannot create a property that does not have a setter or getter.");

			DocumentationHelper.WriteComponentHeader(wr, Summary, indentOffset, Remarks, null, null, Exceptions.ToArray());
			string getAccessString = string.Empty;
			if (GetAccess != null && GetAccess.Length > 0)
				getAccessString = string.Format("{0} ", GetAccess);
			string setAccessString = string.Empty;
			if (SetAccess != null && SetAccess.Length > 0)
				setAccessString = string.Format("{0} ", SetAccess);
			if (GetterLines != null && GetterLines.Length == 0 && SetterLines == null) // default getter only.
				DocumentationHelper.WriteLine(wr, string.Format("{0} {1} {2} {{ {3}get; }}", Access, Type, Name, getAccessString), indentOffset);
			else if (GetterLines == null && SetterLines != null && SetterLines.Length == 0) // default setter only.
				DocumentationHelper.WriteLine(wr, string.Format("{0} {1} {2} {{ {3}set; }}", Access, Type, Name, setAccessString), indentOffset);
			else if (GetterLines != null && GetterLines.Length == 0 && SetterLines != null && SetterLines.Length == 0) // default getter and setter.
				DocumentationHelper.WriteLine(wr, string.Format("{0} {1} {2} {{ {3}get; {4}set; }}", Access, Type, Name, getAccessString, setAccessString), indentOffset);
			else
			{
				DocumentationHelper.WriteLine(wr, string.Format("{0} {1} {2}", Access, Type, Name), indentOffset);
				DocumentationHelper.WriteLine(wr, "{", indentOffset);

				if (GetterLines != null)
				{
					indentOffset++;
					if (GetterLines.Length == 0)
					{
						DocumentationHelper.WriteLine(wr, string.Format("{0}get;", getAccessString), indentOffset);
					}
					else
					{
						DocumentationHelper.WriteLine(wr, string.Format("{0}get", getAccessString), indentOffset);
						DocumentationHelper.WriteLine(wr, "{", indentOffset);
						foreach (string line in GetterLines)
							DocumentationHelper.WriteLine(wr, line, indentOffset + 1);
						DocumentationHelper.WriteLine(wr, "}", indentOffset);
					}
					indentOffset--;
				}

				if (SetterLines != null)
				{
					indentOffset++;
					if (SetterLines.Length == 0)
					{
						DocumentationHelper.WriteLine(wr, string.Format("{0}set;", setAccessString), indentOffset);
					}
					else
					{
						DocumentationHelper.WriteLine(wr, string.Format("{0}set", setAccessString), indentOffset);
						DocumentationHelper.WriteLine(wr, "{", indentOffset);
						foreach (string line in SetterLines)
							DocumentationHelper.WriteLine(wr, line, indentOffset + 1);
						DocumentationHelper.WriteLine(wr, "}", indentOffset);
					}
					indentOffset--;
				}
				DocumentationHelper.WriteLine(wr, "}", indentOffset);
			}
		}

		#endregion Methods
	}
}
