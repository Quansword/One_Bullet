using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace OneBullet
{
	class Player
	{
		public Texture2D pTexture, pTextureR, pTextureL, pJumpTextureR, pJumpTextureL;
		public Texture2D pGunTexture, pGunTextureR, pGunTextureL;
		Texture2D pAliveTexture, pDeadTexture;
		Texture2D[] pLivesTextures;
		public Rectangle pPosition, pCollisionPosition;
		Rectangle newPosition;
		Rectangle spriteSheet;
		public Rectangle pGunPosition, pGunCollisionPosition;
		Vector2 pVelocity;
		int pLevelOffset;
		public int pGunOffset;
		public int pLives = 3;
		const int pAcceleration = 3;
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

		public void Initialize(Texture2D textureR, Texture2D textureL, Texture2D jumpTextureR, Texture2D jumpTextureL, Texture2D gunTextureR, Texture2D gunTextureL, Texture2D aliveGUITexture, Texture2D deadGUITexture, Rectangle position, Rectangle gunPosition, int gunOffset, Bullet bullet, SoundEffect reloadSound, SoundEffect deadSound, int pNum)
		{
			playerNum = pNum;
			pTextureR = textureR;
			pTextureL = textureL;
			pJumpTextureR = jumpTextureR;
			pJumpTextureL = jumpTextureL;
			pGunTextureR = gunTextureR;
			pGunTextureL = gunTextureL;
			pAliveTexture = aliveGUITexture;
			pDeadTexture = deadGUITexture;
			pLivesTextures = new Texture2D[3];
			pLivesTextures[0] = pAliveTexture;
			pLivesTextures[1] = pAliveTexture;
			pLivesTextures[2] = pAliveTexture;
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
			pGunCollisionPosition.X -= pGunPosition.Height / 2;
			pGunCollisionPosition.Y -= pGunPosition.Width / 2;
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
		}

		public void Update(KeyboardState kState, KeyboardState oldKState, GraphicsDevice graphics, GameTime gameTime)
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
					pVelocity.X -= 10;
					if (pTexture == pTextureR)
					{
						Turn(-(int)pPosition.Width / 2);
					}
					WalkAnimate(gameTime);

				}


				if (kState.IsKeyDown(right)) // Move right
				{
					pVelocity.X += 10;
					if (pTexture == pTextureL)
					{
						Turn((int)pPosition.Width / 2);
					}
					WalkAnimate(gameTime);
				}

				if(!kState.IsKeyDown(right) && !kState.IsKeyDown(left)) // if not moving, play still shot
				{
					spriteSheet.X = 214 * 0;
					spriteSheet.Y = 317 * 0;
				}

				if (kState.IsKeyDown(jump)) // Jump
				{
					if (onGround && !jumping)
					{
						pVelocity.Y -= 30;
						jumping = true;
						onGround = false;
					}
					else if (!onGround && jumping && pVelocity.Y < 0)
					{
						pVelocity.Y -= 1;
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
					pGunCollisionPosition.X -= pGunPosition.Width / 2;
					pGunCollisionPosition.Y -= pGunPosition.Height / 4;
					pGunCollisionPosition.Width = 3 * (pGunPosition.Width / 4);
					pGunCollisionPosition.Height = (3 * (pGunPosition.Height / 2));
				}
				else
				{
					pGunCollisionPosition.X -= pGunPosition.Width / 2;
					pGunCollisionPosition.Y -= pGunPosition.Height;
					pGunCollisionPosition.Width = 3 * (pGunPosition.Width / 4);
					pGunCollisionPosition.Height = (3 * (pGunPosition.Height / 2));
				}

				if (pVelocity.Y != 0)
					onGround = false;

				pVelocity.Y += pAcceleration;

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

				if (level == GunLevel.Mid)
				{
					spriteBatch.Draw(pGunTexture, pGunPosition, null, Color.White, 0, new Vector2(pGunTexture.Width / 2, pGunTexture.Height / 2), SpriteEffects.None, 0);
				}
				else if ((level == GunLevel.High && pTexture == pTextureL) || (level == GunLevel.Low && pTexture == pTextureR))
				{
					spriteBatch.Draw(pGunTexture, pGunPosition, null, Color.White, 0.785398f, new Vector2(pGunTexture.Width / 2, pGunTexture.Height / 2), SpriteEffects.None, 0);
				}
				else
				{
					spriteBatch.Draw(pGunTexture, pGunPosition, null, Color.White, -0.785398f, new Vector2(pGunTexture.Width / 2, pGunTexture.Height / 2), SpriteEffects.None, 0);
				}

				if (playerNum == 1)
				{
					for (int i = 0; i < 3; i++)
					{
						spriteBatch.Draw(pLivesTextures[i], new Rectangle((height / 50) + ((height / 15) * i), height / 50, height / 10, height / 10), Color.White);
					}
				}
				else
				{
					for (int i = 2; i > -1; i--)
					{
						spriteBatch.Draw(pLivesTextures[i], new Rectangle(width - ((height / 8) + ((height / 15) * i)), height / 50, height / 10, height / 10), Color.White);
					}
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
					pGunTexture = pGunTextureL;
				}
				else
				{
					pTexture = pTextureR;
					pGunTexture = pGunTextureR;
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
			}
		}

		public void Pickup(Bullet bullet)
		{
			if (!loaded && !dead)
			{
				pBullet = bullet;
				loaded = true;
				sfReload.Play();
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
			pGunCollisionPosition.X -= pGunPosition.Height / 2;
			pGunCollisionPosition.Y -= pGunPosition.Width / 2;
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
