using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ProjectB.States;

namespace ProjectB.Scripts
{
	public abstract class BaseEffect
	{
		public BaseEffect (Action action)
		{
			this.action = action;
		}

		public bool Finished;
		
		public abstract void Update (GameTime gameTime);
		public abstract void Start (GameState gameState);
		
		public virtual void Finish ()
		{
			Finished = true;

			if (action != null)
				action();
		}
		
		
		private Action action;
	}
}
