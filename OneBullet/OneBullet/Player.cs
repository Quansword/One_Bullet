using Microsoft.Xna.Framework;
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
		public bool onGround, jumping, loaded;

		public Bullet pBullet;

		public enum GunLevel
		{
			High,
			Mid,
			Low
		};

		public GunLevel level = GunLevel.Mid;

		public void Initialize(Texture2D texture, Texture2D gunTexture, Rectangle position, int gunOffset, Bullet bullet)
		{
			pTexture = texture;
			pGunTexture = gunTexture;
			pPosition = position;
			pGunOffset = gunOffset;
			pGunPosition = pPosition;
			pGunPosition.X = pPosition.X + gunOffset;
			pBullet = bullet;
			pVelocity = new Vector2(0, 0);
			onGround = true;
			jumping = false;
			loaded = true;
			pLevelOffset = 0;
		}

		public void Update(KeyboardState kState, KeyboardState oldKState, GraphicsDevice graphics, double charSize)
		{
			if (kState.IsKeyDown(Keys.G)) // Jump
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
			if (kState.IsKeyUp(Keys.G) && oldKState.IsKeyDown(Keys.G))
			{
				jumping = false;
			}
			if (kState.IsKeyDown(Keys.W) && oldKState.IsKeyUp(Keys.W)) // Change gun level
			{
				if (level == Player.GunLevel.Low)
				{
					level = Player.GunLevel.Mid;
					pLevelOffset = 0;
				}
				else if (level == Player.GunLevel.Mid)
				{
					level = Player.GunLevel.High;
					pLevelOffset = -((int)charSize / 4);
				}
			}
			if (kState.IsKeyDown(Keys.S) && oldKState.IsKeyUp(Keys.S)) // Change gun level
			{
				if (level == Player.GunLevel.High)
				{
					level = Player.GunLevel.Mid;
					pLevelOffset = 0;
				}
				else if (level == Player.GunLevel.Mid)
				{
					level = Player.GunLevel.Low;
					pLevelOffset = (int)charSize / 4;
				}
			}

			// ------------------------------------------ Falling parameters
			if (pPosition.Y < graphics.Viewport.Height - charSize)
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
			if (pPosition.X > graphics.Viewport.Width - (int)charSize)
			{
				pPosition.X = graphics.Viewport.Width - (int)charSize;
			}
			else if (pPosition.X < 0)
			{
				pPosition.X = 0;
			}
			pPosition.Y += (int)pVelocity.Y;
			if (pPosition.Y > graphics.Viewport.Height - (int)charSize)
				pPosition.Y = graphics.Viewport.Height - (int)charSize;

			pGunPosition.X += (int)pVelocity.X;
			pGunPosition.Y = pPosition.Y + pLevelOffset;

			// ------------------------------------------ Resetting values
			pVelocity.X = 0;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(pTexture, pPosition, Color.White);
			spriteBatch.Draw(pGunTexture, pGunPosition, Color.White);
		}

		public void Turn(Texture2D texture, Texture2D gunTexture, int gunOffset)
		{
			pTexture = texture;
			pGunTexture = gunTexture;
			pGunOffset = gunOffset;
		}

		public void Hit()
		{
			// When a bullet hits the player
		}
	}
}
