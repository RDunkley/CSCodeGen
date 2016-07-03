//********************************************************************************************************************************
// Filename:    GuiComponentInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified C# windows forms GUI component.
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
	///   Represents a windows forms GUI component.
	/// </summary>
	public class GuiComponentInfo
	{
		#region Properties

		/// <summary>
		///   Specifies the documentation summary that will be generated with the type.
		/// </summary>
		public string Summary { get; private set; }

		/// <summary>
		///   Name of the type.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Type of the component (Ex: TableLayoutPanel, Label, etc.).
		/// </summary>
		public string Type { get; private set; }

		/// <summary>
		///   Determines whether the component can be suspended and resumed. True if it can, false otherwise.
		/// </summary>
		public bool CanSuspendResume { get; set; }

		/// <summary>
		///   If <see cref="CanSuspendResume"/> is true, then this property determines it's suspend/resume order. Does nothing if <see cref="CanSuspendResume"/> is false.
		/// </summary>
		public int SuspendResumeOrder { get; set; }

		/// <summary>
		///   Code to initialize the component.
		/// </summary>
		public List<string> InitializationCode { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="GuiComponentInfo"/> object.
		/// </summary>
		/// <param name="type">Type of the component.</param>
		/// <param name="name">Name of the component.</param>
		/// <param name="summary">Description of the component for documentation.</param>
		/// <exception cref="ArgumentNullException"><i>type</i>, <i>name</i>, or <i>summary</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>type</i>, <i>name</i>, or <i>summary</i> is an empty string.</exception>
		public GuiComponentInfo(string type, string name, string summary)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.Length == 0)
				throw new ArgumentException("type is an empty string");
			if (summary == null)
				throw new ArgumentNullException("summary");
			if (summary.Length == 0)
				throw new ArgumentException("summary is an empty string");

			Name = name;
			Type = type;
			Summary = summary;
			InitializationCode = new List<string>();
		}

		#endregion Methods
	}
}
