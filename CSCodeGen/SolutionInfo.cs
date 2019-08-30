//********************************************************************************************************************************
// Filename:    SolutionInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified C# visual studio solution.
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
	///   Represents a Visual Studio Solution file.
	/// </summary>
	public class SolutionInfo
	{
		#region Properties

		/// <summary>
		///   Name of the solution.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///   Lookup table of <see cref="ProjectInfo"/> objects and their associated dependencies. Only project information objects with dependencies are contained in the table.
		/// </summary>
		public Dictionary<BaseProject, List<BaseProject>> ProjectDependencies { get; private set; }

		/// <summary>
		///   Array of <see cref="ProjectInfo"/> objects representing the projects in the solution.
		/// </summary>
		public List<BaseProject> Projects { get; private set; }

		/// <summary>
		///   Visual Studio Version to create the solution file as.
		/// </summary>
		public VisualStudioVersion Version { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="SolutionInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the solution.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> is an empty sting.</exception>
		public SolutionInfo(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");

			Name = name;
			ProjectDependencies = new Dictionary<BaseProject, List<BaseProject>>();
			Version = VisualStudioVersion.VS2017;
			Projects = new List<BaseProject>();
		}

		/// <summary>
		///   Write the solution information out to a file.
		/// </summary>
		/// <param name="rootFolder">Root location of the file.</param>
		/// <exception cref="ArgumentNullException"><paramref name="rootFolder"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="rootFolder"/> is not a valid folder path.</exception>
		/// <exception cref="IOException">An error occurred while writing to the file.</exception>
		public void WriteToFile(string rootFolder)
		{
			if (rootFolder == null)
				throw new ArgumentNullException("rootFolder");
			if (rootFolder.Length == 0)
				throw new ArgumentException("rootFolder is an empty string");
			try
			{
				rootFolder = Path.GetFullPath(rootFolder);
			}
			catch (Exception e)
			{
				throw new ArgumentException("rootFolder is not a valid path (see inner exception).", e);
			}

			if (Projects.Count == 0)
				throw new InvalidOperationException("An attempt was made to write the solution information to a file, but no projects were added to the Projects list. A solution must have at least one project file.");

			string fileName = string.Format("{0}.sln", Name);
			string path = Path.Combine(rootFolder, fileName);
			using (StreamWriter wr = new StreamWriter(path))
			{
				switch(Version)
				{
					case VisualStudioVersion.VS2010:
						wr.WriteLine("Microsoft Visual Studio Solution File, Format Version 11.00");
						wr.WriteLine("# Visual Studio 2010");
						break;
					case VisualStudioVersion.VS2013:
						wr.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
						wr.WriteLine("# Visual Studio 2013");
						wr.WriteLine("VisualStudioVersion = 12.0.31101.0");
						wr.WriteLine("MinimumVisualStudioVersion = 10.0.40219.1");
						break;
					case VisualStudioVersion.VS2015:
						wr.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
						wr.WriteLine("# Visual Studio 14");
						wr.WriteLine("VisualStudioVersion = 14.0.23107.0");
						wr.WriteLine("MinimumVisualStudioVersion = 10.0.40219.1");
						break;
                    case VisualStudioVersion.VS2017:
                        wr.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
                        wr.WriteLine("# Visual Studio 15");
                        wr.WriteLine("VisualStudioVersion = 15.0.28307.421");
                        wr.WriteLine("MinimumVisualStudioVersion = 10.0.40219.1");
                        break;
                    default:
						throw new NotImplementedException("The Visual Studio Version was not recognized as a supported version");
				}

				foreach(BaseProject proj in Projects)
				{
					wr.WriteLine(string.Format("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"", BaseProject.GetProjectTypeGuid(proj.Type), proj.Name, Path.Combine(proj.RelativePath, proj.ProjectFileName), proj.Guid.ToString("B").ToUpper()));
					if (ProjectDependencies.ContainsKey(proj))
					{
						wr.WriteLine("	ProjectSection(ProjectDependencies) = postProject");
						foreach (ProjectInfo depProj in ProjectDependencies[proj])
							wr.WriteLine("		{0} = {0}", depProj.Guid.ToString("B").ToUpper());
						wr.WriteLine("	EndProjectSection");
					}
					wr.WriteLine("EndProject");
				}
				wr.WriteLine("Global");
				wr.WriteLine("	GlobalSection(SolutionConfigurationPlatforms) = preSolution");
				wr.WriteLine("		Debug|Any CPU = Debug|Any CPU");
				wr.WriteLine("		Release|Any CPU = Release|Any CPU");
				wr.WriteLine("	EndGlobalSection");
				wr.WriteLine("	GlobalSection(ProjectConfigurationPlatforms) = postSolution");
				foreach (BaseProject proj in Projects)
				{
					wr.WriteLine(string.Format("		{0}.Debug|Any CPU.ActiveCfg = Debug|Any CPU", proj.Guid.ToString("B").ToUpper()));
					wr.WriteLine(string.Format("		{0}.Debug|Any CPU.Build.0 = Debug|Any CPU", proj.Guid.ToString("B").ToUpper()));
					wr.WriteLine(string.Format("		{0}.Release|Any CPU.ActiveCfg = Release|Any CPU", proj.Guid.ToString("B").ToUpper()));
					wr.WriteLine(string.Format("		{0}.Release|Any CPU.Build.0 = Release|Any CPU", proj.Guid.ToString("B").ToUpper()));
				}
				wr.WriteLine("	EndGlobalSection");
				wr.WriteLine("	GlobalSection(SolutionProperties) = preSolution");
				wr.WriteLine("		HideSolutionNode = FALSE");
				wr.WriteLine("	EndGlobalSection");
				wr.WriteLine("EndGlobal");
			}
		}

		/// <summary>
		///   Writes all the solution information, project, classes, etc. out to various files.
		/// </summary>
		/// <param name="rootFolder">Root location of the files. (The relative path will be added to this folder to generate the files.)</param>
		/// <exception cref="ArgumentNullException"><paramref name="rootFolder"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="rootFolder"/> is not a valid folder path.</exception>
		/// <exception cref="InvalidOperationException">The solution does not contain any projects.</exception>
		/// <exception cref="IOException">An error occurred while writing to one of the files.</exception>
		public void WriteToFiles(string rootFolder)
		{
			if (rootFolder == null)
				throw new ArgumentNullException("rootFolder");
			if (rootFolder.Length == 0)
				throw new ArgumentException("rootFolder is an empty string");
			try
			{
				rootFolder = Path.GetFullPath(rootFolder);
			}
			catch (Exception e)
			{
				throw new ArgumentException("rootFolder is not a valid path (see inner exception).", e);
			}

			if (Projects.Count == 0)
				throw new InvalidOperationException("An attempt was made to write the solution information to a file, but no projects were added to the Projects list. A solution must have at least one project file.");

			// Generate the code files for all the projects.
			foreach(BaseProject proj in Projects)
				proj.WriteToFiles(rootFolder);

			// Write the solution file.
			WriteToFile(rootFolder);
		}

		#endregion Methods
	}
}
