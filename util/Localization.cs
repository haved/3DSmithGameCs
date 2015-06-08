using System.IO;
using System.Collections.Generic;
using System;

namespace DSmithGameCs
{
	public static class Localization
	{
		public static readonly Dictionary<string, string> Map = new Dictionary<string, string> ();

		static readonly string[] spacer = {"==="};

		public static void LoadFromFile(string path)
		{
			try
			{
				var file = new StreamReader (path);

				Map.Clear ();

				string line;
				while ((line = file.ReadLine ()) != null) {
					string[] parts= line.Split (spacer, StringSplitOptions.None);
					Map.Add (parts [0], parts [1]);
				}
				file.Close ();
			}
			catch(Exception e) {
				Console.Error.WriteLine ("Failed to parse localization: '" + path + "'");
				Console.Error.WriteLine ("Syntax:       key"+spacer[0]+"value");
				Console.Error.WriteLine(e);
			}
		}

		public static string GetLocalization(string key)
		{
			return Map.ContainsKey (key) ? Map [key] : key;
		}
	}
}

