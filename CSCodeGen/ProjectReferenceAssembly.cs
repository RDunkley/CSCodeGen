//********************************************************************************************************************************
// Filename:    ClassInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to represent a simplified external reference in a C# project.
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

namespace CSCodeGen
{
	/// <summary>
	///   Represents the information about an assembly that is referenced from the project.
	/// </summary>
	public class ProjectReferenceAssembly
	{
		#region Properties

		/// <summary>
		///   Hint path of where the assembly is located.
		/// </summary>
		public string HintPath { get; set; }

		/// <summary>
		///   Reference name of the assembly.
		/// </summary>
		public string Reference { get; set; }

		/// <summary>
		///   Determines if the specific version entry should be specified or not and if it should be true or false.
		/// </summary>
		/// <remarks>True includes it and sets it to true, false includes it and sets it to false, and null does not include it.</remarks>
		public bool? SpecificVersion { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ProjectReferenceAssembly"/> object.
		/// </summary>
		/// <param name="reference">Reference name of the assembly.</param>
		/// <param name="hintPath">Hint to where the assembly is located. Can be null.</param>
		/// <param name="specificVersion">True if the specific version should be used, false otherwise. Null if it shouldn't be specified one way or the other.</param>
		/// <exception cref="ArgumentNullException"><paramref name="reference"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="reference"/> is an empty string.</exception>
		public ProjectReferenceAssembly(string reference, string hintPath = null, bool? specificVersion = null)
		{
			if (reference == null)
				throw new ArgumentNullException("reference");
			if (reference.Length == 0)
				throw new ArgumentException("reference is an empty string");

			Reference = reference;
			HintPath = hintPath;
			SpecificVersion = specificVersion;
		}

		/// <summary>
		///   Generates the XML lines for the project file that represent this project referenced assembly.
		/// </summary>
		/// <returns>Array of strings representing the lines in the project file.</returns>
		public string[] GenerateProjectXMLLines()
		{
			List<string> codeLines = new List<string>();
			if (HintPath == null && SpecificVersion == null)
			{
				codeLines.Add(string.Format("		<Reference Include=\"{0}\" />", Reference));
			}
			else
			{
				codeLines.Add(string.Format("		<Reference Include=\"{0}\" >", Reference));
				if (HintPath != null)
					codeLines.Add(string.Format("			<HintPath>{0}</HintPath>", HintPath));
				if (SpecificVersion != null)
					codeLines.Add(string.Format("			<SpecificVersion>{0}</SpecificVersion>", SpecificVersion.ToString()));
				codeLines.Add("		</Reference>");
			}
			return codeLines.ToArray();
		}

		#endregion Methods
	}
}
