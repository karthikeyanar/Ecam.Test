using System;
using System.Drawing;

namespace SaveAndRestore
{
	/// Note: If the settings file changes, here is how to deal with it:
	///   read the file in the normal way, but use object.GetType().Name to return
	///   the name of the type. Every time the structure of the settings file changes
	///   we will need to rename the settings file, then test to deal with it in the appropriate
	///   way.
	
	/// <summary>
	/// Any data that is desired to be saved should be stored in this class
	/// as a public field. It is suggested that most variables be given default
	/// values within this class. The class should be defined with the "Serializable"
	/// attribute to facilitate file serialization. Properties should not be defined.
	/// </summary>
	[Serializable]
	public class Settings
	{
		public string FileName="TheRegexAssembly.dll";
		public string ClassName="TheRegexClass";
		public string Namespace="TheRegex";
		public bool IsPublic=true;
		public string InputData= "";
		public string RegularExpression = "";
		public string ReplacementString = "";
		public bool IgnoreCase = true;
		public bool Multiline = true;
		public bool Singleline = false;
		public bool ExplicitCapture = false;
		public bool ECMAScript = false;
		public bool RightToLeft = false;
		public bool IgnorePatternWS = true;
		public bool Compiled = true;
		public int LeftPanelWidth = 470;
		public int TreeHeight = 150;
		public Size Size = new Size(800, 400);
	}

	public class RegistrySettings
	{
		public string[] MRUList = new string[0];
		public Point Location = new Point(100,100);
		public string ProjectFile ="";
		public string OpenPathName="";
		public string SavePathName="";
	}
}
