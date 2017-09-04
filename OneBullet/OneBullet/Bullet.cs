using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Media;

namespace OneBullet
{
	class Bullet
	{
        Texture2D bTexture;
		public Rectangle bPosition;
		const int bSpeed = 15;
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
            var player1 = new System.Media.SoundPlayer(@"C:\Users\u0460939\Documents\GitHub\One_Bullet\OneBullet\OneBullet\Shells_falls-Marcel-829263474.wav");
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

			if (bPosition.X > graphics.Viewport.Width - bulletSize) // hits edge of screen
			{
				bPosition.X = graphics.Viewport.Width - bulletSize;
				Wall();
			}
			else if (bPosition.X < 0)
			{
				bPosition.X = 0;
				Wall();
			}

			if (bPosition.Y > graphics.Viewport.Height - bulletSize) // drops to floor
			{
				bPosition.Y = graphics.Viewport.Height - bulletSize;
				bOnGround = true;
                player1.Play();
				bKill = false;
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
            var player3 = new System.Media.SoundPlayer(@"C:\Users\u0460939\Documents\GitHub\One_Bullet\OneBullet\OneBullet\OneBullet_GunShot.wav");
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
			bMoving = true;
            player3.Play();
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
		}

		public void Catch()
		{
			bMoving = false;
			bIsLoaded = true;
		}

		public void Ricochet(Texture2D texture)
		{
			// When the bullet ricochets
			// if bHasRicocheted is false
			// change texture to opposite direction texture
		}

		public void Dead(Texture2D texture, bool dirRight, Rectangle position) // almost same as fire()
		{
            var player4 = new System.Media.SoundPlayer(@"C:\Users\u0460939\Documents\GitHub\One_Bullet\OneBullet\OneBullet\Pain-SoundBible.com-1883168362.wav");
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
            player4.Play();
        }
	}
}
