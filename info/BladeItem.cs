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
		public static BladeType GreatswordBlade = new BladeType(0, "Greatsword", MeshCollection.GreatswordBlade, 3.5f, 2, new []{0.3f, 0.5f, 0.7f, 0.9f});

		public class BladeType
		{
			public readonly int Id;
			public readonly string Name;
			public readonly Mesh Mesh;
			public readonly float MeshScale;
			public readonly Matrix4 MeshScaleMatrix;
			public readonly Matrix4 CenteredScaledMeshMatirx;
			public readonly float Volume;
			public readonly float[] Points;

			public BladeType(int id, string name, Mesh mesh, float meshScale, float volume, float[] points)
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

		public BladeItem(){
			sharpnessMap = GL.GenTexture ();
			Console.Out.WriteLine("Assigned texture: " + sharpnessMap);
		}

		public BladeItem (BladeType type, int metal, float purity)
		{
			Type = type;
			Metal = metal;
			Purity = purity;
			Sharpness = new float[Type.Points.Length];
			sharpnessMap = GL.GenTexture ();
			Console.Out.WriteLine("Assigned texture: " + sharpnessMap);
		}

		public override void DisposeItem()
		{
			if(GraphicsContext.CurrentContext != null)
				GL.DeleteTexture (sharpnessMap);
			Console.Out.WriteLine("Deleted texture: " + sharpnessMap);
		}

		protected static readonly Matrix4 ItemMatrix = Matrix4.CreateRotationZ (Util.PI / 2 - 0.2f) * Matrix4.CreateRotationX (0.2f) * Matrix4.CreateRotationY (-0.3f) * Matrix4.CreateTranslation (0, 0, -2.8f)
		                                               * Matrix4.CreatePerspectiveFieldOfView (0.85f, 1, 0.1f, 4) * Matrix4.CreateScale (1, 0.5f, 1);
		public override void RenderItem(float x, float y, float width, float height)
		{
			OrthoRenderEngine.DrawColoredMesh (Type.Mesh, Type.CenteredScaledMeshMatirx*ItemMatrix, KnownMetal.GetColor(Metal), x+4, y+4, width-8, height-8);
		}

		static readonly Vector4 diamondColor = new Vector4 (80/255f, 200/255f, 120/255f, 0.5f);
		static readonly Vector4 sharperDiamondColor = new Vector4 (204/255f, 65/255f, 75/255f, 0.5f);
		static readonly Vector4 sharpDiamondColor = new Vector4 (diamondColor.Xyz, 1);
		public void RenderBlade(Matrix4 VP, float x, float y, float z, float zRot, int hotspot, float tempereture)
		{
			Matrix4 modelspace = Type.MeshScaleMatrix * Matrix4.CreateRotationZ (zRot) * Matrix4.CreateTranslation (x, y, z);
			BladeShader Instance = BladeShader.Instance;
			Instance.Bind ();
			Instance.SetModelspaceMatrix (modelspace);
			Instance.SetMVP (modelspace * VP);
			Instance.SetColor (KnownMetal.GetColor (Metal));
			Instance.SetSharpnessMap (sharpnessMap);
			if (hotspot >= 0) {
				float redEmission = KnownMetal.GetRedEmmission (Metal, tempereture);
				Instance.SetHotspotEmission (Util.DefaultEmission.Xyz * redEmission);
				if (redEmission > 0) {
					Instance.SetHotspotMin (Type.Points [hotspot] - 0.12f);
					Instance.SetHotspotMax (Type.Points [hotspot] + 0.12f);
				}
			}
			Type.Mesh.Draw ();

			BasicShader Instance0 = BasicShader.Instance;
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
				DrawStandardTooltip (writer, new []{ GetTooltipName (), null, Localization.GetLocalization ("ui.tooltip.purity:") }, 
					new [] { Util.GetColorFromVector (GetTooltipColor ()) },
					new [] { null, subtitle, (int)(Purity * 100 + 0.5f) + "%" });
			else
				DrawStandardTooltip (writer, new []{ GetTooltipName (), Localization.GetLocalization ("ui.tooltip.purity:") }, 
					new [] { Util.GetColorFromVector (GetTooltipColor ()) },
					new [] { null, (int)(Purity * 100 + 0.5f) + "%" });
		}

		const int pixelCount = 16;
		public void UpdateSharpnessMap()
		{
			var buffer = new byte[2*pixelCount]; //2 colors, 16 pixels, 1 byte per color
			for (int i = 0; i < Sharpness.Length; i++) {
				int pixel = Math.Min (1, Math.Max (0, (int)(Type.Points [i] * pixelCount))) * 2;
				buffer [pixel]   = (byte)(Sharpness [i]*255);
				buffer [pixel+1] = (byte)(Sharpness [i]*255);
			}
			GL.TexImage1D(TextureTarget.Texture1D, 0, PixelInternalFormat.Rg8, pixelCount, 0, PixelFormat.Rg, PixelType.UnsignedByte, buffer); 
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

