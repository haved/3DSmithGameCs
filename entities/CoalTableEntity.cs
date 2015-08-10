using System;
using OpenTK;

namespace DSmithGameCs
{
	public class CoalTableEntity : MeshEntity
	{
		PointLight light = new PointLight(Vector3.Zero, new Vector3(1, -7.5f, 2.2f), 5, 6, 0.1f, 1f, 1.5f);

		readonly Mesh table, coal;
		Vector4 coalColor = new Vector4(1,1,1,1);
		float coalSpeed;
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

		public override void OnAddedToScene(Scene s)
		{
			s.AddLight (light);
		}

		public override void OnRemovedFromScene(Scene s)
		{
			s.RemoveLight (light);
		}

		public override void UpdateModelspaceMatrix()
		{
			light.Position = Pos + new Vector3 (0, 1, 3);
			base.UpdateModelspaceMatrix ();
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

		public override void Draw(Scene s)
		{
			table.Draw ();
		}

		public override void PostRender(Scene s, Matrix4 VP)
		{
			ColorShader.Instance.Bind ();
			ColorShader.Instance.SetMVP (Modelspace * VP);
			ColorShader.Instance.SetColor (coalColor);
			coal.Draw ();
		}
	}
}

