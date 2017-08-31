﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
	class Player
	{
		public Texture2D pTexture;
		public Texture2D pGunTexture;
		public Rectangle pPosition;
		public Rectangle pGunPosition;
		public Vector2 pVelocity;
		public int pLevelOffset;
		public int pGunOffset;
		const int pAcceleration = 3;
		public bool onGround, jumping, loaded, dead;
		Keys jump, lowerGun, raiseGun;
		int playerNum;

		public Bullet pBullet;

		public enum GunLevel
		{
			High,
			Mid,
			Low
		};

		public GunLevel level = GunLevel.Mid;

		public void Initialize(Texture2D texture, Texture2D gunTexture, Rectangle position, Rectangle gunPosition, int gunOffset, Bullet bullet, int pNum)
		{
			pTexture = texture;
			pGunTexture = gunTexture;
			pPosition = position;
			pGunOffset = gunOffset;
			pGunPosition = gunPosition;
			pGunPosition.X += pGunOffset;
			pBullet = bullet;
			playerNum = pNum;
			pVelocity = new Vector2(0, 0);
			onGround = true;
			jumping = false;
			loaded = true;
			dead = false;
			pLevelOffset = 0;
		}

		public void Update(KeyboardState kState, KeyboardState oldKState, GraphicsDevice graphics, double charWidth, double charHeight)
		{
			if (playerNum == 1)
			{
				jump = Keys.G;
				lowerGun = Keys.S;
				raiseGun = Keys.W;
			}
			else if (playerNum == 2)
			{
				jump = Keys.NumPad2;
				lowerGun = Keys.Down;
				raiseGun = Keys.Up;
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
					pLevelOffset = -((int)charHeight / 4);
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
					pLevelOffset = (int)charHeight / 4;
				}
			}

			// ------------------------------------------ Falling parameters
			if (pPosition.Y < graphics.Viewport.Height - charHeight)
			{
				onGround = false;
				pVelocity.Y += pAcceleration;
			}
			else if (!onGround)
			{
				onGround = true;
				pVelocity.Y = 0;
			}

			// ------------------------------------------ Calculating velocity
			pGunPosition.X = pPosition.X + pGunOffset;

			pPosition.X += (int)pVelocity.X;
			if (pPosition.X > graphics.Viewport.Width - (int)charWidth)
			{
				pPosition.X = graphics.Viewport.Width - (int)charWidth;
			}
			else if (pPosition.X < 0)
			{
				pPosition.X = 0;
			}
			pPosition.Y += (int)pVelocity.Y;
			if (pPosition.Y > graphics.Viewport.Height - (int)charHeight)
				pPosition.Y = graphics.Viewport.Height - (int)charHeight;

			pGunPosition.X += (int)pVelocity.X;
			pGunPosition.Y = (pPosition.Y + (int)(charHeight / 3) + pLevelOffset);

			// ------------------------------------------ Resetting values
			pVelocity.X = 0;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (!dead)
			{
				spriteBatch.Draw(pTexture, pPosition, Color.White);
				spriteBatch.Draw(pGunTexture, pGunPosition, Color.White);
			}
		}

		public void Turn(Texture2D texture, Texture2D gunTexture, int gunOffset)
		{
			pTexture = texture;
			pGunTexture = gunTexture;
			pGunOffset = gunOffset;
		}

		public void Fire()
		{
			if (pBullet != null && !dead)
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
			if (loaded && !dead)
			{
				Hit();
			}
			else
			{
				pBullet = bullet;
				loaded = true;
			}
		}

		public void Hit()
		{
			dead = true;
		}
	}
}