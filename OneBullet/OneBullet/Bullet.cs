using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
	class Bullet
	{
		Texture2D bTexture;
		public Rectangle bPosition;
		const int bSpeed = 12;
		const int bAcceleration = 2;
		int bFallVelocity;
		public bool bDirRight, bMoving, bIsLoaded, bHasRicocheted, bOnGround, bKill;

		public void Initialize(Texture2D texture, Rectangle position)
		{
			bTexture = texture;
			bPosition = position;
			bDirRight = true;
			bMoving = false;
			bIsLoaded = true;
			bHasRicocheted = false;
			bOnGround = false;
			bKill = false;
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
			else if (!bMoving && !bOnGround && !bIsLoaded && !bKill) // not moving, bullet is falling after hitting platform or wall
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
			else if (!bMoving && !bOnGround && !bIsLoaded && bKill) // if hit player
			{
				if (bDirRight)
				{
					if (bFallVelocity < 8)
						bPosition.X += bSpeed;
				}
				else
				{
					if (bFallVelocity < 8)
						bPosition.X -= bSpeed;
				}
				bFallVelocity += bAcceleration;
				bPosition.Y += bFallVelocity;
			}

			if (bPosition.X > graphics.Viewport.Width - (bulletSize / 2)) // hits edge of screen
			{
				bPosition.X = graphics.Viewport.Width - (bulletSize / 2);
				Wall();
			}
			else if (bPosition.X < 0)
			{
				bPosition.X = 0;
				Wall();
			}

			if (bPosition.Y > graphics.Viewport.Height - (bulletSize / 2)) // drops to floor
			{
				bPosition.Y = graphics.Viewport.Height - (bulletSize / 2);
				bOnGround = true;
				bKill = false;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (!bIsLoaded)
			{
				//public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
				spriteBatch.Draw(bTexture, bPosition, null, Color.White, 0, new Vector2(bTexture.Width / 2, bTexture.Height/2), SpriteEffects.None, 0);
			}
		}

		public void Fire(Texture2D texture, bool dirRight, Rectangle position)
		{
			bTexture = texture;
			bDirRight = dirRight;
			if (bDirRight)
			{
				bPosition.X = position.X + 100;
			}
			else
			{
				bPosition.X = position.X - 70;
			}
			bPosition.Y = position.Y;
			bIsLoaded = false;
			bMoving = true;
		}

		public void Hit()
		{
			bMoving = false;
			bKill = true;
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
			bHasRicocheted = false;
		}

		public void Catch()
		{
			bMoving = false;
			bIsLoaded = true;
		}

		public void Ricochet(Texture2D texture)
		{
			if (!bHasRicocheted)
			{
				bHasRicocheted = true;
				bDirRight = !bDirRight;
				// change texture to opposite direction texture
			}
			else
			{
				Wall();
			}
		}

		public void Dead(Texture2D texture, bool dirRight, Rectangle position) // almost same as fire()
		{
			bTexture = texture;
			bDirRight = dirRight;
			if (bDirRight)
			{
				bPosition.X = position.X + 100;

            }
            else
			{
				bPosition.X = position.X - 70;

            }
            bPosition.Y = position.Y - 10;
			bIsLoaded = false;
			bMoving = false;
		}
	}
}
