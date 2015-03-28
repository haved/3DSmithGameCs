using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class Smith2DGame
	{
		private const float PI = (float)Math.PI;
		private const float fov = (25f / 180f * PI);
		Matrix4 projectionMatrix;

		Scene currentScene;
		PlayerEntity player;

		public void Init ()
		{
			Console.WriteLine("GL version: " + GL.GetString(StringName.Version));
			currentScene = new Scene ();
			player = new PlayerEntity (new Mesh ("../../res/player.ply"), 2.6f, 1.7f);
			currentScene.AddEntity (player);
			currentScene.AddEntity(new MeshEntity(new Mesh("../../res/floor.ply")));
			Mesh wallMesh = new Mesh("../../res/wall.ply");
			currentScene.AddEntity(new MeshEntity(wallMesh, 0, 10, 0, 0, 0, 0, 40, 1));
			currentScene.AddEntity(new MeshEntity(wallMesh, 0, -10, 0, 0, 0, PI, 40, 1));
			currentScene.AddEntity(new MeshEntity(wallMesh, 15, 0, 0, 0, 0, -PI/2, 1, 30));
			currentScene.AddEntity(new MeshEntity(wallMesh, -15, 0, 0, 0, 0, PI/2, 1, 30));
			currentScene.AddEntity(new InteractiveEntity(new Mesh("../../res/table.ply"), -3, 9, 0, 3, 2));
			currentScene.AddEntity(new InteractiveEntity(new Mesh("../../res/anvil.ply"), 3, 9, 0, 3, 1));
			currentScene.AddEntity(new InteractiveEntity(new Mesh("../../res/forge.ply"), 11.5f, 10-3.5f, 0, 7.3f, 7.3f));
			BasicShader.GetInstance ().Bind ();
		}

		int frames;
		float timeTotal;
		public void Update()
		{
			timeTotal += Time.delta ();
			frames++;

			if (timeTotal > 2) {
				Console.WriteLine ("FPS:" + (frames / 2));
				frames = 0;
				timeTotal = 0;
			}
			currentScene.Update ();
		}

		public void Render()
		{
			currentScene.Render (Matrix4.LookAt (new Vector3 (player.pos.X, player.pos.Y - 15, 60), new Vector3 (player.pos.X, player.pos.Y, 6.6f), new Vector3 (0, 1, 0))*projectionMatrix);
		}

		public void OnResize(int width, int height)
		{
			GL.Viewport (0, 0, width, height);
			projectionMatrix = Matrix4.CreatePerspectiveFieldOfView (fov, width / (float)height, 0.1f, 100);
		}

		public static void Main(string[] args)
		{
			using (SmithGameWindow gw = new SmithGameWindow(new Smith2DGame())) {
				gw.Run (60.00);
			}
		}
	}
}

