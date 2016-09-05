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
using System.Linq;
using System.Text;
using System.IO;

namespace CSCodeGen.SHFB
{
	/// <summary>
	///   Represents a Sandcastle Help File Builder Project.
	/// </summary>
	public class SHFBProjectInfo : BaseProject
	{
		#region Properties

		/// <summary>
		///   Output Path.
		/// </summary>
		public string OutputPath { get; set; }

		public string FooterText {get; set; }

		public List<ExternalAssembly> NamespaceSummaries { get; private set; }

		public string ProductTitle { get; set; }

		public string Vendor { get; set; }

		public string Title {get; set;}

		/// <summary>
		///   Array of <see cref="ProjectInfo"/> objects representing the projects to build documentation for.
		/// </summary>
		public List<ProjectInfo> Projects { get; private set; }

		public List<List<Content>> ItemGroups { get; private set; }

		public Dictionary<string, string> TransformComponentArguments { get; private set; }

		public string ContentLayout { get; private set; }

		#endregion Properties

		#region Methods

		public SHFBProjectInfo(string name, string outputPath, string contentLayout, string relativePath = null) : base(name, ProjectType.SandcastleHelpFileBuilder, relativePath)
		{
			if(outputPath == null)
				throw new ArgumentNullException("outputPath");
			if(outputPath.Length == 0)
				throw new ArgumentException("outputPath is an empty string");

			ContentLayout = contentLayout;
			OutputPath = outputPath;
			References = new List<ProjectReferenceAssembly>();
			NamespaceSummaries = new List<ExternalAssembly>();
			ItemGroups = new List<List<Content>>();
			TransformComponentArguments = new Dictionary<string, string>();
		}

		public void WriteFiles(string rootFolder)
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

			string fullFolderPath;
			if(RelativePath.Length > 0)
				fullFolderPath = Path.Combine(rootFolder, RelativePath);
			else
				fullFolderPath = rootFolder;

			// Convert absolute debug and release paths to relative.
			string outputPath = OutputPath;
			if(Path.IsPathRooted(outputPath))
				outputPath = StringUtility.ConvertAbsolutePathToRelative(outputPath, fullFolderPath);

			// Write out the project file.
			string path = Path.Combine(fullFolderPath, ProjectFileName);
			using(StreamWriter sw = new StreamWriter(path))
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
				sw.WriteLine("		<SchemaVersion>2.0</SchemaVersion>");
				sw.WriteLine("		<SHFBSchemaVersion>1.9.9.0</SHFBSchemaVersion>");
				sw.WriteLine("		<!-- User Defined properties -->");
				sw.WriteLine("		<SolutionDirName>$([System.IO.Directory]::GetParent('$(MSBuildProjectDirectory)\\..\').Name)</SolutionDirName>");
				sw.WriteLine("		<!-- AssemblyName, Name, and RootNamespace are not used by SHFB but Visual");
				sw.WriteLine("		Studio adds them anyway -->");
				sw.WriteLine("		<AssemblyName>Documentation</AssemblyName>");
				sw.WriteLine(string.Format("		<RootNamespace>{0}</RootNamespace>", RootNamespace));
				sw.WriteLine("		<Name>Documentation</Name>");
				sw.WriteLine("		<!-- SHFB properties -->");
				sw.WriteLine("		<HelpFileVersion>$(Major).$(Minor).$(Build).$(Revision)</HelpFileVersion>");
				sw.WriteLine(string.Format("		<OutputPath>{0}</OutputPath>", outputPath));
				sw.WriteLine(string.Format("		<HtmlHelpName>{0}</HtmlHelpName>", Name));
				if(FooterText != null && FooterText.Length > 0)
					sw.WriteLine(string.Format("		<FooterText>{0}</FooterText>", FooterText));
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
				if (Version == VisualStudioVersion.VS2010 || Version == VisualStudioVersion.VS2013)
					sw.WriteLine("		<FrameworkVersion>v4.0</FrameworkVersion>");
				else if(Version == VisualStudioVersion.VS2015)
					sw.WriteLine("		<FrameworkVersion>v4.5.2</FrameworkVersion>");
				else
					throw new NotImplementedException("The Visual Studio version provided was not recognized as a supported version.");
				sw.WriteLine(string.Format("		<HelpTitle>{0}</HelpTitle>", Title));
				sw.WriteLine("		<HelpFileVersion>1.0.0.0</HelpFileVersion>");
				if (Version == VisualStudioVersion.VS2010 || Version == VisualStudioVersion.VS2013)
					sw.WriteLine("		<PresentationStyle>VS2010</PresentationStyle>");
				else if(Version == VisualStudioVersion.VS2015)
					sw.WriteLine("		<PresentationStyle>VS2015</PresentationStyle>");
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
				foreach(ProjectInfo proj in Projects)
					sw.WriteLine(string.Format("			<DocumentationSource sourceFile=\"{0}\" />", proj.RelativePath));
				sw.WriteLine("		</DocumentationSources>");
				sw.WriteLine("		<HelpAttributes>");
				sw.WriteLine("			<HelpAttribute name=\"DocSet\" value=\"{@HtmlHelpName}\" />");
				sw.WriteLine("			<HelpAttribute name=\"DocSet\" value=\"NetFramework\" />");
				sw.WriteLine("			<HelpAttribute name=\"TargetOS\" value=\"Windows\" />");
				sw.WriteLine("		</HelpAttributes>");
				sw.WriteLine("		<NamespaceSummaries>");
				foreach(ExternalAssembly summary in NamespaceSummaries)
				{
					if(summary.Description == null)
						sw.WriteLine(string.Format("			<NamespaceSummaryItem name=\"{0}\" isDocumented=\"{1}\" />", summary.Name, summary.IsDocumented.ToString()));
					else
						sw.WriteLine(string.Format("			<NamespaceSummaryItem name=\"{0}\" isDocumented=\"{1}\">{2}</NamespaceSummaryItem>", summary.Name, summary.IsDocumented.ToString()));
				}
				sw.WriteLine("		</NamespaceSummaries>");
				sw.WriteLine(string.Format("		<ProductTitle>{0}</ProductTitle>", ProductTitle));
				sw.WriteLine(string.Format("		<VendorName>{1}</VendorName>", Vendor));
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
					sw.WriteLine(string.Format("			<Argument Key=\"{0}\" Value=\"{1}\" />"));

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
							string typeName = Enum.GetName(typeof(Content), item.Type);
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
							sw.WriteLine("	</ItemGroup>");
						}
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

			// Create the tokens file.
			path = Path.Combine(fullFolderPath, "TokenFile.tokens");
			using(StreamWriter sw = new StreamWriter(path))
			{
				sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
				sw.WriteLine("<content xml:space=\"preserve\" xmlns:ddue=\"http://ddue.schemas.microsoft.com/authoring/2003/5\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">");
				sw.WriteLine("	<item id=\"BuildDate\">{@BuildDate}</item>");
				sw.WriteLine("	<item id=\"HelpFileVersion\">{@HelpFileVersion}</item>");
				sw.WriteLine("	<item id=\"SolutionName\">{@SolutionName}</item>");
				sw.WriteLine("</content>");
			}
		}

		protected override string GetProjectFileExtension()
		{
			return "shfbproj";
		}

		#endregion Methods
	}
}