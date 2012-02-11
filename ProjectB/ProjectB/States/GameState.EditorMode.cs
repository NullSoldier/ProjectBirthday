using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectB.Objects;

namespace ProjectB.States
{
	public partial class GameState
	{
		public void EditorLoad ()
		{
			debugTexture = new Texture2D (Engine.Graphics.GraphicsDevice, 1, 1);
			debugTexture.SetData (new [] { Color.White });

			debugFont = Engine.ContentManager.Load<SpriteFont> ("DebugFont");
			collisionRectangles = new List<Rectangle>();

			redTransparent = new Color(1f, 0f, 0f, 0.5f);
			greenTransparent = new Color(0f, 1f, 0f, 0.5f);
			blueTransparent = new Color (0f, 0f, 1f, 0.5f);
		}

		public void EditorUpdate (GameTime gameTime)
		{
			lastMouseLoc = camera.ScreenToWorld (new Vector2 (Engine.NewMouse.X, Engine.NewMouse.Y));

			if (Engine.IgnoreInput)
				return;

			if (IsRightClicked ())
			{
				if (hasBuffered)
				{
					collisionRectangles.Add (RectangleHelpers.FromVectors (lastClicked, lastMouseLoc));
					hasBuffered = false;
				}
				else
				{
					lastClicked = camera.ScreenToWorld (new Vector2 (Engine.NewMouse.X, Engine.NewMouse.Y));
					hasBuffered = true;
				}

				//SpawnCat (lastMouseLoc, Directions.Right);
			}

			if (IsMiddleClicked ())
			{
				StringBuilder code = new StringBuilder ();

				foreach (Rectangle rectangle in collisionRectangles)
					code.AppendFormat ("AddGeometry (new Rectangle({0}, {1}, {2}, {3}), CollisionTypes.Impassable);{4}", rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, Environment.NewLine);

				Console.Write(code);
			}
		}

		public void EditorDraw()
		{
			// Draw all transformed elements
			batch.Begin (SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.Transformation);

			foreach (Rectangle rect in collisionRectangles)
				batch.Draw (debugTexture, rect, redTransparent);

			foreach (var geometry in CurrentLevel.CollisionGeometry)
				batch.Draw (debugTexture, geometry.Rectangle, greenTransparent);

			foreach (var entity in CurrentLevel.GameObjects)
			{
				var baseEnemy = entity as BaseMonster;
				if (baseEnemy != null)
					batch.Draw (debugTexture, baseEnemy.GetBounds(), blueTransparent);
			}

			foreach (var cat in cats)
				batch.Draw (debugTexture, cat.GetBounds(), blueTransparent);

			batch.Draw (debugTexture, CurrentLevel.Player.GetBounds(), blueTransparent);

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
		private Color greenTransparent;

		private bool IsMiddleClicked ()
		{
			return Engine.OldMouse.MiddleButton == ButtonState.Released
				&& Engine.NewMouse.MiddleButton == ButtonState.Pressed;
		}

		private bool IsLeftClicked()
		{
			return Engine.OldMouse.LeftButton == ButtonState.Released
				&& Engine.NewMouse.LeftButton == ButtonState.Pressed;
		}

		private bool IsRightClicked()
		{
			return Engine.OldMouse.RightButton == ButtonState.Released
				&& Engine.NewMouse.RightButton == ButtonState.Pressed;
		}
	}
}
