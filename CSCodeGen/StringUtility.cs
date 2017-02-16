//********************************************************************************************************************************
// Filename:    StringUtility.cs
// Owner:       Richard Dunkley
// Description: Static class containing various helper methods for manipulating and generating strings specific to C# code.
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
using System.Globalization;
using System.Text;
using System.IO;

namespace CSCodeGen
{
	/// <summary>
	///   Contains various static methods to aid in string manipulation for code-generation.
	/// </summary>
	public static class StringUtility
	{
		#region Methods

		/// <summary>
		///   Converts an absolute path to a relative one.
		/// </summary>
		/// <param name="absolutePath">Absolute path to be converted.</param>
		/// <param name="reference">The reference folder to generate the relative path from.</param>
		/// <returns>Relative path from the reference.</returns>
		public static string ConvertAbsolutePathToRelative(string absolutePath, string reference)
		{
			if(reference[reference.Length - 1] != '\\')
				reference = string.Format("{0}\\", reference);
			Uri pathUri = new Uri(absolutePath);
			Uri referenceUri = new Uri(reference);
			return referenceUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar);
		}

		/// <summary>
		///   Determines if the name is a valid C# identifier.
		/// </summary>
		/// <param name="name">Name to be checked.</param>
		/// <returns>True if it is valid, false otherwise.</returns>
		public static bool IsValidCSharpIdentifier(string name)
		{
			if (name == null)
				return false;
			if (name.Length == 0)
				return false;

			// Check the first letter.
			if (!IsValidCSharpIdentifierFirstLetter(name[0]))
				return false;

			// Check the remaining letters.
			for (int i = 1; i < name.Length; i++)
			{
				if (!IsValidCSharpIdentifierNonFirstLetter(name[i]))
					return false;
			}

			// Validate that it isn't a keyword.
			List<string> list = GetKeywordList();
			if (list.Contains(name))
				return false;
			return true;
		}

		/// <summary>
		///   Determines if the letter is a valid C# identifier first letter.
		/// </summary>
		/// <param name="letter">Letter to be checked.</param>
		/// <returns>True if the letter is valid, false otherwise.</returns>
		public static bool IsValidCSharpIdentifierFirstLetter(char letter)
		{
			UnicodeCategory cat = char.GetUnicodeCategory(letter);
			if (cat != UnicodeCategory.UppercaseLetter && cat != UnicodeCategory.LowercaseLetter && cat != UnicodeCategory.TitlecaseLetter
				&& cat != UnicodeCategory.ModifierLetter && cat != UnicodeCategory.OtherLetter)
				return false;
			return true;
		}

		/// <summary>
		///   Determines if the letter is a valid C# identifier non-first letter.
		/// </summary>
		/// <param name="letter">Letter to be checked.</param>
		/// <returns>True if the letter is valid, false otherwise.</returns>
		public static bool IsValidCSharpIdentifierNonFirstLetter(char letter)
		{
			UnicodeCategory cat = char.GetUnicodeCategory(letter);
			if (cat != UnicodeCategory.UppercaseLetter && cat != UnicodeCategory.LowercaseLetter && cat != UnicodeCategory.TitlecaseLetter
				&& cat != UnicodeCategory.ModifierLetter && cat != UnicodeCategory.OtherLetter && cat != UnicodeCategory.LetterNumber
				&& cat != UnicodeCategory.NonSpacingMark && cat != UnicodeCategory.DecimalDigitNumber && cat != UnicodeCategory.SpacingCombiningMark
				&& cat != UnicodeCategory.ConnectorPunctuation && cat != UnicodeCategory.Format)
				return false;
			return true;
		}

		/// <summary>
		///   Returns a list of C# keywords.
		/// </summary>
		/// <returns>List containing all the C# keywords including contextual ones.</returns>
		public static List<string> GetKeywordList()
		{
			List<string> keyWordList = new List<string>();
			keyWordList.Add("abstract");
			keyWordList.Add("as");
			keyWordList.Add("base");
			keyWordList.Add("bool");
			keyWordList.Add("break");
			keyWordList.Add("byte");
			keyWordList.Add("case");
			keyWordList.Add("catch");
			keyWordList.Add("char");
			keyWordList.Add("checked");
			keyWordList.Add("class");
			keyWordList.Add("const");
			keyWordList.Add("continue");
			keyWordList.Add("decimal");
			keyWordList.Add("default");
			keyWordList.Add("delegate");
			keyWordList.Add("do");
			keyWordList.Add("double");
			keyWordList.Add("else");
			keyWordList.Add("enum");
			keyWordList.Add("event");
			keyWordList.Add("explicit");
			keyWordList.Add("extern");
			keyWordList.Add("false");
			keyWordList.Add("finally");
			keyWordList.Add("fixed");
			keyWordList.Add("float");
			keyWordList.Add("for");
			keyWordList.Add("foreach");
			keyWordList.Add("goto");
			keyWordList.Add("if");
			keyWordList.Add("implicit");
			keyWordList.Add("in");
			keyWordList.Add("int");
			keyWordList.Add("interface");
			keyWordList.Add("internal");
			keyWordList.Add("is");
			keyWordList.Add("lock");
			keyWordList.Add("long");
			keyWordList.Add("namespace");
			keyWordList.Add("new");
			keyWordList.Add("null");
			keyWordList.Add("object");
			keyWordList.Add("operator");
			keyWordList.Add("out");
			keyWordList.Add("override");
			keyWordList.Add("params");
			keyWordList.Add("private");
			keyWordList.Add("protected");
			keyWordList.Add("public");
			keyWordList.Add("readonly");
			keyWordList.Add("ref");
			keyWordList.Add("return");
			keyWordList.Add("sbyte");
			keyWordList.Add("sealed");
			keyWordList.Add("short");
			keyWordList.Add("sizeof");
			keyWordList.Add("stackalloc");
			keyWordList.Add("static");
			keyWordList.Add("string");
			keyWordList.Add("struct");
			keyWordList.Add("switch");
			keyWordList.Add("this");
			keyWordList.Add("throw");
			keyWordList.Add("true");
			keyWordList.Add("try");
			keyWordList.Add("typeof");
			keyWordList.Add("uint");
			keyWordList.Add("ulong");
			keyWordList.Add("unchecked");
			keyWordList.Add("unsafe");
			keyWordList.Add("ushort");
			keyWordList.Add("using");
			keyWordList.Add("virtual");
			keyWordList.Add("void");
			keyWordList.Add("volatile");
			keyWordList.Add("while");

			// Contextual keywords.
			keyWordList.Add("add");
			keyWordList.Add("alias");
			keyWordList.Add("ascending");
			keyWordList.Add("async");
			keyWordList.Add("await");
			keyWordList.Add("descending");
			keyWordList.Add("dynamic");
			keyWordList.Add("from");
			keyWordList.Add("get");
			keyWordList.Add("global");
			keyWordList.Add("group");
			keyWordList.Add("into");
			keyWordList.Add("join");
			keyWordList.Add("let");
			keyWordList.Add("orderby");
			keyWordList.Add("partial");
			keyWordList.Add("remove");
			keyWordList.Add("select");
			keyWordList.Add("set");
			keyWordList.Add("value");
			keyWordList.Add("var");
			keyWordList.Add("where");
			keyWordList.Add("yield");
			return keyWordList;
		}

		/// <summary>
		///   Gets the lower camel case of the provided name.
		/// </summary>
		/// <param name="name">Name to convert to lower camel case.</param>
		/// <param name="renameKeyWords">True if the method should rename C# keywords, false to allow them to be returned.</param>
		/// <returns>Name in upper camel case.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
		/// <remarks>
		///   This method will also remove '_' characters and treat them as word breaks. For example, this_word would go to 
		///   thisWord. Any invalid C# identifier characters would be omitted with the next character capitalized. so
		///   'some word' would end up as 'someWord' or 'test@mail.com' would be 'testMailCom'.
		/// </remarks>
		public static string GetLowerCamelCase(string name, bool renameKeyWords)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string.");

			StringBuilder builder = new StringBuilder();
			bool capNext = false;
			bool firstLetter = true;
			for (int i = 0; i < name.Length; i++)
			{
				if (firstLetter)
				{
					if (IsValidCSharpIdentifierFirstLetter(name[i]))
					{
						builder.Append(char.ToLower(name[i]));
						firstLetter = false; // No longer the first character.
					}
					// Skip if not a valid character
				}
				else
				{
					if (IsValidCSharpIdentifierNonFirstLetter(name[i]))
					{
						if (name[i] == '_')
						{
							// Skip if underscore and capitalize the next character.
							capNext = true;
						}
						else
						{
							if (capNext)
							{
								builder.Append(char.ToUpper(name[i]));
								capNext = false;
							}
							else
							{
								builder.Append(name[i]);
							}
						}
					}
					else
					{
						// Skip if not a valid character and capitalize next character.
						capNext = true;
					}
				}
			}

			string value = builder.ToString();
			if (renameKeyWords)
			{
				if (GetKeywordList().Contains(value))
					value = string.Format("{0}Value", value);
			}

			return value;
		}

		/// <summary>
		///   Gets the upper camel case of the provided name.
		/// </summary>
		/// <param name="name">Name to convert to upper camel case.</param>
		/// <returns>Name in upper camel case.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference.</exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
		/// <remarks>
		///   This method will also remove '_' characters and treat them as word breaks. For example, 'this_word' would go to 
		///   'ThisWord'. Any invalid C# identifier characters would be omitted with the next character capitalized. so
		///   'some word' would end up as 'SomeWord' or 'test@mail.com' would be 'TestMailCom'.
		/// </remarks>
		public static string GetUpperCamelCase(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.Length == 0)
				throw new ArgumentException("name is an empty string.");

			StringBuilder builder = new StringBuilder();
			bool capNext = false;
			bool firstLetter = true;
			for (int i = 0; i < name.Length; i++)
			{
				if (firstLetter)
				{
					if (IsValidCSharpIdentifierFirstLetter(name[i]))
					{
						builder.Append(char.ToUpper(name[i]));
						firstLetter = false; // No longer the first character.
					}
					// Skip if not a valid character
				}
				else
				{
					if (IsValidCSharpIdentifierNonFirstLetter(name[i]))
					{
						if (name[i] == '_')
						{
							// Skip if underscore and capitalize the next character.
							capNext = true;
						}
						else
						{
							if (capNext)
							{
								builder.Append(char.ToUpper(name[i]));
								capNext = false;
							}
							else
							{
								builder.Append(name[i]);
							}
						}
					}
					else
					{
						// Skip if not a valid character and capitalize next character.
						capNext = true;
					}
				}
			}

			return builder.ToString();
		}

		#endregion Methods
	}
}