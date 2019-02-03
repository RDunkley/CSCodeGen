// ******************************************************************************************************************************
// Filename:    Settings.AutoGen.cs
// ******************************************************************************************************************************
// Copyright © Richard Dunkley 2019
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0  Unless required by applicable
// law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and
// limitations under the License.
// ******************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace CSCodeGen.Parse
{
	/// <summary>
	///   Adds additional functionality to the auto-generated <see cref="Settings"/> partial class.
	/// </summary>
	public partial class Settings
	{
		#region Methods

		/// <summary>
		///   Attempts to get the type array from the child settings.
		/// </summary>
		/// <typeparam name="T">Type to convert the setting's value to.</typeparam>
		/// <param name="name">Name of the setting.</param>
		/// <param name="array">Output array pulled from the settings.</param>
		/// <returns>True if the array was successfully pulled, false otherwise.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference.</exception>
		/// <exception cref="NotSupportedException">Setting was found, but <typeparamref name="T"/> does not support being converted from a string.</exception>
		public bool TryGetArrayFromSettings<T>(string name, out T[] array)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			if (!TryGetTypeFromSetting<int>(GetArraySizeName(name), out int result))
			{
				array = new T[0];
				return false;
			}

			array = new T[result];
			for (int i = 0; i < result; i++)
			{
				string subName = GetArraySubName(name, i);
				if (!TryGetTypeFromSetting<T>(subName, out array[i]))
					throw new InvalidDataException(string.Format("Unable to locate one of the setting values ({0}) that should exists according to the size specified ({1}).", subName, result));
			}
			return true;
		}

		/// <summary>
		///   Gets the name of the array size's setting.
		/// </summary>
		/// <param name="name">Name of the setting.</param>
		/// <returns>String containing the derived name of the size setting.</returns>
		private static string GetArraySizeName(string name)
		{
			return string.Format("{0}_size", name);
		}

		/// <summary>
		///   Gets the name of the array's sub item.
		/// </summary>
		/// <param name="name">Name of the setting.</param>
		/// <param name="index">Index used to generate the derived name for.</param>
		/// <returns>String containing the derived name of the indexed item setting.</returns>
		private static string GetArraySubName(string name, int index)
		{
			return string.Format("{0}_{1}", name, index);
		}

		/// <summary>
		///   Returns <see cref="SettingInfo"/>s generated from the given array.
		/// </summary>
		/// <typeparam name="T">Type of the array to generate settings for.</typeparam>
		/// <param name="name">Name of the setting.</param>
		/// <param name="array">Array to be converted to settings.</param>
		/// <returns><see cref="SettingInfo"/> array containing all the setting information.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> or <paramref name="name"/> is a null reference.</exception>
		public static SettingInfo[] GetSettingsFromArray<T>(string name, T[] array)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (array == null)
				throw new ArgumentNullException("array");

			List<SettingInfo> setList = new List<SettingInfo>(array.Length + 1);
			setList.Add(new SettingInfo(GetArraySizeName(name), array.Length.ToString()));
			for (int i = 0; i < array.Length; i++)
				setList.Add(new SettingInfo(GetArraySubName(name, i), array[i].ToString()));
			return setList.ToArray();
		}

		/// <summary>
		///   Attempts to get the type from a setting.
		/// </summary>
		/// <typeparam name="T">Type of the object to be returned.</typeparam>
		/// <param name="name">Name of the setting.</param>
		/// <param name="type">Type converted from the string value in the setting.</param>
		/// <returns>True if the setting was found and successfully converted, false otherwise.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference.</exception>
		/// <exception cref="NotSupportedException"><paramref name="type"/> does not support being converted from a string.</exception>
		public bool TryGetTypeFromSetting<T>(string name, out T type)
		{
			SettingInfo setting = FindSetting(name);
			if (setting == null)
			{
				type = default(T);
				return false;
			}

			TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
			if(conv == null)
			{
				type = default(T);
				return false;
			}

			type = (T)conv.ConvertFromString(setting.Value);
			return true;
		}

		/// <summary>
		///   Generates a <see cref="SettingInfo"/> object to represent the value specified.
		/// </summary>
		/// <typeparam name="T">Type of the value to generate the setting for.</typeparam>
		/// <param name="name">Name of the setting.</param>
		/// <param name="value">Value the setting should contain.</param>
		/// <returns><see cref="SettingInfo"/> object containing the setting information on the value.</returns>
		/// <exception cref="ArgumentException"><paramref name="name"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="name"/>, or <paramref name="value"/> is a null reference.
		/// </exception>
		public static SettingInfo GetSettingFromType<T>(string name, T value)
		{
			return new SettingInfo(name, value.ToString());
		}

		/// <summary>
		///   Find's a setting with a given name.
		/// </summary>
		/// <param name="name">Name to find the setting for.</param>
		/// <returns><see cref="SettingInfo"/> object containing the setting information, or null if no setting with that name was found.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference.</exception>
		public SettingInfo FindSetting(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			foreach (SettingInfo setting in SettingInfos)
			{
				if (setting.Name == name)
					return setting;
			}
			return null;
		}

		#endregion Methods
	}
}
