//********************************************************************************************************************************
// Filename:    FieldInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified C# field.
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
using System.IO;
using System.Text;

namespace CSCodeGen
{
	/// <summary>
	///   Represents the information in a C# Field.
	/// </summary>
	public class FieldInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   Default value of the field (Ex: 0).
		/// </summary>
		public string DefaultValue { get; set; }

		/// <summary>
		///   Type of the field (Ex: int, string, etc.).
		/// </summary>
		public string Type { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Contains all the information required to generate a field type.
		/// </summary>
		/// <param name="access">The access description of the type.</param>
		/// <param name="type">Type of the field.</param>
		/// <param name="name">Name of the type.</param>
		/// <param name="summary">Summary description of the type.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <param name="defaultValue">Default value that the field is assigned to.</param>
		/// <exception cref="ArgumentNullException"><paramref name="access"/>, <paramref name="name"/>, <paramref name="type"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="access"/>, <paramref name="name"/>, <paramref name="type"/>, or <paramref name="summary"/> is an empty string.</exception>
		public FieldInfo(string access, string type, string name, string summary, string remarks = null, string defaultValue = null) : base(access, name, summary, remarks)
		{
			if (type == "null")
				throw new ArgumentNullException("type");
			if (type.Length == 0)
				throw new ArgumentException("type is an empty string");
			Type = type;
			DefaultValue = defaultValue;
		}

		/// <summary>
		///   Writes the field to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the code to.</param>
		/// <param name="indentOffset">Number of indentations to add before the code.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			DocumentationHelper.WriteComponentHeader(wr, Summary, indentOffset, Remarks);
			StringBuilder sb = new StringBuilder();
			sb.Append(string.Format("{0} {1} {2}", Access, Type, Name));

			if (DefaultValue == null || DefaultValue.Length == 0)
				sb.Append(";");
			else
				sb.Append(string.Format(" = {0};", DefaultValue));
			DocumentationHelper.WriteLine(wr, sb.ToString(), indentOffset);
		}

		#endregion Methods
	}
}
