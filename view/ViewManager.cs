using System;

namespace DSmithGameCs
{
	public class ViewManager
	{
		View currentView;

		public ViewManager (View defaultView)
		{
			this.defaultView = defaultView;
			this.currentView = defaultView;
		}

		public void SetView(View view)
		{
			this.currentView = view;
		}

		public void UseDefaultView()
		{
			SetView (defaultView);
		}

		public void Update(Scene s)
		{
			currentView.Update (this, s);
		}

		public void Render(Scene s)
		{
			currentView.Render (this, s);
		}
	}
}

