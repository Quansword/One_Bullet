using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
	class Bullet
	{
		Texture2D bTexture;
		public Rectangle bPosition;
		Rectangle newPosition;
		Vector2 bVelocity;
		const int bSpeed = 12;
		const int bAcceleration = 2;
		public bool bDirRight, bMoving, bIsLoaded, bHasRicocheted, bOnGround, bKill, bDead;

		int collisionPlatform;
		Level.CollisionDir collisionDir = Level.CollisionDir.None;

		public void Initialize(Texture2D texture, Rectangle position)
		{
			bTexture = texture;
			bPosition = position;
			newPosition = bPosition;
			bDirRight = true;
			bMoving = false;
			bIsLoaded = true;
			bHasRicocheted = false;
			bOnGround = false;
			bKill = false;
			bDead = false;
			bVelocity = new Vector2(0, 0);
			collisionPlatform = -1;
		}

		public void Update(GraphicsDevice graphics, int bulletSize, GameTime gameTime)
		{
			if (bMoving)
			{
				if (bDirRight)
				{
					bVelocity.X = bSpeed;
				}
				else
				{
					bVelocity.X = -bSpeed;
				}
			}
			else if (!bMoving && !bOnGround && !bIsLoaded) // not moving, bullet is falling after hitting platform or wall
			{
				if (!bKill)
				{
					if (!bDead)
					{
						if (bDirRight)
						{
							if (bVelocity.Y < 6)
							{
								bVelocity.X = -10;
							}
							else
							{
								bVelocity.X = -7;
							}
						}
						else
						{
							if (bVelocity.Y < 6)
							{
								bVelocity.X = 10;
							}
							else
							{
								bVelocity.X = 7;
							}
						}
					}
				}
				else// if hit player
				{
					if (bDirRight)
					{
						if (bVelocity.Y < 8)
							bVelocity.X = bSpeed;
					}
					else
					{
						if (bVelocity.Y < 8)
							bVelocity.X = -bSpeed;
					}
				}
				bVelocity.Y += bAcceleration;
			}

			CalcVelocity(gameTime);
			CalcVelocity(gameTime);

			bPosition.X += (int)(bVelocity.X * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 16);
			bPosition.Y += (int)(bVelocity.Y * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 16);
		}

		void CalcVelocity(GameTime gameTime)
		{
			collisionPlatform = -1;
			newPosition = bPosition;
			newPosition.X += (int)(bVelocity.X * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 16);
			newPosition.Y += (int)(bVelocity.Y * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 16);

			collisionPlatform = Level.curLevel.PlatformCollision(newPosition);
			if (collisionPlatform == -1)
			{
				collisionDir = Level.CollisionDir.None;
			}
			else
			{
				collisionDir = Level.curLevel.PlatformDirection(collisionPlatform, bPosition);
				if (collisionDir != Level.CollisionDir.None)
				{
					bVelocity = Level.curLevel.NewVelocity(collisionPlatform, collisionDir, bPosition, bVelocity);
					if (collisionDir == Level.CollisionDir.Bottom || collisionDir == Level.CollisionDir.Top)
					{
						bPosition.Y += (int)bVelocity.Y;
						bVelocity.Y = 0;
						bVelocity.X = 0;
						bOnGround = true;
						bKill = false;
					}
					else
					{
						bPosition.X += (int)bVelocity.X;
						bVelocity.X = 0;
						Wall();
					}
				}
			}

			// ------------------------------------------ Falling parameters
			if (collisionDir == Level.CollisionDir.Bottom)
			{
				bOnGround = true;
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

		public void Fire(bool dirRight, Rectangle position)
		{
			bDirRight = dirRight;
			if (bDirRight)
			{
				bPosition.X = position.X + (position.Width / 4);
				//bTexture = bTextureR;
			}
			else
			{
				bPosition.X = position.X - (position.Width / 4);
				//bTexture = bTextureL;
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
			bDead = false;
			bVelocity.Y = 0;
			bVelocity.X = 0;
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

		public void Dead(bool dirRight, Rectangle position) // almost same as fire()
		{
			bDirRight = dirRight;
			if (bDirRight)
			{
				bPosition.X = position.X;
				//bTexture = bTextureR;

            }
            else
			{
				bPosition.X = position.X;
				//bTexture = bTextureL;
			}
			bPosition.Y = position.Y;
			bIsLoaded = false;
			bMoving = false;
			bDead = true;
		}
	}
}
