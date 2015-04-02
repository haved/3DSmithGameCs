using System;

namespace DSmithGameCs
{
	public static class GUIShader
	{
		private static readonly ColorShader instance = new ColorShader();

		public static ColorShader GetInstance(){
			return instance;
		}
	}
}

