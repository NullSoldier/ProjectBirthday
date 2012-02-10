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
			cats = new List<NyanCat>();

			batch = ProjectB.Batch;
			clearColor = new Color(166, 211, 239);
			catTexture = ProjectB.ContentManager.Load<Texture2D> ("NyanCat");
			SpawnPlayer (CurrentLevel.StartPoint);

			EditorLoad();
		}

		public override void Update (GameTime gameTime)
		{
			cloudManager.Update (gameTime);

			HandleMovement (gameTime);
			HandleControls ();

			foreach (NyanCat cat in cats)
				cat.Update (gameTime);

			camera.CenterOnPoint (player.Location);

			if (editorModeEnabled)
				EditorUpdate (gameTime);
		}

		public override void Draw ()
		{
			ProjectB.Graphics.GraphicsDevice.Clear (clearColor);

			batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.Transformation);

			cloudManager.Draw (batch);

			batch.Draw (CurrentLevel.Level.Texture, Vector2.Zero, Color.White);
			batch.Draw (player.Texture, player.Location, Color.Red);

			foreach (NyanCat cat in cats)
				cat.Draw (batch);

			batch.End();
			
			if (editorModeEnabled)
				EditorDraw();
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
		private bool editorModeEnabled = false;
		private Color clearColor;
		private List<NyanCat> cats;
		private Texture2D catTexture;

		private void SpawnPlayer (Vector2 location)
		{
			player = new GameObject
			{
				Texture = ProjectB.ContentManager.Load<Texture2D> ("Player"),
				Location = location
			};
		}

		private void SpawnCat(Vector2 location, Directions direction)
		{
			NyanCat cat = new NyanCat (direction)
			{
				Texture = catTexture,
				Location = location
			};
			cats.Add (cat);
		}

		private void HandleGravity()
		{

		}

		private void HandleControls ()
		{
			if (ProjectB.NewKeyboard.IsKeyDown (Keys.F1) && ProjectB.OldKeyboard.IsKeyUp (Keys.F1))
				editorModeEnabled = !editorModeEnabled;
		}

		private void HandleMovement (GameTime gameTime)
		{
			float x = player.Location.X;
			float y = player.Location.Y;

			if (ProjectB.NewKeyboard.IsKeyDown (Keys.D))
				x += playerSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			else if (ProjectB.NewKeyboard.IsKeyDown (Keys.A))
				x -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			if (ProjectB.NewKeyboard.IsKeyDown (Keys.W))
				y -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			else if (ProjectB.NewKeyboard.IsKeyDown (Keys.S))
				y += playerSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			player.Location = new Vector2(
				MathHelper.Clamp (x, 0, CurrentLevel.Level.Texture.Width - player.Texture.Width),
				MathHelper.Clamp (y, 0, CurrentLevel.Level.Texture.Height - player.Texture.Height));
		}
	}
}
