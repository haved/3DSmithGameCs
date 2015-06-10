using System;
using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public abstract class MenuView : View
	{
		static readonly Font font = new Font (FontFamily.GenericSansSerif, 18);

		TextWriter[] writers;
		int hovering=-1;

		protected MenuView (string[] options)
		{
			writers = new TextWriter[options.Length];
			for (int i = 0; i < options.Length; i++) {
				writers [i] = new TextWriter (font, 200, 30);
				writers [i].DrawString (options[i], (writers [i].Width - writers [i].GetLineWidth (options[i])) / 2, 0, Color.White);
			}
		}

		#region View implementation

		public void OnViewUsed (View prevView)
		{
			
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		public void UpdateView (Scene s)
		{

		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		public abstract Vector3 GetEyePos ();

		public abstract Vector3 GetEyeTarget ();

		public abstract Vector3 GetEyeUp ();

		public void RenderView (Scene s)
		{

		}

		#endregion
	}
}

