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
		public static Texture2D BlankTexture;
		public static int ScreenWidth = 800;
		public static int ScreenHeight = 600;
		public static KeyboardState NewKeyboard;
		public static KeyboardState OldKeyboard;
		public static MouseState NewMouse;
		public static MouseState OldMouse;
		public static GamePadState NewPad;
		public static GamePadState OldPad;
		public static BaseState NextState;
		public static int NextLevelIndex;
		public static bool displayTeam;

		public static bool IgnoreInput
		{
			get { return !Project.IsActive; }
		}
		
		public BaseLevel CurrentLevel
		{
			get { return levels[levelIndex]; }
		}

		public BaseLevel[] levels;
		public int levelIndex = 0;
		public Dictionary<string, BaseState> states;
 		public BaseState CurrentState;

		private bool fading;
        private float fadeStart;
        private float fadeEnd;
        private float fadeTimePassed;
        private float fadeTimeTotal;
		private Action fadeAction;
        private Texture2D black;
		private Texture2D team;
		private Vector2 teamLocation;

		public Engine()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = ScreenWidth;
			graphics.PreferredBackBufferHeight = ScreenHeight;
			graphics.IsFullScreen = false;
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
				new LevelOne(),
				new LevelOne(),
				new LevelTwo(), 
				new LevelThree(),
				new LevelOutro(),
			};
			levelIndex = 0;

			states = new Dictionary<string, BaseState>
			{
				{"MenuState", new MenuState()},
				{"GameState", new GameState()}
			};

			foreach (BaseState state in states.Values)
				state.Load();

			team = Content.Load<Texture2D> ("Team");
			teamLocation = new Vector2((ScreenWidth / 2) - (team.Width / 2),
				(ScreenHeight/ 2) - (team.Height / 2));

			black = new Texture2D (graphics.GraphicsDevice, 1, 1);
            black.SetData (new [] { Color.Black});
			
			BlankTexture = new Texture2D (graphics.GraphicsDevice, 1, 1);
            BlankTexture.SetData (new [] { Color.White});;

			this.CurrentState = states["MenuState"];
			this.CurrentState.Activate();

			//Fade (0f, 255f, 0.50f);
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update (GameTime gameTime)
		{
			NewKeyboard = Keyboard.GetState();
			NewMouse = Mouse.GetState ();
			NewPad = GamePad.GetState (PlayerIndex.One);

			if (NewKeyboard.IsKeyDown(Keys.Escape)
				|| NewPad.Buttons.Back == ButtonState.Pressed)
			{
				Environment.Exit(1);
			}

			DialogRunner.Update (gameTime);

			if (this.CurrentState != null)
				this.CurrentState.Update (gameTime);

			 if (fading)
                UpdateFade (gameTime);

			OldKeyboard = NewKeyboard;
			OldMouse = NewMouse;
			OldPad = NewPad;

			base.Update(gameTime);
		}

		protected override void Draw (GameTime gameTime)
		{
			if (this.CurrentState != null)
				this.CurrentState.Draw();

			DialogRunner.Draw (spriteBatch);


			if (fading)
            {
                float lerpValue = MathHelper.Lerp (fadeStart, fadeEnd, fadeTimePassed / fadeTimeTotal);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
                spriteBatch.Draw (black, new Rectangle(0, 0, ScreenWidth, ScreenHeight), new Color(255, 255, 255, 255 - (int)lerpValue));
                spriteBatch.End();
            }

			if (displayTeam)
			{
				spriteBatch.Begin();
				spriteBatch.Draw (team, teamLocation, Color.White);
				spriteBatch.End();
			}

			base.Draw(gameTime);
		}

		private void UpdateFade(GameTime gameTime)
        {
            fadeTimePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (fadeTimePassed >= fadeTimeTotal)
            {
                fading = false;

                if (Engine.NextState != null)
                {
                    //if (NextState == States[(int)StateType.Title])
                        //ResetGame ();

					// Go to the next level
                    if (Engine.NextLevelIndex != Engine.Project.levelIndex)
                        Engine.Project.levelIndex = Engine.NextLevelIndex;

                    Engine.NextState.Activate();
                    Engine.Project.CurrentState = Engine.NextState;
                    Engine.NextState = null;

                    Fade (0, 255, 0.75f);
                    //EngineRef.music.PlayMusic ("Song1");
                    //EngineRef.music.FadeIn (1.5f);
                }

				if (fadeAction != null)
					fadeAction();

				// Super hack
				if (displayTeam)
				{
					fading = true;
				}

            }
        }

		public void NextLevel()
        {
            NextLevelIndex = levelIndex + 1;
            NextState = states["GameState"];
            Fade (255, 0, 0.75f);

            //music.FadeOut (0.75f);
        }

		public void SetState (string name)
		{
			this.CurrentState = states[name];
			this.CurrentState.Activate();
		}

		public void Fade (float start, float end, float time, Action action = null)
        {
			fadeAction = action;
            fadeStart = start;
            fadeEnd = end;
            fadeTimeTotal = time;
            fadeTimePassed = 0;
            fading = true;
        }
	}
}
