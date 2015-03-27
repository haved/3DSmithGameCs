using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Smith2D
{
	public class Smith2DGame
	{
		private const float fov = (float)(30f / 180f * Math.PI);
		Matrix4 projectionMatrix;

		Scene currentScene;
		PlayerEntity player;

		public Smith2DGame ()
		{
			currentScene = new Scene ();
			player = new PlayerEntity (new Mesh ("../../res/player.ply"), 2.6f, 1.7f);
			currentScene.AddEntity (player);
			currentScene.AddEntity(new MeshEntity(new Mesh("../../res/floor.ply")));
			currentScene.AddEntity(new InteractiveEntity(new Mesh("../../res/table.ply"), -4, 3, 0, 3, 2));
			currentScene.AddEntity(new InteractiveEntity(new Mesh("../../res/anvil.ply"), 5, 3, 0, 3, 1));
			BasicShader.GetInstance ().Bind ();
		}

		public void Update()
		{
			currentScene.Update ();
		}

		public void Render()
		{
			currentScene.Render (Matrix4.LookAt (new Vector3 (player.pos.X, player.pos.Y - 20, 60), new Vector3 (player.pos.X, player.pos.Y, 6.6f), new Vector3 (0, 1, 0))*projectionMatrix);
		}

		public void OnResize(int width, int height)
		{
			GL.Viewport (0, 0, width, height);
			projectionMatrix = Matrix4.CreatePerspectiveFieldOfView (fov, width / (float)height, 0.1f, 100);
		}
	}
}

