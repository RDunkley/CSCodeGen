//********************************************************************************************************************************
// Filename:    ClassInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components for an exception that could be thrown from a C# component (ex: method or property).
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
	///   Contains all the information about an exception that could be thrown from a method/property.
	/// </summary>
	public class ExceptionInfo : IComparable<ExceptionInfo>
	{
		#region Properties

		/// <summary>
		///   A description of the exception.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		///   The type of exception.
		/// </summary>
		public string Type { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ExceptionInfo"/> object.
		/// </summary>
		/// <param name="type">Type of exception.</param>
		/// <param name="description">Description of the exception (why the exception could be thrown).</param>
		/// <exception cref="ArgumentNullException"><i>type</i>, or <i>description</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>type</i> or <i>description</i> is an empty string.</exception>
		public ExceptionInfo(string type, string description)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.Length == 0)
				throw new ArgumentException("type is an empty string");
			if (description == null)
				throw new ArgumentNullException("description");
			if (description.Length == 0)
				throw new ArgumentException("description is an empty string");
			Type = type;
			Description = description;
		}

		/// <summary>
		///   Compares the current instance with another object of the same type and returns an integer that indicates whether
		///   the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
		/// </summary>
		/// <param name="obj">Another <see cref="ExceptionInfo"/> object to compare with this instance.</param>
		/// <returns>
		///   A value that indicates the relative order of the objects being compared. The return value has these meanings:
		///   Value Meaning Less than zero This instance is less than <paramref name="obj" />. Zero This instance is equal
		///   to <paramref name="obj" />. Greater than zero This instance is greater than <paramref name="obj" />.
		/// </returns>
		public int CompareTo(ExceptionInfo obj)
		{
			return string.Compare(this.Type, obj.Type);
		}

		#endregion Methods
	}
}
