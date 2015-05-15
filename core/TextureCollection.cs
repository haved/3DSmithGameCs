using System;

namespace DSmithGameCs
{
	public static class TextureCollection
	{
		static Texture[] textures;

		public static void Load()
		{
			textures = new Texture[3];
			textures [0] = new Texture ("../../res/textures/button.png");
			textures [1] = new Texture ("../../res/textures/items.png");
			textures [2] = new Texture ("../../res/textures/numbers.png");
		}

		public static void Dispose()
		{
			textures = null;
		}

		public static Texture Button{get {return textures[0];}}
		public static Texture Items{get {return textures[1];}}
		public static Texture Numbers{get {return textures[2];}}
	}
}

