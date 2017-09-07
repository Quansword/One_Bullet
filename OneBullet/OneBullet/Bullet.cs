using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace OneBullet
{
	class Bullet
	{
		Texture2D bTexture;
		public Rectangle bPosition;
		Rectangle newPosition;
		Vector2 bVelocity, bShootDirection;
		const int bSpeed = 16;
		const int bAcceleration = 2;
		public bool bMoving, bIsLoaded, bOnGround, bKill, bDead;

		SoundEffect sfShellFall;
		SoundEffect sfFire;

		int collisionPlatform;
		Level.CollisionDir collisionDir = Level.CollisionDir.None;

		public void Initialize(Texture2D texture, Rectangle position, SoundEffect fallSound, SoundEffect fireSound)
		{
			bTexture = texture;
			bPosition = position;
			newPosition = bPosition;
			sfShellFall = fallSound;
			sfFire = fireSound;
			bMoving = false;
			bIsLoaded = true;
			bOnGround = false;
			bKill = false;
			bDead = false;
			bVelocity = new Vector2(0, 0);
			bShootDirection = new Vector2(0, 0);
			collisionPlatform = -1;
		}

		public void Update(GraphicsDevice graphics, GameTime gameTime)
		{
			if (bMoving)
			{
				if (bShootDirection.X == 1)
				{
					if (bShootDirection.Y == 0)
					{
						bVelocity.X = bSpeed;
					}
					else if (bShootDirection.Y == 1)
					{
						bVelocity.X = 11;
						bVelocity.Y = -11;
					}
					else
					{
						bVelocity.X = 11;
						bVelocity.Y = 11;
					}
				}
				else
				{
					if (bShootDirection.Y == 0)
					{
						bVelocity.X = -bSpeed;
					}
					else if (bShootDirection.Y == 1)
					{
						bVelocity.X = -11;
						bVelocity.Y = -11;
					}
					else
					{
						bVelocity.X = -11;
						bVelocity.Y = 11;
					}
				}
			}
			else if (!bMoving && !bOnGround && !bIsLoaded) // not moving, bullet is falling after hitting platform or wall
			{
				if (!bKill)
				{
					if (!bDead)
					{
						if (bShootDirection.X == 1)
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
					if (bShootDirection.X == 1)
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
						if (!bMoving)
						{
							bVelocity.Y = 0;
							bVelocity.X = 0;
							bOnGround = true;
							sfShellFall.Play();
						}
						else
						{
							bShootDirection.Y = -bShootDirection.Y;
						}
					}
					else
					{
						bPosition.X += (int)bVelocity.X;
						bShootDirection.X = -bShootDirection.X;
					}
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (!bIsLoaded)
			{
				//public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
				spriteBatch.Draw(bTexture, bPosition, null, Color.White); //, 0, new Vector2(bTexture.Width / 2, bTexture.Height/2), SpriteEffects.None, 0);
			}
		}

		public void Fire(Vector2 dir, Rectangle position)
		{
			bShootDirection = dir;
			if (bShootDirection.X == 1)
			{
				bVelocity.X = bSpeed;
				if (bShootDirection.Y == 0)
				{
					bPosition.X = position.X + (position.Width / 4);
					bPosition.Y = position.Y;
				}
				else if (bShootDirection.Y == 1)
				{
					bPosition.X = (int)(position.X + Math.Sqrt(position.Width / 4));
					bPosition.Y = (int)(position.Y - Math.Sqrt(position.Width / 4));
				}
				else
				{
					bPosition.X = (int)(position.X + Math.Sqrt(position.Width / 4));
					bPosition.Y = (int)(position.Y + Math.Sqrt(position.Width / 4));
				}
			}
			else
			{
				bVelocity.X = -bSpeed;
				if (bShootDirection.Y == 0)
				{
					bPosition.X = (position.X - bPosition.Width) - (position.Width / 4);
					bPosition.Y = position.Y;
				}
				else if (bShootDirection.Y == 1)
				{
					bPosition.X = (int)((position.X - bPosition.Width) - Math.Sqrt(position.Width / 4));
					bPosition.Y = (int)(position.Y - Math.Sqrt(position.Width / 4));
				}
				else
				{
					bPosition.X = (int)((position.X - bPosition.Width) - Math.Sqrt(position.Width / 4));
					bPosition.Y = (int)(position.Y + Math.Sqrt(position.Width / 4));
				}
			}
			bIsLoaded = false;
			bMoving = true;
			sfFire.Play();
		}

		public void Hit()
		{
			bMoving = false;
			bKill = true;
			bOnGround = false;
			bIsLoaded = false;
			bVelocity.X = 0;
			bVelocity.Y = 0;
		}

		public void Pickup()
		{
			bIsLoaded = true;
			bOnGround = false;
			bDead = false;
			bKill = false;
			bVelocity.Y = 0;
			bVelocity.X = 0;
		}

		public void Wall()
		{
			bMoving = false;
		}

		public void Catch()
		{
			bMoving = false;
			bIsLoaded = true;
			bOnGround = false;
		}

		public void Dead(Vector2 dir, Rectangle position) // almost same as fire()
		{
			bShootDirection = dir;
			if (bShootDirection.X == 1)
			{
				bPosition.X = position.X;
			}
			else
			{
				bPosition.X = position.X;
			}
			bPosition.Y = position.Y;
			bIsLoaded = false;
			bMoving = false;
			bDead = true;
		}

		public void Respawn()
		{
			bPosition.X = -100;
			bPosition.Y = -100;
			newPosition = bPosition;
			bMoving = false;
			bIsLoaded = true;
			bOnGround = false;
			bKill = false;
			bDead = false;
			bVelocity.X = 0;
			bVelocity.Y = 0;
			bShootDirection.X = 0;
			bShootDirection.Y = 0;
			collisionPlatform = -1;
		}
	}
}
