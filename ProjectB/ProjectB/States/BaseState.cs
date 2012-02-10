using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB
{
	public abstract class BaseState
	{
		public virtual void Draw ()
		{
		}

		public virtual void Update (GameTime gameTime)
		{
		}

		public virtual void Activate ()
		{
		}

		public virtual void Load ()
		{
		}
	}
}
