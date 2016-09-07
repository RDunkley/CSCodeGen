//********************************************************************************************************************************
// Filename:    ClassInfo.cs
// Owner:       Richard Dunkley
// Description: Contains the data components to generate a simplified C# class.
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
	///   Represents a C# class.
	/// </summary>
	public class ClassInfo : NamespaceTypeInfo
	{
		#region Properties

		/// <summary>
		///   Array of <see cref="ClassInfo"/> objects representing the sub-classes of the class. Can be empty.
		/// </summary>
		public ClassInfo[] ChildClasses { get; private set; }

		/// <summary>
		///   List of <see cref="ConstructorInfo"/> objects representing the constructors of the class. Can be null.
		/// </summary>
		public List<ConstructorInfo> Constructors { get; private set; }

		/// <summary>
		///   List of <see cref="EnumInfo"/> objects representing the enumerations of the class. Can be null.
		/// </summary>
		public List<EnumInfo> Enums { get; private set; }

		/// <summary>
		///   List of <see cref="FieldInfo"/> objects representing the fields of the class. Can be null.
		/// </summary>
		public List<FieldInfo> Fields { get; private set; }

		/// <summary>
		///   List of <see cref="MethodInfo"/> objects representing the methods of the class. Can be null.
		/// </summary>
		public List<MethodInfo> Methods { get; private set; }

		/// <summary>
		///   List of <see cref="PropertyInfo"/> objects representing the properties of the class. Can be null.
		/// </summary>
		public List<PropertyInfo> Properties { get; private set; }

		#endregion Properties

		#region Methods

		/// <summary>
		///   Instantiates a new <see cref="ClassInfo"/> object.
		/// </summary>
		/// <param name="access">The access description of the type.</param>
		/// <param name="name">Name of the type.</param>
		/// <param name="baseString">String containing the inheritance or interface support of the class.</param>
		/// <param name="summary">Summary description of the type.</param>
		/// <param name="remarks">Additional remarks to add to the documentation.</param>
		/// <exception cref="ArgumentNullException"><i>access</i>, <i>name</i>, or <i>summary</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>access</i>, <i>name</i>, or <i>summary</i> is an empty string.</exception>
		public ClassInfo(string access, string name, string baseString, string summary, string remarks = null) : base(access, name, summary, baseString, remarks)
		{
			Fields = new List<FieldInfo>();
			Properties = new List<PropertyInfo>();
			Methods = new List<MethodInfo>();
			Enums = new List<EnumInfo>();
			ChildClasses = new ClassInfo[0];
			Constructors = new List<ConstructorInfo>();

			// Add the default that Visual Studio adds.
			AddUsing("System");
			AddUsing("System.Collections.Generic");
			AddUsing("System.Linq");
			AddUsing("System.Text");
			AddUsing("System.Threading.Tasks");
		}

		/// <summary>
		///   Adds a child class to this class.
		/// </summary>
		/// <param name="child"><see cref="ClassInfo"/> object representing the child class.</param>
		public void AddChildClass(ClassInfo child)
		{
			List<ClassInfo> childList = new List<ClassInfo>(ChildClasses);
			if (childList.Contains(child))
				return;
			childList.Add(child);
			AddUsings(child.Usings);

			ChildClasses = childList.ToArray();
		}

		/// <summary>
		///   Checks the class information to ensure it is valid.
		/// </summary>
		public void Validate()
		{
			List<string> nameList = new List<string>();
			foreach (EnumInfo info in Enums)
			{
				// Check for null objects.
				if (info == null)
					throw new InvalidOperationException("A null EnumInfo was added to the Enums list");

				// Check for duplicate names.
				if (nameList.Contains(info.Name))
					throw new InvalidOperationException(string.Format("One or more EnumInfo objects contained the same name ({0})", info.Name));
				nameList.Add(info.Name);
			}

			nameList.Clear();
			foreach (FieldInfo info in Fields)
			{
				// Check for null objects.
				if (info == null)
					throw new InvalidOperationException("A null FieldInfo was added to the Fields list");

				// Check for duplicate names.
				if (nameList.Contains(info.Name))
					throw new InvalidOperationException(string.Format("One or more FieldInfo objects contained the same name ({0})", info.Name));
				nameList.Add(info.Name);
			}

			nameList.Clear();
			foreach (PropertyInfo info in Properties)
			{
				// Check for null objects.
				if (info == null)
					throw new InvalidOperationException("A null PropertyInfo was added to the Properties list");

				// Check for duplicate names.
				if (nameList.Contains(info.Name))
					throw new InvalidOperationException(string.Format("One or more PropertyInfo objects contained the same name ({0})", info.Name));
				nameList.Add(info.Name);
			}

			nameList.Clear();
			List<MethodInfo> methodList = new List<MethodInfo>();
			foreach (MethodInfo info in Methods)
			{
				// Check for null objects.
				if (info == null)
					throw new InvalidOperationException("A null MethodInfo was added to the Methods list");

				// Check for duplicate names.
				if (nameList.Contains(info.Name))
				{
					MethodInfo duplicate = methodList[nameList.IndexOf(info.Name)];

					// Names can be duplicate as long as signatures don't match.
					if (info.Parameters.Count == duplicate.Parameters.Count)
					{
						bool match = true;
						for (int i = 0; i < info.Parameters.Count; i++)
						{
							if (string.Compare(info.Parameters[i].Type, duplicate.Parameters[i].Type) != 0)
							{
								match = false;
								i = info.Parameters.Count;
							}
						}

						if (match)
							throw new InvalidOperationException(string.Format("One or more MethodInfo objects contained the same name ({0}) and duplicate parameter types.", info.Name));
					}
				}
				nameList.Add(info.Name);
				methodList.Add(info);
			}

			nameList.Clear();
			foreach (ClassInfo info in ChildClasses)
			{
				// Check for null objects.
				if (info == null)
					throw new InvalidOperationException("A null ClassInfo was added to the ChildClasses list");

				// Check for duplicate names.
				if (nameList.Contains(info.Name))
					throw new InvalidOperationException(string.Format("One or more ClassInfo objects contained the same name ({0})", info.Name));
				nameList.Add(info.Name);

				// Validate any child classes.
				info.Validate();
			}

			nameList.Clear();
			List<ConstructorInfo> constList = new List<ConstructorInfo>();
			foreach (ConstructorInfo info in Constructors)
			{
				// Check for null objects.
				if (info == null)
					throw new InvalidOperationException("A null ConstructorInfo was added to the Constructors list");

				// Check for duplicate names.
				if (nameList.Contains(info.Name))
				{
					ConstructorInfo duplicate = constList[nameList.IndexOf(info.Name)];

					// Names can be duplicate as long as signatures don't match.
					if (info.Parameters.Count == duplicate.Parameters.Count)
					{
						bool match = true;
						for (int i = 0; i < info.Parameters.Count; i++)
						{
							if (string.Compare(info.Parameters[i].Type, duplicate.Parameters[i].Type) != 0)
							{
								match = false;
								i = info.Parameters.Count;
							}
						}

						if (match)
							throw new InvalidOperationException(string.Format("One or more ConstructorInfo objects contained the same name ({0}) and duplicate parameter types.", info.Name));
					}
				}
				nameList.Add(info.Name);
				constList.Add(info);
			}
		}

		/// <summary>
		///   Writes the class to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> ojbect to write the code to.</param>
		/// <param name="indentOffset">Number of indentations to add before the code.</param>
		/// <exception cref="ArgumentNullException"><i>wr</i> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public void Write(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			DocumentationHelper.WriteComponentHeader(wr, Summary, indentOffset, Remarks);
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0} class {1}", Access, Name);
			if (Base != null && Base.Length > 0)
				sb.AppendFormat(" : {0}", Base);
			DocumentationHelper.WriteLine(wr, sb.ToString(), indentOffset);
			DocumentationHelper.WriteLine(wr, "{", indentOffset);
			indentOffset++;
			bool previous = false;
			if (ChildClasses.Length > 0)
			{
				DocumentationHelper.WriteRegionStart(wr, "Classes", indentOffset);

				List<ClassInfo> childList = new List<ClassInfo>(ChildClasses);
				childList.Sort();
				for (int i = 0; i < childList.Count; i++)
				{
					childList[i].Write(wr, indentOffset);
					if (i != childList.Count - 1)
						DocumentationHelper.WriteLine(wr);
				}
				DocumentationHelper.WriteRegionEnd(wr, "Classes", indentOffset);
				previous = true;
			}

			if (Enums.Count > 0)
			{
				if (previous)
					DocumentationHelper.WriteLine(wr);
				DocumentationHelper.WriteRegionStart(wr, "Enumerations", indentOffset);
				Enums.Sort();
				for (int i = 0; i < Enums.Count; i++)
				{
					Enums[i].Write(wr, indentOffset);
					if (i != Enums.Count - 1)
						DocumentationHelper.WriteLine(wr);
				}
				DocumentationHelper.WriteRegionEnd(wr, "Enumerations", indentOffset);
				previous = true;
			}

			if (Fields.Count > 0)
			{
				if (previous)
					DocumentationHelper.WriteLine(wr);
				DocumentationHelper.WriteRegionStart(wr, "Fields", indentOffset);
				Fields.Sort();
				for (int i = 0; i < Fields.Count; i++)
				{
					Fields[i].Write(wr, indentOffset);
					if (i != Fields.Count - 1)
						DocumentationHelper.WriteLine(wr);
				}
				DocumentationHelper.WriteRegionEnd(wr, "Fields", indentOffset);
				previous = true;
			}

			if (Properties.Count > 0)
			{
				if (previous)
					DocumentationHelper.WriteLine(wr);
				DocumentationHelper.WriteRegionStart(wr, "Properties", indentOffset);
				Properties.Sort();
				for (int i = 0; i < Properties.Count; i++)
				{
					Properties[i].Write(wr, indentOffset);
					if (i != Properties.Count - 1)
						DocumentationHelper.WriteLine(wr);
				}
				DocumentationHelper.WriteRegionEnd(wr, "Properties", indentOffset);
				previous = true;
			}

			if (Methods.Count > 0 || Constructors.Count > 0)
			{
				if (previous)
					DocumentationHelper.WriteLine(wr);
				DocumentationHelper.WriteRegionStart(wr, "Methods", indentOffset);
				int index = 0;
				if (Constructors != null)
					index += Constructors.Count;
				if (Methods != null)
					index += Methods.Count;

				for (int i = 0; i < Constructors.Count; i++)
				{
					Constructors[i].Write(wr, indentOffset);
					if (index != 1)
						DocumentationHelper.WriteLine(wr);
					index--;
				}

				Methods.Sort();
				for (int i = 0; i < Methods.Count; i++)
				{
					Methods[i].Write(wr, indentOffset);
					if (index != 1)
						DocumentationHelper.WriteLine(wr);
					index--;
				}

				DocumentationHelper.WriteRegionEnd(wr, "Methods", indentOffset);
				previous = true;
			}

			indentOffset--;
			DocumentationHelper.WriteLine(wr, "}", indentOffset);
		}

		#endregion Methods
	}
}
