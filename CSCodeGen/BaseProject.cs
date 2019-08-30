//********************************************************************************************************************************
// Filename:    BaseProject.cs
// Owner:       Richard Dunkley
// Description: Base class for all Visual Studio projects.
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
using System.IO;

namespace CSCodeGen
{
	/// <summary>
	///   Base class containing the core functionality of every project in a solution.
	/// </summary>
	public abstract class BaseProject
	{
		#region Properties

		/// <summary>
		///   GUID of the project.
		/// </summary>
		public Guid Guid { get; private set; }

		/// <summary>
		///   Name of the project. Also name of output assembly.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Name of the project file.
		/// </summary>
		public string ProjectFileName { get; private set; }

		/// <summary>
		///   Assemblies that are referenced by the project.
		/// </summary>
		public List<ProjectReferenceAssembly> References { get; set; }

		/// <summary>
		///   RelativePath where the project is represented.
		/// </summary>
		public string RelativePath { get; private set; }

		/// <summary>
		///   Root namespace of the project.
		/// </summary>
		public string RootNamespace { get; set; }

		/// <summary>
		///   Type of project that this object represents.
		/// </summary>
		public ProjectType Type { get; private set; }

		/// <summary>
		///   Visual Studio Version to create the project file as.
		/// </summary>
		public VisualStudioVersion Version { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="BaseProject"/> object.
		/// </summary>
		/// <param name="name">Name of the project.</param>
		/// <param name="type"><see cref="ProjectType"/> of the project.</param>
		/// <param name="relativePath">Relative path that the project is represented in. Can be null.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
		/// <exception cref="ArgumentException"><paramref name="relativePath"/> is not a valid path.</exception>
		public BaseProject(string name, ProjectType type, string relativePath = null)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");

			if (relativePath == null)
				relativePath = string.Empty;

			if (relativePath.Length > 0)
				if (Path.IsPathRooted(relativePath))
					throw new ArgumentException("relativePath is rooted. It must be a relative path.");

			Guid = Guid.NewGuid();
			Name = name;
			ProjectFileName = string.Format("{0}.{1}", name, GetProjectFileExtension());
			Type = type;
			RootNamespace = "CodeGen";
			RelativePath = relativePath;
			References = new List<ProjectReferenceAssembly>();
			Version = VisualStudioVersion.VS2017;
		}

		/// <summary>
		///   Gets the file extension for this project.
		/// </summary>
		/// <returns>File extension associated with this project.</returns>
		protected abstract string GetProjectFileExtension();

		/// <summary>
		///   Returns the Visual Studio project type GUID.
		/// </summary>
		/// <param name="type"><see cref="ProjectType"/> enumerated type to determine the GUID from.</param>
		/// <returns>GUID of the project type.</returns>
		/// <exception cref="NotImplementedException">The specified project type is not supported.</exception>
		public static string GetProjectTypeGuid(ProjectType type)
		{
			switch (type)
			{
				case ProjectType.Library:
					return "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
				case ProjectType.Exe:
					return "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
				case ProjectType.SandcastleHelpFileBuilder:
					return "{7CF6DF6D-3B04-46F8-A40B-537D21BCA0B4}";
				default:
					throw new NotImplementedException("The specified ProjectType is not supported.");
			}
		}

		/// <summary>
		///   Write the project information out to a file.
		/// </summary>
		/// <param name="rootFolder">Root location of the file. (The relative path will be added to this folder to generate the file.)</param>
		/// <exception cref="ArgumentNullException"><paramref name="rootFolder"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="rootFolder"/> is not a valid folder path.</exception>
		/// <exception cref="IOException">An error occurred while writing to the file.</exception>
		public abstract void WriteToFile(string rootFolder);

		/// <summary>
		///   Writes all the project information, classes, etc. out to various files.
		/// </summary>
		/// <param name="rootFolder">Root location of the files. (The relative path will be added to this folder to generate the files.)</param>
		/// <exception cref="ArgumentNullException"><paramref name="rootFolder"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="rootFolder"/> is not a valid folder path.</exception>
		/// <exception cref="IOException">An error occurred while writing to one of the files.</exception>
		public abstract void WriteToFiles(string rootFolder);

		#endregion Methods
	}
}
