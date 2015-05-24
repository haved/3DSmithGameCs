using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class Smith2DGame
	{
		private const float PI = (float)Math.PI;
		private const float fov = (40f / 180f * PI);
		public Matrix4 ProjectionMatrix;

		public View CurrentView;

		public GameInfo GameStats;
		public Scene CurrentScene;
		public PlayerEntity Player;

		public void Init ()
		{
			Console.WriteLine ("GL version: " + GL.GetString (StringName.Version));
			TextureCollection.Load ();
			MeshCollection.Load ();
			BasicShader.MakeInstance ().Bind ();
			ColorShader.MakeInstance ();
			TextureShader.MakeInstance ();
			OrthoRenderEngine.Init ();

			NewGame ();
		}

		private void NewGame()
		{
			CurrentView = new SmithingView (this);
			GameStats = new GameInfo ();
			GameStats.NewGame ();

			CurrentScene = new Scene ();
			CurrentScene.AddEntity (new MeshEntity (new Mesh ("../../res/floor.ply")));
			CurrentScene.AddEntity (new HatchEntity (this, new Mesh ("../../res/hatchHole.ply"), new Mesh ("../../res/hatch.ply"), new Vector3 (-2.85f, 0, 0), -1, 0, 0, 4, 4));
			Player = new PlayerEntity (4, 0, new Mesh ("../../res/player.ply"), new Mesh ("../../res/shadow.ply"), new Vector4 (1, 1, 1, 0.5f), 4, 3);
			CurrentScene.AddEntity (Player);
			Mesh wallMesh = new Mesh ("../../res/wall.ply");
			CurrentScene.AddEntity (new MeshEntity (wallMesh, 0, 10, 0, -0.2f, 0, 0, 30, 1));
			CurrentScene.AddEntity (new MeshEntity (wallMesh, 0, -10, 0, -0.2f, 0, PI, 30, 1));
			CurrentScene.AddEntity (new MeshEntity (wallMesh, 15, 0, 0, -0.2f, 0, -PI / 2, 1, 20));
			CurrentScene.AddEntity (new MeshEntity (wallMesh, -15, 0, 0, -0.2f, 0, PI / 2, 1, 20));
			CurrentScene.AddEntity (new AnvilEntity (this, new Mesh ("../../res/anvil.ply"), -15 + 9, 9, 0, 8, 3));
			CurrentScene.AddEntity (new CoalTableEntity (new Mesh ("../../res/coalTableSmall.ply"), new Mesh ("../../res/coalSmall.ply"), 1, -8.5f, 0, 6, 3));
			CurrentScene.AddEntity (new InteractiveEntity (null, new Mesh ("../../res/table.ply"), 11.5f, -8, 0, 7, 4));
			CurrentScene.AddEntity (new FoundryEntity (this, new Mesh ("../../res/foundry.ply"), new Mesh("../../res/foundryMoltenMetal.ply"), Matrix4.CreateTranslation(0, 0, 2.99f), -11.5f, -6.5f, FoundryMeshInfo.CreateIngotMatrices(), 7, 7));
			CurrentScene.AddEntity (new BellowEntity(this, new Mesh("../../res/bellow.ply"), Matrix4.CreateTranslation(-3, 0, 1.28f), -5, -8.5f, 0, 6, 3));
			CurrentScene.AddEntity (new CastingTableEntity (this, new Mesh ("../../res/castingTable.ply"), -11.5f, -1f, 1.85f, 6.25f, 3.3f));
			CurrentScene.AddEntity (new MailboxEntity(this, new Mesh("../../res/mailboxBox.ply"),new Mesh("../../res/mailboxLid.ply"), Matrix4.CreateTranslation(0, .8f, 3.64f), 12.5f, 9.5f, 2.4f, 3, 2));
		}

		public void Update ()
		{
			CountFPS ();

			if (CurrentView.ShouldUpdateScene ())
				CurrentScene.Update ();
			CurrentView.UpdateView (CurrentScene);
		}

		public void Render ()
		{
			if (CurrentView.ShouldRenderScene ())
				CurrentScene.Render (Matrix4.LookAt (CurrentView.GetEyePos (), CurrentView.GetEyeTarget (), CurrentView.GetEyeUp ()) * ProjectionMatrix);
			CurrentView.RenderView (CurrentScene);
		}

		public void OnResize (int width, int height)
		{
			GL.Viewport (0, 0, width, height);
			ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView (fov, width / (float)height, 0.1f, 100);
			OrthoRenderEngine.OnResize (width, height);
		}

		public void SetView (View v)
		{
			v.OnViewUsed (this.CurrentView);
			this.CurrentView = v;
		}

		int frames;
		float timeTotal;

		private void CountFPS()
		{
			timeTotal += Time.Delta ();
			frames++;

			if (timeTotal > 2) {
				Console.WriteLine ("FPS:" + (frames / 2));
				frames = 0;
				timeTotal = 0;
			}
		}

		public static void Main (string[] args)
		{
			using (SmithGameWindow gw = new SmithGameWindow (new Smith2DGame ())) {
				gw.Run (60.00);
			}
		}
	}
}

