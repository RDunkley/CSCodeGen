//********************************************************************************************************************************
// Filename:    Content.cs
// Owner:       Richard Dunkley
// Description: Contains the data structure representing content in a Sandcastle Help File Builder project file.
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
using System.Linq;
using System.Text;

namespace CSCodeGen.SHFB
{
	/// <summary>
	///   In memory representation of content contained in a Sandcastle Help File Builder project file.
	/// </summary>
	public class Content
	{
		#region Enumerations

		public enum ContentType
		{
			None,
			Folder,
			Image,
			Content,
		}

		#endregion Enumerations

		#region Properties

		public string Include { get; private set; }
		public ContentType Type { get; private set; }
		public string ImageID { get; set; }
		public string AlternateText { get; set; }
		public string Link { get; set; }

		#endregion Properties

		#region Methods

		public Content(string include, ContentType type, string imageID = null, string alternateText = null, string link = null)
		{
			Include = include;
			Type = type;
			ImageID = imageID;
			AlternateText = alternateText;
			Link = link;
		}

		#endregion Methods
	}
}
