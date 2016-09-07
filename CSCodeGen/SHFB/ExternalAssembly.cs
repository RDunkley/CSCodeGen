//********************************************************************************************************************************
// Filename:    ExternalAssembly.cs
// Owner:       Richard Dunkley
// Description: Contains the data structure representing external assemblies that Sandcastle Help File Builder will generate
//              documentation for.
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

namespace CSCodeGen.SHFB
{
	/// <summary>
	///   Provides summary information about a namespace.
	/// </summary>
	public class NamespaceSummary
	{
		#region Properties

		/// <summary>
		///   Gets or sets a description of the namespace. Can be null or empty.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///   Gets true if the namespace summary is documented, false otherwise.
		/// </summary>
		public bool IsDocumented { get; set; }

		/// <summary>
		///   Gets the name or identifier of the namespace summary.
		/// </summary>
		public string Name { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="NamespaceSummary"/> object.
		/// </summary>
		/// <param name="name">Name of the namespace.</param>
		/// <param name="isDocumented">True if the namespace is documented, false otherwise.</param>
		/// <param name="description">Description of the namespace. Can be null or empty.</param>
		/// <exception cref="ArgumentException"><i>name</i> is an empty string.</exception>
		/// <exception cref="ArgumentNullException"><i>name</i> is a null reference.</exception>
		public NamespaceSummary(string name, bool isDocumented, string description)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");

			Name = name;
			IsDocumented = isDocumented;
			Description = description;
		}

		#endregion Methods
	}
}
