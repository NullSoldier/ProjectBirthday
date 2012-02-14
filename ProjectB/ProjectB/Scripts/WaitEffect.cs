using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using ProjectB.States;

namespace ProjectB.Scripts
{
	public class WaitEffect
		: BaseEffect
	{
		public WaitEffect (float time, Action action = null)
			: base (action)
		{
			this.delay = time;
		}

		public override void Update (GameTime gameTime)
		{
		}

		public override void Start (GameState gameState)
		{
			timer = new Timer (waitFinished, null, (int)(delay * 1000), Timeout.Infinite);
		}

		private Timer timer;
		private float delay;

		private void waitFinished (object state)
		{
			Finish();
		}
	}
}
