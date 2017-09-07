using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace OneBullet
{
	class Player
	{
		public Texture2D pTexture, pTextureR, pTextureL;
		public Texture2D pGunTexture, pGunTextureR, pGunTextureL;
		public Rectangle pPosition, pCollisionPosition;
		Rectangle newPosition;
        Rectangle sourceRect;
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
        float elapsed;
        float delay = 60f;
        int Hframes = 0;
        int Vframes = 0;


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

		public void Initialize(Texture2D textureR, Texture2D textureL, Texture2D gunTextureR, Texture2D gunTextureL, Rectangle position, Rectangle gunPosition, int gunOffset, Bullet bullet, SoundEffect reloadSound, SoundEffect deadSound, int pNum)
		{
			playerNum = pNum;
			pTextureR = textureR;
			pTextureL = textureL;
			pGunTextureR = gunTextureR;
			pGunTextureL = gunTextureL;
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
					pGunCollisionPosition = pGunPosition;
					pGunCollisionPosition.X -= pGunPosition.Height / 2;
					pGunCollisionPosition.Y -= pGunPosition.Width / 2;
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

                }


				if (kState.IsKeyDown(right)) // Move right
				{
					pVelocity.X += 10;
					if (pTexture == pTextureL)
					{
						Turn((int)pPosition.Width / 2);
					}

                    
				}

                if(kState.IsKeyDown(right) || kState.IsKeyDown(left)) // If moving play walk animation
                {
                    elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (elapsed >= delay)
                    {
                        if (Hframes >= 2)
                        {
                            Hframes = 0;
                            Vframes++;
                            if (Vframes >= 3)
                            {
                                Vframes = 0;
                            }
                        }
                        else
                        {
                            Hframes++;
                        }
                        elapsed = 0;
                    }
                    sourceRect = new Rectangle(550 * Hframes, 400 * Vframes, 550, 400);

                }


                if(!kState.IsKeyDown(right) && !kState.IsKeyDown(left)) // if not moving, play still shot
                {
                    sourceRect = new Rectangle(550 * 2, 400 *1, 550, 400);
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
				pGunPosition.Y = ((pPosition.Y - (pPosition.Height / 4)) + pLevelOffset);

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

		public void Draw(SpriteBatch spriteBatch)
		{
			if (!dead)
			{
				//public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
				spriteBatch.Draw(pTexture, pPosition, sourceRect, Color.White, 0, new Vector2(pTexture.Width /4 - 120, pTexture.Height/4 - 260), SpriteEffects.None, 0);

				if (level == GunLevel.Mid)
				{
					spriteBatch.Draw(pGunTexture, pGunPosition, null, Color.White, 0, new Vector2(pGunTexture.Width / 2, pGunTexture.Height / 2), SpriteEffects.None, 0);
				}
				else if ((level == GunLevel.High && pTexture == pTextureL) || (level == GunLevel.Low && pTexture == pTextureR))
				{
					spriteBatch.Draw(pGunTexture, pGunPosition, null, Color.White, 45, new Vector2(pGunTexture.Width / 2, pGunTexture.Height / 2), SpriteEffects.None, 0);
				}
				else
				{
					spriteBatch.Draw(pGunTexture, pGunPosition, null, Color.White, -45, new Vector2(pGunTexture.Width / 2, pGunTexture.Height / 2), SpriteEffects.None, 0);
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
			pVelocity.X = 0;
			pVelocity.Y = 0;
			onGround = true;
			jumping = false;
			loaded = true;
			dead = false;
			pLevelOffset = 0;
			collisionPlatform = -1;
		}
	}
}
