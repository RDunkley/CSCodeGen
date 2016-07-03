//********************************************************************************************************************************
// Filename:    EnumValueInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified C# enumerated type value.
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

namespace CSCodeGen
{
	/// <summary>
	///   Represents an enumerated type in a C# enumeration.
	/// </summary>
	public class EnumValueInfo
	{
		#region Properties

		/// <summary>
		///   Value of the enumerated type.
		/// </summary>
		public string Value { get; protected set; }

		/// <summary>
		///   Name of the type.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///   Specifies the documentation summary that will be generated with the type.
		/// </summary>
		public string Summary { get; protected set; }

		/// <summary>
		///   Specifies the documentation remarks that will be generated with the type.
		/// </summary>
		public string Remarks { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="EnumValueInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the enumerated type.</param>
		/// <param name="value">Value of the enumerated type. Can be null or empty.</param>
		/// <param name="summary">Summary description of the enumerated type.</param>
		/// <param name="remarks">Additional remarks to add to the documentation. Can be null.</param>
		/// <exception cref="ArgumentNullException"><i>name</i> or <i>summary</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>name</i> or <i>summary</i> is an empty string.</exception>
		public EnumValueInfo(string name, string value, string summary, string remarks = null)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");
			if (summary == null)
				throw new ArgumentNullException("summary");
			if (summary.Length == 0)
				throw new ArgumentException("summary is an empty string");

			Name = name;
			Value = value;
			Summary = summary;
			Remarks = remarks;
		}

		/// <summary>
		///   Writes the enumerated value to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> ojbect to write the code to.</param>
		/// <param name="indentOffset">Number of indentations to add before the code.</param>
		/// <exception cref="ArgumentNullException"><i>wr</i> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (indentOffset < 0)
				indentOffset = 0;

			DocumentationHelper.WriteComponentHeader(wr, Summary, indentOffset, Remarks);
			if(Value == null || Value.Length == 0)
				DocumentationHelper.WriteLine(wr, string.Format("{0},", Name), indentOffset);
			else
				DocumentationHelper.WriteLine(wr, string.Format("{0} = {1},", Name, Value), indentOffset);
		}

		#endregion Methods
	}
}
