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
		Player player1;
		Bullet bullet1;
		Texture2D megaManXR, megaManXL;
		Texture2D p1GunR, p1GunL;
		Texture2D bullet;
		const int pAcceleration = 3;
		double charSize;
		KeyboardState kState;
		private KeyboardState oldKState;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			// Resolution independence
			//Vector2 virtualScreen = new Vector2(1280, 720);
			//graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			//graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;
			graphics.IsFullScreen = false;
			graphics.ApplyChanges();

			charSize = GraphicsDevice.Viewport.Height / 7;
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
			player1 = new Player();
			bullet1 = new Bullet();
			base.Initialize();
			
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
			Rectangle p1Position = new Rectangle(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height - (int)charSize, (int)(charSize * 1.04), (int)charSize);
			int p1GunOffset = (int)charSize / 2;

			Rectangle b1Position = new Rectangle(-100, -100, (int)(charSize / 3), (int)(charSize / 3));

			megaManXR = Content.Load<Texture2D>("MegaManX_Right");
			megaManXL = Content.Load<Texture2D>("MegaManX_Left");
			p1GunR = Content.Load<Texture2D>("gun_right");
			p1GunL = Content.Load<Texture2D>("gun_left");

			bullet = Content.Load<Texture2D>("shot_poulpi");

			bullet1.Initialize(bullet, b1Position);
			player1.Initialize(megaManXR, p1GunR, p1Position, p1GunOffset, bullet1);
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

			if (kState.IsKeyDown(Keys.F) && player1.loaded) // Shoot bullet
			{
				player1.loaded = false;
				if (player1.pTexture == megaManXR)
				{
					player1.pBullet.Fire(bullet, true, player1.pGunPosition);
				}
				else
				{
					player1.pBullet.Fire(bullet, false, player1.pGunPosition);
				}
			}
			if (kState.IsKeyDown(Keys.A)) // Move left
			{
				player1.pVelocity.X -= 10;
				if (player1.pTexture == megaManXR)
				{
					player1.Turn(megaManXL, p1GunL, -(int)charSize / 2);
				}
			}
			if (kState.IsKeyDown(Keys.D)) // Move right
			{
				player1.pVelocity.X += 10;
				if (player1.pTexture == megaManXL)
				{
					player1.Turn(megaManXR, p1GunR, (int)charSize / 2);
				}
			}

			player1.Update(kState, oldKState, GraphicsDevice, charSize);
			bullet1.Update();

			oldKState = kState;

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
			player1.Draw(spriteBatch);
			bullet1.Draw(spriteBatch);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
