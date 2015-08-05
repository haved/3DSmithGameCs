using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class BladeItem : Item
	{
		public static BladeType[] BladeTypes = new BladeType[1];
		public static BladeType GreatswordBlade = new BladeType(0, "Greatsword", MeshCollection.GreatswordBlade, 3.5f, 2, new []{0.2f, 0.3f, 0.5f, 0.7f, 0.9f, 1f});

		public class BladeType
		{
			public readonly int Id;
			public readonly string Name;
			public readonly BladeMesh Mesh;
			public readonly float MeshScale;
			public readonly Matrix4 MeshScaleMatrix;
			public readonly Matrix4 CenteredScaledMeshMatirx;
			public readonly float Volume;
			public readonly float[] Points;

			public BladeType(int id, string name, BladeMesh mesh, float meshScale, float volume, float[] points)
			{
				Id = id;
				BladeItem.BladeTypes[id] = this;
				Name = name;
				Mesh = mesh;
				MeshScale = meshScale;
				MeshScaleMatrix = Matrix4.CreateScale(MeshScale);
				CenteredScaledMeshMatirx = Matrix4.CreateTranslation(-0.5f,0,0)*MeshScaleMatrix;
				Volume = volume;
				Points = points;
			}
		}

		public BladeType Type { get; private set; }
		public int Metal { get; private set; }
		public float Purity { get; private set; }
		public float[] Sharpness { get; set;}
		int sharpnessMap = -1;
		int heatMap = -1;

		public BladeItem(){
			sharpnessMap = GL.GenTexture ();
			heatMap = GL.GenTexture ();
			GL.BindTexture  (TextureTarget.Texture1D, sharpnessMap);
			GL.TexParameter	(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter	(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter	(TextureTarget.Texture1D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter	(TextureTarget.Texture1D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			GL.BindTexture  (TextureTarget.Texture1D, heatMap);
			GL.TexParameter	(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter	(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter	(TextureTarget.Texture1D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter	(TextureTarget.Texture1D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			Console.Out.WriteLine("Assigned texture: " + sharpnessMap);
		}

		public BladeItem (BladeType type, int metal, float purity) : this()
		{
			Type = type;
			Metal = metal;
			Purity = purity;
			Sharpness = new float[Type.Points.Length];
		}

		public override void DisposeItem()
		{
			if (GraphicsContext.CurrentContext != null) {
				GL.DeleteTexture (sharpnessMap);
				GL.DeleteTexture (heatMap);
			}
		}

		protected static readonly Matrix4 ItemMatrix = Matrix4.CreateRotationZ (Util.PI / 2 - 0.2f) * Matrix4.CreateRotationX (0.2f) * Matrix4.CreateRotationY (-0.3f) * Matrix4.CreateTranslation (0, 0, -2.8f)
		                                               * Matrix4.CreatePerspectiveFieldOfView (0.85f, 1, 0.1f, 4) * Matrix4.CreateScale (1, 0.5f, 1);
		public override void RenderItem(float x, float y, float width, float height)
		{
			BladeShader Instance = BladeShader.Instance;
			Instance.Bind ();
			Instance.SetMVP (OrthoRenderEngine.GetMVPForMesh (Type.CenteredScaledMeshMatirx*ItemMatrix, x+4, y+4, width-8, height-8));
			Instance.SetModelspaceMatrix (OrthoRenderEngine.DefaultModelspace);
			Instance.SetColor (KnownMetal.GetColor(Metal));
			Instance.SetMaps (sharpnessMap);
			Type.Mesh.Draw ();
		}

		static readonly Vector4 diamondColor = new Vector4 (80/255f, 200/255f, 120/255f, 0.5f);
		static readonly Vector4 sharperDiamondColor = new Vector4 (204/255f, 65/255f, 75/255f, 0.5f);
		static readonly Vector4 sharpDiamondColor = new Vector4 (diamondColor.Xyz, 1);
		public void RenderBlade(Matrix4 VP, float x, float y, float z, float zRot, float[] heat, Vector3 eyePos)
		{
			Matrix4 modelspace = Type.MeshScaleMatrix * Matrix4.CreateRotationZ (zRot) * Matrix4.CreateTranslation (x, y, z);
			BladeShader Instance = BladeShader.Instance;
			Instance.Bind ();
			Instance.SetModelspaceMatrix (modelspace);
			Instance.SetMVP (modelspace * VP);
			Instance.SetColor (KnownMetal.GetColor (Metal));
			UpdateHeatMap (heat);
			Instance.SetMaps (sharpnessMap, heatMap);
			Instance.SetEyePos (eyePos);
			Type.Mesh.Draw ();

			BasicShader Instance0 = BasicShader.Instance;
			Instance0.Bind ();
			for (int i = 0; i < Type.Points.Length; i++) {
				Matrix4 diamondModelspace = Matrix4.CreateTranslation (Type.Points [i], -0.15f + (float)Math.Sin (Time.CurrentTime () * 4 + Type.Points [i] * Util.PI) * 0.03f, 0) * modelspace;
				Instance0.SetModelspaceMatrix (diamondModelspace);
				Instance0.SetMVP (diamondModelspace * VP);
				Instance0.SetColor (Sharpness[i]<0.95f?diamondColor*(1-Sharpness[i])+sharperDiamondColor*Sharpness[i]:sharpDiamondColor);
				MeshCollection.Diamond.Draw ();
			}
		}

		public override uint GetSize()
		{
			return 2;
		}

		public string GetTooltipName()
		{
			return KnownMetal.GetName (Metal) + Localization.GetLocalization("ui.blade."+Type.Name) + Localization.GetLocalization("ui.item.blade");
		}

		public Vector4 GetTooltipColor()
		{
			return KnownMetal.GetColor(Metal);
		}

		public override void DrawTooltip(TextWriter writer)
		{
			string subtitle = KnownMetal.Metals [Metal].Subtitle;
			if (subtitle != null)
				writer.DrawStandardTooltip (new []{ GetTooltipName (), null, Localization.GetLocalization ("ui.tooltip.purity:") }, 
					new [] { Util.GetColorFromVector (GetTooltipColor ()) },
					new [] { null, subtitle, (int)(Purity * 100 + 0.5f) + "%" });
			else
				writer.DrawStandardTooltip (new []{ GetTooltipName (), Localization.GetLocalization ("ui.tooltip.purity:") }, 
					new [] { Util.GetColorFromVector (GetTooltipColor ()) },
					new [] { null, (int)(Purity * 100 + 0.5f) + "%" });
		}

		const int sharpnessPixelCount = 16;
		const int sharpnessPixelSize = 1;
		public void UpdateSharpnessMap()
		{
			var buffer = new byte[sharpnessPixelSize*sharpnessPixelCount];
			for (int i = 0; i < Sharpness.Length; i++) {
				int pixel = Math.Min (sharpnessPixelCount-1, Math.Max (0, (int)(Type.Points [i] * sharpnessPixelCount))) * sharpnessPixelSize;
				buffer [pixel]   = (byte)(Sharpness [i]*255);
			}

			GL.BindTexture (TextureTarget.Texture1D, sharpnessMap);
			GL.TexImage1D<byte>(TextureTarget.Texture1D, 0, PixelInternalFormat.R8, sharpnessPixelCount, 0, PixelFormat.Red, PixelType.UnsignedByte, buffer);
		}

		const int heatPixelCount = 64;
		const int heatPixelSize = 2;
		public void UpdateHeatMap(float[] heat)
		{
			GL.BindTexture (TextureTarget.Texture1D, heatMap);
			var buffer = new byte[heatPixelSize * heatPixelCount];
			for (int i = 0; i < heatPixelCount; i++) {
				float pHeat = 0;
				for (int j = 0; j < heat.Length; j++) {
					pHeat += Math.Max (0, heat [j] * (1 - 8 * Math.Abs (Type.Points [j] - (i / (heatPixelCount - 1f)))));
				}
				buffer [i * heatPixelSize] = (byte)(KnownMetal.GetRedEmmission (Metal, pHeat) * 25);
				buffer [i * heatPixelSize + 1] = (byte)(KnownMetal.GetGreenEmmission (Metal, pHeat) * 25);
			}
			GL.TexImage1D<byte> (TextureTarget.Texture1D, 0, PixelInternalFormat.Rg8, heatPixelCount, 0, PixelFormat.Rg, PixelType.UnsignedByte, buffer);
		}

		public override void LoadInfoFromFile(Stream reader)
		{
			Type = BladeTypes [reader.ReadByte ()]; //Type.Id
			Metal = reader.ReadByte(); //MetalId
			Sharpness = new float[Type.Points.Length];
			var buffer = new byte[sizeof(float)*Sharpness.Length];
			reader.Read (buffer, 0, buffer.Length);
			for (int i = 0; i < Sharpness.Length; i++)
				Sharpness [i] = BitConverter.ToSingle (buffer, sizeof(float) * i); //Shaprness [0-Type.Points.Length]
			UpdateSharpnessMap ();
			reader.Read (buffer, 0, sizeof(float));
			Purity = BitConverter.ToSingle (buffer, 0); //Purity
		}

		public override void SaveInfoToFile(Stream writer)
		{
			writer.WriteByte ((byte)Type.Id);  //Type.Id
			writer.WriteByte ((byte)Metal); //MetalId
			for(int i = 0; i < Sharpness.Length; i++)
				writer.Write(BitConverter.GetBytes(Sharpness[i]), 0, sizeof(float)); //Sharpness [0-Sharpness.Length]
			writer.Write (BitConverter.GetBytes (Purity), 0, sizeof(float)); //Purity
		}
	}
}

