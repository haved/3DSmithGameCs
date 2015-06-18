using System;
using OpenTK;

namespace DSmithGameCs
{
	public class Transition
	{
		private Vector3 prevEyePos, prevEyeTarget, prevEyeUp;
		private float transition;
		private float smoothTransition;

		public Transition()
		{
			SetTransition(1);
		}

		public void SetStart(IView prevView)
		{
			SetStart (prevView.GetEyePos(), prevView.GetEyeTarget(), prevView.GetEyeUp());
		}

		public void SetStart(Vector3 prevEyePos, Vector3 prevEyeTarget, Vector3 prevEyeUp)
		{
			this.prevEyePos = prevEyePos;
			this.prevEyeTarget= prevEyeTarget;
			this.prevEyeUp = prevEyeUp;
			SetTransition(0);
		}

		public void UpdateTransition(float speed)
		{
			if (transition < 1) {
				transition += speed;
				SetTransition(Math.Min (1, transition));
			}
		}

		private void SetTransition(float t)
		{
			transition = t;
			smoothTransition = Util.GetSmoothTransition (transition);
		}

		public bool IsDone()
		{
			return transition >= 1;
		}

		public Vector3 GetEyePos(Vector3 newEyePos)
		{
			return smoothTransition < 1 ? newEyePos * smoothTransition + prevEyePos*(1-smoothTransition) : newEyePos;
		}

		public Vector3 GetEyeTarget(Vector3 newEyeTarget)
		{
			return smoothTransition < 1 ? newEyeTarget * smoothTransition + prevEyeTarget*(1-smoothTransition) : newEyeTarget;
		}

		public Vector3 GetEyeUp(Vector3 newEyeUp)
		{
			return smoothTransition < 1 ? newEyeUp * smoothTransition + prevEyeUp * (1-smoothTransition) : newEyeUp;
		}
	}
}

