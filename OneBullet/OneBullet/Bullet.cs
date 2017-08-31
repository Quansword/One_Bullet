using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
	class Bullet
	{
		Texture2D bTexture;
		public Rectangle bPosition;
		const int bSpeed = 15;
		const int bAcceleration = 2;
		int bFallVelocity;
		public bool bDirRight, bMoving, bIsLoaded, bHasRicocheted, bOnGround;

		public void Initialize(Texture2D texture, Rectangle position)
		{
			bTexture = texture;
			bPosition = position;
			bDirRight = true;
			bMoving = false;
			bIsLoaded = true;
			bHasRicocheted = false;
			bOnGround = false;
			bFallVelocity = 0;
		}

		public void Update(GraphicsDevice graphics, int bulletSize)
		{
			if (bMoving)
			{
				if (bDirRight)
				{
					bPosition.X += bSpeed;
				}
				else
				{
					bPosition.X -= bSpeed;
				}
			}
			else if (!bMoving && !bOnGround && !bIsLoaded)
			{
				if (bDirRight)
				{
					if (bFallVelocity < 6)
					{
						bPosition.X -= 10;
					}
					else
					{
						bPosition.X -= 7;
					}
				}
				else
				{
					if (bFallVelocity < 6)
					{
						bPosition.X += 10;
					}
					else
					{
						bPosition.X += 7;
					}
				}
				bFallVelocity += bAcceleration;
				bPosition.Y += bFallVelocity;
			}

			if (bPosition.X > graphics.Viewport.Width - bulletSize)
			{
				bPosition.X = graphics.Viewport.Width - bulletSize;
				Wall();
			}
			else if (bPosition.X < 0)
			{
				bPosition.X = 0;
				Wall();
			}

			if (bPosition.Y > graphics.Viewport.Height - bulletSize)
			{
				bPosition.Y = graphics.Viewport.Height - bulletSize;
				bOnGround = true;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (!bIsLoaded)
			{
				spriteBatch.Draw(bTexture, bPosition, Color.White);
			}
		}

		public void Fire(Texture2D texture, bool dirRight, Rectangle position)
		{
			bTexture = texture;
			bDirRight = dirRight;
			bPosition.X = position.X;
			bPosition.Y = position.Y + 20; // temporary height fix with the +20
			bIsLoaded = false;
			bMoving = true;
		}

		public void Hit()
		{
			// When the bullet hits a player
			// bMoving = false;
			// bIsLoaded = true;
		}

		public void Pickup()
		{
			bIsLoaded = true;
			bOnGround = false;
			bFallVelocity = 0;
		}

		public void Wall()
		{
			bMoving = false;
		}

		public void Catch()
		{
			// When bullet is caught
			// bMoving = false;
			// bIsLoaded = true;
		}

		public void Ricochet(Texture2D texture)
		{
			// When the bullet ricochets
			// if bHasRicocheted is false
			// change texture to opposite direction texture
		}
	}
}
