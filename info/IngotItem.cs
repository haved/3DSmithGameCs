using System;
using System.IO;
using OpenTK;

namespace DSmithGameCs
{
	public class IngotItem : Item
	{
		IMetal metal;
		float solid;

		public IngotItem(){}

		public IngotItem (IMetal metal)
		{
			this.metal = metal;
			solid = 1;
		}

		protected static readonly Matrix4 ItemMatrix = Matrix4.CreateRotationX(-1f)*Matrix4.CreateRotationY(0.6f)*Matrix4.CreateRotationZ(0.4f)*Matrix4.CreateTranslation(0.1f, 0, -2)
			*Matrix4.CreatePerspectiveFieldOfView(0.6f, 16f/9, 0.1f, 4);
		public override void RenderItem(float x, float y, float width, float height)
		{
			OrthoRenderEngine.DrawColoredMesh (MeshCollection.Ingot, ItemMatrix, metal.GetColor(), x+4, y+4, width-8, height-8, -1, -2, 2, 4);
		}

		public void RenderMesh(Matrix4 location, Matrix4 VP)
		{
			BasicShader.GetInstance ().SetColor (metal.GetColor ());
			BasicShader.GetInstance ().SetModelspaceMatrix (location);
			BasicShader.GetInstance ().SetMVP (location*VP);
			MeshCollection.Ingot.Draw();
			BasicShader.GetInstance ().ResetColor ();
		}

		public override uint GetSize()
		{
			return 1;
		}

		public Vector4 GetColor()
		{
			return metal.GetColor ();
		}

		public override string GetTooltipName()
		{
			return metal.GetName() + Localization.GetLocalization("ui.item.ingot");
		}

		public override Vector4 GetTooltipColor()
		{
			return metal.GetColor();
		}


		public float Melt(float temprature)
		{
			float melt = Time.Delta()*(.1f+(temprature - metal.GetMeltingPoint ())/100);
			if (melt<0)
				return -1;

			if (solid - melt < 0) {
				melt = solid;
				solid = 0;
			}
			else
				solid -= melt;

			return melt;
		}

		public float GetSolidProgress()
		{
			return solid;
		}

		public float GetMeltingProgress()
		{
			return 1 - solid;
		}

		public IMetal GetMetal()
		{
			return metal;
		}

		public override void LoadInfoFromFile(Stream reader)
		{
			metal = StreamIO.LoadMetal (reader); 		//metal
			byte[] buffer = new byte[sizeof(float)];
			reader.Read (buffer, 0, buffer.Length);  	//solid
			solid = BitConverter.ToSingle (buffer, 0);
		}

		public override void SaveInfoToFile(Stream writer)
		{
			StreamIO.SaveMetal (metal, writer);								//metal
			writer.Write (BitConverter.GetBytes(solid), 0, sizeof(float)); 	//solid
		}
	}
}

