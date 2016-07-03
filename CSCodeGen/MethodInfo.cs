﻿//********************************************************************************************************************************
// Filename:    MethodInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified C# method.
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
	///   Represents the information in a C# Method.
	/// </summary>
	public class MethodInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   Return type of the method. Can be null if a constructor.
		/// </summary>
		public string ReturnType { get; protected set; }

		/// <summary>
		///   Description of the return type of the method. Unused if <see cref="ReturnType"/> is 'void'. Can not be null if <see cref="ReturnType"/> is not.
		/// </summary>
		public string ReturnTypeDescription { get; set; }

		/// <summary>
		///   List of <see cref="ParameterInfo"/> objects representing the parameters of the method or null if no parameters are specified.
		/// </summary>
		public List<ParameterInfo> Parameters { get; private set; }

		/// <summary>
		///   List of <see cref="ExceptionInfo"/> objects representing the exceptions that can be thrown from the method.
		/// </summary>
		public List<ExceptionInfo> Exceptions { get; private set; }

		/// <summary>
		///   Code lines of the method.
		/// </summary>
		public List<string> CodeLines { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="MethodInfo"/> object.
		/// </summary>
		/// <param name="access">The access description of the type.</param>
		/// <param name="returnType">Return type of the method.</param>
		/// <param name="name">Name of the type.</param>
		/// <param name="summary">Summary description of the type.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <param name="returnTypeDescription">Description of the return type. Can be null if no return type is void.</param>
		/// <exception cref="ArgumentNullException"><i>access</i>, <i>returnType</i>, <i>name</i>, or <i>summary</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>access</i>, <i>returnType</i>, <i>name</i>, or <i>summary</i> is an empty string.</exception>
		public MethodInfo(string access, string returnType, string name, string summary, string remarks = null, string returnTypeDescription = null) : base(access, name, summary, remarks)
		{
			if (returnType == null)
				throw new ArgumentNullException("returnType");
			if (returnType.Length == 0)
				throw new ArgumentException("returnType is an empty string");

			ReturnType = returnType;
			ReturnTypeDescription = returnTypeDescription;
			Parameters = new List<ParameterInfo>();
			Exceptions = new List<ExceptionInfo>();
			CodeLines = new List<string>();
		}

		/// <summary>
		///   Writes the method to the <see cref="StreamWriter"/> object.
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

			DocumentationHelper.WriteComponentHeader(wr, Summary, indentOffset, Remarks, ReturnTypeDescription, Parameters.ToArray(), Exceptions.ToArray());

			// Write the signature.
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0} {1} {2}(", Access, ReturnType, Name);
			int index = 0;
			foreach (ParameterInfo param in Parameters)
			{
				sb.AppendFormat("{0} {1}", param.Type, param.Name);
				if (index < Parameters.Count - 1)
					sb.Append(", ");
				index++;
			}
			sb.Append(")");

			DocumentationHelper.WriteLine(wr, sb.ToString(), indentOffset);
			DocumentationHelper.WriteLine(wr, "{", indentOffset);
			indentOffset++;

			// Generate exception code for the parameters.
			foreach (ParameterInfo param in Parameters)
			{
				if (param.CanBeNull.HasValue && !param.CanBeNull.Value)
				{
					DocumentationHelper.WriteLine(wr, string.Format("if({0} == null)", param.Name), indentOffset);
					DocumentationHelper.WriteLine(wr, string.Format("	throw new ArgumentNullException(\"{0}\");", param.Name), indentOffset);
					if (param.CanBeEmpty.HasValue && !param.CanBeEmpty.Value)
					{
						DocumentationHelper.WriteLine(wr, string.Format("if({0}.Length == 0)", param.Name), indentOffset);
						DocumentationHelper.WriteLine(wr, string.Format("	throw new ArgumentException(\"{0} is empty\");", param.Name), indentOffset);
					}
				}
				else
				{
					if (param.CanBeEmpty.HasValue && !param.CanBeEmpty.Value)
					{
						DocumentationHelper.WriteLine(wr, string.Format("if({0} != null && {0}.Length == 0)", param.Name), indentOffset);
						DocumentationHelper.WriteLine(wr, string.Format("	throw new ArgumentException(\"{0} is empty\");", param.Name), indentOffset);
					}
				}
			}

			// Add the code.
			foreach (string line in CodeLines)
				DocumentationHelper.WriteLine(wr, line, indentOffset);

			indentOffset--;
			DocumentationHelper.WriteLine(wr, "}", indentOffset);
		}

		#endregion Methods
	}
}
