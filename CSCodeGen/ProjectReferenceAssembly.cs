//********************************************************************************************************************************
// Filename:    ClassInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to represent a symplified external reference in a C# project.
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

namespace CSCodeGen
{
	/// <summary>
	///   Represents the information about an assembly that is referenced from the project.
	/// </summary>
	public class ProjectReferenceAssembly
	{
		#region Properties

		/// <summary>
		///   Reference name of the assembly.
		/// </summary>
		public string Reference { get; set; }

		/// <summary>
		///   Hint path of where the assembly is located.
		/// </summary>
		public string HintPath { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ProjectReferenceAssembly"/> object.
		/// </summary>
		/// <param name="reference">Reference name of the assembly.</param>
		/// <param name="hintPath">Hint to where the assembly is located. Can be null.</param>
		/// <exception cref="ArgumentNullException"><i>reference</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>reference</i> is an empty string.</exception>
		public ProjectReferenceAssembly(string reference, string hintPath = null)
		{
			if (reference == null)
				throw new ArgumentNullException("reference");
			if (reference.Length == 0)
				throw new ArgumentException("reference is an empty string");

			Reference = reference;
			HintPath = hintPath;
		}

		#endregion Methods
	}
}
