using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace OneBullet
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Player player1, player2;
		Bullet bullet1, bullet2;
		Level level01;
		const int pAcceleration = 3;
		double charHeight, charWidth;
		KeyboardState kState;
		private KeyboardState oldKState;

        public Rectangle plat1Pos;
        public Rectangle plat2Pos;
        public Rectangle plat3Pos;
        public Rectangle plat4Pos;
        public Rectangle plat5Pos;




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

			charHeight = GraphicsDevice.Viewport.Height / 7;
			charWidth = charHeight / 1.13;
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
			player2 = new Player();
			bullet1 = new Bullet();
			bullet2 = new Bullet();
			level01 = new Level();

            plat1Pos = new Rectangle(100, GraphicsDevice.Viewport.Height - (int)(charHeight * 2), (int)charWidth * 2, (int)(2 * charHeight / 3)); //BOTTOM LEFT
            plat2Pos = new Rectangle(GraphicsDevice.Viewport.Width / 2 + 350, GraphicsDevice.Viewport.Height - (int)(charHeight * 2), (int)charWidth * 2, (int)(2 * charHeight / 3)); //BOTTOM RIGHT
            plat3Pos = new Rectangle(225, GraphicsDevice.Viewport.Height - (int)(charHeight * 2) - 190, (int)charWidth * 2, (int)(2 * charHeight / 3)); //MID LEFT
            plat4Pos = new Rectangle(GraphicsDevice.Viewport.Width / 2 + 250, GraphicsDevice.Viewport.Height - (int)(charHeight * 2) - 190, (int)charWidth * 2, (int)(2 * charHeight / 3)); //MID RIGHT
            plat5Pos = new Rectangle(GraphicsDevice.Viewport.Width / 2 - 100, GraphicsDevice.Viewport.Height - (int)(charHeight * 2) - 300, (int)charWidth * 2, (int)(2 * charHeight / 3)); //CENTER TOP

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
			Rectangle p1Position = new Rectangle(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height - (int)(2 * charHeight), (int)charWidth, (int)charHeight);
			Rectangle p1GunPos = new Rectangle(p1Position.X, p1Position.Y, (int)(charWidth * 0.75), (int)(charHeight / 3));
			int p1GunOffset = (int)charWidth / 2;

			Rectangle p2Position = new Rectangle((3 * (GraphicsDevice.Viewport.Width / 4)), GraphicsDevice.Viewport.Height - (int)(2 * charHeight), (int)charWidth, (int)charHeight);
			Rectangle p2GunPos = new Rectangle(p2Position.X, p2Position.Y, (int)(charWidth * 0.75), (int)(charHeight / 3));
			int p2GunOffset = -(int)charWidth / 2;

			Rectangle b1Position = new Rectangle(-100, -100, (int)(charHeight / 4), (int)(charHeight / 4));
			Rectangle b2Position = new Rectangle(-100, -100, (int)(charHeight / 4), (int)(charHeight / 4));

			Texture2D player1TextureR = Content.Load<Texture2D>("MegaManX_Right");
			Texture2D player1TextureL = Content.Load<Texture2D>("MegaManX_Left");
			Texture2D player2TextureR = Content.Load<Texture2D>("Zero_Right");
			Texture2D player2TextureL = Content.Load<Texture2D>("Zero_Left");
			Texture2D gunR = Content.Load<Texture2D>("gun_right");
			Texture2D gunL = Content.Load<Texture2D>("gun_left");

			Texture2D bullet = Content.Load<Texture2D>("shot_poulpi");

			bullet1.Initialize(bullet, b1Position);
			bullet2.Initialize(bullet, b1Position);
			player1.Initialize(player1TextureR, player1TextureL, gunR, gunL, p1Position, p1GunPos, p1GunOffset, bullet1, 1);
			player2.Initialize(player2TextureR, player2TextureL, gunR, gunL, p2Position, p2GunPos, p2GunOffset, bullet2, 2);

			// ------------------------------------------ Level and platform content
			Texture2D background = Content.Load<Texture2D>("background1");
			Texture2D platform1 = Content.Load<Texture2D>("test_platform");

			Level01Init(background, platform1);
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
			BulletChecks();
            Respawn();

			// ------------------------------------------ Keyboard inputs
			kState = Keyboard.GetState();

			// ------------------------------------------ Updates
			if (!player1.dead)
				player1.Update(kState, oldKState, GraphicsDevice, gameTime);
			if (!player2.dead)
				player2.Update(kState, oldKState, GraphicsDevice, gameTime);
			bullet1.Update(GraphicsDevice, (int)(charHeight / 3), gameTime);
			bullet2.Update(GraphicsDevice, (int)(charHeight / 3), gameTime);

			oldKState = kState;

			base.Update(gameTime);
		}

		void BulletChecks()
		{
			// ------------------------------------------ Setting up the catching mechanic
			Rectangle p1CatchPosition = new Rectangle(player1.pGunPosition.X, player1.pGunPosition.Y, (int)(player1.pGunPosition.Width / 2), (3 * (player1.pGunPosition.Height / 4)));
			Rectangle p2CatchPosition = new Rectangle(player2.pGunPosition.X, player2.pGunPosition.Y, (int)(player2.pGunPosition.Width / 2), (3 * (player2.pGunPosition.Height / 4)));
			Rectangle p1ModPosition = player1.pCollisionPosition;
			Rectangle p2ModPosition = player2.pCollisionPosition;

			if (player1.pTexture == player1.pTextureR)
			{
				p1CatchPosition.X -= (int)(player1.pGunPosition.Width / 4);
			}
			else
			{
				p1CatchPosition.X -= (int)(player1.pGunPosition.Width / 4);
			}

			if (player2.pTexture == player2.pTextureR)
			{
				p2CatchPosition.X -= (int)(player2.pGunPosition.Width / 4);
			}
			else
			{
				p2CatchPosition.X -= (int)(player2.pGunPosition.Width / 4);
			}

			// ------------------------------------------ Bullet 1 checks
			// ------------------------------------------ Checking for pickup
			if (!bullet1.bMoving && !bullet1.bIsLoaded)
			{
				if (!player1.dead)
				{
					if (!player1.loaded)
					{
						if (p1ModPosition.Intersects(bullet1.bPosition))
						{
							player1.Pickup(bullet1);
							bullet1.Pickup();
						}
					}
				}
				if (!player2.dead)
				{
					if (!player2.loaded)
					{
						if (p2ModPosition.Intersects(bullet1.bPosition))
						{
							player2.Pickup(bullet1);
							bullet1.Pickup();
						}
					}
				}
			}
			// ------------------------------------------ Checking for hit or catch
			else if (bullet1.bMoving && !bullet1.bIsLoaded)
			{
				if (!player1.dead)
				{
					if (player1.loaded)
					{
						if (p1ModPosition.Intersects(bullet1.bPosition)) // Hit
						{
							player1.Hit();
							bullet1.Hit();
						}
					}
					else
					{
						if (p1CatchPosition.Intersects(bullet1.bPosition)) // Catch
						{
							player1.Catch(bullet1);
							bullet1.Catch();
						}
						else if (p1ModPosition.Intersects(bullet1.bPosition)) // Hit
						{
							player1.Hit();
							bullet1.Hit();
						}
					}
				}
				if (!player2.dead)
				{
					if (player2.loaded)
					{
						if (p2ModPosition.Intersects(bullet1.bPosition)) // Hit
						{
							player2.Hit();
							bullet1.Hit();
						}
					}
					else
					{
						if (p2CatchPosition.Intersects(bullet1.bPosition)) // Catch
						{
							player2.Catch(bullet1);
							bullet1.Catch();
						}
						else if (p2ModPosition.Intersects(bullet1.bPosition)) // Hit
						{
							player2.Hit();
							bullet1.Hit();
						}
					}
				}
			}
			// ------------------------------------------ Bullet 2 checks
			// ------------------------------------------ Checking for pickup
			if (!bullet2.bMoving && !bullet2.bIsLoaded)
			{
				if (!player1.dead)
				{
					if (!player1.loaded)
					{
						if (p1ModPosition.Intersects(bullet2.bPosition))
						{
							player1.Pickup(bullet2);
							bullet2.Pickup();
						}
					}
				}
				if (!player2.dead)
				{
					if (!player2.loaded)
					{
						if (p2ModPosition.Intersects(bullet2.bPosition))
						{
							player2.Pickup(bullet2);
							bullet2.Pickup();
						}
					}
				}
			}
			// ------------------------------------------ Checking for hit or catch
			else if (bullet2.bMoving && !bullet2.bIsLoaded)
			{
				if (!player1.dead)
				{
					if (player1.loaded)
					{
						if (p1ModPosition.Intersects(bullet2.bPosition)) // Hit
						{
							player1.Hit();
							bullet2.Hit();
						}
					}
					else
					{
						if (p1CatchPosition.Intersects(bullet2.bPosition)) // Catch
						{
							player1.Catch(bullet2);
							bullet2.Catch();
						}
						else if (p1ModPosition.Intersects(bullet2.bPosition)) // Hit
						{
							player1.Hit();
							bullet2.Hit();
						}
					}
				}
				if (!player2.dead)
				{
					if (player2.loaded)
					{
						if (p2ModPosition.Intersects(bullet2.bPosition)) // Hit
						{
							player2.Hit();
							bullet2.Hit();
						}
					}
					else
					{
						if (p2CatchPosition.Intersects(bullet2.bPosition)) // Catch
						{
							player2.Catch(bullet2);
							bullet2.Catch();
						}
						else if (p2ModPosition.Intersects(bullet2.bPosition)) // Hit
						{
							player2.Hit();
							bullet2.Hit();
						}
					}
				}
			}
			// ------------------------------------------ Bullet collision check
			if (bullet1.bMoving && !bullet1.bIsLoaded && bullet2.bMoving && !bullet2.bIsLoaded)
			{
				if (bullet1.bPosition.Intersects(bullet2.bPosition))
				{
					bullet1.Wall();
					bullet2.Wall();
				}
			}
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
			level01.Draw(spriteBatch);
			//spriteBatch.Draw(background, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
			//spriteBatch.Draw(backgroundElements, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
			player1.Draw(spriteBatch);
			player2.Draw(spriteBatch);
			bullet1.Draw(spriteBatch);
			bullet2.Draw(spriteBatch);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		public void Level01Init(Texture2D background, Texture2D platform)
		{
			Rectangle backgroundPos = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

			Rectangle floorPlatPos = new Rectangle(0, GraphicsDevice.Viewport.Height - (int)(charHeight), GraphicsDevice.Viewport.Width, (int)charHeight);
			Platforms floorPlat = new Platforms();
			floorPlat.Initialize(platform, floorPlatPos);

			Rectangle rWallPlatPos = new Rectangle(GraphicsDevice.Viewport.Width, 0, (int)charWidth, GraphicsDevice.Viewport.Height);
			Platforms rWallPlat = new Platforms();
			rWallPlat.Initialize(platform, rWallPlatPos);

			Rectangle lWallPlatPos = new Rectangle(0 - (int)charWidth, 0, (int)charWidth, GraphicsDevice.Viewport.Height);
			Platforms lWallPlat = new Platforms();
			lWallPlat.Initialize(platform, lWallPlatPos);

            Platforms plat1 = new Platforms();
            Platforms plat2 = new Platforms();
            Platforms plat3 = new Platforms();
            Platforms plat4 = new Platforms();
            Platforms plat5 = new Platforms();

            plat1.Initialize(platform, plat1Pos);
            plat2.Initialize(platform, plat2Pos);
            plat3.Initialize(platform, plat3Pos);
            plat4.Initialize(platform, plat4Pos);
            plat5.Initialize(platform, plat5Pos);

            Platforms[] lvlPlats = { plat1, plat2, plat3, plat4, plat5 };

            level01.Initialize(background, backgroundPos, 5, lvlPlats, floorPlat, rWallPlat, lWallPlat);
        }

        void Respawn()
        {

            Vector2 respawnPoint1 = new Vector2(plat1Pos.X, plat1Pos.Y - 10);
            Vector2 respawnPoint2 = new Vector2(plat2Pos.X, plat2Pos.Y - 10);
            Vector2 respawnPoint3 = new Vector2(plat3Pos.X, plat3Pos.Y - 10);
            Vector2 respawnPoint4 = new Vector2(plat4Pos.X, plat4Pos.Y - 10);

            
    
            if (player1.dead == true) 
            {
                
                double x1Distance = Math.Pow(respawnPoint1.X - player2.pPosition.X, 2);
                double y1Distance = Math.Pow(respawnPoint1.Y - player2.pPosition.Y, 2);
                double player1Distance = Math.Sqrt(x1Distance + y1Distance);

                double x2Distance = Math.Pow(respawnPoint2.X - player2.pPosition.X, 2);
                double y2Distance = Math.Pow(respawnPoint2.Y - player2.pPosition.Y, 2);
                double player2Distance = Math.Sqrt(x2Distance + y2Distance);

                double x3Distance = Math.Pow(respawnPoint3.X - player2.pPosition.X, 2);
                double y3Distance = Math.Pow(respawnPoint3.Y - player2.pPosition.Y, 2);
                double player3Distance = Math.Sqrt(x3Distance + y3Distance);

                double x4Distance = Math.Pow(respawnPoint4.X - player2.pPosition.X, 2);
                double y4Distance = Math.Pow(respawnPoint4.Y - player2.pPosition.Y, 2);
                double player4Distance = Math.Sqrt(x4Distance + y4Distance);

                double maxDistance = Math.Max(Math.Max(Math.Max(player1Distance, player2Distance), player3Distance), player4Distance);

                if (maxDistance == player1Distance)
                {
                    player1.pPosition.X = (int)respawnPoint1.X;
                    player1.pPosition.Y = (int)respawnPoint1.Y;
                    player1.dead = false;
                }

                else if (maxDistance == player2Distance)
                {
                    player1.pPosition.X = (int)respawnPoint2.X;
                    player1.pPosition.Y = (int)respawnPoint2.Y;
                    player1.dead = false;
                }

                else if (maxDistance == player3Distance)
                {
                    player1.pPosition.X = (int)respawnPoint3.X;
                    player1.pPosition.Y = (int)respawnPoint3.Y;
                    player1.dead = false;
                }

                else
                {
                    player1.pPosition.X = (int)respawnPoint4.X;
                    player1.pPosition.Y = (int)respawnPoint4.Y;
                    player1.dead = false;
                }

                if (bullet1.bDead == true)
                {
                    player1.pBullet.bDead = false;
                    player1.pBullet.bIsLoaded = true;

                }

                else if(bullet2.bDead == true)
                {
                    player1.pBullet.bDead = false;
                    player1.pBullet.bIsLoaded = true;
                }
            }

            if (player2.dead == true)
            {

                double x1Distance = Math.Pow(respawnPoint1.X - player1.pPosition.X, 2);
                double y1Distance = Math.Pow(respawnPoint1.Y - player1.pPosition.Y, 2);
                double player1Distance = Math.Sqrt(x1Distance + y1Distance);

                double x2Distance = Math.Pow(respawnPoint2.X - player1.pPosition.X, 2);
                double y2Distance = Math.Pow(respawnPoint2.Y - player1.pPosition.Y, 2);
                double player2Distance = Math.Sqrt(x2Distance + y2Distance);

                double x3Distance = Math.Pow(respawnPoint3.X - player1.pPosition.X, 2);
                double y3Distance = Math.Pow(respawnPoint3.Y - player1.pPosition.Y, 2);
                double player3Distance = Math.Sqrt(x3Distance + y3Distance);

                double x4Distance = Math.Pow(respawnPoint4.X - player1.pPosition.X, 2);
                double y4Distance = Math.Pow(respawnPoint4.Y - player1.pPosition.Y, 2);
                double player4Distance = Math.Sqrt(x4Distance + y4Distance);

                double maxDistance = Math.Max(Math.Max(Math.Max(player1Distance, player2Distance), player3Distance), player4Distance);

                if(maxDistance == player1Distance)
                {
                    player2.pPosition.X = (int)respawnPoint1.X;
                    player2.pPosition.Y = (int)respawnPoint1.Y;
                    player2.dead = false;
                }

                else if (maxDistance == player2Distance)
                {
                    player2.pPosition.X = (int)respawnPoint2.X;
                    player2.pPosition.Y = (int)respawnPoint2.Y;
                    player2.dead = false;
                }

                else if (maxDistance == player3Distance)
                {
                    player2.pPosition.X = (int)respawnPoint3.X;
                    player2.pPosition.Y = (int)respawnPoint3.Y;
                    player2.dead = false;
                }

                else
                {
                    player2.pPosition.X = (int)respawnPoint4.X;
                    player2.pPosition.Y = (int)respawnPoint4.Y;
                    player2.dead = false;
                }

                if (bullet1.bDead == true)
                {
                    player2.pBullet.bDead = false;
                    player2.pBullet.bIsLoaded = true;

                }

                else if (bullet2.bDead == true)
                {
                    player2.pBullet.bDead = false;
                    player2.pBullet.bIsLoaded = true;
                }

            }
        }
	}
}
