using System;
using OpenTK;

namespace DSmithGameCs
{
	public class CoalTableEntity : InteractiveEntity
	{
		readonly Mesh table, coal;
		Vector4 coalColor = new Vector4(1,1,1,1);
		float coalSpeed = 0;
		public CoalTableEntity (Mesh table, Mesh coal, float x, float y, float z, float xSize, float ySize) : base(null, x,y,z,xSize,ySize)
		{
			this.table = table;
			this.coal = coal;
		}

		public override void DisposeEntity()
		{
			table.Dispose ();
			coal.Dispose ();
		}

		float flickr = 0;
		public override void Update(Scene s)
		{
			if (coalSpeed > 0)
				coalSpeed += Time.Delta ();
			else
				coalSpeed -= Time.Delta ();

			flickr += Time.Delta ()*2*Util.NextFloat();
			coalColor.X = coalColor.Y = coalColor.Z = 1 + (float)Math.Sin(flickr)/6;
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			Matrix4 MVP = Modelspace * VP;
			BasicShader.Instance.Bind ();
			BasicShader.Instance.SetModelspaceMatrix (Modelspace);
			BasicShader.Instance.SetMVP (MVP);
			table.Draw ();
			ColorShader.Instance.Bind ();
			ColorShader.Instance.SetMVP (MVP);
			ColorShader.Instance.SetColor (coalColor);
			coal.Draw ();
		}
	}
}

