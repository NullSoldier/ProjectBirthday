using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ProjectB.Levels;
using ProjectB.States;

namespace ProjectB
{
	public class Engine
		: Microsoft.Xna.Framework.Game
	{
		public GraphicsDeviceManager graphics;
		public SpriteBatch spriteBatch;
		
		public static ContentManager ContentManager;
		public static SpriteBatch Batch;
		public static GraphicsDeviceManager Graphics;
		public static Engine Project;
		public static DialogRunner DialogRunner;
		public static int ScreenWidth = 800;
		public static int ScreenHeight = 600;
		public static KeyboardState NewKeyboard;
		public static KeyboardState OldKeyboard;
		public static MouseState NewMouse;
		public static MouseState OldMouse;
		public static GamePadState NewPad;
		public static GamePadState OldPad;
		
		public BaseLevel CurrentLevel
		{
			get { return levels[levelIndex]; }
		}

		BaseLevel[] levels;
		int levelIndex = 0;
		Dictionary<string, BaseState> states;
 		BaseState currentState;

		// Fading
		private bool fading;
		private float timePassed;
		private float timeTotal;
		private float start;
		private float end;

		public Engine()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = ScreenWidth;
			graphics.PreferredBackBufferHeight = ScreenHeight;
			graphics.ApplyChanges();

			IsMouseVisible = true;
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			Engine.ContentManager = this.Content;
			Engine.Graphics = this.graphics;
			Engine.Project = this;
			Engine.DialogRunner = new DialogRunner();
			
			base.Initialize();
		}

		protected override void LoadContent()
		{
			Engine.Batch = spriteBatch = new SpriteBatch(GraphicsDevice);

			levels = new BaseLevel[]
			{
				new LevelIntro(),
				new LevelOne()
			};
			levelIndex = 0;

			states = new Dictionary<string, BaseState>
			{
				{"MenuState", new MenuState()},
				{"GameState", new GameState()}
			};

			foreach (BaseState state in states.Values)
				state.Load();

			currentState = states["MenuState"];
			currentState.Activate();
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update (GameTime gameTime)
		{
			NewKeyboard = Keyboard.GetState();
			NewMouse = Mouse.GetState ();
			NewPad = GamePad.GetState (PlayerIndex.One);

			DialogRunner.Update (gameTime);

			if (currentState != null)
				currentState.Update (gameTime);

			OldKeyboard = NewKeyboard;
			OldMouse = NewMouse;
			OldPad = NewPad;

			base.Update(gameTime);
		}

		protected override void Draw (GameTime gameTime)
		{
			if (currentState != null)
				currentState.Draw();

			DialogRunner.Draw (spriteBatch);

			base.Draw(gameTime);
		}

		public void SetState (string name)
		{
			currentState = states[name];
			currentState.Activate();
		}
	}
}
