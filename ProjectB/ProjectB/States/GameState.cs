using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectB.Objects;

namespace ProjectB
{
	public class GameState
		: BaseState
	{
		public override void Activate ()
		{
		}

		public override void Load ()
		{
			camera = new Camera (ProjectB.ScreenWidth, ProjectB.ScreenHeight);
			camera.Bounds = new Rectangle(0, 0, CurrentLevel.Level.Texture.Width, CurrentLevel.Level.Texture.Height);
			camera.UseBounds = true;

			cloudManager = new CloudManager (camera.Bounds.Width, camera.Bounds.Height);

			batch = ProjectB.Batch;
			clearColor = new Color(166, 211, 239);

			SpawnPlayer (CurrentLevel.StartPoint);
		}

		public override void Update (GameTime gameTime)
		{
			cloudManager.Update (gameTime);
			HandleMovement (gameTime);

			camera.CenterOnPoint (player.Location);
		}

		public override void Draw ()
		{
			ProjectB.Graphics.GraphicsDevice.Clear (clearColor);

			batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.Transformation);

			cloudManager.Draw (batch);

			batch.Draw (CurrentLevel.Level.Texture, Vector2.Zero, Color.White);
			batch.Draw (player.Texture, player.Location, Color.Red);

			batch.End();
		}
		
		private BaseLevel CurrentLevel
		{
			get { return ProjectB.Project.CurrentLevel; }
		}

		private SpriteBatch batch;
		private Camera camera;
		private GameObject player;
		private CloudManager cloudManager;
		private float playerSpeed = 0.3f;
		private Color clearColor;

		private void SpawnPlayer (Vector2 location)
		{
			player = new GameObject
			{
				Texture = ProjectB.ContentManager.Load<Texture2D> ("Player"),
				Location = location
			};
		}

		private void HandleMovement (GameTime gameTime)
		{
			float x = player.Location.X;
			float y = player.Location.Y;

			if (ProjectB.NewKeyState.IsKeyDown (Keys.D))
				x += playerSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			else if (ProjectB.NewKeyState.IsKeyDown (Keys.A))
				x -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			if (ProjectB.NewKeyState.IsKeyDown (Keys.W))
				y -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			else if (ProjectB.NewKeyState.IsKeyDown (Keys.S))
				y += playerSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			player.Location = new Vector2(x, y);
		}
	}
}
