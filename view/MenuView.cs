using System;
using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public abstract class MenuView : IView
	{
		const float barXPos = 100;
		const int barWidth=240;
		const int optionHeight = 40;
		static readonly Vector4 gradiantColor = new Vector4(0, 0, 0, 0.7f);
		static readonly Font font = new Font (FontFamily.GenericSansSerif, 18*2);

		readonly string[] options;
		readonly TextWriter[] writers;
		int hovering=-1;

		int languageID = -1;
		protected MenuView (string[] options)
		{
			this.options = options;
			writers = new TextWriter[options.Length];
			for (int i = 0; i < options.Length; i++) {
				writers [i] = new TextWriter (font, barWidth, optionHeight);
				string s = Localization.GetLocalization (options [i]);
				writers [i].DrawString (s, (barWidth - writers [i].GetLineWidth (s)) / 2, 0, Color.White);
			}
			languageID = Localization.GetCurrentLanguageID ();
		}

		#region View implementation

		protected IView PrevView;
		public void OnViewUsed (IView prevView)
		{
			PrevView = prevView;
			if (Localization.GetCurrentLanguageID () != languageID) {
				for (int i = 0; i < options.Length; i++) {
					writers [i].Clear ();
					string s = Localization.GetLocalization (options [i]);
					writers [i].DrawString (s, (barWidth - writers [i].GetLineWidth (s)) / 2, 0, Color.White);
				}
				languageID = Localization.GetCurrentLanguageID ();
			}
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		public void UpdateView (Scene s)
		{
			if (hovering != -16 & Input.MousePressed) {
				OnButtonPressed (hovering);
			}
		}

		public abstract void OnButtonPressed (int button);

		public bool ShouldRenderScene ()
		{
			return true;
		}

		public abstract Vector3 GetEyePos ();

		public abstract Vector3 GetEyeTarget ();

		public abstract Vector3 GetEyeUp ();

		public void RenderView (Matrix4 VP, Scene s)
		{
			if (PrevView != null)
				PrevView.RenderView (VP, s);
			OrthoRenderEngine.DrawColorOnEntireScreen (gradiantColor);
			OrthoRenderEngine.DrawColoredBox (Vector4.UnitW, barXPos, 0, barWidth, OrthoRenderEngine.GetCanvasHeight());
			float maxY = OrthoRenderEngine.GetCanvasHeight () * 0.55f;
			float x = Input.OrthoMouseX - barXPos;
			float y = maxY+optionHeight-Input.OrthoMouseY;
			if (x > 0 & x < barWidth & y > 0 & y < writers.Length * optionHeight)
				hovering = (int)y / optionHeight;
			else
				hovering = -1;
			for (int i = 0; i < writers.Length; i++)
				OrthoRenderEngine.DrawColoredTexturedBox (hovering==i ? Util.White : Util.White60, writers[i].GetTextureID(), barXPos, maxY-optionHeight*i, writers[i].Width, writers[i].Height);
		}

		#endregion
	}
}

