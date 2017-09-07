using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
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
			Rectangle p1GunPos = new Rectangle(p1Position.X, p1Position.Y - (p1Position.Height / 4), (int)(charWidth * 0.75), (int)(charHeight / 3));
			int p1GunOffset = (int)charWidth / 2;

			Rectangle p2Position = new Rectangle((3 * (GraphicsDevice.Viewport.Width / 4)), GraphicsDevice.Viewport.Height - (int)(2 * charHeight), (int)charWidth, (int)charHeight);
			Rectangle p2GunPos = new Rectangle(p2Position.X, p2Position.Y - (p1Position.Height / 4), (int)(charWidth * 0.75), (int)(charHeight / 3));
			int p2GunOffset = -(int)charWidth / 2;

			Rectangle b1Position = new Rectangle(-100, -100, (int)(charHeight / 2), (int)(charHeight / 2));
			Rectangle b2Position = new Rectangle(-100, -100, (int)(charHeight / 2), (int)(charHeight / 2));

			Texture2D player1TextureR = Content.Load<Texture2D>("p1RunR");
			Texture2D player1TextureL = Content.Load<Texture2D>("p1RunL");
			Texture2D player2TextureR = Content.Load<Texture2D>("p2RunR");
			Texture2D player2TextureL = Content.Load<Texture2D>("p2RunL");
            //Texture2D playerJump = Content.Load<Texture2D>("jump");

            Texture2D gunR = Content.Load<Texture2D>("gun_right");
			Texture2D gunL = Content.Load<Texture2D>("gun_left");

			Texture2D bullet = Content.Load<Texture2D>("bullet");

			SoundEffect sfShellFall = Content.Load<SoundEffect>("Shells_falls-Marcel-829263474");
			SoundEffect sfReload = Content.Load<SoundEffect>("50 Cal Machine Gun Load-SoundBible.com-1345076003");
			SoundEffect sfDead = Content.Load<SoundEffect>("Pain-SoundBible.com-1883168362");
			SoundEffect sfFire = Content.Load<SoundEffect>("9_mm_gunshot-mike-koenig-123");

			bullet1.Initialize(bullet, b1Position, sfShellFall, sfFire);
			bullet2.Initialize(bullet, b1Position, sfShellFall, sfFire);
			player1.Initialize(player1TextureR, player1TextureL, gunR, gunL, p1Position, p1GunPos, p1GunOffset, bullet1, sfReload, sfDead, 1);
			player2.Initialize(player2TextureR, player2TextureL, gunR, gunL, p2Position, p2GunPos, p2GunOffset, bullet2, sfReload, sfDead, 2);

			// ------------------------------------------ Level and platform content
			Texture2D background = Content.Load<Texture2D>("background1");
			SoundEffect sfMusic = Content.Load<SoundEffect>("One Bullet OST Main Theme 180bpm");
			Texture2D platform1 = Content.Load<Texture2D>("test_platform");

			Level01Init(background, platform1, sfMusic);
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
			bullet1.Update(GraphicsDevice, gameTime);
			bullet2.Update(GraphicsDevice, gameTime);

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
				p1CatchPosition.X -= (int)(player1.pGunPosition.Width / 2);
			}

			if (player2.pTexture == player2.pTextureR)
			{
				p2CatchPosition.X -= (int)(player2.pGunPosition.Width / 2);
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

		public void Respawn()
		{
			Vector2 respawnPoint1 = new Vector2(Level.curLevel.lPlatforms[0].platPosition.X + (Level.curLevel.lPlatforms[0].platPosition.Width / 2), (Level.curLevel.lPlatforms[0].platPosition.Y - ((int)charHeight / 2)) - 1);
			Vector2 respawnPoint2 = new Vector2(Level.curLevel.lPlatforms[1].platPosition.X + (Level.curLevel.lPlatforms[0].platPosition.Width / 2), (Level.curLevel.lPlatforms[1].platPosition.Y - ((int)charHeight / 2)) - 1);
			Vector2 respawnPoint3 = new Vector2(Level.curLevel.lPlatforms[2].platPosition.X + (Level.curLevel.lPlatforms[0].platPosition.Width / 2), (Level.curLevel.lPlatforms[2].platPosition.Y - ((int)charHeight / 2)) - 1);
			Vector2 respawnPoint4 = new Vector2(Level.curLevel.lPlatforms[3].platPosition.X + (Level.curLevel.lPlatforms[0].platPosition.Width / 2), (Level.curLevel.lPlatforms[3].platPosition.Y - ((int)charHeight / 2)) - 1);
			Vector2 respawnPoint = new Vector2(0, 0);
			bool playerDir = false;
			Bullet respawnBullet = bullet2;

			double x1Distance;
			double y1Distance;
			double playerDistance1;

			double x2Distance;
			double y2Distance;
			double playerDistance2;

			double x3Distance;
			double y3Distance;
			double playerDistance3;

			double x4Distance;
			double y4Distance;
			double playerDistance4;

			double maxDistance;
			double maxYDistance;
			double xDist;
			double yDist;

			if (player1.dead && player1.pLives > 0)
			{
				x1Distance = Math.Pow(respawnPoint1.X - player2.pCollisionPosition.X, 2);
				y1Distance = Math.Pow(respawnPoint1.Y - player2.pCollisionPosition.Y, 2);
				playerDistance1 = Math.Sqrt(x1Distance + y1Distance);

				x2Distance = Math.Pow(respawnPoint2.X - player2.pCollisionPosition.X, 2);
				y2Distance = Math.Pow(respawnPoint2.Y - player2.pCollisionPosition.Y, 2);
				playerDistance2 = Math.Sqrt(x2Distance + y2Distance);

				x3Distance = Math.Pow(respawnPoint3.X - player2.pCollisionPosition.X, 2);
				y3Distance = Math.Pow(respawnPoint3.Y - player2.pCollisionPosition.Y, 2);
				playerDistance3 = Math.Sqrt(x3Distance + y3Distance);

				x4Distance = Math.Pow(respawnPoint4.X - player2.pCollisionPosition.X, 2);
				y4Distance = Math.Pow(respawnPoint4.Y - player2.pCollisionPosition.Y, 2);
				playerDistance4 = Math.Sqrt(x4Distance + y4Distance);

				maxYDistance = Math.Max(Math.Max(Math.Max(y1Distance, y2Distance), y3Distance), y4Distance);

				if (maxYDistance == y1Distance || maxYDistance == y2Distance)
				{
					maxDistance = Math.Max(playerDistance1, playerDistance2);
					if (maxDistance == playerDistance1)
					{
						xDist = Math.Abs(player1.pPosition.X - respawnPoint1.X);
						yDist = Math.Abs(player1.pPosition.Y - respawnPoint1.Y);
						if (xDist > charWidth && yDist > charHeight)
						{
							respawnPoint = respawnPoint1;
						}
						else
						{
							respawnPoint = respawnPoint2;
						}
					}
					else if (maxDistance == playerDistance2)
					{
						xDist = Math.Abs(player1.pPosition.X - respawnPoint2.X);
						yDist = Math.Abs(player1.pPosition.Y - respawnPoint2.Y);
						if (xDist > charWidth && yDist > charHeight)
						{
							respawnPoint = respawnPoint2;
						}
						else
						{
							respawnPoint = respawnPoint1;
						}
					}
				}
				else
				{
					maxDistance = Math.Max(playerDistance3, playerDistance4);
					if (maxDistance == playerDistance3)
					{
						xDist = Math.Abs(player1.pPosition.X - respawnPoint3.X);
						yDist = Math.Abs(player1.pPosition.Y - respawnPoint3.Y);
						if (xDist > charWidth && yDist > charHeight)
						{
							respawnPoint = respawnPoint3;
						}
						else
						{
							respawnPoint = respawnPoint4;
						}
					}
					else
					{
						xDist = Math.Abs(player1.pPosition.X - respawnPoint4.X);
						yDist = Math.Abs(player1.pPosition.Y - respawnPoint4.Y);
						if (xDist > charWidth && yDist > charHeight)
						{
							respawnPoint = respawnPoint4;
						}
						else
						{
							respawnPoint = respawnPoint3;
						}
					}
				}

				if (respawnPoint.X < player2.pPosition.X)
				{
					playerDir = true;
				}

				if (player1.loaded)
				{
					respawnBullet = player1.pBullet;
				}
				else if (!bullet1.bIsLoaded)
				{
					respawnBullet = bullet1;
				}
				else if (!bullet2.bIsLoaded)
				{
					respawnBullet = bullet2;
				}

				player1.Respawn((int)respawnPoint.X, (int)respawnPoint.Y, playerDir, respawnBullet);
				respawnBullet.Respawn();
			}
			else if (player2.dead && player2.pLives > 0)
			{
				x1Distance = Math.Pow(respawnPoint1.X - player1.pCollisionPosition.X, 2);
				y1Distance = Math.Pow(respawnPoint1.Y - player1.pCollisionPosition.Y, 2);
				playerDistance1 = Math.Sqrt(x1Distance + y1Distance);

				x2Distance = Math.Pow(respawnPoint2.X - player1.pCollisionPosition.X, 2);
				y2Distance = Math.Pow(respawnPoint2.Y - player1.pCollisionPosition.Y, 2);
				playerDistance2 = Math.Sqrt(x2Distance + y2Distance);

				x3Distance = Math.Pow(respawnPoint3.X - player1.pCollisionPosition.X, 2);
				y3Distance = Math.Pow(respawnPoint3.Y - player1.pCollisionPosition.Y, 2);
				playerDistance3 = Math.Sqrt(x3Distance + y3Distance);

				x4Distance = Math.Pow(respawnPoint4.X - player1.pCollisionPosition.X, 2);
				y4Distance = Math.Pow(respawnPoint4.Y - player1.pCollisionPosition.Y, 2);
				playerDistance4 = Math.Sqrt(x4Distance + y4Distance);

				maxYDistance = Math.Max(Math.Max(Math.Max(y1Distance, y2Distance), y3Distance), y4Distance);

				if (maxYDistance == y1Distance || maxYDistance == y2Distance)
				{
					maxDistance = Math.Max(playerDistance1, playerDistance2);
					if (maxDistance == playerDistance1)
					{
						xDist = Math.Abs(player2.pPosition.X - respawnPoint1.X);
						yDist = Math.Abs(player2.pPosition.Y - respawnPoint1.Y);
						if (xDist > charWidth / 2 || yDist > charHeight / 2)
						{
							respawnPoint = respawnPoint1;
						}
						else
						{
							respawnPoint = respawnPoint2;
						}
					}
					else if (maxDistance == playerDistance2)
					{
						xDist = Math.Abs(player2.pPosition.X - respawnPoint2.X);
						yDist = Math.Abs(player2.pPosition.Y - respawnPoint2.Y);
						if (xDist > charWidth / 2 || yDist > charHeight / 2)
						{
							respawnPoint = respawnPoint2;
						}
						else
						{
							respawnPoint = respawnPoint1;
						}
					}
				}
				else
				{
					maxDistance = Math.Max(playerDistance3, playerDistance4);
					if (maxDistance == playerDistance3)
					{
						xDist = Math.Abs(player2.pPosition.X - respawnPoint3.X);
						yDist = Math.Abs(player2.pPosition.Y - respawnPoint3.Y);
						if (xDist > charWidth / 2 || yDist > charHeight / 2)
						{
							respawnPoint = respawnPoint3;
						}
						else
						{
							respawnPoint = respawnPoint4;
						}
					}
					else
					{
						xDist = Math.Abs(player2.pPosition.X - respawnPoint4.X);
						yDist = Math.Abs(player2.pPosition.Y - respawnPoint4.Y);
						if (xDist > charWidth / 2 || yDist > charHeight / 2)
						{
							respawnPoint = respawnPoint4;
						}
						else
						{
							respawnPoint = respawnPoint3;
						}
					}
				}

				if (respawnPoint.X < player1.pPosition.X)
				{
					playerDir = true;
				}

				if (player2.loaded)
				{
					respawnBullet = player2.pBullet;
				}
				else if (!bullet1.bIsLoaded)
				{
					respawnBullet = bullet1;
				}
				else if (!bullet2.bIsLoaded)
				{
					respawnBullet = bullet2;
				}

				player2.Respawn((int)respawnPoint.X, (int)respawnPoint.Y, playerDir, respawnBullet);
				respawnBullet.Respawn();
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

		void Level01Init(Texture2D background, Texture2D platform, SoundEffect music)
		{
			Rectangle backgroundPos = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

			Rectangle floorPlatPos = new Rectangle(0, GraphicsDevice.Viewport.Height - (int)(charHeight), GraphicsDevice.Viewport.Width, (int)charHeight);
			Platforms floorPlat = new Platforms();
			floorPlat.Initialize(platform, floorPlatPos);

			Rectangle ceilingPlatPos = new Rectangle(0, 0 - (int)charHeight, GraphicsDevice.Viewport.Width, (int)charHeight);
			Platforms ceilingPlat = new Platforms();
			ceilingPlat.Initialize(platform, ceilingPlatPos);

			Rectangle rWallPlatPos = new Rectangle(GraphicsDevice.Viewport.Width, 0, (int)charWidth, GraphicsDevice.Viewport.Height);
			Platforms rWallPlat = new Platforms();
			rWallPlat.Initialize(platform, rWallPlatPos);

			Rectangle lWallPlatPos = new Rectangle(0 - (int)charWidth, 0, (int)charWidth, GraphicsDevice.Viewport.Height);
			Platforms lWallPlat = new Platforms();
			lWallPlat.Initialize(platform, lWallPlatPos);

			Rectangle plat1Pos = new Rectangle( 100, GraphicsDevice.Viewport.Height - (int)(charHeight * 2), (int)charWidth * 2, (int)(2 * charHeight / 3)); //BOTTOM LEFT
			Rectangle plat2Pos = new Rectangle(GraphicsDevice.Viewport.Width / 2 + 350, GraphicsDevice.Viewport.Height - (int)(charHeight * 2), (int)charWidth * 2, (int)(2 * charHeight / 3)); //BOTTOM RIGHT
			Rectangle plat3Pos = new Rectangle(350 / 2 + 50, GraphicsDevice.Viewport.Height - (int)(charHeight * 2) -190, (int)charWidth * 2, (int)(2 * charHeight / 3)); //MID LEFT
			Rectangle plat4Pos = new Rectangle(GraphicsDevice.Viewport.Width / 2 +250, GraphicsDevice.Viewport.Height - (int)(charHeight * 2) - 190, (int)charWidth * 2, (int)(2 * charHeight / 3)); //MID RIGHTss
			Rectangle plat5Pos = new Rectangle(GraphicsDevice.Viewport.Width / 2 -100, GraphicsDevice.Viewport.Height - (int)(charHeight * 2) - 300, (int)charWidth * 2, (int)(2 * charHeight / 3)); //CENTER TOP

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

			SoundEffectInstance sfMusicInstance = music.CreateInstance();
			sfMusicInstance.IsLooped = true;

			level01.Initialize(background, backgroundPos, sfMusicInstance, 5, lvlPlats, floorPlat, ceilingPlat, rWallPlat, lWallPlat);
		}
	}
}
