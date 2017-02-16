//********************************************************************************************************************************
// Filename:    BaseTypeInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components for a base type of C# components.
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
	///   Abstract base class of all the C# types.
	/// </summary>
	public abstract class BaseTypeInfo : IComparable<BaseTypeInfo>
	{
		#region Properties

		/// <summary>
		///   Specifies the accessibility of the type. (Ex: public, public abstract, etc.).
		/// </summary>
		public string Access { get; protected set; }

		/// <summary>
		///   Name of the type.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///   Summary provided in an overloads section.
		/// </summary>
		/// <remarks>A summary should be provided if this is the first method in a group of overloads. Can be null.</remarks>
		public string OverloadedSummary { get; set; }

		/// <summary>
		///   Specifies the documentation remarks that will be generated with the type. Can be null or empty.
		/// </summary>
		public string Remarks { get; set; }

		/// <summary>
		///   Specifies the documentation summary that will be generated with the type.
		/// </summary>
		public string Summary { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="BaseTypeInfo"/> object.
		/// </summary>
		/// <param name="access">The access description of the type.</param>
		/// <param name="name">Name of the type.</param>
		/// <param name="summary">Summary description of the type.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="access"/>, <paramref name="name"/>, or <paramref name="summary"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="access"/>, <paramref name="name"/>, or <paramref name="summary"/> is an empty string.</exception>
		public BaseTypeInfo(string access, string name, string summary, string remarks = null)
		{
			if (access == null)
				throw new ArgumentNullException("access");
			if (access.Length == 0)
				throw new ArgumentException("access is an empty string");
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");
			if (summary == null)
				throw new ArgumentNullException("summary");
			if (summary.Length == 0)
				throw new ArgumentException("summary is an empty string");

			Access = access;
			Name = name;
			Summary = summary;
			Remarks = remarks;
		}

		/// <summary>
		///   Compares a <see cref="BaseTypeInfo"/> object with this object and returns an integer that indicates their relative position in the sort order.
		/// </summary>
		/// <param name="other">Other <see cref="BaseTypeInfo"/> object to compare this object to.</param>
		/// <returns>
		///   A 32-bit signed integer that indicates the lexical relationship between the two comparands. Less than zero, this object precedes 
		///   <paramref name="other"/>. Zero, they have the same sort order. Greater than zero, this object is after <paramref name="other"/> in the sort order.
		/// </returns>
		public int CompareTo(BaseTypeInfo other)
		{
			return string.Compare(this.Name, other.Name);
		}

		#endregion Methods
	}
}
