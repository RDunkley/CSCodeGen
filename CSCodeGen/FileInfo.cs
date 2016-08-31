//********************************************************************************************************************************
// Filename:    FileInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified C# file.
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
using System.Text;

namespace CSCodeGen
{
	/// <summary>
	///   Represents a C# file.
	/// </summary>
	public class FileInfo
	{
		#region Properties

		/// <summary>
		///   Description of the file.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		///   RelativePath of the folder where the C# file represented.
		/// </summary>
		public string RelativePath { get; private set; }

		/// <summary>
		///   Name of the file.
		/// </summary>
		public string FileName { get; private set; }

		/// <summary>
		///   Extension added to the filename.
		/// </summary>
		public string FileNameExtension { get; private set; }

		/// <summary>
		///   Namespace of the file.
		/// </summary>
		public string NameSpace { get; private set; }

		/// <summary>
		///   Type contained in the namespace.
		/// </summary>
		public NamespaceTypeInfo Type { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="FileInfo"/> object.
		/// </summary>
		/// <param name="nameSpace">Namespace of the file.</param>
		/// <param name="typeObject"><see cref="NamespaceTypeInfo"/> object containing the type to be represented in the file.</param>
		/// <param name="relativePath">Relative path where the file is represented.</param>
		/// <param name="description">Description of the file.</param>
		/// <param name="fileNameExtension">Extension to add to the filename. (Ex: 'designer' would be for filename.designer.cs). Can be null or empty.</param>
		/// <exception cref="ArgumentNullException"><i>nameSpace</i> or <i>info</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>nameSpace</i> is an empty string.</exception>
		/// <exception cref="ArgumentException"><i>relativePath</i> is defined, but is not a relative path.</exception>
		public FileInfo(string nameSpace, NamespaceTypeInfo typeObject, string relativePath = null, string description = null, string fileNameExtension = null)
		{
			if (nameSpace == null)
				throw new ArgumentNullException("nameSpace");
			if (nameSpace.Length == 0)
				throw new ArgumentException("nameSpace is an empty string");
			if (typeObject == null)
				throw new ArgumentNullException("info");

			if (relativePath == null)
				relativePath = string.Empty;

			if(relativePath.Length > 0)
				if (Path.IsPathRooted(relativePath))
					throw new ArgumentException("relativePath is rooted. It must be a relative path.");

			if (fileNameExtension == null || fileNameExtension.Length == 0)
				fileNameExtension = string.Empty;
			else
				fileNameExtension = string.Format(".{0}", fileNameExtension);
			FileName = string.Format("{0}{1}.cs", typeObject.Name, fileNameExtension);
			RelativePath = relativePath;
			NameSpace = nameSpace;
			Type = typeObject;
			Description = description;
			FileNameExtension = fileNameExtension;
		}

		/// <summary>
		///   Writes the source code information in this object out to a file in the form of source code.
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
			catch(Exception e)
			{
				throw new ArgumentException("rootFolder is not a valid path (see inner exception).", e);
			}

			string fullFolderPath;
			if (RelativePath.Length > 0)
				fullFolderPath = Path.Combine(rootFolder, RelativePath);
			else
				fullFolderPath = rootFolder;
			string fullPath = Path.Combine(fullFolderPath, FileName);

			// Validate the class if it is a class.
			if(Type is ClassInfo)
				((ClassInfo)Type).Validate();

			// Generate any needed directories.
			DefaultValues.CreateFolderPath(fullFolderPath);
			using (StreamWriter wr = new StreamWriter(fullPath))
			{
				DocumentationHelper.WriteFileHeader(wr, FileName, Description);

				if(DefaultValues.IncludeSubHeader)
					WriteFileSubHeader(wr);

				// Add usings.
				if(Type.Usings.Length > 0)
				{
					foreach (string item in Type.Usings)
						DocumentationHelper.WriteLine(wr, string.Format("using {0};", item), 0);
					DocumentationHelper.WriteLine(wr);
				}

				// Write the namespace.
				DocumentationHelper.WriteLine(wr, string.Format("namespace {0}", NameSpace), 0);
				DocumentationHelper.WriteLine(wr, "{", 0);

				// Check if its an enumeration.
				EnumInfo enumInfo = Type as EnumInfo;
				if (enumInfo != null)
					enumInfo.Write(wr, 1);

				// Check if its a class.
				ClassInfo classInfo = Type as ClassInfo;
				if (classInfo != null)
					classInfo.Write(wr, 1);

				DocumentationHelper.WriteLine(wr, "}", 0);
			}
		}

		/// <summary>
		///   Writes the file to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> ojbect to write the code to.</param>
		/// <exception cref="ArgumentNullException"><i>wr</i> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		private void WriteFileSubHeader(StreamWriter wr)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			EnumInfo enumInfo = Type as EnumInfo;
			if (enumInfo != null)
			{
				// Write the enumeration information to the sub-header.
				if (!DocumentationHelper.WriteFlowerLine(wr, 0))
					DocumentationHelper.WriteLine(wr, "//", 0);
				DocumentationHelper.WriteLine(wr, string.Format("// {0}.{1} (enum) ({2})", NameSpace, enumInfo.Name, enumInfo.Access), 0);
				DocumentationHelper.WriteLine(wr, "//", 0);
				DocumentationHelper.WriteLine(wr, "//   Names:", 0);
				for (int i = 0; i < enumInfo.Values.Count; i++)
					DocumentationHelper.WriteLine(wr, string.Format("//          {0}", enumInfo.Values[i].Name), 0);
				DocumentationHelper.WriteFlowerLine(wr, 0);
			}

			ClassInfo classInfo = Type as ClassInfo;
			if (classInfo != null)
			{
				if (!DocumentationHelper.WriteFlowerLine(wr, 0))
					DocumentationHelper.WriteLine(wr, "//", 0);
				DocumentationHelper.WriteLine(wr, string.Format("// {0}.{1} (class) ({2})", NameSpace, classInfo.Name, classInfo.Access), 0);
				int numCharsForType = 0;
				int numCharsForName = 0;
				if (classInfo.Fields.Count > 0)
				{
					MaxStringLength("Fields", ref numCharsForType);
					foreach (FieldInfo info in classInfo.Fields)
						MaxStringLength(info.Name, ref numCharsForName);
				}
				if (classInfo.Properties.Count > 0)
				{
					MaxStringLength("Properties", ref numCharsForType);
					foreach (PropertyInfo info in classInfo.Properties)
						MaxStringLength(info.Name, ref numCharsForName);
				}
				if (classInfo.Constructors.Count > 0)
				{
					MaxStringLength("Constructors", ref numCharsForType);
					foreach (ConstructorInfo info in classInfo.Constructors)
						MaxStringLength(info.Name, ref numCharsForName);
				}
				if (classInfo.Enums.Count > 0)
				{
					MaxStringLength("Enumerations", ref numCharsForType);
					foreach (EnumInfo info in classInfo.Enums)
						MaxStringLength(info.Name, ref numCharsForName);
				}
				if (classInfo.Methods.Count > 0)
				{
					MaxStringLength("Methods", ref numCharsForType);
					foreach (MethodInfo info in classInfo.Methods)
						MaxStringLength(info.Name, ref numCharsForName);
				}

				numCharsForType += 2;
				numCharsForName += 1;

				if (classInfo.Fields.Count > 0)
				{
					DocumentationHelper.WriteLine(wr, "// ", 0);
					DocumentationHelper.WriteLine(wr, "//   Fields:", 0);
					classInfo.Fields.Sort();
					foreach (FieldInfo info in classInfo.Fields)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append("//   ");
						for (int i = 0; i < numCharsForType; i++)
							sb.Append(" ");
						sb.Append(info.Name);
						int left = numCharsForName - info.Name.Length;
						for (int i = 0; i < left; i++)
							sb.Append(" ");
						sb.AppendFormat("({0})", info.Access);
						DocumentationHelper.WriteLine(wr, sb.ToString(), 0);
					}
				}
				if (classInfo.Properties.Count > 0)
				{
					DocumentationHelper.WriteLine(wr, "// ", 0);
					DocumentationHelper.WriteLine(wr, "//   Properties:", 0);
					classInfo.Properties.Sort();
					foreach (PropertyInfo info in classInfo.Properties)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append("//   ");
						for (int i = 0; i < numCharsForType; i++)
							sb.Append(" ");
						sb.Append(info.Name);
						int left = numCharsForName - info.Name.Length;
						for (int i = 0; i < left; i++)
							sb.Append(" ");
						sb.AppendFormat("({0})", info.Access);
						DocumentationHelper.WriteLine(wr, sb.ToString(), 0);
					}
				}
				if (classInfo.Enums.Count > 0)
				{
					DocumentationHelper.WriteLine(wr, "// ", 0);
					DocumentationHelper.WriteLine(wr, "//   Enumerations:", 0);
					classInfo.Enums.Sort();
					foreach (EnumInfo info in classInfo.Enums)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append("//   ");
						for (int i = 0; i < numCharsForType; i++)
							sb.Append(" ");
						sb.Append(info.Name);
						int left = numCharsForName - info.Name.Length;
						for (int i = 0; i < left; i++)
							sb.Append(" ");
						sb.AppendFormat("({0})", info.Access);
						DocumentationHelper.WriteLine(wr, sb.ToString(), 0);
					}
				}
				if (classInfo.Constructors.Count > 0 || classInfo.Methods.Count > 0)
				{
					DocumentationHelper.WriteLine(wr, "// ", 0);
					DocumentationHelper.WriteLine(wr, "//   Methods:", 0);
				}
				if (classInfo.Constructors.Count > 0)
				{
					// Don't sort th constructors (should have the same name anyway).
					foreach (ConstructorInfo info in classInfo.Constructors)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append("//   ");
						for (int i = 0; i < numCharsForType; i++)
							sb.Append(" ");
						sb.Append(info.Name);
						int left = numCharsForName - info.Name.Length;
						for (int i = 0; i < left; i++)
							sb.Append(" ");
						sb.AppendFormat("({0})", info.Access);
						DocumentationHelper.WriteLine(wr, sb.ToString(), 0);
					}
				}
				if (classInfo.Methods.Count > 0)
				{
					classInfo.Methods.Sort();
					foreach (MethodInfo info in classInfo.Methods)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append("//   ");
						for (int i = 0; i < numCharsForType; i++)
							sb.Append(" ");
						sb.Append(info.Name);
						int left = numCharsForName - info.Name.Length;
						for (int i = 0; i < left; i++)
							sb.Append(" ");
						sb.AppendFormat("({0})", info.Access);
						DocumentationHelper.WriteLine(wr, sb.ToString(), 0);
					}
				}
				DocumentationHelper.WriteFlowerLine(wr, 0);
			}
		}

		/// <summary>
		///   Determines the maximum string length from the previous max and a text string.
		/// </summary>
		/// <param name="text">Text to evaluate if larger.</param>
		/// <param name="previousMax">Previous maximum length.</param>
		private void MaxStringLength(string text, ref int previousMax)
		{
			if (text.Length > previousMax)
				previousMax = text.Length;
		}

		#endregion Methods
	}
}
