//********************************************************************************************************************************
// Filename:    VisualStudioVersion.cs
// Owner:       Richard Dunkley
// Description: Contains an enumeration of a subset of the Visual Studio versions.
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
	///   Enumerates the various visual studio versions supported by this assembly.
	/// </summary>
	public enum VisualStudioVersion
	{
		#region Names

		/// <summary>
		///   Visual Studio 2010 (11.0).
		/// </summary>
		VS2010,

		/// <summary>
		///   Visual Studio 2013 (12.0).
		/// </summary>
		VS2013,

		/// <summary>
		///   Visual Studio 2015 (14.0).
		/// </summary>
		VS2015,

		#endregion Names
	}
}
