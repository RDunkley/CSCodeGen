//********************************************************************************************************************************
// Filename:    SHFBProject.cs
// Owner:       Richard Dunkley
// Description: Represents a Sandcastle Help File Builder project.
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

namespace CSCodeGen.SHFB
{
	/// <summary>
	///   Represents a Sandcastle Help File Builder Project.
	/// </summary>
	public class SHFBProject : BaseProject
	{
		#region Properties

		/// <summary>
		///   Specifies the location of the content layout file relative to the project directory.
		/// </summary>
		public string ContentLayout { get; private set; }

		/// <summary>
		///   Gets or sets the additional message in the footer of each page. Can be null or empty.
		/// </summary>
		public string FooterText { get; set; }

		/// <summary>
		///   Gets or sets the additional message in the header of each page. Can be null or empty.
		/// </summary>
		public string HeaderText { get; set; }

		/// <summary>
		///   Gets or sets the title for the help file. This text will appear as the title of the help file's window.
		/// </summary>
		public string HelpTitle { get; set; }

		/// <summary>
		///   Contains an array of content groups. This allows custom item groups to be added to the project.
		/// </summary>
		public List<List<Content>> ItemGroups { get; private set; }

		/// <summary>
		///   Array of namespace summaries contained in the project.
		/// </summary>
		public List<NamespaceSummary> NamespaceSummaries { get; private set; }

		/// <summary>
		///   Specifies where the compiled help file should be placed.
		/// </summary>
		public string OutputPath { get; set; }

		/// <summary>
		///   This property is used to set the product title that appears in the help content setup file. If not set, the <see cref="HelpTitle"/> value will be used.
		/// </summary>
		public string ProductTitle { get; set; }

		/// <summary>
		///   Array of relative paths to projects to build the documentation for.
		/// </summary>
		public List<string> Projects { get; private set; }

		/// <summary>
		///   Lookup table of transform component arguments to change reflection data, XML comments, and/or MAML topics to a format suitable for use in the resulting help file.
		/// </summary>
		public Dictionary<string, string> TransformComponentArguments { get; private set; }

		/// <summary>
		///   This property is used to set the vendor name that appears in the help content setup file. If not set, a default value of "VendorName" will be used.
		/// </summary>
		public string Vendor { get; set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="SHFBProject"/> object.
		/// </summary>
		/// <param name="name">Name of the project.</param>
		/// <param name="outputPath">Path to place the output in. Must be relative to the project path.</param>
		/// <param name="contentLayout">Location of the content layout file relative to the project path.</param>
		/// <param name="relativePath">Relative path of this project.</param>
		/// <exception cref="ArgumentNullException"><i>name</i>, <i>outputPath</i>, or <i>contentLayout</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>name</i>, <i>outputPath</i>, or <i>contentLayout</i> is an empty string.</exception>
		/// <exception cref="ArgumentException"><i>relativePath</i> is not a valid path.</exception>
		public SHFBProject(string name, string outputPath, string contentLayout, string relativePath = null) : base(name, ProjectType.SandcastleHelpFileBuilder, relativePath)
		{
			if(outputPath == null)
				throw new ArgumentNullException("outputPath");
			if(outputPath.Length == 0)
				throw new ArgumentException("outputPath is an empty string");

			if (contentLayout == null)
				throw new ArgumentNullException("contentLayout");
			if (contentLayout.Length == 0)
				throw new ArgumentException("contentLayout is an empty string");

			ContentLayout = contentLayout;
			OutputPath = outputPath;
			Projects = new List<string>();
			References = new List<ProjectReferenceAssembly>();
			NamespaceSummaries = new List<NamespaceSummary>();
			ItemGroups = new List<List<Content>>();
			TransformComponentArguments = new Dictionary<string, string>();
		}

		/// <summary>
		///   Gets the file extension for this project.
		/// </summary>
		/// <returns>'shfbproj' is the extension for Sandcastle Help File Builder projects.</returns>
		protected override string GetProjectFileExtension()
		{
			return "shfbproj";
		}

		/// <summary>
		///   Write the project information out to a file.
		/// </summary>
		/// <param name="rootFolder">Root location of the file. (The relative path will be added to this folder to generate the file.)</param>
		/// <exception cref="ArgumentNullException"><i>rootFolder</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>rootFolder</i> is not a valid folder path.</exception>
		/// <exception cref="InvalidOperationException">No C# projects have been added to <see cref="Projects"/>.</exception>
		/// <exception cref="IOException">An error occurred while writing to the file.</exception>
		public override void WriteToFile(string rootFolder)
		{
			if(rootFolder == null)
				throw new ArgumentNullException("rootFolder");
			if(rootFolder.Length == 0)
				throw new ArgumentException("rootFolder is an empty string");
			try
			{
				rootFolder = Path.GetFullPath(rootFolder);
			}
			catch(Exception e)
			{
				throw new ArgumentException("rootFolder is not a valid path (see inner exception).", e);
			}

			if(Projects.Count == 0)
				throw new InvalidOperationException("An attempt was made to write the project information to a file, but no C# projects sources have been added to the Projects list. A documentation project must have at least one C# project.");

			string fullFolderPath;
			if(RelativePath.Length > 0)
				fullFolderPath = Path.Combine(rootFolder, RelativePath);
			else
				fullFolderPath = rootFolder;

			string outputPath = OutputPath;
			if(Path.IsPathRooted(outputPath))
				outputPath = StringUtility.ConvertAbsolutePathToRelative(outputPath, fullFolderPath);

			string path = Path.Combine(fullFolderPath, ProjectFileName);
			using(StreamWriter sw = new StreamWriter(path))
			{
				sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
				if(Version == VisualStudioVersion.VS2010 || Version == VisualStudioVersion.VS2013)
					sw.WriteLine("<Project ToolsVersion=\"4.0\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
				else if(Version == VisualStudioVersion.VS2015)
					sw.WriteLine("<Project ToolsVersion=\"14.0\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
				else
					throw new NotImplementedException("The Visual Studio version provided was not recognized as a supported version.");
				sw.WriteLine("	<PropertyGroup>");
				sw.WriteLine("		<Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>");
				sw.WriteLine("		<Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>");
				sw.WriteLine(string.Format("		<ProjectGuid>{0}</ProjectGuid>", Guid.ToString("B")));
				sw.WriteLine("		<SchemaVersion>2.0</SchemaVersion>");
				sw.WriteLine("		<SHFBSchemaVersion>1.9.9.0</SHFBSchemaVersion>");
				sw.WriteLine("		<!-- User Defined properties -->");
				sw.WriteLine("		<SolutionDirName>$([System.IO.Directory]::GetParent('$(MSBuildProjectDirectory)\\..\\').Name)</SolutionDirName>");
				sw.WriteLine("		<!-- AssemblyName, Name, and RootNamespace are not used by SHFB but Visual");
				sw.WriteLine("		Studio adds them anyway -->");
				sw.WriteLine("		<AssemblyName>Documentation</AssemblyName>");
				sw.WriteLine(string.Format("		<RootNamespace>{0}</RootNamespace>", RootNamespace));
				sw.WriteLine("		<Name>Documentation</Name>");
				sw.WriteLine("		<!-- SHFB properties -->");
				sw.WriteLine("		<HelpFileVersion>$(Major).$(Minor).$(Build).$(Revision)</HelpFileVersion>");
				sw.WriteLine(string.Format("		<OutputPath>{0}</OutputPath>", outputPath));
				sw.WriteLine(string.Format("		<HtmlHelpName>{0}</HtmlHelpName>", Name));
				if (HeaderText != null && HeaderText.Length > 0)
					sw.WriteLine(string.Format("		<HeaderText>{0}</HeaderText>", HeaderText));
				if(FooterText != null && FooterText.Length > 0)
					sw.WriteLine(string.Format("		<FooterText>{0}</FooterText>", FooterText));
				if (DefaultValues.CopyrightTemplate != null && DefaultValues.CopyrightTemplate.Length > 0)
					sw.WriteLine(string.Format("		<CopyrightText>{0}</CopyrightText>", DocumentationHelper.ConvertTemplateLineToActual(DefaultValues.CopyrightTemplate, null, null, true)));
				sw.WriteLine("		<ProjectSummary>");
				sw.WriteLine("		</ProjectSummary>");
				sw.WriteLine("		<MissingTags>None</MissingTags>");
				sw.WriteLine("		<VisibleItems>Attributes, ExplicitInterfaceImplementations, Internals, Privates, PrivateFields, Protected, SealedProtected, ProtectedInternalAsProtected</VisibleItems>");
				sw.WriteLine(string.Format("		<HtmlHelp1xCompilerPath>{0}</HtmlHelp1xCompilerPath>", DefaultValues.HtmlHelp1xCompilerPath));
				sw.WriteLine(string.Format("		<HtmlHelp2xCompilerPath>{0}</HtmlHelp2xCompilerPath>", DefaultValues.HtmlHelp2xCompilerPath));
				sw.WriteLine(string.Format("		<SandcastlePath>{0}</SandcastlePath>", DefaultValues.SandcastlePath));
				sw.WriteLine("		<WorkingPath>");
				sw.WriteLine("		</WorkingPath>");
				sw.WriteLine("		<BuildLogFile>");
				sw.WriteLine("		</BuildLogFile>");
				sw.WriteLine("		<HelpFileFormat>HtmlHelp1, MSHelpViewer</HelpFileFormat>");
				if(Version == VisualStudioVersion.VS2010 || Version == VisualStudioVersion.VS2013)
					sw.WriteLine("		<FrameworkVersion>.NET Framework 4.0</FrameworkVersion>");
				else if(Version == VisualStudioVersion.VS2015)
					sw.WriteLine("		<FrameworkVersion>.NET Framework 4.5.2</FrameworkVersion>");
				else
					throw new NotImplementedException("The Visual Studio version provided was not recognized as a supported version.");
				sw.WriteLine(string.Format("		<HelpTitle>{0}</HelpTitle>", HelpTitle));
				sw.WriteLine("		<HelpFileVersion>1.0.0.0</HelpFileVersion>");
				if(Version == VisualStudioVersion.VS2010)
					sw.WriteLine("		<PresentationStyle>VS2010</PresentationStyle>");
				else if(Version == VisualStudioVersion.VS2013 || Version == VisualStudioVersion.VS2015)
					sw.WriteLine("		<PresentationStyle>VS2013</PresentationStyle>");
				else
					throw new NotImplementedException("The Visual Studio version provided was not recognized as a supported version.");
				sw.WriteLine("		<ComponentConfigurations>");
				sw.WriteLine("			<ComponentConfig id=\"IntelliSense Component\" enabled=\"True\">");
				sw.WriteLine("			<component id=\"IntelliSense Component\" type=\"SandcastleBuilder.Components.IntelliSenseComponent\" assembly=\"{@SHFBFolder}SandcastleBuilder.Components.dll\">");
				sw.WriteLine("			<!-- Output options (optional)");
				sw.WriteLine("				Attributes:");
				sw.WriteLine("					Include Namespaces (false by default)");
				sw.WriteLine("					Namespaces filename (\"Namespaces\" if not specified or empty)");
				sw.WriteLine("					Directory (current folder if not specified or empty) -->");
				sw.WriteLine("			<output includeNamespaces=\"false\" namespacesFile=\"Namespaces\" folder=\"{@OutputFolder}\" />");

				sw.WriteLine("			</component>");
				sw.WriteLine("			</ComponentConfig>");
				sw.WriteLine("		</ComponentConfigurations>");
				sw.WriteLine("		<DocumentationSources>");
				foreach(string projPath in Projects)
					sw.WriteLine(string.Format("			<DocumentationSource sourceFile=\"{0}\" />", projPath));
				sw.WriteLine("		</DocumentationSources>");
				sw.WriteLine("		<HelpAttributes>");
				sw.WriteLine("			<HelpAttribute name=\"DocSet\" value=\"{@HtmlHelpName}\" />");
				sw.WriteLine("			<HelpAttribute name=\"DocSet\" value=\"NetFramework\" />");
				sw.WriteLine("			<HelpAttribute name=\"TargetOS\" value=\"Windows\" />");
				sw.WriteLine("		</HelpAttributes>");
				sw.WriteLine("		<NamespaceSummaries>");
				foreach(NamespaceSummary summary in NamespaceSummaries)
				{
					if(summary.Description == null)
						sw.WriteLine(string.Format("			<NamespaceSummaryItem name=\"{0}\" isDocumented=\"{1}\" />", summary.Name, summary.IsDocumented.ToString()));
					else
						sw.WriteLine(string.Format("			<NamespaceSummaryItem name=\"{0}\" isDocumented=\"{1}\">{2}</NamespaceSummaryItem>", summary.Name, summary.IsDocumented.ToString(), summary.Description));
				}
				sw.WriteLine("		</NamespaceSummaries>");
				if(ProductTitle != null && ProductTitle.Length > 0)
					sw.WriteLine(string.Format("		<ProductTitle>{0}</ProductTitle>", ProductTitle));
				if (Vendor != null && Vendor.Length > 0)
					sw.WriteLine(string.Format("		<VendorName>{0}</VendorName>", Vendor));
				sw.WriteLine("		<ApiFilter />");
				sw.WriteLine("		<PlugInConfigurations>");
				sw.WriteLine("		</PlugInConfigurations>");
				sw.WriteLine("		<SyntaxFilters>C#</SyntaxFilters>");
				sw.WriteLine("		<SdkLinkTarget>Blank</SdkLinkTarget>");
				sw.WriteLine("		<RootNamespaceContainer>False</RootNamespaceContainer>");
				sw.WriteLine("		<Preliminary>False</Preliminary>");
				sw.WriteLine("		<NamingMethod>Guid</NamingMethod>");
				sw.WriteLine("		<Language>en-US</Language>");
				sw.WriteLine("		<ContentPlacement>AboveNamespaces</ContentPlacement>");
				sw.WriteLine("		<BuildAssemblerVerbosity>OnlyErrors</BuildAssemblerVerbosity>");
				sw.WriteLine("		<IndentHtml>False</IndentHtml>");
				sw.WriteLine("		<KeepLogFile>True</KeepLogFile>");
				sw.WriteLine("		<DisableCodeBlockComponent>False</DisableCodeBlockComponent>");
				sw.WriteLine("		<CppCommentsFixup>False</CppCommentsFixup>");
				sw.WriteLine("		<CleanIntermediates>True</CleanIntermediates>");
				sw.WriteLine("		<TransformComponentArguments>");
				foreach(string key in TransformComponentArguments.Keys)
					sw.WriteLine(string.Format("			<Argument Key=\"{0}\" Value=\"{1}\" />", key, TransformComponentArguments[key]));

				sw.WriteLine("		</TransformComponentArguments>");
				sw.WriteLine("		<ComponentPath />");
				sw.WriteLine("		<MaximumGroupParts>2</MaximumGroupParts>");
				sw.WriteLine("		<NamespaceGrouping>False</NamespaceGrouping>");
				sw.WriteLine("		<PreBuildEvent>");
				sw.WriteLine("		</PreBuildEvent>");
				sw.WriteLine("		<RunPostBuildEvent>OnBuildSuccess</RunPostBuildEvent>");
				sw.WriteLine("		<PostBuildEvent>");
				sw.WriteLine("		</PostBuildEvent>");
				sw.WriteLine("	</PropertyGroup>");
				sw.WriteLine("	<!-- There are no properties for these groups.  AnyCPU needs to appear in");
				sw.WriteLine("	     order for Visual Studio to perform the build.  The others are optional");
				sw.WriteLine("	     common platform types that may appear. -->");
				sw.WriteLine("	<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
				sw.WriteLine("	</PropertyGroup>");
				sw.WriteLine("	<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
				sw.WriteLine("	</PropertyGroup>");
				sw.WriteLine("	<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|x86' \">");
				sw.WriteLine("	</PropertyGroup>");
				sw.WriteLine("	<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|x86' \">");
				sw.WriteLine("	</PropertyGroup>");
				sw.WriteLine("	<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|x64' \">");
				sw.WriteLine("	</PropertyGroup>");
				sw.WriteLine("	<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|x64' \">");
				sw.WriteLine("	</PropertyGroup>");
				sw.WriteLine("	<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|Win32' \">");
				sw.WriteLine("	</PropertyGroup>");
				sw.WriteLine("	<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|Win32' \">");
				sw.WriteLine("	</PropertyGroup>");
				sw.WriteLine("	<ItemGroup>");
				sw.WriteLine(string.Format("		<ContentLayout Include=\"{0}\" />", ContentLayout));
				sw.WriteLine("	</ItemGroup>");
				foreach(List<Content> content in ItemGroups)
				{
					if(content.Count > 0)
					{
						sw.WriteLine("	<ItemGroup>");
						foreach(Content item in content)
						{
							string typeName = Enum.GetName(typeof(Content.ContentType), item.Type);
							if(item.ImageID == null && item.AlternateText == null && item.Link == null)
							{
								sw.WriteLine(string.Format("		<{0} Include=\"{1}\" />", typeName, item.Include));
							}
							else
							{
								sw.WriteLine(string.Format("		<{0} Include=\"{1}\">", typeName, item.Include));
								if(item.ImageID != null)
									sw.WriteLine(string.Format("			<ImageId>{0}</ImageId>", item.ImageID));
								if(item.AlternateText != null)
									sw.WriteLine(string.Format("			<AlternateText>{0}</AlternateText>", item.AlternateText));
								if(item.Link != null)
									sw.WriteLine(string.Format("			<Link>{0}</Link>", item.Link));
								sw.WriteLine(string.Format("		</{0}>", typeName));
							}
						}
						sw.WriteLine("	</ItemGroup>");
					}
				}
				sw.WriteLine("	<ItemGroup>");
				foreach(ProjectReferenceAssembly assembly in References)
				{
					sw.WriteLine(string.Format("		<Reference Include=\"{0}\">", assembly.Reference));
					if(assembly.HintPath != null)
						sw.WriteLine(string.Format("			<HintPath>{0}</HintPath>", assembly.HintPath));
					sw.WriteLine("		</Reference>");
				}
				sw.WriteLine("	</ItemGroup>");
				sw.WriteLine("	<ItemGroup>");
				sw.WriteLine("		<Tokens Include=\"TokenFile.tokens\" />");
				sw.WriteLine("	</ItemGroup>");
				sw.WriteLine("	<!-- Import the SHFB build targets -->");
				sw.WriteLine("	<Import Project=\"$(SHFBROOT)\\SandcastleHelpFileBuilder.targets\" />");
				sw.WriteLine("</Project>");
			}
		}

		/// <summary>
		///   Writes all the project information, classes, etc. out to various files.
		/// </summary>
		/// <param name="rootFolder">Root location of the files. (The relative path will be added to this folder to generate the files.)</param>
		/// <exception cref="ArgumentException"><i>rootFolder</i> is not a valid folder path.</exception>
		/// <exception cref="ArgumentNullException"><i>rootFolder</i> is a null reference.</exception>
		/// <exception cref="InvalidOperationException">No C# projects have been added to <see cref="Projects"/>.</exception>
		/// <exception cref="IOException">An error occurred while writing to one of the files.</exception>
		public override void WriteToFiles(string rootFolder)
		{
			if(rootFolder == null)
				throw new ArgumentNullException("rootFolder");
			if(rootFolder.Length == 0)
				throw new ArgumentException("rootFolder is an empty string");
			try
			{
				rootFolder = Path.GetFullPath(rootFolder);
			}
			catch(Exception e)
			{
				throw new ArgumentException("rootFolder is not a valid path (see inner exception).", e);
			}

			if(Projects.Count == 0)
				throw new InvalidOperationException("An attempt was made to write the project information to a file, but no C# projects have been added to the Projects list. A documentation project must have at least one C# project.");

			string fullFolderPath;
			if(RelativePath.Length > 0)
				fullFolderPath = Path.Combine(rootFolder, RelativePath);
			else
				fullFolderPath = rootFolder;

			// Generate any needed directories.
			DefaultValues.CreateFolderPath(fullFolderPath);

			// Create the tokens file.
			string path = Path.Combine(fullFolderPath, "TokenFile.tokens");
			using(StreamWriter sw = new StreamWriter(path))
			{
				sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
				sw.WriteLine("<content xml:space=\"preserve\" xmlns:ddue=\"http://ddue.schemas.microsoft.com/authoring/2003/5\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">");
				sw.WriteLine("	<item id=\"BuildDate\">{@BuildDate}</item>");
				sw.WriteLine("	<item id=\"HelpFileVersion\">{@HelpFileVersion}</item>");
				sw.WriteLine("	<item id=\"SolutionName\">{@SolutionName}</item>");
				sw.WriteLine("</content>");
			}

			// Write the project file.
			WriteToFile(rootFolder);
		}

		#endregion Methods
	}
}