using System;
using System.Drawing;
using System.IO;
using OpenTK;

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

		public BladeItem(){
		}

		public BladeType Type { get; private set; }
		public int Metal { get; private set; }
		public float Purity { get; private set; }

		public BladeItem (BladeType type, int metal, float purity)
		{
			Type = type;
			Metal = metal;
			Purity = purity;
		}

		protected static readonly Matrix4 ItemMatrix = Matrix4.CreateRotationZ(Util.PI/2-0.2f)*Matrix4.CreateRotationX(0.2f)*Matrix4.CreateRotationY(-0.3f)*Matrix4.CreateTranslation(0, 0, -2.8f)
			*Matrix4.CreatePerspectiveFieldOfView(0.85f, 1, 0.1f, 4);
		public override void RenderItem(float x, float y, float width, float height)
		{
			OrthoRenderEngine.DrawColoredMesh (Type.Mesh, Type.CenteredScaledMeshMatirx*ItemMatrix, KnownMetal.GetColor(Metal), x+4, y+4, width-8, height-8, -1, -2, 2, 4);
		}

		static readonly Vector4 diamondColor = new Vector4 (80/255f, 200/255f, 120/255f, 0.5f);
		public void RenderBlade(Matrix4 VP, float x, float y, float z, float zRot)
		{
			BasicShader Instance = BasicShader.GetInstance ();
			Instance.Bind ();
			Matrix4 modelspace = Type.MeshScaleMatrix * Matrix4.CreateRotationZ(zRot) * Matrix4.CreateTranslation (x, y, z);
			Instance.SetModelspaceMatrix(modelspace);
			Instance.SetMVP (modelspace * VP);
			Instance.SetColor (KnownMetal.GetColor(Metal));
			Type.Mesh.Draw ();
			for (int i = 0; i < Type.Points.Length; i++) {
				Matrix4 diamondModelspace = modelspace * Matrix4.CreateTranslation (-Type.Points[i]*Type.MeshScale, Type.MeshScale*0.15f+(float)Math.Sin(Time.CurrentTime()*4)*0.03f, 0);
				Instance.SetModelspaceMatrix (diamondModelspace);
				Instance.SetMVP (diamondModelspace * VP);
				Instance.SetColor (diamondColor);
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
			writer.Clear ();
			string name = GetTooltipName ();
			string purityText = Localization.GetLocalization ("ui.tooltip.purity:");
			string purityValue = (int)(Purity * 100 + 0.5f) + "%";

			float purityValudeWidth = writer.GetLineWidth (purityValue);
			int width = Math.Max((int)writer.GetLineWidth(name), (int)(writer.GetLineWidth(purityText)+purityValudeWidth));

			writer.Resize (width, (int)writer.GetLineHeight ()*2);

			Color c = Util.GetColorFromVector (GetTooltipColor ());
			writer.DrawString (name, 0, 0, c);
			writer.DrawString (purityText, 0, writer.GetLineHeight(), c);
			writer.DrawString (purityValue, writer.Width-purityValudeWidth, writer.GetLineHeight(), Color.White);
		}

		public override void LoadInfoFromFile(Stream reader)
		{
			Type = BladeTypes [reader.ReadByte ()]; //Type.Id
			Metal = reader.ReadByte(); //MetalId
			var buffer = new byte[sizeof(float)];
			reader.Read (buffer, 0, buffer.Length);
			Purity = BitConverter.ToSingle (buffer, 0); //Purity
		}

		public override void SaveInfoToFile(Stream writer)
		{
			writer.WriteByte ((byte)Type.Id);  //Type.Id
			writer.WriteByte ((byte)Metal); //MetalId
			writer.Write (BitConverter.GetBytes (Purity), 0, sizeof(float)); //Purity
		}
	}
}

