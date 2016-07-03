//********************************************************************************************************************************
// Filename:    EnumInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified C# enumeration.
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
	///   Represents a C# enumeration.
	/// </summary>
	public class EnumInfo : NamespaceTypeInfo
	{
		#region Properties

		/// <summary>
		///   List of <see cref="EnumValueInfo"/> objects representing the values in the enumeration.
		/// </summary>
		public List<EnumValueInfo> Values { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="EnumInfo"/> object.
		/// </summary>
		/// <param name="access">The access description of the type.</param>
		/// <param name="name">Name of the type.</param>
		/// <param name="summary">Summary description of the type.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <param name="baseType">The base type of the enumeration. Can be null.</param>
		/// <exception cref="ArgumentNullException"><i>access</i>, <i>name</i>, or <i>summary</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>access</i>, <i>name</i>, or <i>summary</i> is an empty string.</exception>
		public EnumInfo(string access, string name, string summary, string remarks = null, string baseType = null) : base(access, name, summary, baseType, remarks)
		{
			Values = new List<EnumValueInfo>();
		}

		/// <summary>
		///   Writes the enumeration to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> ojbect to write the code to.</param>
		/// <param name="indentOffset">Number of indentations to add before the code.</param>
		/// <exception cref="ArgumentNullException"><i>wr</i> is a null reference.</exception>
		/// <exception cref="InvalidOperationException"><see cref="Values"/> array is null or empty.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (indentOffset < 0)
				indentOffset = 0;

			if (Values.Count == 0)
				throw new InvalidOperationException("An attempt was made to write the Enumeration out to a file, but no Values were specified.");

			DocumentationHelper.WriteComponentHeader(wr, Summary, indentOffset);
			if (Base != null && Base.Length > 0)
				DocumentationHelper.WriteLine(wr, string.Format("{0} enum {1} : {2}", Access, Name, Base), indentOffset);
			else
				DocumentationHelper.WriteLine(wr, string.Format("{0} enum {1}", Access, Name), indentOffset);
			DocumentationHelper.WriteLine(wr, "{", indentOffset);
			DocumentationHelper.WriteRegionStart(wr, "Enumerations", indentOffset + 1);

			// Write out each value in the enumeration.
			for(int i = 0; i < Values.Count; i++)
			{
				Values[i].Write(wr, indentOffset + 1);
				if (i < Values.Count - 1)
					wr.WriteLine();
			}

			DocumentationHelper.WriteRegionEnd(wr, "Enumerations", indentOffset + 1);
			DocumentationHelper.WriteLine(wr, "}", indentOffset);
		}

		#endregion Methods
	}
}
