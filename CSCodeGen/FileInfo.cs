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
		///   RelativePath of the folder where the C# file represented.
		/// </summary>
		public string RelativePath { get; private set; }

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
		/// <exception cref="ArgumentNullException"><paramref name="nameSpace"/> or <paramref name="info"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="nameSpace"/> is an empty string.</exception>
		/// <exception cref="ArgumentException"><paramref name="relativePath"/> is defined, but is not a relative path.</exception>
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
		///   Combines 'Name' strings with the same name in the list of string values.
		/// </summary>
		/// <typeparam name="T"><see cref="BaseTypeInfo"/> class or derives class.</typeparam>
		/// <param name="values">List of objects to combine the names of.</param>
		/// <returns>
		///   A list containing Tuples. The Tuple is composed of the unique name (combined with a number if more than one object
		///   with that name was found) and the visibility of that object. For example, if a MethodInfo object was passed in it
		///   would coalesce all methods with the same name and visibility, place the unique name with a number after it 
		///   (Ex: Write(2)), and then place the visibility in a visibility list in preparation for sub-header documentation 
		///   (Ex: (public, partial)). This information is used to build the file's sub-header information.
		/// </returns>
		private List<Tuple<string, string>> CoalesceStrings<T>(List<T> values) where T : BaseTypeInfo
		{
			List<Tuple<string, string>> returnList = new List<Tuple<string, string>>(values.Count);
			int index = 0;
			while (index < values.Count)
			{
				int newIndex = index;
				int count = 0;
				for (int j = index; j < values.Count; j++)
				{
					// Check all the other values for the same name and visibility.
					if ((values[index].Name == values[j].Name) && (values[index].Access == values[j].Access))
					{
						count++;
						newIndex++;
					}
					else
					{
						break;
					}
				}

				string name = values[index].Name;
				if (count > 1)
					name = string.Format("{0}({1})", values[index].Name, count);
				returnList.Add(Tuple.Create<string, string>(name, GetAccessString(values[index].Access)));
				index = newIndex;
			}
			return returnList;
		}

		/// <summary>
		///   This method generates the class information for the file's sub-header, including the sub-information (child classes, enums, etc.).
		/// </summary>
		/// <param name="baseClassName">Name of the base class or namespace of this class.</param>
		/// <param name="info"><see cref="ClassInfo"/> object to generate the information on.</param>
		/// <param name="rootInfo">Root information lookup table to add this class to.</param>
		/// <param name="subInfo">Sub-information lookup table to add this classes sub-information to.</param>
		private void GenerateClassInfo(string baseClassName, ClassInfo info, out Dictionary<string, string> rootInfo, out List<Dictionary<string, List<Tuple<string, string>>>> subInfo)
		{
			subInfo = new List<Dictionary<string, List<Tuple<string, string>>>>();
			rootInfo = new Dictionary<string, string>();

			string rootClassName = string.Format("{0}.{1}", baseClassName, info.Name);

			// Add the actual class first.
			subInfo.Add(GenerateClassSubInfo(info));
			rootInfo.Add(string.Format("{0} (class)", rootClassName), GetAccessString(info.Access));

			// Add child classes next.
			if (info.ChildClasses != null && info.ChildClasses.Length > 0)
			{
				foreach (ClassInfo child in info.ChildClasses)
				{
					List<Dictionary<string, List<Tuple<string, string>>>> childSubInfo;
					Dictionary<string, string> childRootInfo;
					GenerateClassInfo(rootClassName, child, out childRootInfo, out childSubInfo);

					subInfo.AddRange(childSubInfo);
					foreach (string key in childRootInfo.Keys)
						rootInfo.Add(key, childRootInfo[key]);
				}
			}

			// Add child enumerations next.
			if (info.Enums != null && info.Enums.Count > 0)
			{
				foreach (EnumInfo en in info.Enums)
				{
					subInfo.Add(GenerateEnumSubInfo(en));
					rootInfo.Add(string.Format("{0}.{1} (enum)", rootClassName, en.Name), GetAccessString(en.Access));
				}
			}
		}

		/// <summary>
		///   Generates the class sub-information.
		/// </summary>
		/// <param name="info"><see cref="ClassInfo"/> object to generate the sub-information on.</param>
		/// <returns>Lookup table of the sub-information.</returns>
		private Dictionary<string, List<Tuple<string, string>>> GenerateClassSubInfo(ClassInfo info)
		{
			Dictionary<string, List<Tuple<string, string>>> lookup = new Dictionary<string, List<Tuple<string, string>>>();

			// Add Classes First.
			if (info.ChildClasses != null && info.ChildClasses.Length > 0)
			{
				List<ClassInfo> classList = new List<ClassInfo>(info.ChildClasses);
				classList.Sort();
				lookup.Add("Classes:", new List<Tuple<string, string>>(info.ChildClasses.Length));
				foreach (ClassInfo child in classList)
					lookup["Classes:"].Add(Tuple.Create<string, string>(child.Name, GetAccessString(child.Access)));
			}

			// Add Enumerations Next.
			if (info.Enums != null && info.Enums.Count > 0)
			{
				info.Enums.Sort();
				lookup.Add("Enumerations:", new List<Tuple<string, string>>(info.Enums.Count));
				foreach (EnumInfo en in info.Enums)
					lookup["Enumerations:"].Add(Tuple.Create<string, string>(en.Name, GetAccessString(en.Access)));
			}

			// Add Fields Next.
			if (info.Fields != null && info.Fields.Count > 0)
			{
				info.Fields.Sort();
				lookup.Add("Fields:", new List<Tuple<string, string>>(info.Fields.Count));
				foreach (FieldInfo field in info.Fields)
					lookup["Fields:"].Add(Tuple.Create<string, string>(field.Name, GetAccessString(field.Access)));
			}

			// Add Properties Next.
			if (info.Properties != null && info.Properties.Count > 0)
			{
				info.Properties.Sort();
				lookup.Add("Properties:", new List<Tuple<string, string>>(info.Properties.Count));
				foreach (PropertyInfo property in info.Properties)
					lookup["Properties:"].Add(Tuple.Create<string, string>(property.Name, GetAccessString(property.Access)));
			}

			// Add Constructors and Methods Next.
			if ((info.Constructors != null && info.Constructors.Count > 0) || (info.Methods != null && info.Methods.Count > 0))
			{
				int count = 0;
				if (info.Constructors != null)
					count += info.Constructors.Count;
				if (info.Methods != null)
					count += info.Methods.Count;

				lookup.Add("Methods:", new List<Tuple<string, string>>(count));
				if (info.Constructors != null && info.Constructors.Count > 0)
				{
					info.Constructors.Sort();
					lookup["Methods:"].AddRange(CoalesceStrings(info.Constructors));
				}
				if (info.Methods != null && info.Methods.Count > 0)
				{
					info.Methods.Sort();
					lookup["Methods:"].AddRange(CoalesceStrings(info.Methods));
				}
			}

			return lookup;
		}

		/// <summary>
		///   Generates the enum sub-information.
		/// </summary>
		/// <param name="info"><see cref="EnumInfo"/> object to generate the sub-information on.</param>
		/// <returns>Lookup table containing the sub-information.</returns>
		private Dictionary<string, List<Tuple<string, string>>> GenerateEnumSubInfo(EnumInfo info)
		{
			Dictionary<string, List<Tuple<string, string>>> lookup = new Dictionary<string, List<Tuple<string, string>>>(1);
			lookup.Add("Names:", new List<Tuple<string, string>>(info.Values.Count));
			foreach (EnumValueInfo item in info.Values)
				lookup["Names:"].Add(Tuple.Create<string, string>(item.Name, null));
			return lookup;
		}

		/// <summary>
		///   Gets a sub-header display string representing the specified visibility (Accessibility).
		/// </summary>
		/// <param name="access">Access string to parse.</param>
		/// <returns>Formatted string ready for the sub-header.</returns>
		/// <remarks>For example, a 'public partial' access string would be converted to '(public, partial)'.</remarks>
		private string GetAccessString(string access)
		{
			string[] splits = access.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder sb = new StringBuilder();
			sb.Append("(");
			for (int i = 0; i < splits.Length; i++)
			{
				sb.Append(splits[i]);
				if (i != splits.Length - 1)
					sb.Append(", ");
			}
			sb.Append(")");
			return sb.ToString();
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

		/// <summary>
		///   Writes the file to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the code to.</param>
		/// <exception cref="ArgumentNullException"><paramref name="wr"/> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		private void WriteFileSubHeader(StreamWriter wr)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			List<Dictionary<string, List<Tuple<string, string>>>> subInfo = null;
			Dictionary<string, string> info = null;

			EnumInfo enumInfo = Type as EnumInfo;
			if (enumInfo != null)
			{
				subInfo = new List<Dictionary<string, List<Tuple<string, string>>>>(1);
				info = new Dictionary<string, string>(1);

				// First check to see if the file is only composed of an enumerated type.
				info.Add(string.Format("{0}.{1} (enum)", NameSpace, enumInfo.Name), GetAccessString(enumInfo.Access));
				subInfo.Add(GenerateEnumSubInfo(enumInfo));
			}

			ClassInfo classInfo = Type as ClassInfo;
			if (classInfo != null)
			{
				GenerateClassInfo(NameSpace, classInfo, out info, out subInfo);
			}

			// Determine the first part maximum string length.
			int maxSizeTypes = 0;
			int maxSizeRegions = 0;
			int maxSizeNames = 0;
			foreach(string key in info.Keys)
				MaxStringLength(key, ref maxSizeTypes);
			foreach(Dictionary<string, List<Tuple<string, string>>> lookup in subInfo)
			{
				foreach(string key in lookup.Keys)
				{
					MaxStringLength(key, ref maxSizeRegions);
					for(int i = 0; i < lookup[key].Count; i++)
						MaxStringLength(lookup[key][i].Item1, ref maxSizeNames);
				}
			}
			int col1 = maxSizeRegions + 6; // 6 is for "//   " and space after.
			int col2 = maxSizeTypes + 4; // 4 is for "//." and space after.
			if(col2 < (col1 + maxSizeNames + 1))
				col2 = col1 + maxSizeNames + 1;

			int index = 0;
			foreach(string key in info.Keys)
			{
				// Write the class or enumeration of the file.
				StringBuilder sb = new StringBuilder();
				sb.Append("// ");
				sb.Append(key);
				for(int i = key.Length + 3; i < col2; i++)
					sb.Append(" ");
				sb.Append(info[key]);
				DocumentationHelper.WriteLine(wr, sb.ToString(), 0);

				Dictionary<string, List<Tuple<string, string>>> lookup = subInfo[index++];
				int subIndex = 0;
				foreach(string subKey in lookup.Keys)
				{
					DocumentationHelper.WriteLine(wr, string.Format("//   {0}", subKey), 0);
					for(int i = 0; i < lookup[subKey].Count; i++)
					{
						sb.Clear();
						sb.Append("//");
						for(int j = 2; j < col1; j++)
							sb.Append(" ");
						sb.Append(lookup[subKey][i].Item1);
						for(int j = col1 + lookup[subKey][i].Item1.Length; j < col2; j++)
							sb.Append(" ");
						sb.Append(lookup[subKey][i].Item2);
						DocumentationHelper.WriteLine(wr, sb.ToString(), 0);
					}
					subIndex++;

					if(subIndex != lookup.Keys.Count)
						DocumentationHelper.WriteLine(wr, "// ", 0);
				}
				DocumentationHelper.WriteFlowerLine(wr, 0);
			}
		}

		/// <summary>
		///   Writes the source code information in this object out to a file in the form of source code.
		/// </summary>
		/// <param name="rootFolder">Root location of the file. (The relative path will be added to this folder to generate the file.)</param>
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

			string fullFolderPath;
			if (RelativePath.Length > 0)
				fullFolderPath = Path.Combine(rootFolder, RelativePath);
			else
				fullFolderPath = rootFolder;
			string fullPath = Path.Combine(fullFolderPath, FileName);

			// Validate the class if it is a class.
			if (Type is ClassInfo)
				((ClassInfo)Type).Validate();

			// Generate any needed directories.
			DefaultValues.CreateFolderPath(fullFolderPath);
			using (StreamWriter wr = new StreamWriter(fullPath))
			{
				DocumentationHelper.WriteFileHeader(wr, FileName, Description);

				if (DefaultValues.IncludeSubHeader)
					WriteFileSubHeader(wr);

				// Add usings.
				if (Type.Usings.Length > 0)
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

		#endregion Methods
	}
}
