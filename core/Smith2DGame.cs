using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class Smith2DGame
	{
		const float PI = (float)Math.PI;
		const float fov = (40f / 180f * PI);

		public Matrix4 ProjectionMatrix;

		public IView CurrentView;

		public GameInfo GameStats;
		public Scene CurrentScene;
		public PlayerEntity Player;

		public TooltipHelper TooltipHelper;
		public ErrorTooltipHelper ErrortipHelper;

		Scene menuScene;
		Scene smithScene;

		public MainMenuView MainMenu;
		public PauseMenuView PauseMenu;

		public void Init ()
		{
			Console.WriteLine ("GL version: " + GL.GetString (StringName.Version));
			TextureCollection.Load ();
			MeshCollection.Load ();
			BasicShader.MakeInstance ();
			ColorShader.MakeInstance ();
			TextureShader.MakeInstance ();
			LiquidShader.MakeInstance (new Texture("../../res/textures/liquid.png"));
			BladeShader.MakeInstance ();
			OrthoRenderEngine.Init ();
			TooltipHelper = new TooltipHelper ();
			ErrortipHelper = new ErrorTooltipHelper ();
			Localization.LoadFromFile ("../../res/localization/no_NO.txt");
			MainMenu = new MainMenuView (this);
			PauseMenu = new PauseMenuView (this);

			CurrentScene = MakeMenuScene();
			CurrentView = MainMenu;
			CurrentView.OnViewUsed (null);
		}

		public void NewGame()
		{
			CurrentView = null;
			SetView(new SmithingView (this));
			GameStats = new GameInfo ();
			GameStats.NewGame ();

			CurrentScene = MakeSmithScene ();
		}

		public void LoadGame()
		{
			CurrentView = null;
			SetView(new SmithingView (this));
			GameStats = new GameInfo ();
			GameStats.LoadGame ();

			CurrentScene = MakeSmithScene ();
		}

		public Scene MakeMenuScene()
		{
			if (menuScene != null)
				return menuScene;
			Console.WriteLine ("MenuScene made");
			menuScene = new Scene ();
			menuScene.AddEntity(new MeshEntity(new Mesh("../../res/mesh/menuBG.ply")));
			return menuScene;
		}

		public Scene MakeSmithScene()
		{
			if (smithScene != null)
				return smithScene;
			Console.Out.WriteLine ("SmithScene made");
			smithScene = new Scene ();
			smithScene.AddEntity (new MeshEntity (new Mesh ("../../res/mesh/floor.ply")));
			smithScene.AddEntity (new HatchEntity (this, new Mesh ("../../res/mesh/hatchHole.ply"), new Mesh ("../../res/mesh/hatch.ply"), new Vector3 (-2.85f, 0, 0), -1, 0, 0, 4, 4));
			Player = new PlayerEntity (4, 0, new Mesh ("../../res/mesh/player.ply"), new Mesh ("../../res/mesh/shadow.ply"), new Vector4 (1, 1, 1, 0.5f), 4, 3);
			smithScene.AddEntity (Player);
			Mesh wallMesh = new Mesh ("../../res/mesh/wall.ply");
			smithScene.AddEntity (new MeshEntity (wallMesh, 0, 10, 0, -0.2f, 0, 0, 30, 1));
			smithScene.AddEntity (new MeshEntity (wallMesh, 0, -10, 0, -0.2f, 0, PI, 30, 1));
			smithScene.AddEntity (new MeshEntity (wallMesh, 15, 0, 0, -0.2f, 0, -PI / 2, 1, 20));
			smithScene.AddEntity (new MeshEntity (wallMesh, -15, 0, 0, -0.2f, 0, PI / 2, 1, 20));
			AnvilEntity anvil = new AnvilEntity (this, new Mesh ("../../res/mesh/anvil.ply"), -15 + 9, 9, 0, 8, 3);
			CoalStripTable table = new CoalStripTable(this, new Mesh("../../res/mesh/coalStripTable.ply"),new Mesh("../../res/mesh/coalStrip.ply"), -12, 6.5f, 0, 3, 6, 4.3f);
			anvil.SetCoalStripTable(table);
			table.SetAnvil(anvil);
			smithScene.AddEntity(anvil);
			smithScene.AddEntity(table);
			smithScene.AddEntity (new CastingTableEntity (this, new Mesh ("../../res/mesh/castingTable.ply"), new Mesh ("../../res/mesh/castFill.ply"), new Mesh ("../../res/mesh/moltenMetalFall.ply"), -11.5f, -1f, 1.55f, 6.25f, 3.3f));
			smithScene.AddEntity (new FoundryEntity (this, new Mesh ("../../res/mesh/foundry.ply"), new Mesh("../../res/mesh/foundryMoltenMetal.ply"), Matrix4.CreateTranslation(0, 0, 2.99f), -11.5f, -6.5f, FoundryMeshInfo.CreateIngotMatrices(), 7, 7));
			smithScene.AddEntity (new BellowEntity(this, new Mesh("../../res/mesh/bellow.ply"), Matrix4.CreateTranslation(-3, 0, 1.28f), -5, -8.5f, 0, 6, 3));
			smithScene.AddEntity (new CoalTableEntity (new Mesh ("../../res/mesh/coalTableSmall.ply"), new Mesh ("../../res/mesh/coalSmall.ply"), 1, -8.5f, 0, 6, 3));
			smithScene.AddEntity (new InteractiveEntity (null, new Mesh ("../../res/mesh/table.ply"), 11.5f, -8, 0, 7, 4));
			smithScene.AddEntity (new MailboxEntity (this, new Mesh ("../../res/mesh/mailboxBox.ply"), new Mesh ("../../res/mesh/mailboxLid.ply"), Matrix4.CreateTranslation (0, .8f, 3.64f), 12.5f, 9.5f, 2.4f, 3, 2));
			return smithScene;
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
			Matrix4 VP = Matrix4.LookAt (CurrentView.GetEyePos (), CurrentView.GetEyeTarget (), CurrentView.GetEyeUp ()) * ProjectionMatrix;
			if (CurrentView.ShouldRenderScene ())
				CurrentScene.Render (VP);
			CurrentView.RenderView (VP, CurrentScene);
			ErrortipHelper.Render ();
		}

		public void OnResize (int width, int height)
		{
			GL.Viewport (0, 0, width, height);
			ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView (fov, width / (float)height, 0.1f, 50);
			OrthoRenderEngine.OnResize (width, height);
		}

		public void SetView (IView v)
		{
			TooltipHelper.UnClaim ();
			ErrortipHelper.UnClaim ();
			v.OnViewUsed (CurrentView);
			CurrentView = v;
		}

		/*<summary>
			A SetView method perfect for a view-in-the-middle attack
		</summary>*/
		public void SetView (IView v, IView prevView)
		{
			TooltipHelper.UnClaim ();
			ErrortipHelper.UnClaim ();
			v.OnViewUsed (prevView);
			CurrentView = v;
		}

		public void ShowMainMenu()
		{
			CurrentView = null;
			SetView (MainMenu);
			CurrentScene = MakeMenuScene ();
		}

		int frames;
		float timeTotal;

		void CountFPS()
		{
			timeTotal += Time.Delta ();
			frames++;

			if (timeTotal > 2) {
				Console.WriteLine ("FPS:" + (frames / 2));
				frames = 0;
				timeTotal = 0;
			}
		}

		ICloseable closeable;
		public void SetCloser(ICloseable closeable)
		{
			this.closeable = closeable;
		}

		public void Exit()
		{
			closeable.Close ();
		}

		public static void Main (string[] args)
		{
			using (var gw = new SmithGameWindow (new Smith2DGame ())) {
				gw.Run (1, 60.00);
			}
		}
	}
}

