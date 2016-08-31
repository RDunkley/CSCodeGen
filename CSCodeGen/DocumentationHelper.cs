//********************************************************************************************************************************
// Filename:    DocumentationHelper.cs
// Owner:       Richard Dunkley
// Description: Static class used to aid in the generation of documentation for C# files.
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
	///   Utility class to aid in documentation generation.
	/// </summary>
	public static class DocumentationHelper
	{
		/// <summary>
		///   Writes a flower box text with the specified indentation number to the <see cref="StreamWriter"/>.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="indentOffset">Number of indentations to add to the text.</param>
		/// <returns>True if a line was written, false otherwise. A line is not written if the <see cref="DefaultValues.FlowerBoxCharacter"/> is null.</returns>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static bool WriteFlowerLine(StreamWriter wr, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			// Don't do anything if the flower box character is not defined.
			if (DefaultValues.FlowerBoxCharacter == null)
				return false;

			wr.WriteLine(GenerateFullFlowerLine(indentOffset));
			return true;
		}

		/// <summary>
		///   Generates a flower box text, based on the given number of indentations.
		/// </summary>
		/// <param name="indentOffset">Number of indentations before the flower box text begins.</param>
		/// <returns>String containing the entire text (whitespace and flower box).</returns>
		private static string GenerateFullFlowerLine(int indentOffset)
		{
			int size;
			string whitespace = GenerateLeadingWhitespace(indentOffset, out size);
			string line = GenerateRemainingFlowerLine(size);
			return string.Format("{0}{1}", whitespace, line);
		}

		/// <summary>
		///   Generates the a flower text to consume the rest of the text.
		/// </summary>
		/// <param name="numCharactersConsumed">Number of characters already in the text.</param>
		/// <returns>String containing the flower text.</returns>
		private static string GenerateRemainingFlowerLine(int numCharactersConsumed)
		{
			int numFlowers = DefaultValues.NumCharactersPerLine - 2 - numCharactersConsumed;
			if (numFlowers <= 0)
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			sb.Append("//");
			for (int i = 0; i < numFlowers; i++)
				sb.Append(DefaultValues.FlowerBoxCharacter.Value);
			return sb.ToString();
		}

		/// <summary>
		///   Generates the leading whitespace based on the specified number of indents.
		/// </summary>
		/// <param name="indents">Number of indentations to include in the whitespace. These typically represent the number of tabs.</param>
		/// <returns>String representing the whitespace.</returns>
		/// <remarks>If using spaces instead of tabs, the <see cref="DefaultValues.TabSize"/> value represents the number of spaces per indentation.</remarks>
		public static string GenerateLeadingWhitespace(int indents)
		{
			int num;
			return GenerateLeadingWhitespace(indents, out num);
		}

		/// <summary>
		///   Generates the leading whitespace based on the specified number of indents.
		/// </summary>
		/// <param name="indents">Number of indentations to include in the whitespace. These typically represent the number of tabs.</param>
		/// <param name="numCharacters">Number of characters the whitespace contains.</param>
		/// <returns>String representing the whitespace.</returns>
		/// <remarks>If using spaces instead of tabs, the <see cref="DefaultValues.TabSize"/> value represents the number of spaces per indentation.</remarks>
		public static string GenerateLeadingWhitespace(int indents, out int numCharacters)
		{
			// Generate the leading whitespace for each text.
			StringBuilder sb = new StringBuilder();
			numCharacters = 0;
			for (int i = 0; i < indents; i++)
			{
				numCharacters += DefaultValues.TabSize;
				if (DefaultValues.UseTabs)
				{
					sb.Append("	");
				}
				else
				{
					for (int j = 0; j < DefaultValues.TabSize; j++)
						sb.Append(" ");
				}
			}
			return sb.ToString();
		}

		/// <summary>
		///   Writes the region start text.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="nameOfRegion">Name of the region.</param>
		/// <param name="indentOffset">Number of indentations to include before the text.</param>
		/// <remarks>Also adds a blank text after the text.</remarks>
		/// <exception cref="ArgumentNullException"><i>wr</i> or <i>nameOfRegion</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>nameOfRegion</i> is an empty string.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WriteRegionStart(StreamWriter wr, string nameOfRegion, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (nameOfRegion == null)
				throw new ArgumentNullException("nameOfRegion");
			if (nameOfRegion.Length == 0)
				throw new ArgumentException("nameOfRegion is an empty string");

			WriteLine(wr, string.Format("#region {0}", nameOfRegion), indentOffset);
			wr.WriteLine();
		}

		/// <summary>
		///   Writes the region end text.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="nameOfRegion">Name of the region.</param>
		/// <param name="indentOffset">Number of indentations to include before the text.</param>
		/// <remarks>Also adds a blank text before the text.</remarks>
		/// <exception cref="ArgumentNullException"><i>wr</i> or <i>nameOfRegion</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>nameOfRegion</i> is an empty string.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WriteRegionEnd(StreamWriter wr, string nameOfRegion, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (nameOfRegion == null)
				throw new ArgumentNullException("nameOfRegion");
			if (nameOfRegion.Length == 0)
				throw new ArgumentException("nameOfRegion is an empty string");

			wr.WriteLine();
			WriteLine(wr, string.Format("#endregion {0}", nameOfRegion), indentOffset);
		}

		/// <summary>
		///   Writes the text to the <see cref="StreamWriter"/> object while adding indentation whitespace.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="line">Line to be written.</param>
		/// <param name="indentOffset">Number of indentations to include before the text.</param>
		/// <returns>Number of characters written on the text.</returns>
		/// <exception cref="ArgumentNullException"><i>wr</i> or <i>text</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>text</i> is an empty sting.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static int WriteLine(StreamWriter wr, string line, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (line == null)
				throw new ArgumentNullException("text");
			line = line.TrimEnd();
			if (line.Length == 0)
			{
				WriteLine(wr);
				return 0;
			}

			int numWhiteSpace;
			string whiteSpace = GenerateLeadingWhitespace(indentOffset, out numWhiteSpace);

			if(numWhiteSpace + line.Length > DefaultValues.NumCharactersPerLine)
			{
				// Line is too long. If there is a comment we can split it up.
				// TODO: Add the ability to split the text up.
			}
			wr.WriteLine(string.Format("{0}{1}", whiteSpace, line));
			return numWhiteSpace + line.Length;
		}

		/// <summary>
		///   Writes a blank text (without whitespace) to the <see cref="StreamWriter"/> object.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <exception cref="ArgumentNullException"><i>wr</i> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WriteLine(StreamWriter wr)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			wr.WriteLine();
		}

		/// <summary>
		///   Writes text to the <see cref="StreamWriter"/> object. Does not add any whitespace.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="line">Text to be written.</param>
		/// <returns>Number of characters that were written.</returns>
		/// <exception cref="ArgumentNullException"><i>wr</i> or <i>text</i> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static int Write(StreamWriter wr, string line)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (line == null)
				throw new ArgumentNullException("text");
			wr.Write(line);
			return line.Length;
		}

		/// <summary>
		///   Writes text to the <see cref="StreamWriter"/> object (including whitespace).
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="text">Text to be written.</param>
		/// <param name="indentOffset">Number of indentations to include before the text.</param>
		/// <returns>Number of characters that were written.</returns>
		/// <exception cref="ArgumentNullException"><i>wr</i> or <i>text</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>text</i> is an empty sting.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static int Write(StreamWriter wr, string text, int indentOffset)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (text == null)
				throw new ArgumentNullException("text");
			if (text.Length == 0)
				throw new ArgumentException("text is an empty string");

			int numWhiteSpace;
			string whiteSpace = GenerateLeadingWhitespace(indentOffset, out numWhiteSpace);

			if (numWhiteSpace + text.Length > DefaultValues.NumCharactersPerLine)
			{
				// Line is too long. If there is a comment we can split it up.
				// TODO: Add the ability to split the text up.
			}
			wr.Write(string.Format("{0}{1}", whiteSpace, text));
			return numWhiteSpace + text.Length;
		}

		/// <summary>
		///   Converts a template string to an actual string (template items implemented).
		/// </summary>
		/// <param name="template">Template string.</param>
		/// <param name="fileName">Name of the file that will be containing the template text. Can be null.</param>
		/// <param name="description">Description of the file that will be containing the template text. Can be null.</param>
		/// <param name="isCopyRight">True if the line is part of the copyright template (prevents endless cycles).</param>
		/// <returns>String with the template items replaced.</returns>
		/// <exception cref="ArgumentNullException">template is a null reference.</exception>
		public static string ConvertTemplateLineToActual(string template, string fileName, string description, bool? isCopyRight = null, bool? isLicense = null)
		{
			if (template == null)
				throw new ArgumentNullException("template");

			if (template.Length == 0)
				return template;

			if (fileName == null)
				fileName = string.Empty;
			if (description == null)
				description = string.Empty;

			// Generic template items.
			if (template.Contains("<%year%>"))
				template = template.Replace("<%year%>", DateTime.Now.Year.ToString());
			if (template.Contains("<%date%>"))
				template = template.Replace("<%date%>", DateTime.Now.ToShortDateString());
			if (template.Contains("<%time%>"))
				template = template.Replace("<%time%>", DateTime.Now.ToLongTimeString());
			if (template.Contains("<%datetime%>"))
				template = template.Replace("<%datetime%>", DateTime.Now.ToString());

			// Global items.
			if (template.Contains("<%developer%>"))
				template = template.Replace("<%developer%>", DefaultValues.Developer);
			if (template.Contains("<%company%>"))
				template = template.Replace("<%company%>", DefaultValues.CompanyName);
			if (template.Contains("<%appversion%>"))
				template = template.Replace("<%appversion%>", DefaultValues.ApplicationVersion);
			if (template.Contains("<%appname%>"))
				template = template.Replace("<%appname%>", DefaultValues.ApplicationName);
			if (template.Contains("<%libraryversion%>"))
				template = template.Replace("<%libraryversion%>", DefaultValues.LibraryVersion);
			if (template.Contains("<%libraryname%>"))
				template = template.Replace("<%libraryname%>", DefaultValues.LibraryName);
			if(template.Contains("<%copyright%>"))
			{
				if (isCopyRight.HasValue && isCopyRight.Value)
					template = template.Replace("<%copyright%>", string.Empty); // Avoid an infinite loop.
				else
				{
					StringBuilder sb = new StringBuilder();
					foreach (string line in DefaultValues.CopyrightTemplate)
						sb.AppendLine(ConvertTemplateLineToActual(line, fileName, description, true, isLicense));
					template = template.Replace("<%copyright%>", sb.ToString());
				}
			}
			if (template.Contains("<%license%>"))
			{
				if (isLicense.HasValue && isLicense.Value)
					template = template.Replace("<%license%>", string.Empty); // Avoid an infinite loop.
				else
				{
					StringBuilder sb = new StringBuilder();
					foreach (string line in DefaultValues.LicenseTemplate)
						sb.AppendLine(ConvertTemplateLineToActual(line, fileName, description, isCopyRight, true));
					template = template.Replace("<%license%>", sb.ToString());
				}
			}


			// File Specific items.
			if (template.Contains("<%filename%>"))
				template = template.Replace("<%filename%>", fileName);
			if (template.Contains("<%description%>"))
				template = template.Replace("<%description%>", description);

			return template;
		}

		/// <summary>
		///   Generates a file header for a C# file.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the file header to.</param>
		/// <param name="fileName">Name of thee C# file to generate the header for.</param>
		/// <param name="description">Description of the file. Can be null.</param>
		/// <exception cref="ArgumentNullException"><i>wr</i> or <i>fileName</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>fileName</i> is an empty string.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WriteFileHeader(StreamWriter wr, string fileName, string description = null)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (fileName.Length == 0)
				throw new ArgumentException("fileName is an empty string");

			// Write the file information section.
			if (DefaultValues.FileInfoTemplate != null && DefaultValues.FileInfoTemplate.Length > 0)
			{
				foreach (string templateLine in DefaultValues.FileInfoTemplate)
					WriteLine(wr, string.Format("// {0}", ConvertTemplateLineToActual(templateLine, fileName, description)), 0);
			}

			// Write the copyright statement section.
			if (DefaultValues.CopyrightTemplate != null && DefaultValues.CopyrightTemplate.Length > 0)
			{
				foreach (string line in DefaultValues.CopyrightTemplate)
					WriteLine(wr, string.Format("// {0}", ConvertTemplateLineToActual(line, fileName, description)), 0);
			}

			if(DefaultValues.LicenseTemplate != null && DefaultValues.LicenseTemplate.Length > 0)
			{
				foreach (string line in DefaultValues.LicenseTemplate)
					WriteLine(wr, string.Format("// {0}", ConvertTemplateLineToActual(line, fileName, description)), 0);
			}
		}

		/// <summary>
		///   Splits the comment text on a space between words, or splits a word and hyphens it.
		/// </summary>
		/// <param name="text">Text to evaluate for splitting.</param>
		/// <param name="lineOffset">Offset in the line where the text will start.</param>
		/// <param name="lineText">Text to be written to the line. Can be empty if there is no room to write any text.</param>
		/// <param name="remainingText">Text remaining that could not fit on the line. Can be null if no text is remaining.</param>
		private static void SplitOnEndOfLine(string text, int lineOffset, out string lineText, out string remainingText)
		{
			// remove any leading or trailing whitespace.
			text = text.Trim();

			int remainingSpace = DefaultValues.NumCharactersPerLine - lineOffset - 1;
			if(text.Length <= remainingSpace)
			{
				// The text will fit in the space.
				lineText = text;
				remainingText = null;
				return;
			}

			if(remainingSpace < 1)
			{
				// Not enought space to add any text.
				lineText = string.Empty;
				remainingText = text;
				return;
			}

			if(remainingSpace == 1)
			{
				// Might not be enough space if we need a hyphen.
				if (text[1] == ' ')
				{
					// Add single character to text.
					lineText = text[0].ToString();
					remainingText = text.Substring(2);
				}
				else
				{
					// Not enough space to add one character and hyphen.
					lineText = string.Empty;
					remainingText = text;
				}
			}

			int splitIndex = text.LastIndexOf(' ', remainingSpace-1, remainingSpace);
			if(splitIndex == -1)
			{
				// Break the file on the end of the text and place a hyphen.
				lineText = string.Format("{0}-", text.Substring(0, remainingSpace - 1)); // Should fill remaining space.
				remainingText = text.Substring(remainingSpace - 1);
			}
			else
			{
				lineText = text.Substring(0, splitIndex);
				remainingText = text.Substring(splitIndex + 1);
			}
			return;
		}

		/// <summary>
		///   Adds a comment string to the end of the text.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the text to.</param>
		/// <param name="comment">Comment string to be added.</param>
		/// <param name="previousTextSize">Size of any previous text written to the text (beyond the whitespace).</param>
		/// <param name="indentOffset">Number of indentations in the code where the text begins (applies to both the previous written text and newly created lines).</param>
		/// <param name="newLineText">
		///   If the comment is too long then this string is used to specify what text to place before the comment starts up 
		///   again. (Should not include indentation.)
		/// </param>
		/// <remarks>
		///   If the text is longer than <see cref="DefaultValues.NumCharactersPerLine"/> then it will be split to multiple 
		///   lines. If a space could not be found on the string to break it up on then the string will be broken on the 
		///   boundary and a hyphen added. Identations are added to the subsequent lines of the comment prior to the 
		///   <i>newLineText</i> being added. If the indent is so much that the comment cannot be added at all it is added
		///    to the initial text and <see cref="DefaultValues.NumCharactersPerLine"/> is ignored.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><i>wr</i> is a null reference.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		private static void AddCommentToEndOfLine(StreamWriter wr, string comment, int previousTextSize, int indentOffset, string newLineText = null)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");

			if (comment == null || comment.Length == 0)
				return;

			if (newLineText == null)
				newLineText = string.Empty;

			if (previousTextSize < 0)
				previousTextSize = 0;

			int offset = indentOffset * DefaultValues.TabSize + previousTextSize;
			bool cantfitText = false;
			while(comment != null)
			{
				string lineText, remainingText;
				SplitOnEndOfLine(comment, offset, out lineText, out remainingText);
				if (lineText.Length == 0)
				{
					// Can't fit any text on this text.
					if(cantfitText)
					{
						// This is the second time we can't fit text, which means we can't write any of the text so just write the whole text.
						wr.WriteLine(comment);
						return;
					}
					else
					{
						cantfitText = true;
					}
				}
				else
				{
					// Write the rest of this text and create the new text.
					wr.WriteLine(lineText);
					if (remainingText != null)
					{
						int wsLength;
						wr.Write(GenerateLeadingWhitespace(indentOffset, out wsLength));
						wr.Write(newLineText);
						offset = wsLength + newLineText.Length;
					}
				}
				comment = remainingText;
			}
		}

		/// <summary>
		///   Creates a documentation XML element.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the documentation to.</param>
		/// <param name="tagName">Name of the XML element tag.</param>
		/// <param name="text">Text contained inside the XML element.</param>
		/// <param name="indentOffset">Number of indentations, before the code/comments start.</param>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		private static void WriteGeneralDocumentationElement(StreamWriter wr, string tagName, string text, int indentOffset)
		{
			text = text.Trim();
			int wsLength;
			string ws = GenerateLeadingWhitespace(indentOffset, out wsLength);
			string newLine = "///   ";

			// The 9 below is for the '/// <></>' characters.
			if (text.Length + wsLength + (2 * tagName.Length) + 9 > DefaultValues.NumCharactersPerLine)
			{
				// Place the tags on a separate text.
				wr.WriteLine(string.Format("{0}/// <{1}>", ws, tagName));
				wr.Write(ws);
				wr.Write(newLine);
				AddCommentToEndOfLine(wr, text, newLine.Length, indentOffset, newLine);
				wr.WriteLine(string.Format("{0}/// </{1}>", ws, tagName));
			}
			else
			{
				// Place on the same text.
				wr.WriteLine(string.Format("{0}/// <{1}>{2}</{1}>", ws, tagName, text));
			}
		}

		/// <summary>
		///   Creates a parameter documentation XML element.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the documentation to.</param>
		/// <param name="paramName">Name of the parameter.</param>
		/// <param name="description">Description of the parameter.</param>
		/// <param name="indentOffset">Number of indentations, before the code/comments start.</param>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		private static void WriteParamDocumentationElement(StreamWriter wr, string paramName, string description, int indentOffset)
		{
			paramName = paramName.Trim();
			description = description.Trim();
			int wsLength;
			string ws = GenerateLeadingWhitespace(indentOffset, out wsLength);
			string newLine = "///   ";

			// 27 below is for the '/// <param name=""></param>'.
			if (description.Length + wsLength + 27 + paramName.Length > DefaultValues.NumCharactersPerLine)
			{
				// Place the tags on a separate text.
				wr.WriteLine(string.Format("{0}/// <param name=\"{1}\">", ws, paramName));
				wr.Write(ws);
				wr.Write(newLine);
				AddCommentToEndOfLine(wr, description, newLine.Length, indentOffset, newLine);
				wr.WriteLine(string.Format("{0}/// </param>", ws));
			}
			else
			{
				// Place on the same text.
				wr.WriteLine(string.Format("{0}/// <param name=\"{1}\">{2}</param>", ws, paramName, description));
			}
		}

		/// <summary>
		///   Creates a exception documentation XML element.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the documentation to.</param>
		/// <param name="exceptionName">Name of the exception.</param>
		/// <param name="description">Description of when the exception will occur.</param>
		/// <exception cref="ArgumentException"></exception>
		/// <param name="indentOffset">Number of indentations, before the code/comments start.</param>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		private static void WriteExceptionDocumentationElement(StreamWriter wr, string exceptionName, string description, int indentOffset)
		{
			exceptionName = exceptionName.Trim();
			description = description.Trim();
			int wsLength;
			string ws = GenerateLeadingWhitespace(indentOffset, out wsLength);
			string newLine = "///   ";

			// The 35 below is for the '/// <exception cref=""></exception>'
			if (description.Length + wsLength + 35 + exceptionName.Length > DefaultValues.NumCharactersPerLine)
			{
				// Place the tags on a separate text.
				wr.WriteLine(string.Format("{0}/// <exception cref=\"{1}\">", ws, exceptionName));
				wr.Write(ws);
				wr.Write(newLine);
				int size = wsLength + 6; // 6 is for '///   '
				AddCommentToEndOfLine(wr, description, newLine.Length, indentOffset, newLine);
				wr.WriteLine(string.Format("{0}/// </exception>", ws));
			}
			else
			{
				// Place on the same text.
				wr.WriteLine(string.Format("{0}/// <exception cref=\"{1}\">{2}</exception>", ws, exceptionName, description));
			}
		}

		/// <summary>
		///   Write the component header.
		/// </summary>
		/// <param name="wr"><see cref="StreamWriter"/> object to write the header to.</param>
		/// <param name="summary">Summary of the component.</param>
		/// <param name="indentOffset">Number of indents to add before any documentation begins.</param>
		/// <param name="returns">Description of the return value. Can be null or empty if it doesn't apply to the component.</param>
		/// <param name="remarks">Additional remarks for the component. Can be null or empty if a 'remarks' section should not be added.</param>
		/// <param name="parameters">Parameters of the component. Can be null or empty if it doesn't apply to the component.</param>
		/// <param name="exceptions">Exceptions thrown by the component. Can be null or empty if it doesn't apply to the component.</param>
		/// <exception cref="ArgumentNullException"><i>wr</i>, or <i>summary</i> is a null reference.</exception>
		/// <exception cref="ArgumentException"><i>summary</i> is an empty string.</exception>
		/// <exception cref="IOException">An error occurred while writing to the <see cref="StreamWriter"/> object.</exception>
		public static void WriteComponentHeader(StreamWriter wr, string summary, int indentOffset, string remarks = null, string returns = null, ParameterInfo[] parameters = null, ExceptionInfo[] exceptions = null)
		{
			if (wr == null)
				throw new ArgumentNullException("wr");
			if (summary == null)
				throw new ArgumentNullException("summary");
			summary = summary.Trim();
			if (summary.Length == 0)
				throw new ArgumentException("summary is an empty string");
			if (indentOffset < 0)
				indentOffset = 0;

			WriteFlowerLine(wr, indentOffset);
			WriteGeneralDocumentationElement(wr, "summary", summary, indentOffset);

			if (parameters != null && parameters.Length > 0)
			{
				WriteLine(wr, "///", indentOffset);
				foreach (ParameterInfo param in parameters)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(param.Description);
					if (param.CanBeNull.HasValue && param.CanBeNull.Value)
						sb.Append(" Can be null.");
					if (param.CanBeEmpty.HasValue && param.CanBeEmpty.Value)
						sb.Append(" Can be empty.");
					WriteParamDocumentationElement(wr, param.Name, sb.ToString(), indentOffset);
				}
			}

			if (returns != null && returns.Length > 0)
			{
				WriteLine(wr, "///", indentOffset);
				WriteGeneralDocumentationElement(wr, "returns", returns, indentOffset);
			}

			if (remarks != null && remarks.Length > 0)
			{
				WriteLine(wr, "///", indentOffset);
				WriteGeneralDocumentationElement(wr, "remarks", remarks, indentOffset);
			}

			// Determine what exceptions can be thrown from parameters.
			List<ParameterInfo> nullList = new List<ParameterInfo>();
			List<ParameterInfo> emptyList = new List<ParameterInfo>();
			if (parameters != null && parameters.Length > 0)
			{
				foreach (ParameterInfo param in parameters)
				{
					if (param.CanBeNull.HasValue && !param.CanBeNull.Value)
						nullList.Add(param);
					if (param.CanBeEmpty.HasValue && !param.CanBeEmpty.Value)
						emptyList.Add(param);
				}
			}

			if (nullList.Count > 0 || emptyList.Count > 0 || (exceptions != null && exceptions.Length > 0))
				WriteLine(wr, "///", indentOffset);

			if (nullList.Count > 0)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < nullList.Count; i++)
				{
					sb.Append(string.Format("<i>{0}</i>", nullList[i].Name));
					if (i == nullList.Count - 2)
						sb.Append(", or "); // Second to last.
					else if (i == nullList.Count - 1)
						sb.Append(" "); // last.
					else
						sb.Append(", ");
				}
				sb.Append("is a null reference.");
				WriteExceptionDocumentationElement(wr, "ArgumentNullException", sb.ToString(), indentOffset);
			}

			if (emptyList.Count > 0)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < emptyList.Count; i++)
				{
					sb.Append(string.Format("<i>{0}</i>", emptyList[i].Name));
					if (i == emptyList.Count - 2)
						sb.Append(", or "); // Second to last.
					else if (i == emptyList.Count - 1)
						sb.Append(" "); // last.
					else
						sb.Append(", ");
				}
				sb.Append("is an empty array.");
				WriteExceptionDocumentationElement(wr, "ArgumentException", sb.ToString(), indentOffset);
			}

			if (exceptions != null && exceptions.Length > 0)
			{
				foreach (ExceptionInfo exception in exceptions)
					WriteExceptionDocumentationElement(wr, exception.Type, exception.Description, indentOffset);
			}
			WriteFlowerLine(wr, indentOffset);
		}
	}
}
