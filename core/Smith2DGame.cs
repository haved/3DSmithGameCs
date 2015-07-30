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

		public GameInfo GameStats; // { private set { if (gameInfo != null) { gameInfo.Dispose (); } gameInfo = value; } get { return gameInfo; } }
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
			ForAmbientShader.MakeInstance ();
			ForDirectionalShader.MakeInstance ();
			ForPointShader.MakeInstance ();
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
			if (GameStats != null)
				GameStats.Dispose ();
			GameStats = new GameInfo ();
			GameStats.NewGame ();

			CurrentScene = MakeSmithScene ();
		}

		public void LoadGame()
		{
			GameInfo loadedGame = new GameInfo ();
			if (!loadedGame.LoadGame ()) {
				ErrortipHelper.ShowError (Localization.GetLocalization("ui.error.savecorruption"), Input.OrthoMouseX, Input.OrthoMouseY, 2, false);
				return;
			}
			if (GameStats != null)
				GameStats.Dispose ();
			GameStats = loadedGame;
			CurrentView = null;
			SetView(new SmithingView (this));
				
			CurrentScene = MakeSmithScene ();
		}

		public void ShowMainMenu()
		{
			if (GameStats != null)
				GameStats.Dispose ();
			GameStats = null;
			smithScene.Dispose ();
			smithScene = null;
			CurrentView = null;
			SetView (MainMenu);
			CurrentScene = MakeMenuScene ();
		}

		public Scene MakeMenuScene()
		{
			if (menuScene != null)
				return menuScene;
			Console.WriteLine ("MenuScene made");
			menuScene = new Scene ();
			menuScene.AddEntity(new MeshEntity(new Mesh("../../res/mesh/menuBG.ply")));
			menuScene.AddLight(new DirectionalLight(Util.White3, 1f, -Vector3.UnitZ));
			menuScene.AddLight (new DirectionalLight(Util.White3, 0.4f, Vector3.UnitZ));
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
			AnvilEntity anvil = new AnvilEntity (this, new Mesh ("../../res/mesh/anvil.ply"), -15 + 9, 8.7f, 0, 8, 3, 3f);
			CoalStripTable table = new CoalStripTable(this, new Mesh("../../res/mesh/coalStripTable.ply"),new Mesh("../../res/mesh/coalStrip.ply"), -12, 6.5f, 0, 3, 6, 3.5f);
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

			smithScene.AddLight (new DirectionalLight(Util.White3, 1f, -Vector3.UnitZ));
			smithScene.AddLight (new DirectionalLight(Util.White3, 0.4f, Vector3.UnitZ));
			smithScene.AddLight (new PointLight(new Vector3(1, 0, 0), new Vector3(1, -7.5f, 2.2f), 8, 12, 0.1f, 1f, 1.5f));

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
			Vector3 eyePos = CurrentView.GetEyePos ();
			Matrix4 VP = Matrix4.LookAt (eyePos, CurrentView.GetEyeTarget (), CurrentView.GetEyeUp ()) * ProjectionMatrix;
			if (CurrentView.ShouldRenderScene ())
				CurrentScene.Render (VP, eyePos);
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
			int buffer = GL.GenBuffer ();
			int texture = GL.GenTexture ();

			Console.Out.WriteLine ("Biggest GL buffer: " + buffer);
			Console.Out.WriteLine ("Biggest GL texture: " + texture);

			GL.DeleteBuffer (buffer);
			GL.DeleteTexture (texture);

			Console.In.Read ();

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
