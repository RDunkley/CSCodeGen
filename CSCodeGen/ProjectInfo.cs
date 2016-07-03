//********************************************************************************************************************************
// Filename:    ProjectInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified C# project.
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
using System.Linq;

namespace CSCodeGen
{
	/// <summary>
	///   Represents a Visual Studio Project file.
	/// </summary>
	public class ProjectInfo
	{
		#region Properties

		/// <summary>
		///   Type of project that this object represents.
		/// </summary>
		public ProjectType Type { get; private set; }

		/// <summary>
		///   GUID of the project.
		/// </summary>
		public Guid Guid { get; private set; }

		/// <summary>
		///   Name of the project. Also name of output assembly.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Debug path.
		/// </summary>
		public string DebugPath { get; set; }

		/// <summary>
		///   Name of the project file.
		/// </summary>
		public string ProjectFileName { get; private set; }

		/// <summary>
		///   Release path.
		/// </summary>
		public string ReleasePath { get; set; }

		/// <summary>
		///   Assemblies that are referenced by the project.
		/// </summary>
		public List<ProjectReferenceAssembly> References { get; set; }

		/// <summary>
		///   Files contained in the project.
		/// </summary>
		public List<ProjectFile> Files { get; private set; }

		/// <summary>
		///   Information about the assembly.
		/// </summary>
		public AssemblyInfo AssemblyInformation { get; set; }

		/// <summary>
		///   Root namespace of the project.
		/// </summary>
		public string RootNamespace { get; set; }

		/// <summary>
		///   RelativePath where the project is represented.
		/// </summary>
		public string RelativePath { get; private set; }

		/// <summary>
		///   Visual Studio Version to create the project file as.
		/// </summary>
		public VisualStudioVersion Version { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ProjectInfo"/> object.
		/// </summary>
		/// <param name="name">Name of the project and the created assembly.</param>
		/// <param name="debugPath">Debug output path.</param>
		/// <param name="releasePath">Release output path.</param>
		/// <param name="type"><see cref="ProjectType"/> of the project.</param>
		/// <param name="relativePath">Relative path that the project is represented in. Can be null.</param>
		/// <exception cref="ArgumentNullException"><i>name</i>, <i>debugPath</i>,  or <i>releasePath</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>name</i>, <i>debugPath</i>, or <i>releasePath</i> is an empty string.</exception>
		/// <exception cref="ArgumentException"><i>debugPath</i>, <i>releasePath</i>, or <i>relativePath</i> is not a valid path.</exception>
		public ProjectInfo(string name, string debugPath, string releasePath, ProjectType type, string relativePath = null)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string");
			if (debugPath == null)
				throw new ArgumentNullException("debugPath");
			if (debugPath.Length == 0)
				throw new ArgumentException("debugPath is an empty string");
			if (releasePath == null)
				throw new ArgumentNullException("releasePath");
			if (releasePath.Length == 0)
				throw new ArgumentException("releasePath is an empty string");

			if (relativePath == null)
				relativePath = string.Empty;

			if (relativePath.Length > 0)
				if (Path.IsPathRooted(relativePath))
					throw new ArgumentException("relativePath is rooted. It must be a relative path.");

			DebugPath = debugPath;
			ReleasePath = releasePath;
			Guid = Guid.NewGuid();
			Name = name;
			ProjectFileName = string.Format("{0}.csproj", name);
			Type = type;
			RootNamespace = "CodeGen";
			RelativePath = relativePath;
			References = new List<ProjectReferenceAssembly>();
			Version = VisualStudioVersion.VS2015;
			Files = new List<ProjectFile>();
		}

		/// <summary>
		///   Adds a file to the project.
		/// </summary>
		/// <param name="file"><see cref="FileInfo"/> object to add to the project.</param>
		/// <exception cref="ArgumentNullException"><i>file</i> is a null reference.</exception>
		public void AddFile(FileInfo file)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			Files.Add(new ProjectFile(file));
		}

		/// <summary>
		///   Adds a file to the project.
		/// </summary>
		/// <param name="nameSpace">Namespace of the file.</param>
		/// <param name="typeObject"><see cref="NamespaceTypeInfo"/> object containing the type to be represented in the file.</param>
		/// <param name="usings">Usings contained in the file.</param>
		/// <param name="relativePath">Relative path where the file is represented.</param>
		/// <param name="description">Description of the file.</param>
		/// <param name="fileNameExtension">Extension to add to the filename. (Ex: 'designer' would be for filename.designer.cs). Can be null or empty.</param>
		/// <exception cref="ArgumentNullException"><i>nameSpace</i>, <i>info</i>, <i>usings</i>, or one of the strings in <i>usings</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>nameSpace</i>, or one of the strings in <i>usings</i> is an empty string.</exception>
		/// <exception cref="ArgumentException"><i>relativePath</i> is defined, but is not a relative path.</exception>
		public void AddFile(string nameSpace, NamespaceTypeInfo typeObject, string[] usings, string relativePath = null, string description = null, string fileNameExtension = null)
		{
			FileInfo file = new FileInfo(nameSpace, typeObject, relativePath, description, fileNameExtension);
			file.Usings.AddRange(usings);
			Files.Add(new ProjectFile(file));
		}

		/// <summary>
		///   Adds a GUI item to the project.
		/// </summary>
		/// <param name="nameSpace">Namespace of the file.</param>
		/// <param name="userClass">User class of the GUI item.</param>
		/// <param name="designer"><see cref="DesignerInfo"/> object containing the designer file information.</param>
		/// <param name="usings">Usings contained in the file.</param>
		/// <param name="relativePath">Relative path where the file is represented.</param>
		/// <param name="description">Description of the file.</param>
		/// <param name="fileNameExtension">Extension to add to the user class filename. Can be null or empty.</param>
		/// <exception cref="ArgumentNullException"><i>nameSpace</i>, <i>info</i>, <i>usings</i>, or one of the strings in <i>usings</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>nameSpace</i>, or one of the strings in <i>usings</i> is an empty string.</exception>
		/// <exception cref="ArgumentException"><i>relativePath</i> is defined, but is not a relative path.</exception>
		/// <exception cref="ArgumentException"><i>fileNameExtension</i> was specified as 'designer'. (Can not specify designer for this method).</exception>
		public void AddGUIFile(string nameSpace, ClassInfo userClass, DesignerInfo designer, string[] usings, string relativePath = null, string description = null, string fileNameExtension = null)
		{
			if (string.Compare(fileNameExtension, "designer", true) == 0)
				throw new ArgumentException("fileNameExtension cannot be 'designer' since a designer file will already be added.");

			FileInfo userFile = new FileInfo(nameSpace, userClass, relativePath, description, fileNameExtension);
			userFile.Usings.AddRange(usings);

			// Create a new class to use as the designer portion.
			ClassInfo info = new ClassInfo("partial", userClass.Name, null, userClass.Summary, userClass.Remarks);

			// Add a field for each component.
			info.Fields.Add(new FieldInfo("private", "System.ComponentModel.IContainer", "components", "Required designer variable.", null, "null"));
			foreach (GuiComponentInfo component in designer.Components)
				info.Fields.Add(new FieldInfo("private", component.Type, component.Name, component.Summary));

			// Add the dispose method.
			MethodInfo method = new MethodInfo("protected override", "void", "Dispose", "Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.");
			method.Parameters.Add(new ParameterInfo("disposing", "bool", "True if managed resources should be disposed; otherwise false.", null, null));
			method.CodeLines.Add("if(disposing && (components != null))");
			method.CodeLines.Add("{");
			method.CodeLines.Add("	components.Dispose();");
			method.CodeLines.Add("}");
			method.CodeLines.Add("base.Dispose(disposing);");
			info.Methods.Add(method);

			// Add the initialization method.
			method = new MethodInfo("private", "void", "InitializeComponent", "Required method for Designer support - do not modify the contents of this method with the code editor.");
			foreach (GuiComponentInfo component in designer.Components)
				method.CodeLines.Add(string.Format("this.{0} = new {1}();", component.Name, component.Type));

			// Order the suspend and resume.
			Dictionary<int, List<string>> orderedComponents = designer.GetOrderedSuspendResumeLookup();
			List<int> sortedKeys = orderedComponents.Keys.ToList();
			sortedKeys.Sort();

			// Suspend the components.
			foreach (int order in sortedKeys)
			{
				foreach (string component in orderedComponents[order])
					method.CodeLines.Add(string.Format("this.{0}.SuspendLayout();", component));
			}
			method.CodeLines.Add("this.SuspendLayout();");

			// Add each components initialization.
			foreach (GuiComponentInfo component in designer.Components)
			{
				method.CodeLines.Add("// ");
				method.CodeLines.Add(string.Format("// {0}", component.Name));
				method.CodeLines.Add("// ");
				method.CodeLines.AddRange(component.InitializationCode);
			}

			// Add initialization code.
			method.CodeLines.Add("// ");
			method.CodeLines.Add(string.Format("// {0}", info.Name));
			method.CodeLines.Add("// ");
			if (designer.MainInitializationCode != null && designer.MainInitializationCode.Count > 0)
				method.CodeLines.AddRange(designer.MainInitializationCode);

			// Resume the components.
			foreach (int order in sortedKeys)
			{
				foreach (string component in orderedComponents[order])
				{
					method.CodeLines.Add(string.Format("this.{0}.ResumeLayout(false);", component));
					method.CodeLines.Add(string.Format("this.{0}.PerformLayout();", component));
				}
			}
			method.CodeLines.Add("this.ResumeLayout(false);");
			method.CodeLines.Add("this.PerformLayout();");
			info.Methods.Add(method);

			FileInfo designerFile = new FileInfo(nameSpace, info, relativePath, description, "designer");

			// Add the files with the designer dependency.
			Files.Add(new ProjectFile(userFile, "UserControl", null));
			Files.Add(new ProjectFile(designerFile, null, userFile.FileName));
		}

		/// <summary>
		///   Write the project information out to a file.
		/// </summary>
		/// <param name="rootFolder">Root location of the file. (The relative path will be added to this folder to generate the file.)</param>
		/// <exception cref="ArgumentNullException"><i>rootFolder</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>rootFolder</i> is not a valid folder path.</exception>
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

			if (Files.Count == 0)
				throw new InvalidOperationException("An attempt was made to write the project information to a file, but no files have been added to the Files list. A project must have at least one code file.");

			string fullFolderPath;
			if (RelativePath.Length > 0)
				fullFolderPath = Path.Combine(rootFolder, RelativePath);
			else
				fullFolderPath = rootFolder;

			// Convert absolute debug and release paths to relative.
			string debugPath = DebugPath;
			if (Path.IsPathRooted(debugPath))
				debugPath = ConvertAbsolutePathToRelative(debugPath, fullFolderPath);
			string releasePath = ReleasePath;
			if (Path.IsPathRooted(releasePath))
				releasePath = ConvertAbsolutePathToRelative(releasePath, fullFolderPath);

			string path = Path.Combine(fullFolderPath, ProjectFileName);
			using (StreamWriter sw = new StreamWriter(path))
			{
				sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
				if (Version == VisualStudioVersion.VS2010 || Version == VisualStudioVersion.VS2013)
					sw.WriteLine("<Project ToolsVersion=\"4.0\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
				else if (Version == VisualStudioVersion.VS2015)
					sw.WriteLine("<Project ToolsVersion=\"14.0\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
				else
					throw new NotImplementedException("The Visual Studio version provided was not recognized as a supported version.");
				sw.WriteLine("	<PropertyGroup>");
				sw.WriteLine("		<Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>");
				sw.WriteLine("		<Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>");
				sw.WriteLine(string.Format("		<ProjectGuid>{0}</ProjectGuid>", Guid.ToString("B")));
				sw.WriteLine(string.Format("		<OutputType>{0}</OutputType>", GetProjectType(Type)));
				sw.WriteLine("		<AppDesignerFolder>Properties</AppDesignerFolder>");
				sw.WriteLine(string.Format("		<RootNamespace>{0}</RootNamespace>", RootNamespace));
				sw.WriteLine(string.Format("		<AssemblyName>{0}</AssemblyName>", Name));
				if (Version == VisualStudioVersion.VS2010 || Version == VisualStudioVersion.VS2013)
					sw.WriteLine("		<TargetFrameworkVersion>v4.0</TargetFrameworkVersion>");
				else if(Version == VisualStudioVersion.VS2015)
					sw.WriteLine("		<TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>");
				else
					throw new NotImplementedException("The Visual Studio version provided was not recognized as a supported version.");
				sw.WriteLine("		<FileAlignment>512</FileAlignment>");
				sw.WriteLine("	</PropertyGroup>");
				sw.WriteLine("	<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
				sw.WriteLine("		<DebugSymbols>true</DebugSymbols>");
				sw.WriteLine("		<DebugType>full</DebugType>");
				sw.WriteLine("		<Optimize>false</Optimize>");
				sw.WriteLine(string.Format("		<OutputPath>{0}</OutputPath>", debugPath));
				sw.WriteLine("		<DefineConstants>DEBUG;TRACE</DefineConstants>");
				sw.WriteLine("		<ErrorReport>prompt</ErrorReport>");
				sw.WriteLine("		<WarningLevel>4</WarningLevel>");
				sw.WriteLine("	</PropertyGroup>");
				sw.WriteLine("	<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
				sw.WriteLine("		<DebugType>pdbonly</DebugType>");
				sw.WriteLine("		<Optimize>true</Optimize>");
				sw.WriteLine(string.Format("		<OutputPath>{0}</OutputPath>", releasePath));
				sw.WriteLine("		<DefineConstants>TRACE</DefineConstants>");
				sw.WriteLine("		<ErrorReport>prompt</ErrorReport>");
				sw.WriteLine("		<WarningLevel>4</WarningLevel>");
				sw.WriteLine("	</PropertyGroup>");
				sw.WriteLine("	<ItemGroup>");
				foreach (ProjectReferenceAssembly nameSpace in References)
				{
					if (nameSpace.HintPath == null)
					{
						sw.WriteLine(string.Format("		<Reference Include=\"{0}\" />", nameSpace.Reference));
					}
					else
					{
						sw.WriteLine(string.Format("		<Reference Include=\"{0}\" >", nameSpace.Reference));
						sw.WriteLine(string.Format("			<HintPath>{0}</HintPath>", nameSpace.HintPath));
						sw.WriteLine("		</Reference>");
					}
				}
				sw.WriteLine("	</ItemGroup>");
				sw.WriteLine("	<ItemGroup>");
				foreach (ProjectFile projectFile in Files)
				{
					if (projectFile.SubType == null && projectFile.DependentUpon == null)
					{
						sw.WriteLine(string.Format("		<Compile Include=\"{0}\" />", Path.Combine(projectFile.Source.RelativePath, projectFile.Source.FileName)));
					}
					else
					{
						sw.WriteLine(string.Format("		<Compile Include=\"{0}\" >", Path.Combine(projectFile.Source.RelativePath, projectFile.Source.FileName)));
						if (projectFile.SubType != null)
							sw.WriteLine(string.Format("			<SubType>{0}</SubType>", projectFile.SubType));
						if (projectFile.DependentUpon != null)
							sw.WriteLine(string.Format("			<DependentUpon>{0}</DependentUpon>", projectFile.DependentUpon));
						sw.WriteLine("		</Compile>");
					}
				}
				if(AssemblyInformation != null)
					sw.WriteLine("		<Compile Include=\"Properties\\AssemblyInfo.cs\" />");
				sw.WriteLine("	</ItemGroup>");
				sw.WriteLine("	<Import Project=\"$(MSBuildToolsPath)\\Microsoft.CSharp.targets\" />");
				sw.WriteLine("	<!-- To modify your build process, add your task inside one of the targets below and uncomment it. ");
				sw.WriteLine("	Other similar extension points exist, see Microsoft.Common.targets.");
				sw.WriteLine("	<Target Name=\"BeforeBuild\">");
				sw.WriteLine("	</Target>");
				sw.WriteLine("	<Target Name=\"AfterBuild\">");
				sw.WriteLine("	</Target>");
				sw.WriteLine("	-->");
				sw.Write("</Project>");
			}
		}

		/// <summary>
		///   Writes all the project information, classes, etc. out to various files.
		/// </summary>
		/// <param name="rootFolder">Root location of the files. (The relative path will be added to this folder to generate the files.)</param>
		/// <exception cref="ArgumentNullException"><i>rootFolder</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>rootFolder</i> is not a valid folder path.</exception>
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

			if (Files.Count == 0)
				throw new InvalidOperationException("An attempt was made to write the project information to a file, but no files have been added to the Files list. A project must have at least one code file.");

			string fullFolderPath;
			if (RelativePath.Length > 0)
				fullFolderPath = Path.Combine(rootFolder, RelativePath);
			else
				fullFolderPath = rootFolder;

			// Generate the code files.
			foreach (ProjectFile file in Files)
				file.Source.WriteToFile(fullFolderPath);

			// Generate the assemby info file if it exists.
			if (AssemblyInformation != null)
				AssemblyInformation.WriteToFile(fullFolderPath);

			// Write the project file.
			WriteToFile(rootFolder);
		}

		/// <summary>
		///   Converts an absolute path to a relative one.
		/// </summary>
		/// <param name="absolutePath">Absolute path to be converted.</param>
		/// <param name="reference">The reference folder to generate the relative path from.</param>
		/// <returns>Relative path from the reference.</returns>
		private string ConvertAbsolutePathToRelative(string absolutePath, string reference)
		{
			if (reference[reference.Length - 1] != '\\')
				reference = string.Format("{0}\\", reference);
			Uri pathUri = new Uri(absolutePath);
			Uri referenceUri = new Uri(reference);
			return referenceUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar);
		}

		/// <summary>
		///   Gets a string of the project type.
		/// </summary>
		/// <param name="type"><see cref="ProjectType"/> of the project.</param>
		/// <returns>String which indicates in the project file what project type it is.</returns>
		/// <exception cref="NotImplementedException">The specified project type is not supported.</exception>
		private string GetProjectType(ProjectType type)
		{
			switch (type)
			{
				case ProjectType.Library:
					return "Library";
				case ProjectType.Exe:
					return "WinExe";
				default:
					throw new NotImplementedException("The specified ProjectType is not supported.");
			}
		}

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
				default:
					throw new NotImplementedException("The specified ProjectType is not supported.");
			}
		}

		#endregion Methods
	}
}
