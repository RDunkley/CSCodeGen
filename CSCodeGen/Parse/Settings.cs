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
using System.Reflection;

namespace CSCodeGen.Parse
{
	/// <summary>
	///   Adds additional functionality to the auto-generated <see cref="Settings"/> partial class.
	/// </summary>
	public partial class Settings
	{
		#region Methods

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
		public static SettingInfo GetSettingInfo<T>(string name, T value)
		{
			return new SettingInfo(name, value.ToString());
		}

		/// <summary>
		///   Returns <see cref="SettingInfo"/>s generated from the given array.
		/// </summary>
		/// <typeparam name="T">Type of the array to generate settings from.</typeparam>
		/// <param name="name">Name of the setting.</param>
		/// <param name="array">Array to be converted to settings.</param>
		/// <returns><see cref="SettingInfo"/> array containing all the setting information.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="array"/> or <paramref name="name"/> is a null reference.</exception>
		public static SettingInfo[] GetSettingInfos<T>(string name, T[] array)
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
		///   Returns <see cref="SettingInfo"/>s generated from the given class's public properties.
		/// </summary>
		/// <param name="classType">Type of the <paramref name="classObject"/> to pull the settings for.</param>
		/// <param name="classObject">Instance object used to pull the property values from. If null then the static properties will be used.</param>
		/// <returns><see cref="SettingInfo"/> array containing all the setting information.</returns>
		/// <remarks>Only properties with public getters and setters will be used.</remarks>
		/// <exception cref="ArgumentException"><paramref name="classType"/> does not represent a class type.</exception>
		/// <exception cref="InvalidCastException">A property in <paramref name="classObject"/> does not support the <see cref="IConvertible"/> interface.</exception>
		/// <remarks>
		///   This method only gets properties declared in the actual type specified. To get properties on base or derived classes, then the method must
		///   be called with their corresponding types specified.
		/// </remarks>
		public static SettingInfo[] GetSettingInfos(Type classType, object classObject)
		{
			if (!classType.IsClass)
				throw new ArgumentException("classType does not represent a class type.");

			BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly;
			if (classObject == null)
				flags |= BindingFlags.Static;
			else
				flags |= BindingFlags.Instance;
			System.Reflection.PropertyInfo[] props = classType.GetProperties(flags);
			List<SettingInfo> settingList = new List<SettingInfo>(props.Length);
			foreach (System.Reflection.PropertyInfo prop in props)
			{
				// Make sure the getter and setter are both public as well as the property.
				if (prop.SetMethod != null && prop.SetMethod.IsPublic && prop.GetMethod != null && prop.GetMethod.IsPublic && prop.PropertyType.IsPublic)
				{
					// Special case arrays so that we can break out each component separate.
					if (prop.PropertyType.IsArray && prop.GetValue(classObject) is Array array)
					{
						settingList.Add(new SettingInfo(GetArraySizeName(prop.Name), array.Length.ToString()));
						for (int i = 0; i < array.Length; i++)
							settingList.Add(new SettingInfo(GetArraySubName(prop.Name, i), (string)Convert.ChangeType(array.GetValue(i), typeof(string))));
					}
					else
					{
						settingList.Add(new SettingInfo(prop.Name, (string)Convert.ChangeType(prop.GetValue(classObject), typeof(string))));
					}
				}
			}
			return settingList.ToArray();
		}

		/// <summary>
		///   Attempts to get the type array from the child settings.
		/// </summary>
		/// <typeparam name="T">Type to convert the setting's value to.</typeparam>
		/// <param name="name">Name of the setting.</param>
		/// <param name="array">Output array pulled from the settings.</param>
		/// <returns>True if the array was successfully pulled, false otherwise.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference.</exception>
		/// <exception cref="NotSupportedException">Setting was found, but <typeparamref name="T"/> does not support being converted from a string.</exception>
		public bool TryGetArray<T>(string name, out T[] array)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			if (!TryGetType<int>(GetArraySizeName(name), out int result))
			{
				array = new T[0];
				return false;
			}

			array = new T[result];
			for (int i = 0; i < result; i++)
			{
				string subName = GetArraySubName(name, i);
				if (!TryGetType<T>(subName, out array[i]))
					throw new InvalidDataException(string.Format("Unable to locate one of the setting values ({0}) that should exists according to the size specified ({1}).", subName, result));
			}
			return true;
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
		public bool TryGetType<T>(string name, out T type)
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

		/// <summary>
		///   Sets the give object property values from the settings found in <see cref="SettingInfos"/>.
		/// </summary>
		/// <param name="classType">Type of the <paramref name="classObject"/> to pull the settings for.</param>
		/// <param name="classObject">Instance of a class that the properties will be set on. If null then the static properties will be used.</param>
		/// <exception cref="ArgumentException"><paramref name="classType"/> does not represent a class type.</exception>
		/// <exception cref="InvalidDataException">A corresponding setting was found for a property, but the setting could not be assigned.</exception>
		/// <remarks>
		///   This method only sets properties declared in the actual type specified. To set properties on base or derived classes, then the method must
		///   be called with their corresponding types specified.
		/// </remarks>
		public void SetProperties(Type classType, object classObject, bool overwriteIfNull = false)
		{
			if (!classType.IsClass)
				throw new ArgumentException("classType does not represent a class type.");

			BindingFlags flags = BindingFlags.Public|BindingFlags.DeclaredOnly;
			if (classObject == null)
				flags |= BindingFlags.Static;
			else
				flags |= BindingFlags.Instance;
			System.Reflection.PropertyInfo[] props = classType.GetProperties(flags);
			foreach (System.Reflection.PropertyInfo prop in props)
			{
				// Make sure the getter and setter are both public as well as the property.
				if (prop.SetMethod != null && prop.SetMethod.IsPublic && prop.GetMethod != null && prop.GetMethod.IsPublic && prop.PropertyType.IsPublic)
				{
					if (prop.PropertyType.IsArray)
					{
						// Property is an array so attempt to pull it from the settings.
						if (TryGetType<int>(GetArraySizeName(prop.Name), out int result))
						{
							// Build up a string array containing the string values.
							string[] array = new string[result];
							for (int i = 0; i < result; i++)
							{
								string subName = GetArraySubName(prop.Name, i);
								SettingInfo setting = FindSetting(subName);
								if (setting == null)
									throw new InvalidDataException(string.Format("Unable to locate one of the setting values ({0}) that should exists according to the size specified ({1}).", subName, result));
								array[i] = setting.Value;
							}

							// Assign it to the property.
							SetValue(prop, classObject, array);
						}
					}
					else
					{
						SettingInfo setting = FindSetting(prop.Name);
						if(setting != null)
						{
							// Assign it to the property.
							SetValue(prop, classObject, setting.Value);
						}
					}
				}
			}

		}

		/// <summary>
		///   Sets the value of a property in the given class object to the specified value.
		/// </summary>
		/// <param name="prop">Property determined from <paramref name="classObject"/> to set the value of.</param>
		/// <param name="classObject">Instance of the class object to set the property of. If null, then the value will be set to the static property.</param>
		/// <param name="value">Value to set the property to. Must be able to be converted to the specified property type.</param>
		/// <exception cref="InvalidDataException">Unable to convert the value to the property's type.</exception>
		private void SetValue(System.Reflection.PropertyInfo prop, object classObject, object value, bool overwriteIfNull = false)
		{
			if (value == null && !overwriteIfNull)
				return;

			// Assign it to the property.
			try
			{
				if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					// Value is nullable.
					if (value == null)
						prop.SetValue(classObject, null);
					else
						prop.SetValue(classObject, Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType)), null);
				}
				else
				{
					prop.SetValue(classObject, Convert.ChangeType(value, prop.PropertyType), null);
				}
			}
			catch (Exception e)
			{
				if (e is InvalidCastException || e is FormatException || e is TargetException || e is TargetInvocationException)
				{
					throw new InvalidDataException(string.Format("A setting was found for the {0} property, but we cannot convert the found string(s) to the property type: {1}", prop.Name, e.Message), e);
				}
				if (e is OverflowException)
				{
					throw new InvalidDataException(string.Format("A setting was found for the {0} property, but we cannot convert the found string(s) to the property value because it is out of range of the type: {1}", prop.Name, e.Message), e);
				}
			}
		}

		#endregion Methods
	}
}
