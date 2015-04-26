using System;
using OpenTK;

namespace DSmithGameCs
{
	public class CoalTableEntity : InteractiveEntity
	{
		readonly Mesh table, coal;
		Vector4 coalColor = new Vector4(1,1,1,1);
		float coalValue = 1;
		float coalSpeed = 0;
		public CoalTableEntity (Mesh table, Mesh coal, float x, float y, float z, float xSize, float ySize) : base(null, x,y,z,xSize,ySize)
		{
			this.table = table;
			this.coal = coal;
		}

		float flickr = 0;
		public override void Update(Scene s)
		{
			if (coalSpeed > 0)
				coalSpeed += Time.delta ();
			else
				coalSpeed -= Time.delta ();

			//coalValue += coalSpeed * Time.delta();

			if (coalValue > 2)
				coalSpeed = -0.1f;
			else if (coalValue < 0)
				coalSpeed = 0.1f;

			flickr += Time.delta ()*3*Util.NextFloat();
			coalColor.X = coalColor.Y = coalColor.Z = coalValue + (float)Math.Sin(flickr)/4;
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			Matrix4 MVP = modelspace * VP;
			BasicShader.GetInstance ().SetModelspaceMatrix (modelspace);
			BasicShader.GetInstance ().SetMVP (MVP);
			table.Draw ();
			ColorShader.GetInstance ().Bind ();
			ColorShader.GetInstance ().SetMVP (MVP);
			ColorShader.GetInstance ().SetColor (coalColor);
			coal.Draw ();
			BasicShader.GetInstance ().Bind ();
		}
	}
}

