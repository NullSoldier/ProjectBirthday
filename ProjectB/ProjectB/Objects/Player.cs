using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB.Objects
{
	public class Player
		: Character
	{
		public Player ()
		{
            idleAnimation = new Animation(Engine.ContentManager.Load<Texture2D> ("Player/Idle"), 0.1f, true);
            moveAnimation = new Animation(Engine.ContentManager.Load<Texture2D> ("Player/Move"), 0.1f, true);
            jumpAnimation = new Animation(Engine.ContentManager.Load<Texture2D> ("Player/Jump"), 0.1f, false);

			Health = 100;
			Sprite.PlayAnimation  (idleAnimation);
		}
	}
}
