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

		/// <summary>
		///   True if the flags attribute should be added to the enumeration, false if not.
		/// </summary>
		public bool Flags { get; set; }

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
		/// <exception cref="ArgumentNullException"><paramref name="access"/>, <paramref name="name"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="access"/>, <paramref name="name"/>, or <paramref name="summary"/> is an empty string.</exception>
		public EnumInfo(string access, string name, string summary, string remarks = null, string baseType = null) : base(access, name, summary, baseType, remarks)
		{
			Values = new List<EnumValueInfo>();
			Flags = false;
		}

		/// <summary>
		///   Writes the enumeration to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the code to.</param>
		/// <param name="indentOffset">Number of indentations to add before the code.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
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

			if (Flags)
				DocumentationHelper.WriteLine(wr, "[Flags]", indentOffset);

			if (Base != null && Base.Length > 0)
				DocumentationHelper.WriteLine(wr, string.Format("{0} enum {1} : {2}", Access, Name, Base), indentOffset);
			else
				DocumentationHelper.WriteLine(wr, string.Format("{0} enum {1}", Access, Name), indentOffset);
			DocumentationHelper.WriteLine(wr, "{", indentOffset);
			DocumentationHelper.WriteRegionStart(wr, "Names", indentOffset + 1);

			// Write out each value in the enumeration.
			for(int i = 0; i < Values.Count; i++)
			{
				Values[i].Write(wr, indentOffset + 1);
				if (i < Values.Count - 1)
					wr.WriteLine();
			}

			DocumentationHelper.WriteRegionEnd(wr, "Names", indentOffset + 1);
			DocumentationHelper.WriteLine(wr, "}", indentOffset);
		}

		/// <summary>
		///   Generates a method that will take in one of the enumerated types and return the associated summary of the item.
		/// </summary>
		/// <returns><see cref="MethodInfo"/> containing the method to return the summary.</returns>
		/// <remarks>
		///   This method is generated to prevent having to parse the XML documentation to obtain the values if needed. If
		///   <see cref="Flags"/> property is set to true then this method will return a comma separated list of the summaries.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///   An attempt was made to generate the method when this <see cref="EnumInfo"/>.<see cref="EnumInfo.Flags"/> property is set to true.
		/// </exception>
		public MethodInfo GenerateGetSummaryMethod()
		{
			string summary = "Gets the summary associated with the specified enumerated item.";
			string returns = "String representing the summary of the enumerated item.";
			if (Flags)
			{
				summary = "Gets a comma separated string of summaries associated with the specified enumerated flagged item.";
				returns = "String representing the summaries of the enumerated item.";
			}
			string exception = string.Format("throw new ArgumentException(string.Format(\"The {0} type ({{0}}) was not recognized as a supported type.\", item));", Name);

			MethodInfo method = new MethodInfo
			(
				"public",
				"string",
				string.Format("Get{0}Summary", Name),
				summary,
				null,
				returns
			);

			method.Parameters.Add(new ParameterInfo(Name, "item", "Item to return the associated summary of."));
			method.Exceptions.Add(new ExceptionInfo("ArgumentException", "The <paramref name=\"item\"/> was not recognized as a valid type."));

			if (Flags)
			{
				method.CodeLines.Add("List<string> summaryList = new List<string>();");
				foreach (EnumValueInfo info in Values)
				{
					method.CodeLines.Add(string.Format("if(item.HasFlag({0}.{1}))", Name, info.Name));
					method.CodeLines.Add(string.Format("	summaryList.Add(\"{0}\");", info.Summary));
				}
				method.CodeLines.Add(string.Empty);
				method.CodeLines.Add("if(summaryList.Count == 0)");
				method.CodeLines.Add(string.Format("	{0}", exception));
				method.CodeLines.Add(string.Empty);
				method.CodeLines.Add("StringBuilder sb = new StringBuilder();");
				method.CodeLines.Add("for(int i = 0; i < summaryList.Count; i++)");
				method.CodeLines.Add("{");
				method.CodeLines.Add("	sb.Append(sumaryList[i]);");
				method.CodeLines.Add("	if(i != summaryList.Count - 1)");
				method.CodeLines.Add("		sb.Append(\", \");");
				method.CodeLines.Add("}");
				method.CodeLines.Add("return sb.ToString();");
			}
			else
			{
				method.CodeLines.Add("switch(item)");
				method.CodeLines.Add("{");
				foreach (EnumValueInfo info in Values)
				{
					method.CodeLines.Add(string.Format("	case {0}.{1}:", Name, info.Name));
					method.CodeLines.Add(string.Format("		return \"{0}\";", info.Summary));
				}
				method.CodeLines.Add("	default:");
				method.CodeLines.Add(string.Format("		{0}", exception));
				method.CodeLines.Add("}");
			}
			return method;
		}

		/// <summary>
		///   Generates a method that will take in one of the enumerated types and return the associated remarks of the item.
		/// </summary>
		/// <returns><see cref="MethodInfo"/> containing the method to return the remarks.</returns>
		/// <remarks>
		///   This method is generated to prevent having to parse the XML documentation to obtain the values if needed. If
		///   <see cref="Flags"/> property is set to true then this method will return a comma separated list of the remarks.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		///   An attempt was made to generate the method when this <see cref="EnumInfo"/>.<see cref="EnumInfo.Flags"/> property is set to true.
		/// </exception>
		public MethodInfo GenerateGetRemarksMethod()
		{
			string summary = "Gets the remarks associated with the specified enumerated item.";
			string returns = "String representing the remarks of the enumerated item.";
			if (Flags)
			{
				summary = "Gets a comma separated string of the remarks associated with the specified enumerated flagged item.";
				returns = "Comma separated string representing the remarks.";
			}
			string exception = string.Format("throw new ArgumentException(string.Format(\"The {0} type ({{0}}) was not recognized as a supported type.\", item));", Name);

			MethodInfo method = new MethodInfo
			(
				"public",
				"string",
				string.Format("Get{0}Remarks", Name),
				summary,
				null,
				returns
			);

			method.Parameters.Add(new ParameterInfo(Name, "item", "Item to return the associated remarks of."));
			method.Exceptions.Add(new ExceptionInfo("ArgumentException", "The <paramref name=\"item\"/> was not recognized as a valid type."));

			if (Flags)
			{
				method.CodeLines.Add("List<string> remarksList = new List<string>();");
				foreach (EnumValueInfo info in Values)
				{
					method.CodeLines.Add(string.Format("if(item.HasFlag({0}.{1}))", Name, info.Name));
					method.CodeLines.Add(string.Format("	remarksList.Add(\"{0}\");", info.Remarks));
				}
				method.CodeLines.Add(string.Empty);
				method.CodeLines.Add("if(remarksList.Count == 0)");
				method.CodeLines.Add(string.Format("	{0}", exception));
				method.CodeLines.Add(string.Empty);
				method.CodeLines.Add("StringBuilder sb = new StringBuilder();");
				method.CodeLines.Add("for(int i = 0; i < remarksList.Count; i++)");
				method.CodeLines.Add("{");
				method.CodeLines.Add("	sb.Append(remarksList[i]);");
				method.CodeLines.Add("	if(i != remarksList.Count - 1)");
				method.CodeLines.Add("		sb.Append(\", \");");
				method.CodeLines.Add("}");
				method.CodeLines.Add("return sb.ToString();");
			}
			else
			{
				method.CodeLines.Add("switch(item)");
				method.CodeLines.Add("{");
				foreach (EnumValueInfo info in Values)
				{
					method.CodeLines.Add(string.Format("	case {0}.{1}:", Name, info.Name));
					method.CodeLines.Add(string.Format("		return \"{0}\";", info.Remarks));
				}
				method.CodeLines.Add("	default:");
				method.CodeLines.Add(string.Format("		{0}", exception));
				method.CodeLines.Add("}");
			}
			return method;
		}

		#endregion Methods
	}
}
