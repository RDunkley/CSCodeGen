//********************************************************************************************************************************
// Filename:    ParameterInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to describe a simplified parameter in a C# method.
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
	///   Represents the information of a C# Method parameter.
	/// </summary>
	public class ParameterInfo
	{
		#region Properties

		/// <summary>
		///   Determines whether the parameter can be empty. If true, additional comments will be added in the 
		///   documentation that this parameter can be empty. If false, then the parameter will be checked for Length = 0 and an
		///    exception will be thrown if so. If null, no additional comments, exceptions, or error checking will be added.
		/// </summary>
		public bool? CanBeEmpty { get; set; }

		/// <summary>
		///   Determines whether the parameter can be null or not. If true, additional comments will be added in the 
		///   documentation that this parameter can be null. If false, then the parameter will be checked for null and an
		///    exception will be thrown. If null, no additional comments, exceptions, or error checking will be added.
		/// </summary>
		public bool? CanBeNull { get; set; }

		/// <summary>
		///   The default value specified in the method signature.
		/// </summary>
		public string Default { get; set; }

		/// <summary>
		///   Description of the parameter.
		/// </summary>
		public string Description { get; protected set; }

		/// <summary>
		///   Name of the parameter.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///   Type of the parameter (Ex: int, string, etc.).
		/// </summary>
		public string Type { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ParameterInfo"/> object.
		/// </summary>
		/// <param name="type">Type of the parameter.</param>
		/// <param name="name">Name of the parameter.</param>
		/// <param name="description">Description of the parameter.</param>
		/// <param name="canBeNull">Determines whether the parameter can be null or not.</param>
		/// <param name="canBeEmpty">Determines whether the parameter can be empty or not.</param>
		/// <param name="defaultValue">Default value of the parameter.</param>
		/// <exception cref="ArgumentNullException"><paramref name="type"/>, <paramref name="name"/>, or <paramref name="description"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/>, <paramref name="name"/>, or <paramref name="description"/> is an empty string.</exception>
		public ParameterInfo(string type, string name, string description, bool? canBeNull = null, bool? canBeEmpty = null, string defaultValue = null)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.Length == 0)
				throw new ArgumentException("type is an empty string");
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");
			if (description == null)
				throw new ArgumentNullException("description");
			if (description.Length == 0)
				throw new ArgumentException("description is an empty string");

			Name = name;
			Type = type;
			Description = description;
			Default = defaultValue;
			CanBeNull = canBeNull;
			CanBeEmpty = canBeEmpty;
		}

		#endregion Methods
	}
}
