using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;

namespace OneBullet
{
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

			graphics.PreferredBackBufferWidth = 1920;
			graphics.PreferredBackBufferHeight = 1080;
			graphics.IsFullScreen = true;
			graphics.ApplyChanges();

			charHeight = GraphicsDevice.Viewport.Height / 7;
			charWidth = charHeight / 1.48;
		}

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

			Texture2D player1TextureR = Content.Load<Texture2D>("p1RunR");
			Texture2D player1TextureL = Content.Load<Texture2D>("p1RunL");
			Texture2D player1JumpR = Content.Load<Texture2D>("p1JumpR");
			Texture2D player1JumpL = Content.Load<Texture2D>("p1JumpL");
			Texture2D player2TextureR = Content.Load<Texture2D>("p2RunR");
			Texture2D player2TextureL = Content.Load<Texture2D>("p2RunL");
			Texture2D player2JumpR = Content.Load<Texture2D>("p2JumpR");
			Texture2D player2JumpL = Content.Load<Texture2D>("p2JumpL");

			Texture2D gunR = Content.Load<Texture2D>("gunR_loaded");
			Texture2D gunL = Content.Load<Texture2D>("gunL_loaded");
			Texture2D gunUR = Content.Load<Texture2D>("gunR_unloaded");
			Texture2D gunUL = Content.Load<Texture2D>("gunL_unloaded");

			Texture2D bullet = Content.Load<Texture2D>("bullet");

			Texture2D p1Life = Content.Load<Texture2D>("p1Alive");
			Texture2D p1Dead = Content.Load<Texture2D>("p1Dead");

			Texture2D p2Life = Content.Load<Texture2D>("p2Alive");
			Texture2D p2Dead = Content.Load<Texture2D>("p2Dead");

			SoundEffect sfShellFall = Content.Load<SoundEffect>("Shells_falls-Marcel-829263474");
			SoundEffect sfReload = Content.Load<SoundEffect>("50 Cal Machine Gun Load-SoundBible.com-1345076003");
			SoundEffect sfDead = Content.Load<SoundEffect>("Pain-SoundBible.com-1883168362");
			SoundEffect sfFire = Content.Load<SoundEffect>("9_mm_gunshot-mike-koenig-123");

			bullet1.Initialize(bullet, b1Position, sfShellFall, sfFire);
			bullet2.Initialize(bullet, b1Position, sfShellFall, sfFire);
			player1.Initialize(player1TextureR, player1TextureL, player1JumpR, player1JumpL, gunR, gunL, gunUR, gunUL, p1Life, p1Dead, p1Position, p1GunPos, p1GunOffset, bullet1, sfReload, sfDead, 1);
			player2.Initialize(player2TextureR, player2TextureL, player2JumpR, player2JumpL, gunR, gunL, gunUR, gunUL, p2Life, p2Dead, p2Position, p2GunPos, p2GunOffset, bullet2, sfReload, sfDead, 2);

			// ------------------------------------------ Level and platform content
			Level01Init();
		}

		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

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
			Rectangle p1CatchPosition = player1.pGunCollisionPosition;
			Rectangle p2CatchPosition = player2.pGunCollisionPosition;
			Rectangle p1ModPosition = player1.pCollisionPosition;
			Rectangle p2ModPosition = player2.pCollisionPosition;

			p1CatchPosition.Width = p1CatchPosition.Width / 2;
			p2CatchPosition.Width = p2CatchPosition.Width / 2;

			if (player1.pTexture == player1.pTextureL)
			{
				p1CatchPosition.X += p1CatchPosition.Width;
			}

			if (player2.pTexture == player2.pTextureL)
			{
				p2CatchPosition.X += p1CatchPosition.Width;
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
			Vector2 respawnPoint1 = new Vector2(Level.curLevel.floor.platPosition.X + (Level.curLevel.floor.platPosition.Width / 4), (Level.curLevel.floor.platPosition.Y - ((int)charHeight / 2)) - 1);
			Vector2 respawnPoint2 = new Vector2(Level.curLevel.floor.platPosition.X + (3 * (Level.curLevel.floor.platPosition.Width / 4)), (Level.curLevel.floor.platPosition.Y - ((int)charHeight / 2)) - 1);
			Vector2 respawnPoint3 = new Vector2(Level.curLevel.lPlatforms[0].platPosition.X + (Level.curLevel.lPlatforms[0].platPosition.Width / 2), (Level.curLevel.lPlatforms[0].platPosition.Y - ((int)charHeight / 2)) - 1);
			Vector2 respawnPoint4 = new Vector2(Level.curLevel.lPlatforms[1].platPosition.X + (Level.curLevel.lPlatforms[1].platPosition.Width / 2), (Level.curLevel.lPlatforms[1].platPosition.Y - ((int)charHeight / 2)) - 1);
			Vector2 respawnPoint5 = new Vector2(Level.curLevel.lPlatforms[7].platPosition.X + (Level.curLevel.lPlatforms[7].platPosition.Width / 2), (Level.curLevel.lPlatforms[7].platPosition.Y - ((int)charHeight / 2)) - 1);
			Vector2 respawnPoint6 = new Vector2(Level.curLevel.lPlatforms[8].platPosition.X + (Level.curLevel.lPlatforms[8].platPosition.Width / 2), (Level.curLevel.lPlatforms[8].platPosition.Y - ((int)charHeight / 2)) - 1);
			Vector2 respawnPoint7 = new Vector2(Level.curLevel.lPlatforms[9].platPosition.X + (Level.curLevel.lPlatforms[9].platPosition.Width / 2), (Level.curLevel.lPlatforms[9].platPosition.Y - ((int)charHeight / 2)) - 1);
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

			double x5Distance;
			double y5Distance;
			double playerDistance5;

			double x6Distance;
			double y6Distance;
			double playerDistance6;

			double x7Distance;
			double y7Distance;
			double playerDistance7;

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

				x5Distance = Math.Pow(respawnPoint5.X - player2.pCollisionPosition.X, 2);
				y5Distance = Math.Pow(respawnPoint5.Y - player2.pCollisionPosition.Y, 2);
				playerDistance5 = Math.Sqrt(x5Distance + y5Distance);

				x6Distance = Math.Pow(respawnPoint6.X - player2.pCollisionPosition.X, 2);
				y6Distance = Math.Pow(respawnPoint6.Y - player2.pCollisionPosition.Y, 2);
				playerDistance6 = Math.Sqrt(x6Distance + y6Distance);

				x7Distance = Math.Pow(respawnPoint7.X - player2.pCollisionPosition.X, 2);
				y7Distance = Math.Pow(respawnPoint7.Y - player2.pCollisionPosition.Y, 2);
				playerDistance7 = Math.Sqrt(x7Distance + y7Distance);

				maxYDistance = Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(y1Distance, y2Distance), y3Distance), y4Distance), y5Distance), y6Distance), y7Distance);

				if (maxYDistance == y7Distance)
				{
					xDist = Math.Abs(player1.pPosition.X - respawnPoint7.X);
					yDist = Math.Abs(player1.pPosition.Y - respawnPoint7.Y);
					if (xDist > charWidth / 2 && yDist > charHeight / 2)
					{
						respawnPoint = respawnPoint7;
					}
					else
					{
						maxYDistance = Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(y1Distance, y2Distance), y3Distance), y4Distance), y5Distance), y6Distance);
					}
				}

				if (maxYDistance == y1Distance || maxYDistance == y2Distance)
				{
					maxDistance = Math.Max(playerDistance1, playerDistance2);
					if (maxDistance == playerDistance1)
					{
						xDist = Math.Abs(player1.pPosition.X - respawnPoint1.X);
						yDist = Math.Abs(player1.pPosition.Y - respawnPoint1.Y);
						if (xDist > charWidth / 2 && yDist > charHeight / 2)
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
						if (xDist > charWidth / 2 && yDist > charHeight / 2)
						{
							respawnPoint = respawnPoint2;
						}
						else
						{
							respawnPoint = respawnPoint1;
						}
					}
				}
				else if (maxYDistance == y3Distance || maxYDistance == y4Distance)
				{
					maxDistance = Math.Max(playerDistance3, playerDistance4);
					if (maxDistance == playerDistance3)
					{
						xDist = Math.Abs(player1.pPosition.X - respawnPoint3.X);
						yDist = Math.Abs(player1.pPosition.Y - respawnPoint3.Y);
						if (xDist > charWidth / 2 && yDist > charHeight / 2)
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
						if (xDist > charWidth / 2 && yDist > charHeight / 2)
						{
							respawnPoint = respawnPoint4;
						}
						else
						{
							respawnPoint = respawnPoint3;
						}
					}
				}
				else if (maxYDistance == y5Distance || maxYDistance == y6Distance)
				{
					maxDistance = Math.Max(playerDistance5, playerDistance6);
					if (maxDistance == playerDistance5)
					{
						xDist = Math.Abs(player1.pPosition.X - respawnPoint5.X);
						yDist = Math.Abs(player1.pPosition.Y - respawnPoint5.Y);
						if (xDist > charWidth / 2 && yDist > charHeight / 2)
						{
							respawnPoint = respawnPoint5;
						}
						else
						{
							respawnPoint = respawnPoint6;
						}
					}
					else
					{
						xDist = Math.Abs(player1.pPosition.X - respawnPoint6.X);
						yDist = Math.Abs(player1.pPosition.Y - respawnPoint6.Y);
						if (xDist > charWidth / 2 && yDist > charHeight / 2)
						{
							respawnPoint = respawnPoint6;
						}
						else
						{
							respawnPoint = respawnPoint5;
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

				x5Distance = Math.Pow(respawnPoint5.X - player1.pCollisionPosition.X, 2);
				y5Distance = Math.Pow(respawnPoint5.Y - player1.pCollisionPosition.Y, 2);
				playerDistance5 = Math.Sqrt(x5Distance + y5Distance);

				x6Distance = Math.Pow(respawnPoint6.X - player1.pCollisionPosition.X, 2);
				y6Distance = Math.Pow(respawnPoint6.Y - player1.pCollisionPosition.Y, 2);
				playerDistance6 = Math.Sqrt(x6Distance + y6Distance);

				x7Distance = Math.Pow(respawnPoint7.X - player1.pCollisionPosition.X, 2);
				y7Distance = Math.Pow(respawnPoint7.Y - player1.pCollisionPosition.Y, 2);
				playerDistance7 = Math.Sqrt(x7Distance + y7Distance);

				maxYDistance = Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(y1Distance, y2Distance), y3Distance), y4Distance), y5Distance), y6Distance), y7Distance);

				if (maxYDistance == y7Distance)
				{
					xDist = Math.Abs(player2.pPosition.X - respawnPoint7.X);
					yDist = Math.Abs(player2.pPosition.Y - respawnPoint7.Y);
					if (xDist > charWidth / 2  && yDist > charHeight / 2)
					{
						respawnPoint = respawnPoint7;
					}
					else
					{
						maxYDistance = Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(y1Distance, y2Distance), y3Distance), y4Distance), y5Distance), y6Distance);
					}
				}

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
				else if (maxYDistance == y3Distance || maxYDistance == y4Distance)
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
				else if (maxYDistance == y5Distance || maxYDistance == y6Distance)
				{
					maxDistance = Math.Max(playerDistance5, playerDistance6);
					if (maxDistance == playerDistance5)
					{
						xDist = Math.Abs(player2.pPosition.X - respawnPoint5.X);
						yDist = Math.Abs(player2.pPosition.Y - respawnPoint5.Y);
						if (xDist > charWidth && yDist > charHeight)
						{
							respawnPoint = respawnPoint5;
						}
						else
						{
							respawnPoint = respawnPoint6;
						}
					}
					else
					{
						xDist = Math.Abs(player2.pPosition.X - respawnPoint6.X);
						yDist = Math.Abs(player2.pPosition.Y - respawnPoint6.Y);
						if (xDist > charWidth / 2  && yDist > charHeight / 2)
						{
							respawnPoint = respawnPoint6;
						}
						else
						{
							respawnPoint = respawnPoint5;
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

		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();
			level01.Draw(spriteBatch);
			player1.Draw(spriteBatch, GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width);
			player2.Draw(spriteBatch, GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width);
			bullet1.Draw(spriteBatch);
			bullet2.Draw(spriteBatch);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		void Level01Init()
		{
			SoundEffect sfMusic = Content.Load<SoundEffect>("One Bullet OST Main Theme 180bpm");
			Texture2D background = Content.Load<Texture2D>("Level01/FINAL");
			Texture2D l1B1 = Content.Load<Texture2D>("Level01/Final_B1");
			Texture2D l1B2 = Content.Load<Texture2D>("Level01/Final_B2");
			Texture2D l1T1 = Content.Load<Texture2D>("Level01/Final_T1");
			Texture2D l1T2 = Content.Load<Texture2D>("Level01/Final_T2");
			Texture2D l1T3 = Content.Load<Texture2D>("Level01/Final_T3");
			Texture2D l1T4 = Content.Load<Texture2D>("Level01/Final_T4");
			Texture2D l1P1 = Content.Load<Texture2D>("Level01/Final_P1");
			Texture2D l1P2 = Content.Load<Texture2D>("Level01/Final_P2");
			Texture2D l1Floor = Content.Load<Texture2D>("Level01/FINAL_Border_Bottom");
			Texture2D l1Walls = Content.Load<Texture2D>("Level01/FINAL_Border_Side");
			Texture2D l1Ceiling = Content.Load<Texture2D>("Level01/FINAL_Border_Top");

			Rectangle backgroundPos = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

			Rectangle floorPlatPos = new Rectangle(0, (int)(GraphicsDevice.Viewport.Height * 0.95037), GraphicsDevice.Viewport.Width, (int)(GraphicsDevice.Viewport.Height * 0.04963));
			Platforms floorPlat = new Platforms();
			floorPlat.Initialize(l1Floor, floorPlatPos);

			Rectangle ceilingPlatPos = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, (int)(GraphicsDevice.Viewport.Height * 0.0163));
			Platforms ceilingPlat = new Platforms();
			ceilingPlat.Initialize(l1Ceiling, ceilingPlatPos);

			Rectangle rWallPlatPos = new Rectangle((int)(GraphicsDevice.Viewport.Width * 0.99104), 0, (int)(GraphicsDevice.Viewport.Width * 0.00813), GraphicsDevice.Viewport.Height);
			Platforms rWallPlat = new Platforms();
			rWallPlat.Initialize(l1Walls, rWallPlatPos);

			Rectangle lWallPlatPos = new Rectangle(0, 0, (int)(GraphicsDevice.Viewport.Width * 0.00813), GraphicsDevice.Viewport.Height);
			Platforms lWallPlat = new Platforms();
			lWallPlat.Initialize(l1Walls, lWallPlatPos);

			Rectangle l1B1Pos = new Rectangle(0, (int)(GraphicsDevice.Viewport.Height * 0.72556), (int)(GraphicsDevice.Viewport.Width * 0.16396), (int)(GraphicsDevice.Viewport.Height * 0.27444));
			Rectangle l1B2Pos = new Rectangle((int)(GraphicsDevice.Viewport.Width * 0.84175), (int)(GraphicsDevice.Viewport.Height * 0.72556), (int)(GraphicsDevice.Viewport.Width * 0.15813), (int)(GraphicsDevice.Viewport.Height * 0.27444));

			Platforms platB1 = new Platforms();
			Platforms platB2 = new Platforms();

			platB1.Initialize(l1B1, l1B1Pos);
			platB2.Initialize(l1B2, l1B2Pos);

			Rectangle l1T1Pos = new Rectangle(0, (int)(GraphicsDevice.Viewport.Height * 0.0037), (int)(GraphicsDevice.Viewport.Width * 0.14917), (int)(GraphicsDevice.Viewport.Height * 0.13111));
			Rectangle l1T2Pos = new Rectangle((int)(GraphicsDevice.Viewport.Width * 0.00542), (int)(GraphicsDevice.Viewport.Height * 0.13222), (int)(GraphicsDevice.Viewport.Width * 0.07375), (int)(GraphicsDevice.Viewport.Height * 0.14667));
			Rectangle l1T3Pos = new Rectangle((int)(GraphicsDevice.Viewport.Width * 0.84521), (int)(GraphicsDevice.Viewport.Height * 0.01185), (int)(GraphicsDevice.Viewport.Width * 0.15458), (int)(GraphicsDevice.Viewport.Height * 0.13185));
			Rectangle l1T4Pos = new Rectangle((int)(GraphicsDevice.Viewport.Width * 0.92521), (int)(GraphicsDevice.Viewport.Height * 0.1437), (int)(GraphicsDevice.Viewport.Width * 0.07479), (int)(GraphicsDevice.Viewport.Height * 0.12556));

			Platforms platT1 = new Platforms();
			Platforms platT2 = new Platforms();
			Platforms platT3 = new Platforms();
			Platforms platT4 = new Platforms();

			platT1.Initialize(l1T1, l1T1Pos);
			platT2.Initialize(l1T2, l1T2Pos);
			platT3.Initialize(l1T3, l1T3Pos);
			platT4.Initialize(l1T4, l1T4Pos);

			Rectangle l1P1Pos = new Rectangle((int)(GraphicsDevice.Viewport.Width * 0.47), (int)(GraphicsDevice.Viewport.Height * 0.73704), (int)(GraphicsDevice.Viewport.Width * 0.09188), (int)(GraphicsDevice.Viewport.Height * 0.03444)); // Bottom
			Rectangle l1P2Pos = new Rectangle((int)(GraphicsDevice.Viewport.Width * 0.24688), (int)(GraphicsDevice.Viewport.Height * 0.52556), (int)(GraphicsDevice.Viewport.Width * 0.14521), (int)(GraphicsDevice.Viewport.Height * 0.03519)); // Mid left
			Rectangle l1P3Pos = new Rectangle((int)(GraphicsDevice.Viewport.Width * 0.64354), (int)(GraphicsDevice.Viewport.Height * 0.52333), (int)(GraphicsDevice.Viewport.Width * 0.14521), (int)(GraphicsDevice.Viewport.Height * 0.03519)); // Mid Right
			Rectangle l1P4Pos = new Rectangle((int)(GraphicsDevice.Viewport.Width * 0.44354), (int)(GraphicsDevice.Viewport.Height * 0.31481), (int)(GraphicsDevice.Viewport.Width * 0.14521), (int)(GraphicsDevice.Viewport.Height * 0.03519)); // Top

			Platforms platP1 = new Platforms();
			Platforms platP2 = new Platforms();
			Platforms platP3 = new Platforms();
			Platforms platP4 = new Platforms();

			platP1.Initialize(l1P2, l1P1Pos);
			platP2.Initialize(l1P1, l1P2Pos);
			platP3.Initialize(l1P1, l1P3Pos);
			platP4.Initialize(l1P1, l1P4Pos);

			Platforms[] lvlPlats = { platB1, platB2, platT1, platT2, platT3, platT4, platP1, platP2, platP3, platP4 };

			SoundEffectInstance sfMusicInstance = sfMusic.CreateInstance();
			sfMusicInstance.IsLooped = true;

			level01.Initialize(background, backgroundPos, sfMusicInstance, 10, lvlPlats, floorPlat, ceilingPlat, rWallPlat, lWallPlat);
		}
	}
}
