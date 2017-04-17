using System;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Drawing;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;

namespace SaveAndRestore
{
	/// This namespace is used for saving data to the registry. A very simple class
	/// is customized for each application by storing all the variables
	/// that are to be saved as public fields, with default values specified.
	/// The work is done by the "Savior" class. It has constructors
	/// and methods to set the registry key and to Save and Read the data from 
	/// the registry.
	/// 
	/// Currently, the following data types are supported:
	/// 
	/// string, bool, decimal, int, float, double, Color, Point, Size, Font, 
	/// DateTime, TimeSpan, int[], string[], bool[], float[], double[],
	/// as well as any enum class whose definition is visible to the class.
	///
 
	/// We start with the class in which all the settings are stored. This will
	/// be customized for each application and multiple such classes may be defined
	/// to facilitate more complex scenarios. The suggested name for the class is
	/// "Settings", but any name may be used since an instantiation of this class is
	/// passed to another class ("Savior") to do the work.
	
	
	/// <summary>
	/// Savior is the class that is used to save and restore data using the registry
	/// or by binary serialization in a file. It works in conjuction with another class
	/// (with the recommended name "Settings") that contains all the data to be
	/// stored. 
	/// </summary>
	public class Savior
	{		

		/// <summary>
		/// Returns a list of all the fields and field types for the specified class
		/// Useful for debugging.
		/// </summary>
		public static string ToString(object settings)
		{
			string msg="";
			foreach(FieldInfo fi in settings.GetType().GetFields(
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
			{ 
				string TheValue="";
				if(fi.GetValue(settings)!=null)TheValue=fi.GetValue(settings).ToString();
				msg+="Name: "+fi.Name+" = "+TheValue+
					"\n    FieldType: "+fi.FieldType+"\n\n";
			}
			return msg;
		}		
				
		/// <summary>
		/// Save all the information in a class to the registry. This method iterates through
		/// all members of the specified class and saves them to the registry.
		/// </summary>
		/// <param name="settings">An object that holds all the desired settings</param>
		/// <param name="Key">The registry key in which to save data</param>
		public static void Save(object settings, RegistryKey Key)
		{
			RegistryKey subkey;

			// Iterate through each Field of the specified class using "Reflection".
			// The name of each Field is used to generate the name of the registry
			// value, sometimes with appended characters to specify elements of more
			// complex Field types such as a Font or a Point. Arrays are stored by
			// creating a new subkey with the same name as the Field. The subkey holds
			// values with names that are just the integers 0,1,2,... giving the index
			// of each value.
			//

			foreach(FieldInfo fi in settings.GetType().GetFields(
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
			{ 
				// If this field is an Enum it needs to be handled separately. The text
				// name for the enum is written to the registry.
				if(fi.FieldType.IsEnum)
				{
					Key.SetValue(fi.Name,Enum.GetName(fi.FieldType,fi.GetValue(settings)));
				}

				// Otherwise each different field type is handled case by case using
				// a large switch statement
				else 
				{
					// Test the name of the Field type, converted to lower case
					switch (fi.FieldType.Name.ToLower())
					{
						case "string":
							Key.SetValue(fi.Name,(string)fi.GetValue(settings));
							break;
						case "boolean":
							Key.SetValue(fi.Name,(bool)fi.GetValue(settings));
							break;
						case "int32":
							Key.SetValue(fi.Name,(int)fi.GetValue(settings));
							break;
						case "decimal":
							decimal TheDecimal=(decimal)fi.GetValue(settings);
							Key.SetValue(fi.Name,(int)TheDecimal);
							break;
						case "single":
							Key.SetValue(fi.Name,(float)fi.GetValue(settings));
							break;
						case "double":
							Key.SetValue(fi.Name,(double)fi.GetValue(settings));
							break;
						case "point": // Store as two separate integers
							Point point=(Point)fi.GetValue(settings);
							Key.SetValue(fi.Name+".X",point.X);
							Key.SetValue(fi.Name+".Y",point.Y);
							//MessageBox.Show("X:"+point.X+" Y:"+point.Y);
							break;
						case "size":
							Size size=(Size)fi.GetValue(settings);
							Key.SetValue(fi.Name+".Height",size.Height);
							Key.SetValue(fi.Name+".Width",size.Width);
							break;

						// For arrays, first delete the subkey if it already exists
						// then refill it with all the values of the array
						case "int32[]":
							int[] numbers = (int[])fi.GetValue(settings);
							if(numbers==null)break;
							Key.DeleteSubKey(fi.Name,false);   // first delete all the old values
							subkey=Key.CreateSubKey(fi.Name);
							for(int i=0;i<numbers.Length;i++)
							{
								subkey.SetValue(i.ToString(),numbers[i]);
							}
							break;
						case "string[]":
							string[] strings = (string[])fi.GetValue(settings);
							if(strings==null)break;
							Key.DeleteSubKey(fi.Name,false);   // first delete all the old values
							subkey=Key.CreateSubKey(fi.Name);
							for(int i=0;i<strings.Length;i++)
							{
								subkey.SetValue(i.ToString(),strings[i]);
							}
							break;
						case "boolean[]":
							bool[] bools = (bool[])fi.GetValue(settings);
							if(bools==null)break;
							Key.DeleteSubKey(fi.Name,false);   // first delete all the old values
							subkey=Key.CreateSubKey(fi.Name);
							for(int i=0;i<bools.Length;i++)
							{
								subkey.SetValue(i.ToString(),bools[i]);
							}
							break;
						case "single[]":
							float[] floats = (float[])fi.GetValue(settings);
							if(floats==null)break;
							Key.DeleteSubKey(fi.Name,false);   // first delete all the old values
							subkey=Key.CreateSubKey(fi.Name);
							for(int i=0;i<floats.Length;i++)
							{
								subkey.SetValue(i.ToString(),floats[i]);
							}
							break;
						case "double[]":
							double[] doubles = (double[])fi.GetValue(settings);
							if(doubles==null)break;
							Key.DeleteSubKey(fi.Name,false);   // first delete all the old values
							subkey=Key.CreateSubKey(fi.Name);
							for(int i=0;i<doubles.Length;i++)
							{
								subkey.SetValue(i.ToString(),doubles[i]);
							}
							break;
						case "color":
							Key.SetValue(fi.Name,((Color)fi.GetValue(settings)).Name);
							break;
						case "timespan":
							Key.SetValue(fi.Name,((TimeSpan)fi.GetValue(settings)).ToString());
							break;
						case "datetime":
							Key.SetValue(fi.Name,((DateTime)fi.GetValue(settings)).ToString());
							break;
						case "font":
							Key.SetValue(fi.Name+".Name",((Font)fi.GetValue(settings)).Name);
							Key.SetValue(fi.Name+".Size",((Font)fi.GetValue(settings)).Size);
							Key.SetValue(fi.Name+".Style",((Font)fi.GetValue(settings)).Style);
							break;
						default:
							MessageBox.Show("This Field type cannot be saved by the Savior class: "+fi.FieldType);
							break;
					}
				}
			}
		}

		// Here are several overloads for the Save routine, specifying the RegistryKey in
		// several different ways

		/// <summary>
		/// Save to the registry using the specified key
		/// </summary>
		/// <param name="key">A string giving the registry key path relative to HKCU</param>
		public static void Save(object settings, string key)
		{
			RegistryKey Key = Registry.CurrentUser.CreateSubKey(key);
			Save(settings,Key);
		}

		/// <summary>
		/// Save to the registry using the default key, the standard user application
		/// data registry key. To use this effectively, be sure to specify the
		/// appropriate information in the AssemblyInfo file. 
		/// </summary>
		public static void Save(object settings)
		{
			RegistryKey Key=Application.UserAppDataRegistry;
			Save(settings,Key);
		}

		
		/// <summary>
		/// Read all the information in a class to the registry. This method iterates through
		/// all members of the specified class and reads them to the registry.
		/// </summary>
		/// <param name="settings">An object that holds all the desired settings</param>
		/// <param name="Key">The registry key in which to save data</param>
		public static void Read(object settings, RegistryKey Key)
		{
			// Iterate through each Field of the specified class using "Reflection".
			// The name of each Field is used to generate the name of the registry
			// value, sometimes with appended characters to specify elements of more
			// complex Field types such as a Font or a Point. Arrays are read from
			// a subkey with the same name as the Field. The subkey holds
			// values with names that are just the integers 0,1,2,... giving the index
			// of each value.
			//
			foreach(FieldInfo fi in settings.GetType().GetFields(
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
			{ 
				object obj;
				int X,Y,Height,Width;
				string name;
				float emSize;
				FontStyle style;
				RegistryKey subkey;

				// If this field is an Enum it needs to be handled separately. The text
				// name for the enum is written to the registry.
				if(fi.FieldType.IsEnum)
				{
					if((obj=Key.GetValue(fi.Name))!=null)
						fi.SetValue(settings,Enum.Parse(fi.FieldType,(string)obj));
				}

				// Otherwise each different field type is handled case by case using
				// a large switch statement that tests the lower case name of the Field type
				else
				{
					switch (fi.FieldType.Name.ToLower())
					{
						case "string":
							if((obj=Key.GetValue(fi.Name))!=null)
								fi.SetValue(settings,(string)obj);
							break;
						case "boolean":
							if((obj=Key.GetValue(fi.Name))!=null)
								fi.SetValue(settings,bool.Parse((string)obj));
							break;
						case "int32":
							if((obj=Key.GetValue(fi.Name))!=null)
								fi.SetValue(settings,(int)obj);
							break;
						case "decimal":
							if((obj=Key.GetValue(fi.Name))!=null)
							{
								int TheInt=(int)obj;
								fi.SetValue(settings,(decimal)TheInt);
							}
							break;
						case "single":
							if((obj=Key.GetValue(fi.Name))!=null)
								fi.SetValue(settings,float.Parse((string)obj));
							break;
						case "double":
							if((obj=Key.GetValue(fi.Name))!=null)
								fi.SetValue(settings,double.Parse((string)obj));
							break;
						case "point":
							if((obj=Key.GetValue(fi.Name+".X"))!=null)
							{
								X=(int)obj;
								if((obj=Key.GetValue(fi.Name+".Y"))!=null)
								{
									Y=(int)obj;
									fi.SetValue(settings,new Point(X,Y));
									//MessageBox.Show("X:"+point.X+" Y:"+point.Y);
								}
							}
							break;
						case "size":
							if((obj=Key.GetValue(fi.Name+".Height"))!=null)
							{
								Height=(int)obj;
								if((obj=Key.GetValue(fi.Name+".Width"))!=null)
								{
									Width=(int)obj;
									fi.SetValue(settings,new Size(Width,Height));
									//MessageBox.Show("X:"+point.X+" Y:"+point.Y);
								}
							}
							break;
						case "string[]":  // Get an array of strings
							if((subkey=Key.OpenSubKey(fi.Name))!=null)
							{
								///string msg="";
								int i=0;
								int N=subkey.ValueCount;
								string[] strings = new string[N];
								while((obj=subkey.GetValue(i.ToString()))!=null)
								{
									//msg+="Strings["+i.ToString()+"]="+(string)obj+"\n";
									strings[i++]=(string)obj;
								}
								//MessageBox.Show(msg);
								fi.SetValue(settings,strings);
							}
							break;
						case "int32[]":  // Get an array of ints
							if((subkey=Key.OpenSubKey(fi.Name))!=null)
							{
								//string msg="";
								int i=0;
								int N=subkey.ValueCount;
								int[] integers = new int[N];
								while((obj=subkey.GetValue(i.ToString()))!=null)
								{
									//msg+="Integer["+i.ToString()+"]="+(int)obj+"\n";
									integers[i++]=(int)obj;
								}
								//MessageBox.Show(msg);
								fi.SetValue(settings,integers);
							}
							break;
						case "single[]":  // Get an array of floats
							if((subkey=Key.OpenSubKey(fi.Name))!=null)
							{
								int i=0;
								int N=subkey.ValueCount;
								float[] floats = new float[N];
								while((obj=subkey.GetValue(i.ToString()))!=null)
								{
									floats[i++]=float.Parse((string)obj);
								}
								fi.SetValue(settings,floats);
							}
							break;
						case "double[]":  // Get an array of doubles
							if((subkey=Key.OpenSubKey(fi.Name))!=null)
							{
								int i=0;
								int N=subkey.ValueCount;
								double[] doubles = new double[N];
								while((obj=subkey.GetValue(i.ToString()))!=null)
								{
									doubles[i++]=double.Parse((string)obj);
								}
								fi.SetValue(settings,doubles);
							}
							break;
						case "boolean[]":  // Get an array of booleans
							if((subkey=Key.OpenSubKey(fi.Name))!=null)
							{
								//string msg="";
								int i=0;
								int N=subkey.ValueCount;
								bool[] bools = new bool[N];
								while((obj=subkey.GetValue(i.ToString()))!=null)
								{
									bools[i]=bool.Parse((string)obj);
									//msg+="Bools["+i.ToString()+"]="+bools[i].ToString()+"\n";
									i++;
								}
								//MessageBox.Show(msg);
								fi.SetValue(settings,bools);
							}
							break;
						case "color":  // Get a Color
							if((obj=Key.GetValue(fi.Name))!=null)
								fi.SetValue(settings,Color.FromName((string)obj));
							break;
						case "timespan":  // Get a TimeSpan
							if((obj=Key.GetValue(fi.Name))!=null)
								fi.SetValue(settings,TimeSpan.Parse((string)obj));
							break;
						case "datetime":  // Get a DateTime
							if((obj=Key.GetValue(fi.Name))!=null)
								fi.SetValue(settings,DateTime.Parse((string)obj));
							break;
						case "font":
							if((obj=Key.GetValue(fi.Name+".Name"))!=null)
							{
								name=(string)obj;
								if((obj=Key.GetValue(fi.Name+".Size"))!=null)
								{
									emSize=float.Parse((string)obj);
									if((obj=Key.GetValue(fi.Name+".Style"))!=null)
									{
										style=(FontStyle)Enum.Parse(typeof(FontStyle),(string)obj);
										fi.SetValue(settings,new Font(name,emSize,style));
										//MessageBox.Show("Style: "+(string)obj+"\n"+style.ToString());
									}
								}
							}
							break;
						default:
							MessageBox.Show("This type has not been implemented: "+fi.FieldType);
							break;
					}
				}
			}
		}

		// Here are several overloads for the Read routine, specifying the RegistryKey in
		// several different ways

		/// <summary>
		/// Read from the registry using the specified key
		/// </summary>
		/// <param name="key">A string giving the registry key path relative to HKCU</param>
		public static void Read(object settings, string key)
		{
			RegistryKey Key = Registry.CurrentUser.CreateSubKey(key);
			Read(settings,Key);
		}

		/// <summary>
		/// Read from the registry using the default key, the standard user application
		/// data registry key. To use this effectively, be sure to specify the
		/// appropriate information in the AssemblyInfo file. 
		/// </summary>
		public static void Read(object settings)
		{
			RegistryKey Key=Application.UserAppDataRegistry;
			Read(settings,Key);
		}

		
		/// <summary>
		/// Save settings to a file using binary serialization
		/// </summary>
		/// <param name="settings">This is the object that we want to serialize</param>
		/// <param name="FileName">The name of the file in which to store settings</param>
		public static bool SaveToFile(object settings, string FileName)
		{
			try
			{
				IFormatter formatter = new BinaryFormatter();
				Stream stream = new FileStream(FileName, FileMode.Create,
					FileAccess.Write, FileShare.None);
				formatter.Serialize(stream, settings);
				stream.Close();
				return true;
			}
			catch
			{
				MessageBox.Show("Error attempting to save the settings to a file\n\n"+FileName,
					"Expresso Error",
					MessageBoxButtons.OK,MessageBoxIcon.Error);
				return false;
			}
		}

		/// <summary>
		/// Read from a file using binary serialization
		/// </summary>
		/// <param name="settings">The original settings object, needed to restore the correct Registry Key</param>
		/// <param name="FileName">The name of the file from which to read settings</param>
		/// <returns>A Savior object is returned</returns>
		public static object ReadFromFile(object settings, string FileName)
		{
			try
			{
				// First try to read the version information				
				IFormatter formatter = new BinaryFormatter();
				Stream stream = new FileStream(FileName, FileMode.Open,
					FileAccess.Read, FileShare.Read);
				object NewSettings = (object)formatter.Deserialize(stream);
				stream.Close();
				return NewSettings;
			}
			catch
			{
				MessageBox.Show("Error attempting to read the settings from a file\n\n"+FileName,
					"Expresso Error",
					MessageBoxButtons.OK,MessageBoxIcon.Error);
				return null;   // If there is an error return null
			}
		}
	}	
}
