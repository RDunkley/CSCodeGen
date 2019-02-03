// ******************************************************************************************************************************
// Filename:    SettingsFile.AutoGen.cs
// ******************************************************************************************************************************
// Copyright © Richard Dunkley 2019
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
// ******************************************************************************************************************************
using System;

namespace CSCodeGen.Parse
{
	/// <summary>
	///   Adds additional functionality to the auto-generated <see cref="SettingsFile"/> partial class.
	/// </summary>
	public partial class SettingsFile
	{
		//***********************************************************************************************************************
		/// <overloads><summary>Instantiates a new SettingsFile object.</summary></overloads>
		///
		/// <summary>Instantiates a new SettingsFile object using the provided root object.</summary>
		///
		/// <param name="root">Root object of the XML file.</param>
		///
		/// <exception cref="ArgumentNullException"><paramref name="root"/> is a null reference.</exception>
		//***********************************************************************************************************************
		public SettingsFile(Settings root) : this(root, null, null)
		{
		}
	}
}
