//********************************************************************************************************************************
// Filename:    NamespaceTypeInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the base data components of a simplified C# namespace type (class, enum, etc).
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2016
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//********************************************************************************************************************************

namespace CSCodeGen
{
	/// <summary>
	///   Base class for types that can be declared in a namespace (Ex: class, struct, enum).
	/// </summary>
	public abstract class NamespaceTypeInfo : BaseTypeInfo
	{
		#region Properties

		/// <summary>
		///   String containing the base class or interface support. (Ex: BaseTypeInfo or IDisposable, IComparable).
		/// </summary>
		public string Base { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="NamespaceTypeInfo"/> object.
		/// </summary>
		/// <param name="access">The access description of the type.</param>
		/// <param name="name">Name of the type.</param>
		/// <param name="summary">Summary description of the type.</param>
		/// <param name="baseString">String containing the inheritance or interface support of the type. Can be null.</param>
		/// <param name="remarks">Additional remarks to add to the documentation. Can be null.</param>
		/// <exception cref="ArgumentNullException"><i>access</i>, <i>name</i>, or <i>summary</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>access</i>, <i>name</i>, or <i>summary</i> is an empty string.</exception>
		public NamespaceTypeInfo(string access, string name, string summary, string baseString = null, string remarks = null) : base(access, name, summary, remarks)
		{
			Base = baseString;
		}

		#endregion Methods
	}
}
