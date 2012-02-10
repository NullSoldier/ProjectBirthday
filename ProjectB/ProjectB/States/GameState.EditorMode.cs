using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectB.States
{
	public partial class GameState
	{
		public void EditorLoad ()
		{
			debugTexture = new Texture2D (ProjectB.Graphics.GraphicsDevice, 1, 1);
			debugTexture.SetData (new [] { Color.White });

			debugFont = ProjectB.ContentManager.Load<SpriteFont> ("DebugFont");
			collisionRectangles = new List<Rectangle>();

			redTransparent = new Color(1f, 0f, 0f, 0.5f);
			blueTransparent = new Color(0f, 0f, 1f, 0.5f);
		}

		public void EditorUpdate (GameTime gameTime)
		{
			lastMouseLoc = camera.ScreenToWorld (new Vector2 (ProjectB.NewMouse.X, ProjectB.NewMouse.Y));

			if (IsLeftClicked ())
			{
				if (hasBuffered)
				{
					//collisionRectangles.Add (RectangleHelpers.FromVectors (lastClicked, lastMouseLoc));
					//hasBuffered = false;
				}
				else
				{
					//lastClicked = camera.ScreenToWorld (new Vector2 (ProjectB.NewMouse.X, ProjectB.NewMouse.Y));
					//hasBuffered = true;
				}

				SpawnCat (lastMouseLoc, Directions.Right);
			}
		}

		public void EditorDraw()
		{
			// Draw all transformed elements
			batch.Begin (SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.Transformation);

			foreach (Rectangle rect in collisionRectangles)
				batch.Draw (debugTexture, rect, redTransparent);

			if (hasBuffered)
				batch.Draw (debugTexture, RectangleHelpers.FromVectors (lastMouseLoc, lastClicked), blueTransparent);

			batch.End ();

			// Draw non transformed elements
			batch.Begin (SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			batch.DrawString (debugFont, string.Format ("Mouse Location: ({0}, {1})", lastMouseLoc.X, lastMouseLoc.Y), Vector2.Zero, Color.Black);
			batch.End ();
		}

		private List<Rectangle> collisionRectangles;
		private Vector2 lastClicked;
		private Vector2 lastMouseLoc;
		private bool hasBuffered;
		private Texture2D debugTexture;
		private SpriteFont debugFont;
		private Color redTransparent;
		private Color blueTransparent;

		private bool IsLeftClicked ()
		{
			return ProjectB.OldMouse.LeftButton == ButtonState.Released
				&& ProjectB.NewMouse.LeftButton == ButtonState.Pressed;
		}
	}
}
