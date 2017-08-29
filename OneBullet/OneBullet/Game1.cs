using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D megaManXR;
		Texture2D megaManXL;
		Texture2D p1Sprite;
        Rectangle p1Position;
		Vector2 p1Velocity;
		const int p1Acceleration = 3;
		bool onGround, jumping;
        KeyboardState kState;
		private KeyboardState oldKState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            p1Position = new Rectangle(200, 200, 260, 250);
			p1Velocity = new Vector2(0, 0);
			onGround = true;
			jumping = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            megaManXR = Content.Load<Texture2D>("MegaManX_Right");
			megaManXL = Content.Load<Texture2D>("MegaManX_Left");
			p1Sprite = megaManXR;
		}

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

			// ------------------------------------------ Keyboard inputs
            kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.A))
            {
				p1Velocity.X -= 10;
				p1Sprite = megaManXL;
            }
			if (kState.IsKeyDown(Keys.D))
			{
				p1Velocity.X += 10;
				p1Sprite = megaManXR;
			}
			if (kState.IsKeyDown(Keys.G))
			{
				if (onGround && !jumping)
				{
					p1Velocity.Y -= 30;
					jumping = true;
				}
				else if (!onGround && jumping && p1Velocity.Y < 0)
				{
					p1Velocity.Y -= 1;
				}
			}
			if (kState.IsKeyUp(Keys.G) && oldKState.IsKeyDown(Keys.G))
			{
				jumping = false;
			}

			oldKState = kState;

			// ------------------------------------------ Falling parameters
			if (p1Position.Y < 200)
			{
				onGround = false;
				p1Velocity.Y += p1Acceleration;
			}
			else if (!onGround)
			{
				onGround = true;
				p1Velocity.Y = 0;
			}

			// ------------------------------------------ Calculating velocity
			p1Position.X += (int)p1Velocity.X;
			p1Position.Y += (int)p1Velocity.Y;
			if (p1Position.Y > 200)
				p1Position.Y = 200;

			// ------------------------------------------ Resetting values
			p1Velocity.X = 0;

			base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(p1Sprite, p1Position, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
