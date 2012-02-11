using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB.Objects
{
	public class Boss
		: Character
	{
		public Boss ()
		{
			// Load animated textures.
            idleAnimation = new Animation (Engine.ContentManager.Load<Texture2D> ("Boss"), 0.1f, true);
            moveAnimation = new Animation (Engine.ContentManager.Load<Texture2D> ("Boss"), 0.1f, true);
            jumpAnimation = new Animation (Engine.ContentManager.Load<Texture2D> ("Boss"), 0.1f, false);
			
			Sprite.PlayAnimation  (idleAnimation);
		}
	}
}
