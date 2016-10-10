//********************************************************************************************************************************
// Filename:    AssemblyInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified AssemblyInfo.cs file.
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
using System.IO;

namespace CSCodeGen
{
	/// <summary>
	///   Represents the information in an AssemblyInfo.cs file.
	/// </summary>
	public class AssemblyInfo
	{
		#region Properties

		/// <summary>
		///   Description of the assembly.
		/// </summary>
		public string Description { get; protected set; }

		/// <summary>
		///   GUID of the assembly.
		/// </summary>
		public Guid Guid { get; protected set; }

		/// <summary>
		///   Product of the assembly.
		/// </summary>
		public string Product { get; set; }

		/// <summary>
		///   Product version of the assembly.
		/// </summary>
		public Version ProductVersion { get; set; }

		/// <summary>
		///   Title of the assembly.
		/// </summary>
		public string Title { get; protected set; }

		/// <summary>
		///   Version of the assembly.
		/// </summary>
		public Version Version { get; protected set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="AssemblyInfo"/> object.
		/// </summary>
		/// <param name="title">Title of the assembly.</param>
		/// <param name="version"><see cref="Version"/> object representing the version of the assembly.</param>
		/// <param name="description">Description of the assembly.</param>
		/// <param name="product">Product name of the assembly. Can be null.</param>
		/// <param name="productVersion">Product version of the assembly. Can be null.</param>
		/// <exception cref="ArgumentNullException"><paramref name="title"/>, <paramref name="version"/>, or <paramref name="description"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="title"/>, or <paramref name="description"/> is an empty string.</exception>
		public AssemblyInfo(string title, Version version, string description, string product = null, Version productVersion = null)
		{
			if (title == null)
				throw new ArgumentNullException("title");
			if (title.Length == 0)
				throw new ArgumentException("title is an empty string");
			if (version == null)
				throw new ArgumentNullException("version");
			if (description == null)
				throw new ArgumentNullException("description");
			if (description.Length == 0)
				throw new ArgumentException("description is an empty string");

			Title = title;
			Version = version;
			Description = description;
			Product = product;
			ProductVersion = productVersion;
			Guid = Guid.NewGuid();
		}

		/// <summary>
		///   Writes the assembly information to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the code to.</param>
		/// <param name="indentOffset">Number of indentations to add before the code.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			DocumentationHelper.WriteLine(wr, "// General Information about an assembly is controlled through the following", indentOffset);
			DocumentationHelper.WriteLine(wr, "// set of attributes. Change these attribute values to modify the information", indentOffset);
			DocumentationHelper.WriteLine(wr, "// associated with an assembly.", indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("[assembly: AssemblyTitle(\"{0}\")]", Title), indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("[assembly: AssemblyDescription(\"{0}\")]", Description), indentOffset);
			if(DefaultValues.CompanyName != null && DefaultValues.CompanyName.Length > 0)
				DocumentationHelper.WriteLine(wr, string.Format("[assembly: AssemblyCompany(\"{0}\")]", DefaultValues.CompanyName), indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("[assembly: AssemblyVersion(\"{0}\")]", Version.ToString()), indentOffset);
			if(Product != null && Product.Length > 0)
				DocumentationHelper.WriteLine(wr, string.Format("[assembly: AssemblyProduct(\"{0}\")]", Product), indentOffset);
			if (ProductVersion != null)
				DocumentationHelper.WriteLine(wr, string.Format("[assembly: AssemblyInformationalVersion(\"{0}\")]", ProductVersion.ToString()), indentOffset);
			if(DefaultValues.CopyrightTemplate != null && DefaultValues.CopyrightTemplate.Length > 0)
			{
				string copyright = DocumentationHelper.ConvertTemplateLineToActual(DefaultValues.CopyrightTemplate, null, null);
				DocumentationHelper.WriteLine(wr, string.Format("[assembly: AssemblyCopyright(\"{0}\")]", copyright), indentOffset);
			}
			DocumentationHelper.WriteLine(wr);
			DocumentationHelper.WriteLine(wr, "// Setting ComVisible to false makes the types in this assembly not visible", indentOffset);
			DocumentationHelper.WriteLine(wr, "// to COM components.  If you need to access a type in this assembly from", indentOffset);
			DocumentationHelper.WriteLine(wr, "// COM, set the ComVisible attribute to true on that type.", indentOffset);
			DocumentationHelper.WriteLine(wr, "[assembly: ComVisible(false)]", indentOffset);
			DocumentationHelper.WriteLine(wr);
			DocumentationHelper.WriteLine(wr, "// The following GUID is for the ID of the typelib if this project is exposed to COM", indentOffset);
			DocumentationHelper.WriteLine(wr, string.Format("[assembly: Guid(\"{0}\")]", Guid.ToString()), indentOffset);
		}

		/// <summary>
		///   Writes the assembly information out to a file.
		/// </summary>
		/// <param name="filePath">Directory that the file should be written to. If it doesn't exist it will be created.</param>
		/// <exception cref="ArgumentNullException"><paramref name="folder"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="folder"/> is not a valid folder.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public void WriteToFile(string filePath)
		{
			filePath = DefaultValues.CreateFolderPath(filePath);

			string fileName = "AssemblyInfo.cs";
			using (StreamWriter wr = new StreamWriter(Path.Combine(filePath, fileName)))
			{
				DocumentationHelper.WriteFileHeader(wr, fileName, "Contains the assembly attributes.");
				DocumentationHelper.WriteLine(wr, "using System.Reflection;", 0);
				DocumentationHelper.WriteLine(wr, "using System.Runtime.InteropServices;", 0);
				DocumentationHelper.WriteLine(wr);
				Write(wr, 0);
			}
		}

		#endregion Methods
	}
}