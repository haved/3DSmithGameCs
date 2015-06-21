using System;
using System.Drawing;
using System.IO;
using OpenTK;

namespace DSmithGameCs
{
	public class IngotItem : Item
	{
		int metal;
		float purity;
		float solid;

		public IngotItem(){}

		public IngotItem(KnownMetal metal, float purity) : this(metal.Id, purity) {}

		public IngotItem (int metal, float purity)
		{
			this.metal = metal;
			this.purity = purity;
			solid = 1;
		}

		protected static readonly Matrix4 ItemMatrix = Matrix4.CreateRotationX(-1f)*Matrix4.CreateRotationY(0.6f)*Matrix4.CreateRotationZ(0.4f)*Matrix4.CreateTranslation(0.1f, 0, -2)
			*Matrix4.CreatePerspectiveFieldOfView(0.6f, 16f/9, 0.1f, 4)*Matrix4.CreateScale(1, 0.5f, 1);
		public override void RenderItem(float x, float y, float width, float height)
		{
			OrthoRenderEngine.DrawColoredMesh (MeshCollection.Ingot, ItemMatrix, Metal.Color, x+4, y+4, width-8, height-8);
		}

		public void RenderMesh(Matrix4 location, Matrix4 VP)
		{
			BasicShader.Instance.SetColor (Metal.Color);
			BasicShader.Instance.SetModelspaceMatrix (location);
			BasicShader.Instance.SetMVP (location*VP);
			MeshCollection.Ingot.Draw();
			BasicShader.Instance.ResetColor ();
		}

		public override uint GetSize()
		{
			return 1;
		}

		public Vector4 GetColor()
		{
			return Metal.Color;
		}

		public string GetTooltipName()
		{
			return Metal.Name + Localization.GetLocalization("ui.item.ingot");
		}

		public Vector4 GetTooltipColor()
		{
			return Metal.Color;
		}

		public override void DrawTooltip (TextWriter writer)
		{
			if(Metal.Subtitle==null)
				DrawStandardTooltip (writer, new []{GetTooltipName(), Localization.GetLocalization("ui.tooltip.purity:")}, new []{Util.GetColorFromVector(Metal.Color)}, new []{null, (int)(purity*100+0.5f)+"%"});
			else
				DrawStandardTooltip (writer, new []{GetTooltipName(), null, Localization.GetLocalization("ui.tooltip.purity:")}, new []{Util.GetColorFromVector(Metal.Color)}, new []{null, Metal.Subtitle, (int)(purity*100+0.5f)+"%"});
		}

		/*public override void DrawTooltip(TextWriter writer)
		{
			writer.Clear ();
			string name = GetTooltipName ();
			string purityText = Localization.GetLocalization ("ui.tooltip.purity:");
			string purityValue = (int)(purity * 100 + 0.5f) + "%";

			float purityValudeWidth = writer.GetLineWidth (purityValue);
			int width = Math.Max((int)writer.GetLineWidth(name), (int)(writer.GetLineWidth(purityText)+purityValudeWidth));

			writer.Resize (width, (int)writer.GetLineHeight ()*2);

			Color c = Util.GetColorFromVector (GetTooltipColor ());
			writer.DrawString (name, 0, 0, c);
			writer.DrawString (purityText, 0, writer.GetLineHeight(), c);
			writer.DrawString (purityValue, writer.Width-purityValudeWidth, writer.GetLineHeight(), Color.White);
		}*/

		public float Melt(float temprature)
		{
			float melt = Time.Delta()*(.1f+(temprature - Metal.MeltingPoint)/100);
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

		public int MetalID
		{
			get{ return metal; }
		}

		public float Purity{
			get{ return purity; }
		}

		public KnownMetal Metal
		{
			get { return KnownMetal.Metals [metal]; }
		}

		public override void LoadInfoFromFile(Stream reader)
		{
			metal = reader.ReadByte ();					//metal
			var buffer = new byte[sizeof(float)];
			reader.Read (buffer, 0, buffer.Length);  	//purity
			purity = BitConverter.ToSingle (buffer, 0);
			reader.Read (buffer, 0, buffer.Length);  	//solid
			solid = BitConverter.ToSingle (buffer, 0);
		}

		public override void SaveInfoToFile(Stream writer)
		{
			writer.WriteByte ((byte)metal);									//metal
			writer.Write (BitConverter.GetBytes(purity), 0, sizeof(float));	//purity
			writer.Write (BitConverter.GetBytes(solid), 0, sizeof(float)); 	//solid
		}
	}
}

