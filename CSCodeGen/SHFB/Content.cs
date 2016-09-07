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

namespace CSCodeGen.SHFB
{
	/// <summary>
	///   In memory representation of content contained in a Sandcastle Help File Builder project file.
	/// </summary>
	public class Content
	{
		#region Enumerations

		/// <summary>
		///   Represents different types of content that can be added to a SHFB project.
		/// </summary>
		public enum ContentType
		{
			#region Names

			/// <summary>
			///   General content included in the project.
			/// </summary>
			Content,

			/// <summary>
			///   A folder designated within the project structure.
			/// </summary>
			Folder,

			/// <summary>
			///   Image file included in the project.
			/// </summary>
			Image,

			/// <summary>
			///   
			/// </summary>
			None,

			#endregion Names
		}

		#endregion Enumerations

		#region Properties

		/// <summary>
		///   Gets or sets the alternate text associated with the content. Can be null or empty.
		/// </summary>
		public string AlternateText { get; set; }

		/// <summary>
		///   Gets or sets the image ID of the content. Can be null or empty.
		/// </summary>
		public string ImageID { get; set; }

		/// <summary>
		///   Gets the content's location.
		/// </summary>
		public string Include { get; private set; }

		/// <summary>
		///   Gets or sets a link associated with the content. Can be null or empty.
		/// </summary>
		public string Link { get; set; }

		/// <summary>
		///   Gets the type of content.
		/// </summary>
		public ContentType Type { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="Content"/> object.
		/// </summary>
		/// <param name="include">Content's location.</param>
		/// <param name="type">Type of content.</param>
		/// <param name="imageID">Image ID of the content.</param>
		/// <param name="alternateText">Alternate text associated with the content.</param>
		/// <param name="link">Link associated with the content.</param>
		/// <exception cref="ArgumentException"><i>include</i> is an empty string.</exception>
		/// <exception cref="ArgumentNullException"><i>include</i> is a null reference.</exception>
		public Content(string include, ContentType type, string imageID = null, string alternateText = null, string link = null)
		{
			if (include == null)
				throw new ArgumentNullException("include");
			if (include.Length == 0)
				throw new ArgumentException("include is an empty string");

			Include = include;
			Type = type;
			ImageID = imageID;
			AlternateText = alternateText;
			Link = link;
		}

		#endregion Methods
	}
}
