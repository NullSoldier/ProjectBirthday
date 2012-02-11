using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB.States
{
	public class MenuState
		: BaseState
	{
		public override void Draw ()
		{
		}

		public override void Update (GameTime gameTime)
		{
		}

		public override void Activate ()
		{
			Engine.Project.SetState ("GameState");
		}

		public override void Load ()
		{
		}
	}
}