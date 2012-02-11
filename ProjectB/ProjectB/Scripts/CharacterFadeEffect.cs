using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using ProjectB.States;

namespace ProjectB.Scripts
{
	public class CharacterFadeEffect
		: BaseEffect
	{
		public CharacterFadeEffect (Character character, float start, float end, float time, Action action = null)
			: base (action)
		{
			this.startFade = start;
			this.endFade = end;
			this.timeTotal = time;
			this.character = character;
		}

		public override void Update (GameTime gameTime)
		{
			timePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

			this.character.Alpha = MathHelper.Lerp (startFade, endFade, timePassed / timeTotal);

			if (timePassed >= timeTotal)
				Finish();
		}

		public override void Start (GameState gameState)
		{
			
		}

		private Character character;
		private float timeTotal;
		private float timePassed;
		private float startFade;
		private float endFade;

		private void waitFinished (object state)
		{
			Finish();
		}
	}
}
