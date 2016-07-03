//********************************************************************************************************************************
// Filename:    DesignerInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified C# UserControl designer class for windows forms.
//********************************************************************************************************************************
// Copyright © Richard Dunkley 2016
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//********************************************************************************************************************************
using System.Collections.Generic;

namespace CSCodeGen
{
	/// <summary>
	///   Represents a <see cref="UserControl"/> designer class.
	/// </summary>
	public class DesignerInfo
	{
		#region Properties

		/// <summary>
		///   Contains the GUI components that are included in the control.
		/// </summary>
		public List<GuiComponentInfo> Components { get; private set; }

		/// <summary>
		///   Contains the main initialization code for the control.
		/// </summary>
		public List<string> MainInitializationCode { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="DesignerInfo"/> object.
		/// </summary>
		public DesignerInfo()
		{
			Components = new List<GuiComponentInfo>();
			MainInitializationCode = new List<string>();
		}

		/// <summary>
		///   Gets an ordered list of the suspend/resume components.
		/// </summary>
		/// <returns>Dictionary containing the components, indexed by their order.</returns>
		public Dictionary<int, List<string>> GetOrderedSuspendResumeLookup()
		{
			Dictionary<int, List<string>> orderedComponents = new Dictionary<int, List<string>>();
			foreach (GuiComponentInfo component in Components)
			{
				if (component.CanSuspendResume)
				{
					if (!orderedComponents.ContainsKey(component.SuspendResumeOrder))
						orderedComponents.Add(component.SuspendResumeOrder, new List<string>());
					orderedComponents[component.SuspendResumeOrder].Add(component.Name);
				}
			}

			return orderedComponents;
		}

		#endregion Methods
	}
}
