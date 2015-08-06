using System;
using OpenTK;

namespace DSmithGameCs
{
	public class FloorEntity : Entity
	{
		readonly Texture texture;
		readonly float sampleUSize, sampleVSize;
		readonly float xSize;
		readonly float ySize;

		public FloorEntity (Texture texture, float sampleUSize, float sampleVSize, float x, float y, float z, float xSize, float ySize)
		{
			this.texture = texture;
			this.sampleUSize = sampleUSize;
			this.sampleVSize = sampleVSize;
			Pos.X = x;
			Pos.Y = y;
			Pos.Z = z;
			this.xSize = xSize;
			this.ySize = ySize;

			UpdateModelspaceMatrix ();
		}

		public override void DisposeEntity()
		{
			texture.Dispose ();
		}
			
		static readonly Matrix4 bias = Matrix4.CreateTranslation(-0.5f, -0.5f, 0);
		public override void UpdateModelspaceMatrix()
		{
			Modelspace = bias * Matrix4.CreateRotationX (Rot.X) * Matrix4.CreateRotationY (Rot.Y) * Matrix4.CreateRotationZ (Rot.Z) * Matrix4.CreateTranslation (Pos) * Matrix4.CreateScale(xSize, ySize, 1);
		}

		public override void Update(Scene s){}

		public override void PostRender(Scene s, Matrix4 VP)
		{
			TextureShader Instance = TextureShader.Instance;
			Instance.Bind ();
			Instance.SetTexture (texture);
			Instance.SetMVP (Modelspace * VP);
			Instance.ResetColor ();
			Instance.SetSampleUV (0, 0);
			Instance.SetSampleSize (sampleUSize, sampleVSize);
			OrthoRenderEngine.Flat.Draw ();
		}
	}
}

