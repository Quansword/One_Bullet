using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace OneBullet
{
	class Player
	{
		public Texture2D pTexture, pTextureR, pTextureL, pJumpTextureR, pJumpTextureL;
		public Texture2D pGunTexture, pGunTextureR, pGunTextureL, pGunTextureUnloadedR, pGunTextureUnloadedL;
		Texture2D pAliveTexture, pDeadTexture;
		Texture2D[] pLivesTextures;
		public Rectangle pPosition, pCollisionPosition;
		Rectangle newPosition;
		Rectangle spriteSheet;
		public Rectangle pGunPosition, pGunCollisionPosition, pGunDisplayLoaded, pGunDisplayUnloaded;
		Vector2 pVelocity;
		int pLevelOffset;
		public int pGunOffset;
		public int pLives = 10;
		float pAcceleration;
		public bool onGround, jumping, loaded, dead;
		Keys jump, lowerGun, raiseGun, shoot, left, right;
		int playerNum;
		SoundEffect sfReload;
		SoundEffect sfDead;
		float pTimeElapsed;
		const float spriteDelay = 60f;
		int spriteXFrames = 0;
		int spriteYFrames = 0;


		public Bullet pBullet;

		public enum GunLevel
		{
			High,
			Mid,
			Low
		};

		public GunLevel level = GunLevel.Mid;

		int collisionPlatform;
		Level.CollisionDir collisionDir = Level.CollisionDir.None;

		public void Initialize(Texture2D textureR, Texture2D textureL, Texture2D jumpTextureR, Texture2D jumpTextureL, Texture2D gunTextureR, Texture2D gunTextureL, Texture2D gunUnloadedR, Texture2D gunUnloadedL, Texture2D aliveGUITexture, Texture2D deadGUITexture, Rectangle position, Rectangle gunPosition, int gunOffset, Bullet bullet, SoundEffect reloadSound, SoundEffect deadSound, int pNum, GraphicsDevice graphics)
		{
			playerNum = pNum;
			pTextureR = textureR;
			pTextureL = textureL;
			pJumpTextureR = jumpTextureR;
			pJumpTextureL = jumpTextureL;
			pGunTextureR = gunTextureR;
			pGunTextureL = gunTextureL;
			pGunTextureUnloadedR = gunUnloadedR;
			pGunTextureUnloadedL = gunUnloadedL;
			pAliveTexture = aliveGUITexture;
			pDeadTexture = deadGUITexture;
			pLivesTextures = new Texture2D[10];
			for (int i = 0; i < 10; i++)
			{
				pLivesTextures[i] = pAliveTexture;
			}
			if (playerNum % 2 == 1)
			{
				pTexture = pTextureR;
				pGunTexture = pGunTextureR;
			}
			else
			{
				pTexture = pTextureL;
				pGunTexture = pGunTextureL;
			}
			pPosition = position;
			newPosition = pPosition;
			pCollisionPosition = pPosition;
			pCollisionPosition.X -= (int)(pCollisionPosition.Width / 2);
			pCollisionPosition.Y -= (int)(pCollisionPosition.Height / 2);
			pGunOffset = gunOffset;
			pGunPosition = gunPosition;
			pGunPosition.X += pGunOffset;
			pGunCollisionPosition = pGunPosition;
			pGunCollisionPosition.Height = (3 * (pGunPosition.Height / 2));
			pGunCollisionPosition.Width = (int)(pGunPosition.Width * 1.2);
			pGunCollisionPosition.X -= pGunPosition.Width / 2;
			pGunCollisionPosition.Y -= pGunPosition.Height / 2;
			pGunDisplayLoaded = pGunPosition;
			pGunDisplayLoaded.X -= pGunPosition.Width;
			pGunDisplayLoaded.Width = pGunDisplayLoaded.Height * 3;
			pGunDisplayUnloaded = pGunDisplayLoaded;
			pGunDisplayUnloaded.Height = (int)(pGunDisplayUnloaded.Height * 1.5);
			pBullet = bullet;
			sfReload = reloadSound;
			sfDead = deadSound;
			pVelocity = new Vector2(0, 0);
			onGround = true;
			jumping = false;
			loaded = true;
			dead = false;
			pLevelOffset = 0;
			collisionPlatform = -1;
			pTimeElapsed = 0;
			spriteSheet = new Rectangle(214 * 0, 317 * 0, 214, 317);
			pAcceleration = 3 * ((float)graphics.Viewport.Height / 720);
		}

		public void Update(KeyboardState kState, KeyboardState oldKState, GamePadState cState, GamePadState oldCState, GraphicsDevice graphics, GameTime gameTime)
		{
			if (!dead)
			{
				if (playerNum == 1)
				{
					shoot = Keys.F;
					left = Keys.A;
					right = Keys.D;
					jump = Keys.G;
					lowerGun = Keys.S;
					raiseGun = Keys.W;
				}
				else if (playerNum == 2)
				{
					shoot = Keys.NumPad1;
					left = Keys.Left;
					right = Keys.Right;
					jump = Keys.NumPad2;
					lowerGun = Keys.Down;
					raiseGun = Keys.Up;
				}

				if(cState.IsConnected)
				{
					if ((cState.Triggers.Right > 0.2 || kState.IsKeyDown(shoot)) && oldCState.Triggers.Right <= 0.2 && loaded && oldKState.IsKeyUp(shoot)) // Shoot bullet
					{
						if (Level.curLevel.PlatformCollision(pGunCollisionPosition) == -1)
						{
							if (pTexture == pTextureR)
							{
								if (level == GunLevel.Mid)
								{
									pBullet.Fire(new Vector2(1, 0), pGunPosition);
								}
								else if (level == GunLevel.High)
								{
									pBullet.Fire(new Vector2(1, 1), pGunPosition);
								}
								else
								{
									pBullet.Fire(new Vector2(1, -1), pGunPosition);
								}
							}
							else
							{
								if (level == GunLevel.Mid)
								{
									pBullet.Fire(new Vector2(-1, 0), pGunPosition);
								}
								else if (level == GunLevel.High)
								{
									pBullet.Fire(new Vector2(-1, 1), pGunPosition);
								}
								else
								{
									pBullet.Fire(new Vector2(-1, -1), pGunPosition);
								}
							}
							Fire();
						}
					}

					if (cState.ThumbSticks.Left.X < -0.2 || kState.IsKeyDown(left)) // Move left
					{
						pVelocity.X -= ((float)graphics.Viewport.Width / 128);
						if (pTexture == pTextureR)
						{
							Turn(-(int)pPosition.Width / 2);
						}
						WalkAnimate(gameTime);
					}

					if (cState.ThumbSticks.Left.X > 0.2 || kState.IsKeyDown(right)) // Move right
					{
						pVelocity.X += ((float)graphics.Viewport.Width / 128);
						if (pTexture == pTextureL)
						{
							Turn((int)pPosition.Width / 2);
						}
						WalkAnimate(gameTime);
					}

					if (!kState.IsKeyDown(right) && !kState.IsKeyDown(left) && cState.ThumbSticks.Left.X < 0.2 && cState.ThumbSticks.Left.X > -0.2) // if not moving, play still shot
					{
						spriteSheet.X = 214 * 0;
						spriteSheet.Y = 317 * 0;
					}

					if (cState.Buttons.A == ButtonState.Pressed || cState.Buttons.LeftShoulder == ButtonState.Pressed || kState.IsKeyDown(jump)) // Jump
					{
						if (onGround && !jumping)
						{
							pVelocity.Y -= 30 * ((float)graphics.Viewport.Height / 720);
							jumping = true;
							onGround = false;
						}
						else if (!onGround && jumping && pVelocity.Y < 0)
						{
							pVelocity.Y -= ((float)graphics.Viewport.Height / 720);
						}
					}
					if (kState.IsKeyUp(jump) && cState.Buttons.A == ButtonState.Released && cState.Buttons.LeftShoulder == ButtonState.Released && (oldCState.Buttons.A == ButtonState.Pressed || cState.Buttons.LeftShoulder == ButtonState.Pressed || oldKState.IsKeyDown(jump)))
					{
						jumping = false;
					}
					// if ((cState.ThumbSticks.Right.Y > 0.2 || cState.ThumbSticks.Left.Y > 0.2 || kState.IsKeyDown(raiseGun)) && oldKState.IsKeyUp(raiseGun) && oldCState.ThumbSticks.Right.Y < 0.2 && oldCState.ThumbSticks.Left.Y < 0.2)
					if ((cState.ThumbSticks.Right.Y > 0.2 || kState.IsKeyDown(raiseGun)) && oldKState.IsKeyUp(raiseGun) && oldCState.ThumbSticks.Right.Y < 0.2) // Change gun level
					{
						if (level == Player.GunLevel.Low)
						{
							level = Player.GunLevel.Mid;
							pLevelOffset = 0;
						}
						else if (level == Player.GunLevel.Mid)
						{
							level = Player.GunLevel.High;
							pLevelOffset = -((int)pPosition.Height / 4);
						}
					}
					// if ((cState.ThumbSticks.Right.Y < -0.2 || cState.ThumbSticks.Left.Y < -0.2 || kState.IsKeyDown(lowerGun)) && oldKState.IsKeyUp(lowerGun) && oldCState.ThumbSticks.Right.Y > -0.2 && oldCState.ThumbSticks.Left.Y > -0.2)
					if ((cState.ThumbSticks.Right.Y < -0.2 || kState.IsKeyDown(lowerGun)) && oldKState.IsKeyUp(lowerGun) && oldCState.ThumbSticks.Right.Y > -0.2) // Change gun level
					{
						if (level == Player.GunLevel.High)
						{
							level = Player.GunLevel.Mid;
							pLevelOffset = 0;
						}
						else if (level == Player.GunLevel.Mid)
						{
							level = Player.GunLevel.Low;
							pLevelOffset = (int)pPosition.Height / 4;
						}
					}
				}
				else
				{
					if (kState.IsKeyDown(shoot) && loaded && oldKState.IsKeyUp(shoot)) // Shoot bullet
					{
						if (Level.curLevel.PlatformCollision(pGunCollisionPosition) == -1)
						{
							if (pTexture == pTextureR)
							{
								if (level == GunLevel.Mid)
								{
									pBullet.Fire(new Vector2(1, 0), pGunPosition);
								}
								else if (level == GunLevel.High)
								{
									pBullet.Fire(new Vector2(1, 1), pGunPosition);
								}
								else
								{
									pBullet.Fire(new Vector2(1, -1), pGunPosition);
								}
							}
							else
							{
								if (level == GunLevel.Mid)
								{
									pBullet.Fire(new Vector2(-1, 0), pGunPosition);
								}
								else if (level == GunLevel.High)
								{
									pBullet.Fire(new Vector2(-1, 1), pGunPosition);
								}
								else
								{
									pBullet.Fire(new Vector2(-1, -1), pGunPosition);
								}
							}
							Fire();
						}
					}

					if (kState.IsKeyDown(left)) // Move left
					{
						pVelocity.X -= ((float)graphics.Viewport.Width / 128);
						if (pTexture == pTextureR)
						{
							Turn(-(int)pPosition.Width / 2);
						}
						WalkAnimate(gameTime);
					}

					if (kState.IsKeyDown(right)) // Move right
					{
						pVelocity.X += ((float)graphics.Viewport.Width / 128);
						if (pTexture == pTextureL)
						{
							Turn((int)pPosition.Width / 2);
						}
						WalkAnimate(gameTime);
					}

					if (!kState.IsKeyDown(right) && !kState.IsKeyDown(left)) // if not moving, play still shot
					{
						spriteSheet.X = 214 * 0;
						spriteSheet.Y = 317 * 0;
					}

					if (kState.IsKeyDown(jump)) // Jump
					{
						if (onGround && !jumping)
						{
							pVelocity.Y -= 30 * ((float)graphics.Viewport.Height / 720);
							jumping = true;
							onGround = false;
						}
						else if (!onGround && jumping && pVelocity.Y < 0)
						{
							pVelocity.Y -= ((float)graphics.Viewport.Height / 720);
						}
					}
					if (kState.IsKeyUp(jump) && oldKState.IsKeyDown(jump))
					{
						jumping = false;
					}
					if (kState.IsKeyDown(raiseGun) && oldKState.IsKeyUp(raiseGun)) // Change gun level
					{
						if (level == Player.GunLevel.Low)
						{
							level = Player.GunLevel.Mid;
							pLevelOffset = 0;
						}
						else if (level == Player.GunLevel.Mid)
						{
							level = Player.GunLevel.High;
							pLevelOffset = -((int)pPosition.Height / 4);
						}
					}
					if (kState.IsKeyDown(lowerGun) && oldKState.IsKeyUp(lowerGun)) // Change gun level
					{
						if (level == Player.GunLevel.High)
						{
							level = Player.GunLevel.Mid;
							pLevelOffset = 0;
						}
						else if (level == Player.GunLevel.Mid)
						{
							level = Player.GunLevel.Low;
							pLevelOffset = (int)pPosition.Height / 4;
						}
					}
				}

				// ------------------------------------------ Calculating velocity
				CalcVelocity(gameTime);
				CalcVelocity(gameTime);

				pPosition.X += (int)(pVelocity.X * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 16);
				pPosition.Y += (int)(pVelocity.Y * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 16);

				pCollisionPosition = pPosition;
				pCollisionPosition.X -= (int)(pCollisionPosition.Width / 2);
				pCollisionPosition.Y -= (int)(pCollisionPosition.Height / 2);

				pGunPosition.X = pPosition.X + pGunOffset;
				pGunPosition.Y = pPosition.Y + pLevelOffset;
				pGunCollisionPosition = pGunPosition;
				if (pTexture == pTextureL)
				{
					pGunCollisionPosition.X -= (int)(pGunPosition.Width * 0.2);
				}
				if (level == GunLevel.Mid)
				{
					pGunCollisionPosition.X -= pGunPosition.Width / 2;
					pGunCollisionPosition.Y -= pGunPosition.Height / 2;
					pGunCollisionPosition.Height = (3 * (pGunPosition.Height / 2));
					pGunCollisionPosition.Width = (int)(1.2 * pGunPosition.Width);
				}
				else if (level == GunLevel.Low)
				{
					if (pTexture == pTextureR)
					{
						pGunCollisionPosition.X -= pGunPosition.Width / 2;
					}
					else
					{
						pGunCollisionPosition.X -= pGunPosition.Width / 4;
					}
					pGunCollisionPosition.Y -= pGunPosition.Height / 4;
					pGunCollisionPosition.Width = 3 * (pGunPosition.Width / 4);
					pGunCollisionPosition.Height = (3 * (pGunPosition.Height / 2));
				}
				else
				{
					if (pTexture == pTextureR)
					{
						pGunCollisionPosition.X -= pGunPosition.Width / 2;
					}
					else
					{
						pGunCollisionPosition.X -= pGunPosition.Width / 4;
					}
					pGunCollisionPosition.Y -= pGunPosition.Height;
					pGunCollisionPosition.Width = 3 * (pGunPosition.Width / 4);
					pGunCollisionPosition.Height = (3 * (pGunPosition.Height / 2));
				}

				if (pVelocity.Y != 0)
					onGround = false;

				if (!jumping)
				{
					pVelocity.Y += pAcceleration - ((float)graphics.Viewport.Height / 720);
				}
				else
				{
					pVelocity.Y += pAcceleration;
				}

				// ------------------------------------------ Resetting values
				pVelocity.X = 0;
			}
			// ------------------------------------------ Death handle
			else if (dead && loaded)
			{
				if (pTexture == pTextureR)
				{
					pBullet.Dead(new Vector2(1, 0), pGunPosition);
				}
				else
				{
					pBullet.Dead(new Vector2(-1, 0), pGunPosition);
				}
				Fire();
			}
		}

		void CalcVelocity(GameTime gameTime)
		{
			collisionPlatform = -1;
			newPosition = pCollisionPosition;
			newPosition.X += (int)(pVelocity.X * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 16);
			newPosition.Y += (int)(pVelocity.Y * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 16);

			collisionPlatform = Level.curLevel.PlatformCollision(newPosition);
			if (collisionPlatform == -1)
			{
				collisionDir = Level.CollisionDir.None;
			}
			else
			{
				collisionDir = Level.curLevel.PlatformDirection(collisionPlatform, pCollisionPosition);
				if (collisionDir != Level.CollisionDir.None)
				{
					pVelocity = Level.curLevel.NewVelocity(collisionPlatform, collisionDir, pCollisionPosition, pVelocity);
					if (collisionDir == Level.CollisionDir.Bottom || collisionDir == Level.CollisionDir.Top)
					{
						pPosition.Y += (int)pVelocity.Y;
						pCollisionPosition.Y += (int)pVelocity.Y;
						pVelocity.Y = 0;
					}
					else
					{
						pPosition.X += (int)pVelocity.X;
						pCollisionPosition.X += (int)pVelocity.X;
						pVelocity.X = 0;
					}
				}
			}

			// ------------------------------------------ Falling parameters
			if (collisionDir == Level.CollisionDir.Bottom)
			{
				onGround = true;
			}
		}

		void WalkAnimate(GameTime gameTime)
		{
			pTimeElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			if (pTimeElapsed >= spriteDelay)
			{
				if (spriteXFrames >= 2)
				{
					spriteXFrames = 0;
					spriteYFrames++;
					if (spriteYFrames >= 3)
					{
						spriteYFrames = 0;
					}
				}
				else
				{
					spriteXFrames++;
				}
				pTimeElapsed = 0;
			}
			spriteSheet.X = 214 * spriteXFrames;
			spriteSheet.Y = 317 * spriteYFrames;
		}

		public void Draw(SpriteBatch spriteBatch, int height, int width)
		{
			//public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
			if (!dead)
			{
				if (!onGround)
				{
					if (pTexture == pTextureR)
					{
						spriteBatch.Draw(pJumpTextureR, pPosition, null, Color.White, 0, new Vector2(spriteSheet.Width / 3 + 30, spriteSheet.Height / 4 + 65), SpriteEffects.None, 0);
					}
					else
					{
						spriteBatch.Draw(pJumpTextureL, pPosition, null, Color.White, 0, new Vector2(spriteSheet.Width / 3 + 30, spriteSheet.Height / 4 + 65), SpriteEffects.None, 0);
					}
				}
				else
				{
					spriteBatch.Draw(pTexture, pPosition, spriteSheet, Color.White, 0, new Vector2(spriteSheet.Width / 3 + 30, spriteSheet.Height / 4 + 65), SpriteEffects.None, 0);
				}

				if (pTexture == pTextureR)
				{
					pGunDisplayLoaded.X = pGunPosition.X - (pGunPosition.Width / 4);
					pGunDisplayUnloaded.X = pGunPosition.X - (pGunPosition.Width / 4);
				}
				else
				{
					pGunDisplayLoaded.X = pGunPosition.X + (pGunPosition.Width / 4);
					pGunDisplayUnloaded.X = pGunPosition.X + (pGunPosition.Width / 4);
				}
				pGunDisplayLoaded.Y = pGunPosition.Y + (pGunPosition.Width / 10);
				pGunDisplayUnloaded.Y = pGunPosition.Y + (pGunPosition.Width / 5);
				if (level == GunLevel.Mid)
				{
					if (loaded)
					{
						if (pTexture == pTextureR)
						{
							spriteBatch.Draw(pGunTexture, pGunDisplayLoaded, null, Color.White, 0, new Vector2(pGunTexture.Width / 1.8f, pGunTexture.Height / 2), SpriteEffects.None, 0);
						}
						else
						{
							spriteBatch.Draw(pGunTexture, pGunDisplayLoaded, null, Color.White, 0, new Vector2(pGunTexture.Width / 2.2f, pGunTexture.Height / 2), SpriteEffects.None, 0);
						}
					}
					else
					{
						if (pTexture == pTextureR)
						{
							spriteBatch.Draw(pGunTexture, pGunDisplayUnloaded, null, Color.White, 0, new Vector2(pGunTexture.Width / 1.8f, pGunTexture.Height / 2.3f), SpriteEffects.None, 0);
						}
						else
						{
							spriteBatch.Draw(pGunTexture, pGunDisplayUnloaded, null, Color.White, 0, new Vector2(pGunTexture.Width / 2.2f, pGunTexture.Height / 2.3f), SpriteEffects.None, 0);
						}
					}
				}
				else if ((level == GunLevel.High && pTexture == pTextureL) || (level == GunLevel.Low && pTexture == pTextureR))
				{
					if (loaded)
					{
						if (level == GunLevel.Low && pTexture == pTextureR)
						{
							spriteBatch.Draw(pGunTexture, pGunDisplayLoaded, null, Color.White, 0.785398f, new Vector2(3.6f * (pGunTexture.Width / 5), pGunTexture.Height / 3.1f), SpriteEffects.None, 0);
						}
						else
						{
							spriteBatch.Draw(pGunTexture, pGunDisplayLoaded, null, Color.White, 0.785398f, new Vector2(pGunTexture.Width / 2.3f, pGunTexture.Height / 2.8f), SpriteEffects.None, 0);
						}
					}
					else
					{
						if (level == GunLevel.Low && pTexture == pTextureR)
						{
							spriteBatch.Draw(pGunTexture, pGunDisplayUnloaded, null, Color.White, 0.785398f, new Vector2(3.8f * (pGunTexture.Width / 5), pGunTexture.Height / 3.4f), SpriteEffects.None, 0);
						}
						else
						{
							spriteBatch.Draw(pGunTexture, pGunDisplayUnloaded, null, Color.White, 0.785398f, new Vector2(pGunTexture.Width / 2.14f, pGunTexture.Height / 3.2f), SpriteEffects.None, 0);
						}
					}
				}
				else
				{
					if (loaded)
					{
						if (level == GunLevel.Low && pTexture == pTextureL)
						{
							spriteBatch.Draw(pGunTexture, pGunDisplayLoaded, null, Color.White, -0.785398f, new Vector2(1.38f * (pGunTexture.Width / 5), pGunTexture.Height / 2.9f), SpriteEffects.None, 0);
						}
						else
						{
							spriteBatch.Draw(pGunTexture, pGunDisplayLoaded, null, Color.White, -0.785398f, new Vector2(1.13f * (pGunTexture.Width / 2), pGunTexture.Height / 2.8f), SpriteEffects.None, 0);
						}
					}
					else
					{
						if (level == GunLevel.Low && pTexture == pTextureL)
						{
							spriteBatch.Draw(pGunTexture, pGunDisplayUnloaded, null, Color.White, -0.785398f, new Vector2(1.2f * (pGunTexture.Width / 5), pGunTexture.Height / 3.3f), SpriteEffects.None, 0);
						}
						else
						{
							spriteBatch.Draw(pGunTexture, pGunDisplayUnloaded, null, Color.White, -0.785398f, new Vector2(1.06f * (pGunTexture.Width / 2), pGunTexture.Height / 3.2f), SpriteEffects.None, 0);
						}
					}
				}
			}
			if (playerNum == 1)
			{
				for (int i = 0; i < 10; i++)
				{
					spriteBatch.Draw(pLivesTextures[i], new Rectangle((height / 50) + ((height / 15) * i), height / 50, height / 10, height / 10), Color.White);
				}
			}
			else
			{
				for (int i = 9; i > -1; i--)
				{
					spriteBatch.Draw(pLivesTextures[i], new Rectangle(width - ((height / 8) + ((height / 15) * i)), height / 50, height / 10, height / 10), Color.White);
				}
			}
		}

		public void Turn(int gunOffset)
		{
			if (!dead)
			{
				if (pTexture == pTextureR)
				{
					pTexture = pTextureL;
					if (loaded)
					{
						pGunTexture = pGunTextureL;
					}
					else
					{
						pGunTexture = pGunTextureUnloadedL;
					}
				}
				else
				{
					pTexture = pTextureR;
					if (loaded)
					{
						pGunTexture = pGunTextureR;
					}
					else
					{
						pGunTexture = pGunTextureUnloadedR;
					}
				}
				pGunOffset = gunOffset;
			}
		}

		public void Fire()
		{
			if (pBullet != null)
			{
				pBullet = null;
				loaded = false;
				if (pTexture == pTextureR)
				{
					pGunTexture = pGunTextureUnloadedR;
				}
				else
				{
					pGunTexture = pGunTextureUnloadedL;
				}
			}
		}

		public void Pickup(Bullet bullet)
		{
			if (!loaded && !dead)
			{
				pBullet = bullet;
				loaded = true;
				sfReload.Play();
				if (pTexture == pTextureR)
				{
					pGunTexture = pGunTextureR;
				}
				else
				{
					pGunTexture = pGunTextureL;
				}
			}
		}

		public void Catch(Bullet bullet)
		{
			if (!dead)
			{
				if (loaded)
				{
					Hit();
				}
				else
				{
					pBullet = bullet;
					loaded = true;
					sfReload.Play();
					if (pTexture == pTextureR)
					{
						pGunTexture = pGunTextureR;
					}
					else
					{
						pGunTexture = pGunTextureL;
					}
				}
			}
		}

		public void Hit()
		{
			dead = true;
			pLives--;
			pLivesTextures[pLives] = pDeadTexture;
			sfDead.Play();
		}

		public void Respawn(int xPos, int yPos, bool spawnRight, Bullet bullet)
		{
			pBullet = bullet;
			if (spawnRight)
			{
				pTexture = pTextureR;
				pGunTexture = pGunTextureR;
				pGunOffset = pPosition.Width / 2;
			}
			else
			{
				pTexture = pTextureL;
				pGunTexture = pGunTextureL;
				pGunOffset = -(pPosition.Width / 2);
			}
			pPosition.X = xPos;
			pPosition.Y = yPos;
			newPosition = pPosition;
			pCollisionPosition = pPosition;
			pCollisionPosition.X -= (int)(pCollisionPosition.Width / 2);
			pCollisionPosition.Y -= (int)(pCollisionPosition.Height / 2);
			pGunPosition.X = pPosition.X + pGunOffset;
			pGunPosition.Y = pPosition.Y - (pPosition.Height / 4);
			pGunCollisionPosition = pGunPosition;
			pGunCollisionPosition.Height = (3 * (pGunPosition.Height / 2));
			pGunCollisionPosition.Width = (int)(pGunPosition.Width * 1.2);
			pGunCollisionPosition.X -= pGunPosition.Width / 2;
			pGunCollisionPosition.Y -= pGunPosition.Height / 2;
			pVelocity.X = 0;
			pVelocity.Y = 0;
			onGround = true;
			jumping = false;
			loaded = true;
			dead = false;
			pLevelOffset = 0;
			collisionPlatform = -1;
			level = GunLevel.Mid;
			pTimeElapsed = 0;
		}
	}
}
