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
			ForShadowDirShader.MakeInstance ();
			ForPointShader.MakeInstance ();
			ShadowGenShader.MakeInstance ();
			ColorShader.MakeInstance ();
			TextureShader.MakeInstance ();
			LiquidShader.MakeInstance (new Texture(Util.PATH+"res/textures/liquid.png"));
			BladeShader.MakeInstance ();
			OrthoRenderEngine.Init ();
			TooltipHelper = new TooltipHelper ();
			ErrortipHelper = new ErrorTooltipHelper ();
			Localization.LoadFromFile (Util.PATH+"res/localization/no_NO.txt");
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
			var loadedGame = new GameInfo ();
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
			if (GameStats != null) {
				GameStats.Dispose ();
				GameStats = null;
			}
			if (smithScene != null) {
				smithScene.Dispose ();
				smithScene = null;
			}
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
			menuScene.AddEntity(new MeshEntity(new Mesh(Util.MESHPATH+"menuBG.plybin")));
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

			smithScene.AddLight (new ShadowDirectionalLight(Util.White3, 1f, -Vector3.UnitZ, 2048));
			smithScene.AddLight (new DirectionalLight(Util.White3, 0.4f, Vector3.UnitZ));

			smithScene.AddEntity (new MeshEntity (new Mesh (Util.MESHPATH+"house/floorWalls/floor.plybin")));
			smithScene.AddEntity (new HatchEntity (this, new Mesh (Util.MESHPATH+"house/hatch/hatchHole.plybin"), new Mesh (Util.MESHPATH+"house/hatch/hatch.plybin"), new Vector3 (-2.85f, 0, 0), 0, 1, 0, 4, 4));
			Player = new PlayerEntity (4, 0, new Mesh (Util.MESHPATH+"player.plybin"), new Vector4 (1, 1, 1, 0.5f), 4, 3);
			smithScene.AddEntity (Player);

			var longWallMesh = new Mesh (Util.MESHPATH+"house/floorWalls/longWall.plybin");
			smithScene.AddEntity (new MeshEntity (longWallMesh, -16f, 0, 0, -0.2f, 0, PI / 2, 1, 24));
			smithScene.AddEntity (new MeshEntity (longWallMesh, 0, 12f, 0, -0.2f, 0, 0, 32, 1));
			smithScene.AddEntity (new MeshEntity (new Mesh (Util.MESHPATH+"house/floorWalls/cornerWall.plybin"), 16, 0, 0, 0, 0, 0, 1, 32));
			smithScene.AddEntity (new MeshEntity (new Mesh(Util.MESHPATH+"house/floorWalls/extraLongWall.plybin"), 3.5f, -11.5f, 0, -0.2f, 0, PI, 40, 0.2f));

			smithScene.AddEntity (new FoundryEntity (this, new Mesh (Util.MESHPATH+"house/foundry/foundry.plybin"), new Mesh(Util.MESHPATH+"house/foundry/foundryMetal.plybin"), Matrix4.CreateTranslation(0, 0, 2.49f), new Mesh (Util.MESHPATH+"house/foundry/foundryFall.plybin"), new Vector3(1.5f, 2, 1.5f), 11, -8, FoundryMeshInfo.CreateIngotMatrices(), 7, 7));
			smithScene.AddEntity (new BigBellowEntity(this, new Mesh(Util.MESHPATH+"house/bellow/bigBellow.plybin"), Matrix4.CreateTranslation(0, -4, 4.32f), new Mesh(Util.MESHPATH+"house/bellow/bigBellowRig.plybin"), 18, -5.5f, 0, 7, 12));
			smithScene.AddEntity (new CastingTableEntity (this, new Mesh (Util.MESHPATH+"house/castingTable/castingTable.plybin"), new Mesh (Util.MESHPATH+"house/castingTable/castingTableFill.plybin"), 2, -9, 8, 1.55f, 8.26f, 3.3f));

			smithScene.AddEntity (new MeshEntity(new Mesh(Util.MESHPATH+"house/table.plybin"), -13.8f, -7.8f, 0, 4, 7));

			var anvil = new AnvilEntity (this, new Mesh (Util.MESHPATH+"house/anvil/anvil.plybin"), new Mesh (Util.PATH+"res/meshes/house/anvil/hammer.plybin"), -8, 9.7f, 0, 8, 3, 3f);
			var table = new CoalStripTable(this, new Mesh(Util.MESHPATH+"house/coalStripTable/coalstriptable.plybin"), new Mesh(Util.PATH+"res/meshes/house/coalStripTable/coalStrip.plybin"), -14, 7.5f, 0, 3, 6, 3.5f);
			anvil.SetCoalStripTable(table);
			table.SetAnvil(anvil);
			smithScene.AddEntity(anvil);
			smithScene.AddEntity(table);

			smithScene.AddEntity (new FloorEntity(new Texture(Util.PATH+"res/textures/brickTiles.png"), 16, 10, 0, 0, -1, 60, 40));

			/*var anvil = new AnvilEntity (this, new Mesh (Util.PATH+"res/mesh/anvil.plybin"), -15 + 9, 8.7f, 0, 8, 3, 3f);
			var table = new CoalStripTable(this, new Mesh(Util.PATH+"res/mesh/coalStripTable.plybin"),new Mesh(Util.PATH+"res/mesh/coalStrip.plybin"), -12, 6.5f, 0, 3, 6, 3.5f);
			anvil.SetCoalStripTable(table);
			table.SetAnvil(anvil);
			smithScene.AddEntity(anvil);
			smithScene.AddEntity(table);
			smithScene.AddEntity (new BellowEntity(this, new Mesh(Util.PATH+"res/mesh/bellow.plybin"), Matrix4.CreateTranslation(-3, 0, 1.28f), -5, -8.5f, 0, 6, 3));
			smithScene.AddEntity (new CoalTableEntity (new Mesh (Util.PATH+"res/mesh/coalTableSmall.plybin"), new Mesh (Util.PATH+"res/mesh/coalSmall.plybin"), 1, -8.5f, 0, 6, 3));
			smithScene.AddEntity (new MeshEntity (new Mesh (Util.PATH+"res/mesh/table.plybin"), 11.5f, -8, 0, 7, 4));
			smithScene.AddEntity (new MailboxEntity (this, new Mesh (Util.PATH+"res/mesh/mailboxBox.plybin"), new Mesh (Util.PATH+"res/mesh/mailboxLid.plybin"), Matrix4.CreateTranslation (0, .8f, 3.64f), 12.5f, 9.5f, 2.4f, 3, 2));
			*/
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
			if (args.Length > 0) {
				MainPlyBinary.Main (args);
				return;
			}

			using (var gw = new SmithGameWindow (new Smith2DGame ())) {
				gw.Run ();
			}
		}
	}
}
