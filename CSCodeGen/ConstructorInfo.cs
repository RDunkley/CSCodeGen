//********************************************************************************************************************************
// Filename:    ConstructorInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified C# class constructor.
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
using System.Text;

namespace CSCodeGen
{
	/// <summary>
	///   Represents the information in a C# constructor.
	/// </summary>
	public class ConstructorInfo : BaseTypeInfo, IComparable<ConstructorInfo>
	{
		#region Properties

		/// <summary>
		///   Specifies any base parameters that should be passed to the base class. Must be in order. Can be null if no base class is called.
		/// </summary>
		public SortedDictionary<int,ParameterInfo> BaseParameters { get; private set; }

		/// <summary>
		///   List of <see cref="ParameterInfo"/> objects representing the parameters of the constructor or null if no parameters are specified.
		/// </summary>
		public List<ParameterInfo> Parameters { get; private set; }

		/// <summary>
		///   List of <see cref="ExceptionInfo"/> objects representing the exceptions that can be thrown from the constructor.
		/// </summary>
		public List<ExceptionInfo> Exceptions { get; private set; }

		/// <summary>
		///   Code lines of the constructor.
		/// </summary>
		public List<string> CodeLines { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ConstructorInfo"/> object.
		/// </summary>
		/// <param name="access">The access description of the type.</param>
		/// <param name="name">Name of the type.</param>
		/// <param name="summary">Summary description of the type.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><i>access</i>, <i>name</i>, or <i>summary</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>access</i>, <i>name</i>, or <i>summary</i> is an empty string.</exception>
		public ConstructorInfo(string access, string name, string summary, string remarks = null) : base(access, name, summary, remarks)
		{
			BaseParameters = new SortedDictionary<int, ParameterInfo>();
			Parameters = new List<ParameterInfo>();
			Exceptions = new List<ExceptionInfo>();
			CodeLines = new List<string>();
		}

		/// <summary>
		///   Writes the constructor to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the code to.</param>
		/// <param name="indentOffset">Number of indentations to add before the code.</param>
		/// <exception cref="ArgumentNullException"><i>wr</i> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			DocumentationHelper.WriteComponentHeader(wr, Summary, indentOffset, Remarks, null, Parameters.ToArray(), Exceptions.ToArray(), OverloadedSummary);

			// Write the signature.
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0} {1}(", Access, Name);
			int index = 0;
			foreach(ParameterInfo param in Parameters)
			{
				sb.AppendFormat("{0} {1}", param.Type, param.Name);
				if (index < Parameters.Count - 1)
					sb.Append(", ");
				index++;
			}
			sb.Append(")");

			// Add the base constructor parameters if needed.
			if (BaseParameters.Count > 0)
			{
				sb.Append(" : base(");
				SortedDictionary<int, ParameterInfo>.ValueCollection values = BaseParameters.Values;
				int pIndex = 0;
				foreach(ParameterInfo param in values)
				{
					sb.Append(param.Name);
					if (pIndex < values.Count - 1)
						sb.Append(", ");
					pIndex++;
				}
				sb.Append(")");
			}

			DocumentationHelper.WriteLine(wr, sb.ToString(), indentOffset);
			DocumentationHelper.WriteLine(wr, "{", indentOffset);
			indentOffset++;

			// Generate exception code for the parameters.
			foreach (ParameterInfo param in Parameters)
			{
				if(param.CanBeNull.HasValue)
				{
					if(!param.CanBeNull.Value)
					{
						DocumentationHelper.WriteLine(wr, string.Format("if({0} == null)", param.Name), indentOffset);
						DocumentationHelper.WriteLine(wr, string.Format("	throw new ArgumentNullException(\"{0}\");", param.Name), indentOffset);
						if(param.CanBeEmpty.HasValue)
						{
							if(!param.CanBeEmpty.Value)
							{
								DocumentationHelper.WriteLine(wr, string.Format("if({0}.Length == 0)", param.Name), indentOffset);
								DocumentationHelper.WriteLine(wr, string.Format("	throw new ArgumentException(\"{0} is empty\");", param.Name), indentOffset);
							}
						}
					}
					else
					{
						if (param.CanBeEmpty.HasValue)
						{
							if (!param.CanBeEmpty.Value)
							{
								DocumentationHelper.WriteLine(wr, string.Format("if({0} != null && {0}.Length == 0)", param.Name), indentOffset);
								DocumentationHelper.WriteLine(wr, string.Format("	throw new ArgumentException(\"{0} is empty\");", param.Name), indentOffset);
							}
						}
					}
				}
			}

			// Add the code.
			foreach (string line in CodeLines)
				DocumentationHelper.WriteLine(wr, line, indentOffset);

			indentOffset--;
			DocumentationHelper.WriteLine(wr, "}", indentOffset);
		}

		/// <summary>
		///   Compares a <see cref="ConstructorInfo"/> object with this object and returns an integer that indicates their
		///   relative position in the sort order.
		/// </summary>
		/// <param name="other">Other <see cref="ConstructorInfo"/> object to compare this object to.</param>
		/// <returns>
		///   A 32-bit signed integer that indicates the lexical relationship between the two comparands. Less than zero,
		///   this object proceeds <i>other</i>. Zero, they have the same sort order. Greater than zero, this object is
		///   after <i>other</i> in the sort order.
		/// </returns>
		/// <remarks>This method sorts the constructors using the same means that Visual Studio does.</remarks>
		public int CompareTo(ConstructorInfo other)
		{
			int compare = string.Compare(this.Name, other.Name);

			if(compare != 0)
				return compare;

			int compareLength = this.Parameters.Count;
			if(other.Parameters.Count < compareLength)
				compareLength = other.Parameters.Count;

			// Compare the parameters.
			for(int i = 0; i < compareLength; i++)
			{
				// Compare the type first.
				compare = string.Compare(this.Parameters[i].Type, other.Parameters[i].Type);
				if(compare != 0)
					return compare;

				// Compare the parameter name next.
				compare = string.Compare(this.Parameters[i].Name, other.Parameters[i].Name);
				if(compare != 0)
					return compare;
			}

			// Check if any have more parameters than the other.
			if(this.Parameters.Count > compareLength)
				return 1;
			if(other.Parameters.Count > compareLength)
				return -1;
			return 0;
		}

		#endregion Methods
	}
}
