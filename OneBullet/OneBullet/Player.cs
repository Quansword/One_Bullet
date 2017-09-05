using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
	class Player
	{
		public Texture2D pTexture, pTextureR, pTextureL;
		public Texture2D pGunTexture, pGunTextureR, pGunTextureL;
		public Rectangle pPosition, newPosition;
		public Rectangle pCollisionPosition;
		public Rectangle pGunPosition;
		Vector2 pVelocity;
		int pLevelOffset;
		public int pGunOffset;
		const int pAcceleration = 3;
		public bool onGround, jumping, loaded, dead;
		Keys jump, lowerGun, raiseGun, shoot, left, right;
		int playerNum;

		public Bullet pBullet;

		public enum GunLevel
		{
			High,
			Mid,
			Low
		};

		public GunLevel level = GunLevel.Mid;

		public void Initialize(Texture2D textureR, Texture2D textureL, Texture2D gunTextureR, Texture2D gunTextureL, Rectangle position, Rectangle gunPosition, int gunOffset, Bullet bullet, int pNum)
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
			pBullet = bullet;
			pVelocity = new Vector2(0, 0);
			onGround = true;
			jumping = false;
			loaded = true;
			dead = false;
			pLevelOffset = 0;
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
					if (pTexture == pTextureR)
					{
						pBullet.Fire(true, pGunPosition);
					}
					else
					{
						pBullet.Fire(false, pGunPosition);
					}
					Fire();
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
				if (kState.IsKeyDown(jump)) // Jump
				{
					if (onGround && !jumping)
					{
						pVelocity.Y -= 30;
						jumping = true;
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
				newPosition = pCollisionPosition;
				newPosition.X += (int)(pVelocity.X * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 16);
				newPosition.Y += (int)(pVelocity.Y * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 16);
				if (Level.curLevel.PlatformCollision(newPosition))
				{
					pPosition.X += (int)(pVelocity.X * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 16);
					if (pPosition.X > graphics.Viewport.Width - (int)(pPosition.Width / 2))
					{
						pPosition.X = graphics.Viewport.Width - (int)(pPosition.Width / 2);
					}
					else if (pPosition.X < (pPosition.Width / 2))
					{
						pPosition.X = (int)(pPosition.Width / 2);
					}
					pPosition.Y += (int)(pVelocity.Y * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 16);
					if (pPosition.Y > graphics.Viewport.Height - (int)(pPosition.Height / 2))
						pPosition.Y = graphics.Viewport.Height - (int)(pPosition.Height / 2);

					pCollisionPosition = pPosition;
					pCollisionPosition.X -= (int)(pCollisionPosition.Width / 2);
					pCollisionPosition.Y -= (int)(pCollisionPosition.Height / 2);
				}

				pGunPosition.X = pPosition.X + pGunOffset;
				pGunPosition.Y = (pPosition.Y + pLevelOffset);

				// ------------------------------------------ Falling parameters
				if (pCollisionPosition.Y < graphics.Viewport.Height - pCollisionPosition.Height)
				{
					onGround = false;
					pVelocity.Y += pAcceleration;
				}
				else if (!onGround)
				{
					onGround = true;
					pVelocity.Y = 0;
				}

				// ------------------------------------------ Resetting values
				pVelocity.X = 0;
			}
			// ------------------------------------------ Death handle
			else if (dead && loaded)
			{
				if (pTexture == pTextureR)
				{
					pBullet.Dead(true, pGunPosition);
				}
				else
				{
					pBullet.Dead(false, pGunPosition);
				}
				Fire();
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (!dead)
			{
				//public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
				spriteBatch.Draw(pTexture, pPosition, null, Color.White, 0, new Vector2(pTexture.Width / 2, pTexture.Height / 2), SpriteEffects.None, 0);
				spriteBatch.Draw(pGunTexture, pGunPosition, null, Color.White, 0, new Vector2(pGunTexture.Width / 2, pGunTexture.Height / 2), SpriteEffects.None, 0);
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
				}
			}
		}

		public void Hit()
		{
			dead = true;
		}
	}
}
