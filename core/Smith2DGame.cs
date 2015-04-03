﻿using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class Smith2DGame
	{
		private const float PI = (float)Math.PI;
		private const float fov = (40f / 180f * PI);
		public Matrix4 projectionMatrix;

		public View currentView;

		public Scene currentScene;
		public PlayerEntity player;

		public void Init ()
		{
			Console.WriteLine("GL version: " + GL.GetString(StringName.Version));
			currentScene = new Scene ();
			currentScene.AddEntity (new MeshEntity (new Mesh ("../../res/floor.ply")));
			currentScene.AddEntity (new HatchEntity (new Mesh ("../../res/hatchHole.ply"), new Mesh ("../../res/hatch.ply"), new Vector3 (-2.85f, 0, 0), -1, 0, 0, 4, 4));
			player = new PlayerEntity (4, 0, new Mesh ("../../res/player.ply"), new Mesh ("../../res/shadow.ply"), new Vector4 (1, 1, 1, 0.5f), 4, 3);
			currentScene.AddEntity (player);
			Mesh wallMesh = new Mesh ("../../res/wall.ply");
			currentScene.AddEntity (new MeshEntity (wallMesh, 0, 10, 0, -0.2f, 0, 0, 30, 1));
			currentScene.AddEntity (new MeshEntity (wallMesh, 0, -10, 0, -0.2f, 0, PI, 30, 1));
			currentScene.AddEntity (new MeshEntity (wallMesh, 15, 0, 0, -0.2f, 0, -PI / 2, 1, 20));
			currentScene.AddEntity (new MeshEntity (wallMesh, -15, 0, 0, -0.2f, 0, PI / 2, 1, 20));
			currentScene.AddEntity (new InteractiveEntity (null, new Mesh ("../../res/table.ply"), 5.5f, -8, 0, 7, 4));
			currentScene.AddEntity (new AnvilEntity (this, new Mesh ("../../res/anvil.ply"), -15 + 9, 9, 0, 8, 3));
			BasicShader.GetInstance ().Bind ();
			ColorShader.GetInstance ();
		}

		int frames;
		float timeTotal;
		public void Update ()
		{
			timeTotal += Time.delta ();
			frames++;

			if (timeTotal > 2) {
				Console.WriteLine ("FPS:" + (frames / 2));
				frames = 0;
				timeTotal = 0;
			}

			if (currentView != null) {
				if (currentView.ShouldUpdateScene ())
					currentScene.Update ();
				currentView.UpdateView (currentScene);
			} else
				currentScene.Update ();
		}

		public void Render()
		{
			if (currentView != null) {
				if (currentView.ShouldRenderScene ())
					currentScene.Render (currentView.GetViewMatrix () * projectionMatrix);
				currentView.RenderView (currentScene);
			}
			else
				currentScene.Render (Matrix4.LookAt (new Vector3 (player.pos.X, player.pos.Y - 10, 40), new Vector3 (player.pos.X, player.pos.Y, 1.73f), new Vector3 (0, 1, 0))*projectionMatrix);
		}

		public void OnResize (int width, int height)
		{
			GL.Viewport (0, 0, width, height);
			projectionMatrix = Matrix4.CreatePerspectiveFieldOfView (fov, width / (float)height, 0.1f, 100);
		}

		public void SetView(View v)
		{
			this.currentView = v;
		}

		public static void Main (string[] args)
		{
			using (SmithGameWindow gw = new SmithGameWindow(new Smith2DGame())) {
				gw.Run (60.00);
			}
		}
	}
}

