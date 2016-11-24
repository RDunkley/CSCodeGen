//********************************************************************************************************************************
// Filename:    BinaryUtility.cs
// Owner:       Richard Dunkley
// Description: Static class containing various helper methods for manipulating binary values.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCodeGen
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Integer type (byte, sbyte, ushort, short, uint, int, ulong, or long).</typeparam>
	public static class BinaryUtility<T> where T : struct, IComparable
	{
		/// <summary>
		///   Various formats that an integer can be in.
		/// </summary>
		[Flags]
		public enum IntegerFormat
		{
			/// <summary>
			///   Hexadecimal type 1 format. The hexadecimal is preceeded by a '0x' (Ex: 0xA5).
			/// </summary>
			HexType1,

			/// <summary>
			///   Hexadecimal type 2 format. The hexadecimal is proceeded by a 'h' (Ex: A5h).
			/// </summary>
			HexType2,

			/// <summary>
			///   The integer is represented in binary followed by a 'b'. (Ex: 10100101b).
			/// </summary>
			Binary,

			/// <summary>
			///   The integer is displayed as a number. May or may not have ',' to separate the thousands digits.
			/// </summary>
			Integer,
		}

		public enum BinaryType
		{
			Byte,
			SByte,
			UInt16,
			Int16,
			UInt32,
			Int32,
			UInt64,
			Int64,
		}

		/// <summary>
		///   Gets the maximum length of the integral type in bits.
		/// </summary>
		/// <returns>Bit length of the underlying type (8, 16, 32, or 64).</returns>
		public static int GettMaxBitLength()
		{
			if (typeof(T) != typeof(byte) && typeof(T) != typeof(sbyte) && typeof(T) != typeof(ushort) && typeof(T) != typeof(short) &&
				typeof(T) != typeof(uint) && typeof(T) != typeof(int) && typeof(T) != typeof(ulong) && typeof(T) != typeof(long))
				throw new ArgumentException(string.Format("T Type ({0}) is not a valid type for this method (must be an integer type: byte, sbyte, ushort, short, uint, int, ulong, and long).", typeof(T).ToString()));

			switch (typeof(T).Name.ToLower())
			{
				case "byte":
				case "sbyte":
					return 8;
				case "uint16":
				case "int16":
					return 16;
				case "uint32":
				case "int32":
					return 32;
				case "uint64":
				case "int64":
					return 64;
				default:
					throw new NotImplementedException(string.Format("The type of this method ({0}) is not recognized as a supported integer type", typeof(T).Name.ToLower()));
			}
		}

		/// <summary>
		///   Gets the maximum value of the specified integer type.
		/// </summary>
		/// <returns>The maximum value of the type.</returns>
		public static T GetMaxValue()
		{
			if (typeof(T) != typeof(byte) && typeof(T) != typeof(sbyte) && typeof(T) != typeof(ushort) && typeof(T) != typeof(short) &&
				typeof(T) != typeof(uint) && typeof(T) != typeof(int) && typeof(T) != typeof(ulong) && typeof(T) != typeof(long))
				throw new ArgumentException(string.Format("T Type ({0}) is not a valid type for this method (must be an integer type: byte, sbyte, ushort, short, uint, int, ulong, and long).", typeof(T).ToString()));

			switch (typeof(T).Name.ToLower())
			{
				case "byte":
					return (T)(object)byte.MaxValue;
				case "sbyte":
					return (T)(object)sbyte.MaxValue;
				case "uint16":
					return (T)(object)ushort.MaxValue;
				case "int16":
					return (T)(object)short.MaxValue;
				case "uint32":
					return (T)(object)uint.MaxValue;
				case "int32":
					return (T)(object)int.MaxValue;
				case "uint64":
					return (T)(object)ulong.MaxValue;
				case "int64":
					return (T)(object)long.MaxValue;
				default:
					throw new NotImplementedException(string.Format("The type of this method ({0}) is not recognized as a supported integer type", typeof(T).Name.ToLower()));
			}
		}

		/// <summary>
		///   Gets the minimum value of the specified integer type.
		/// </summary>
		/// <returns>The minimum value of the type.</returns>
		public static T GetMinValue()
		{
			if (typeof(T) != typeof(byte) && typeof(T) != typeof(sbyte) && typeof(T) != typeof(ushort) && typeof(T) != typeof(short) &&
				typeof(T) != typeof(uint) && typeof(T) != typeof(int) && typeof(T) != typeof(ulong) && typeof(T) != typeof(long))
				throw new ArgumentException(string.Format("T Type ({0}) is not a valid type for this method (must be an integer type: byte, sbyte, ushort, short, uint, int, ulong, and long).", typeof(T).ToString()));

			switch (typeof(T).Name.ToLower())
			{
				case "byte":
					return (T)(object)byte.MinValue;
				case "sbyte":
					return (T)(object)sbyte.MinValue;
				case "uint16":
					return (T)(object)ushort.MinValue;
				case "int16":
					return (T)(object)short.MinValue;
				case "uint32":
					return (T)(object)uint.MinValue;
				case "int32":
					return (T)(object)int.MinValue;
				case "uint64":
					return (T)(object)ulong.MinValue;
				case "int64":
					return (T)(object)long.MinValue;
				default:
					throw new NotImplementedException(string.Format("The type of this method ({0}) is not recognized as a supported integer type", typeof(T).Name.ToLower()));
			}
		}

		/// <summary>
		///   Parses the <paramref name="text"/> string and returns the parsed value.
		/// </summary>
		/// <param name="text">Text to be parsed</param>
		/// <param name="style"><see cref="NumberStyles"/> representing the style of the text to be parsed.</param>
		/// <returns>Value parsed from the string.</returns>
		/// <exception cref="FormatException"><paramref name="text"/> is not of the correct format.</exception>
		/// <exception cref="OverflowException"><paramref name="text"/> represents a number less than the minimum value for that type or a number greater than the maximum value for that type.</exception>
		public static T Parse(string text, NumberStyles style)
		{
			if (typeof(T) != typeof(byte) && typeof(T) != typeof(sbyte) && typeof(T) != typeof(ushort) && typeof(T) != typeof(short) &&
				typeof(T) != typeof(uint) && typeof(T) != typeof(int) && typeof(T) != typeof(ulong) && typeof(T) != typeof(long))
				throw new ArgumentException(string.Format("T Type ({0}) is not a valid type for this method (must be an integer type: byte, sbyte, ushort, short, uint, int, ulong, and long).", typeof(T).ToString()));

			switch (typeof(T).Name.ToLower())
			{
				case "byte":
					return (T)Convert.ChangeType(byte.Parse(text, style), typeof(T));
				case "sbyte":
					return (T)Convert.ChangeType(sbyte.Parse(text, style), typeof(T));
				case "uint16":
					return (T)Convert.ChangeType(ushort.Parse(text, style), typeof(T));
				case "int16":
					return (T)Convert.ChangeType(short.Parse(text, style), typeof(T));
				case "uint32":
					return (T)Convert.ChangeType(uint.Parse(text, style), typeof(T));
				case "int32":
					return (T)Convert.ChangeType(int.Parse(text, style), typeof(T));
				case "uint64":
					return (T)Convert.ChangeType(ulong.Parse(text, style), typeof(T));
				case "int64":
					return (T)Convert.ChangeType(long.Parse(text, style), typeof(T));
				default:
					throw new NotImplementedException(string.Format("The type of this method ({0}) is not recognized as a supported integer type", typeof(T).Name.ToLower()));
			}
		}

		/// <summary>
		///   Parses the integer value.
		/// </summary>
		/// <param name="value">String integer value to be parsed.</param>
		/// <param name="allowedFormats">Formats that are allowed for the string. See <see cref="IntegerFormat"/> for more information.</param>
		/// <returns>The integer value of type T.</returns>
		public static T Parse(string value, IntegerFormat allowedFormats = IntegerFormat.Binary | IntegerFormat.HexType1 | IntegerFormat.HexType2 | IntegerFormat.Integer)
		{
			return Parse(value, GetMinValue(), GetMaxValue(), allowedFormats);
		}

		/// <summary>
		///   Parses the integer value and validates that it is between the specified minimum and maximum values.
		/// </summary>
		/// <param name="value">String integer value to be parsed.</param>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Maximum value.</param>
		/// <param name="allowedFormats">Formats that are allowed for the string. See <see cref="IntegerFormat"/> for more information.</param>
		/// <returns>The integer value of type T.</returns>
		public static T Parse(string value, T minValue, T maxValue, IntegerFormat allowedFormats = IntegerFormat.Binary | IntegerFormat.HexType1 | IntegerFormat.HexType2 | IntegerFormat.Integer)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			if (value.Length == 0)
				throw new ArgumentException("value is an empty string");
			if (typeof(T) != typeof(byte) && typeof(T) != typeof(sbyte) && typeof(T) != typeof(ushort) && typeof(T) != typeof(short) &&
				typeof(T) != typeof(uint) && typeof(T) != typeof(int) && typeof(T) != typeof(ulong) && typeof(T) != typeof(long))
				throw new ArgumentException(string.Format("T Type ({0}) is not a valid type for this method (must be an integer type: byte, sbyte, ushort, short, uint, int, ulong, and long).", typeof(T).ToString()));

			T returnValue = GetMinValue();
			bool parsed = false;
			try
			{
				if (allowedFormats.HasFlag(IntegerFormat.HexType1) && value.Length > 2 && value[0] == '0' && char.ToLower(value[1]) == 'x')
				{
					// Number is specified as a hexadecimal number.
					returnValue = Parse(value.Substring(2), NumberStyles.AllowHexSpecifier);
					parsed = true;
				}
				else if (allowedFormats.HasFlag(IntegerFormat.HexType2) && value.Length > 1 && char.ToLower(value[value.Length - 1]) == 'h')
				{
					// Number is specified as a hexadecimal number.
					returnValue = Parse(value.Substring(0, value.Length - 1), NumberStyles.AllowHexSpecifier);
					parsed = true;
				}
				else if (allowedFormats.HasFlag(IntegerFormat.Binary) && char.ToLower(value[value.Length - 1]) == 'b')
				{
					// Number is specified as a binary number.
					returnValue = ParseBitString(value.Substring(0, value.Length - 1));
					parsed = true;
				}
				else if (allowedFormats.HasFlag(IntegerFormat.Integer))
				{
					// Attempt to parse the number as just an integer.
					returnValue = Parse(value, NumberStyles.Integer | NumberStyles.AllowThousands);
					parsed = true;
				}
			}
			catch (FormatException e)
			{
				throw new ArgumentException(string.Format("The integral value specified ({0}) is not in a valid format: {1}.", value, e.Message), e);
			}
			catch (OverflowException e)
			{
				throw new ArgumentException(string.Format("The integral value specified ({0}) is larger or smaller than the integral type allows (Min: {1}, Max: {2}).", value, GetMinValue().ToString(), GetMaxValue().ToString()), e);
			}

			if (!parsed)
				throw new ArgumentException(string.Format("The integral value specified ({0}) is not in a valid format.", value));

			if (maxValue.CompareTo(GetMaxValue()) < 0)
			{
				// Verify that the value has not excedded the specified maximum size.
				if (returnValue.CompareTo(maxValue) > 0)
					throw new ArgumentException(string.Format("The integral value specified ({0}) is larger than the maximum value allowed ({1}).", value, GetMaxValue().ToString()));
			}
			if (minValue.CompareTo(GetMinValue()) > 0)
			{
				// Verify that the value has not exceeded the specified minimum size.
				if (returnValue.CompareTo(minValue) < 0)
					throw new ArgumentException(string.Format("The integral value specified ({0}) is smaller than the minimal value allowed ({1}).", value, GetMinValue().ToString()));
			}
			return returnValue;
		}

		/// <summary>
		///   Parses the bit string to the integral type.
		/// </summary>
		/// <param name="bitString">String of '1's and '0's making up the bitstream.</param>
		/// <returns>Parsed integral type.</returns>
		/// <exception cref="ArgumentException"><paramref name="bitString"/> length is longer than the number of bits allowed in the integral type or one of the characters is not a '1' or '0'.</exception>
		public static T ParseBitString(string bitString)
		{
			string typeName = typeof(T).Name.ToLower();

			// Check for an invalid size.
			int maxBitSize = GettMaxBitLength();
			if (bitString.Length - 1 > maxBitSize)
				throw new ArgumentException(string.Format("The value specified ({0}) was determined to be a binary type but had more bits ({1}) than can be contained in the {2} type ({3}).", bitString, bitString.Length, typeName, maxBitSize));

			// Check for invalid characters.
			for (int i = 0; i < bitString.Length; i++)
			{
				if (bitString[i] != '0' && bitString[i] != '1')
					throw new ArgumentException(string.Format("The value specified ({0}) was determined to be a binary type but had a character ({1}) at index {2} than is not a '1' or '0'.", bitString, bitString[i], i));
			}

			switch (typeName)
			{
				case "byte":
					return (T)Convert.ChangeType(Convert.ToByte(bitString, 2), typeof(T));
				case "sbyte":
					return (T)Convert.ChangeType(Convert.ToSByte(bitString, 2), typeof(T));
				case "uint16":
					return (T)Convert.ChangeType(Convert.ToUInt16(bitString, 2), typeof(T));
				case "int16":
					return (T)Convert.ChangeType(Convert.ToInt16(bitString, 2), typeof(T));
				case "uint32":
					return (T)Convert.ChangeType(Convert.ToUInt32(bitString, 2), typeof(T));
				case "int32":
					return (T)Convert.ChangeType(Convert.ToInt32(bitString, 2), typeof(T));
				case "uint64":
					return (T)Convert.ChangeType(Convert.ToUInt64(bitString, 2), typeof(T));
				case "int64":
					return (T)Convert.ChangeType(Convert.ToInt64(bitString, 2), typeof(T));
				default:
					throw new NotImplementedException(string.Format("The type of this method ({0}) is not recognized as a supported integer type", typeof(T).Name.ToLower()));
			}
		}

		/// <summary>
		///   Attempts to parse the text to the integral value based on the specified <see cref="NumberStyles"/>.
		/// </summary>
		/// <param name="text">Text to be parsed.</param>
		/// <param name="style"><see cref="NumberStyles"/> of the string.</param>
		/// <param name="value">Integral type parsed from the text.</param>
		/// <returns>True if the parsing was successful, false otherwise.</returns>
		public static bool TryParse(string text, NumberStyles style, out T value)
		{
			bool returnValue = false;
			switch (typeof(T).Name.ToLower())
			{
				case "byte":
					byte byteValue;
					returnValue = byte.TryParse(text, style, CultureInfo.CurrentCulture.NumberFormat, out byteValue);
					value = (T)(object)byteValue;
					break;
				case "sbyte":
					sbyte sbyteValue;
					returnValue = sbyte.TryParse(text, style, CultureInfo.CurrentCulture.NumberFormat, out sbyteValue);
					value = (T)(object)sbyteValue;
					break;
				case "uint16":
					ushort ushortValue;
					returnValue = ushort.TryParse(text, style, CultureInfo.CurrentCulture.NumberFormat, out ushortValue);
					value = (T)(object)ushortValue;
					break;
				case "int16":
					short shortValue;
					returnValue = short.TryParse(text, style, CultureInfo.CurrentCulture.NumberFormat, out shortValue);
					value = (T)(object)shortValue;
					break;
				case "uint32":
					uint uintValue;
					returnValue = uint.TryParse(text, style, CultureInfo.CurrentCulture.NumberFormat, out uintValue);
					value = (T)(object)uintValue;
					break;
				case "int32":
					int intValue;
					returnValue = int.TryParse(text, style, CultureInfo.CurrentCulture.NumberFormat, out intValue);
					value = (T)(object)intValue;
					break;
				case "uint64":
					ulong ulongValue;
					returnValue = ulong.TryParse(text, style, CultureInfo.CurrentCulture.NumberFormat, out ulongValue);
					value = (T)(object)ulongValue;
					break;
				case "int64":
					long longValue;
					returnValue = long.TryParse(text, style, CultureInfo.CurrentCulture.NumberFormat, out longValue);
					value = (T)(object)longValue;
					break;
				default:
					throw new NotImplementedException("The type of this class is not recognized as a supported type");
			}
			return returnValue;
		}

		/// <summary>
		///   Attempts to parse the <paramref name="value"/> string to the integral type.
		/// </summary>
		/// <param name="value">String to be parsed.</param>
		/// <param name="maxValue">Maximum value the parsed integral can have.</param>
		/// <param name="minValue">Minimum value the parsed integral can have.</param>
		/// <param name="returnValue">Parsed value or zero if the parsing was unsuccessful.</param>
		/// <returns>True if the value was parsed successfully, false otherwise.</returns>
		public static bool TryParse(string value, T minValue, T maxValue, bool allowHexType1, bool allowHexType2, bool allowBinary, bool allowInteger)
		{
			if (value == null)
				return false;
			if (value.Length == 0)
				return false;

			T returnValue = GetMinValue();
			bool parsed = false;
			if (allowHexType2 && value.Length > 2 && value[0] == '0' && char.ToLower(value[1]) == 'x')
			{
				// Number is specified as a hexadecimal number.
				if (!TryParse(value.Substring(2), NumberStyles.AllowHexSpecifier, out returnValue))
					return false;
				parsed = true;
			}
			else if (allowHexType1 && value.Length > 1 && char.ToLower(value[value.Length - 1]) == 'h')
			{
				// Number is specified as a hexadecimal number.
				if (!TryParse(value.Substring(0, value.Length - 1), NumberStyles.AllowHexSpecifier, out returnValue))
					return false;
				parsed = true;
			}
			else if (allowBinary && char.ToLower(value[value.Length - 1]) == 'b')
			{
				// Number is specified as a binary number.
				if (!TryParseBitString(value.Substring(0, value.Length - 1), out returnValue))
					return false;
				parsed = true;
			}
			else if (allowInteger)
			{
				// Attempt to parse the number as just an integer.");
				if (!TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, out returnValue))
					return false;
				parsed = true;
			}

			if (!parsed)
				return false;

			if (maxValue.CompareTo(GetMaxValue()) < 0)
			{
				// Verify that the value has not excedded the specified maximum size.
				if (returnValue.CompareTo(maxValue) > 0)
					return false;
			}
			if (minValue.CompareTo(GetMinValue()) > 0)
			{
				// Verify that the value has not exceeded the specified minimum size.
				if (returnValue.CompareTo(minValue) < 0)
					return false;
			}
			return true;
		}

		/// <summary>
		///   Attempts to parse the bit string.
		/// </summary>
		/// <param name="bitString">String of '1's and '0's making up the bitstream.</param>
		/// <param name="value">Parsed integral type or zero if method returns false.</param>
		/// <returns>True if the binary string was successfully parsed, false otherwise.</returns>
		public static bool TryParseBitString(string bitString, out T value)
		{
			value = GetMinValue();

			// Check for an invalid size.
			if (bitString.Length - 1 > GettMaxBitLength())
				return false;

			// Check for invalid characters.
			for (int i = 0; i < bitString.Length; i++)
			{
				if (bitString[i] != '0' && bitString[i] != '1')
					return false;
			}

			value = ParseBitString(bitString);
			return true;
		}


	}
}
