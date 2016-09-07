//********************************************************************************************************************************
// Filename:    ProjectFile.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to represent a C# file in a C# project.
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
	///   Represents a C# file contained in a project.
	/// </summary>
	public class ProjectFile
	{
		#region Properties

		/// <summary>
		///   Specifies the classes that the file may be dependent upon.
		/// </summary>
		public string DependentUpon { get; set; }

		/// <summary>
		///   Reference to the <see cref="FileInfo"/> object containing information of the contents of the file.
		/// </summary>
		public FileInfo Source { get; private set; }

		/// <summary>
		///   Sub-type of the file. Matches the sub-type specified in the visual studio project files.
		/// </summary>
		public string SubType { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ProjectFile"/> object.
		/// </summary>
		/// <param name="source"><see cref="FileInfo"/> object representing the contents of the file.</param>
		/// <exception cref="ArgumentNullException"><i>source</i> is a null reference.</exception>
		public ProjectFile(FileInfo source) : this(source, null, null)
		{
		}

		/// <summary>
		///   Instantiates a new <see cref="ProjectFile"/> object.
		/// </summary>
		/// <param name="source"><see cref="FileInfo"/> object representing the contents of the file.</param>
		/// <param name="subType">Visual Studio project sub-type of the file.</param>
		/// <param name="dependentUpon">Other file which this file is dependent upon.</param>
		/// <exception cref="ArgumentNullException"><i>source</i> is a null reference.</exception>
		public ProjectFile(FileInfo source, string subType, string dependentUpon)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			Source = source;
			SubType = subType;
			DependentUpon = dependentUpon;
		}

		#endregion Methods
	}
}
