using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectB.States;

namespace ProjectB.Scripts
{
	public class EffectManager
	{
		public EffectManager (GameState gameState)
		{
			this.Effects = new Dictionary<string, Queue<BaseEffect>>();
			this.CurrentEffects = new Dictionary<string, BaseEffect>();
			this.gameState = gameState;
		}

		public void Add (string group, BaseEffect effect)
		{
			if (!Effects.ContainsKey(group))
				Effects.Add(group, new Queue<BaseEffect>());

			Effects[group].Enqueue (effect);
		}

		public void Update (GameTime gameTime)
		{
			foreach (var kvp in CurrentEffects)
			{
				if (kvp.Value == null || kvp.Value.Finished)
					continue;

				kvp.Value.Update (gameTime);
			}

			// Start new effects
			foreach (var kvp in Effects)
			{
				if (!CurrentEffects.ContainsKey(kvp.Key)
					|| CurrentEffects[kvp.Key] == null
					|| CurrentEffects[kvp.Key].Finished)
				{
					// Make sure there are more effects to be queued
					if (Effects[kvp.Key].Count <= 0)
						continue;

					CurrentEffects[kvp.Key] = Effects[kvp.Key].Dequeue();
					CurrentEffects[kvp.Key].Start (gameState);
				}
			}
		}

		private Dictionary<string, Queue<BaseEffect>> Effects;
		private Dictionary<string, BaseEffect> CurrentEffects;
		private GameState gameState;
	}
}
