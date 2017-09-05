using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
	class Level
	{
		public static Level curLevel;
		public Texture2D background;
		public Rectangle backgroundPosition;
		public Platforms floor, ceiling, rightWall, leftWall;
		public Platforms[] lPlatforms;
		public int platNum;

		public enum CollisionDir
		{
			Top,
			Bottom,
			Left,
			Right,
			None
		};

		public void Initialize(Texture2D bg, Rectangle bgPos, int pNum, Platforms[] lPlat, Platforms fPlat = null, Platforms cPlat = null, Platforms rWall = null, Platforms lWall = null)
		{
			background = bg;
			backgroundPosition = bgPos;
			platNum = pNum;
			lPlatforms = new Platforms[platNum];
			lPlatforms = lPlat;
			floor = fPlat;
			ceiling = cPlat;
			rightWall = rWall;
			leftWall = lWall;
			Level.curLevel = this;
		}

		public int PlatformCollision(Rectangle playerCollision)
		{
			if (floor != null)
			{
				if (floor.platPosition.Intersects(playerCollision))
				{
					return 0;
				}
                else if (floor.platPosition.Contains(playerCollision))
                {
                    return 0;
                }
			}
			if (rightWall != null)
			{
				if (rightWall.platPosition.Intersects(playerCollision))
                {
                    return 1;
                }
                else if (rightWall.platPosition.Contains(playerCollision))
                {
                    return 1;
                }
            }
			if (leftWall != null)
			{
				if (leftWall.platPosition.Intersects(playerCollision))
				{
					return 2;
				}
				else if (leftWall.platPosition.Contains(playerCollision))
				{
					return 2;
				}
			}
			if (ceiling != null)
			{
				if (ceiling.platPosition.Intersects(playerCollision))
				{
					return 3;
				}
				else if (ceiling.platPosition.Contains(playerCollision))
				{
					return 3;
				}
			}
			for (int i = 0; i < platNum; i++)
			{
				if (lPlatforms[i].platPosition.Intersects(playerCollision))
				{
					return i + 4;
				}
				else if (lPlatforms[i].platPosition.Contains(playerCollision))
				{
					return i + 4;
				}
			}
			return -1;
		}

		public CollisionDir PlatformDirection(int platformIndex, Rectangle playerPos)
		{
			int distDown = -1;
			int distUp = -1;
			int distRight = -1;
			int distLeft = -1;
			int leastDist = 0;
			CollisionDir dir = CollisionDir.None;

			if (platformIndex == 0)
			{
				distDown = floor.platPosition.Top - playerPos.Bottom;
				distUp = playerPos.Top - floor.platPosition.Bottom;
				distRight = floor.platPosition.Left - playerPos.Right;
				distLeft = playerPos.Left - floor.platPosition.Right;
			}
			else if (platformIndex == 1)
			{
				distDown = rightWall.platPosition.Top - playerPos.Bottom;
				distUp = playerPos.Top - rightWall.platPosition.Bottom;
				distRight = rightWall.platPosition.Left - playerPos.Right;
				distLeft = playerPos.Left - rightWall.platPosition.Right;
			}
			else if (platformIndex == 2)
			{
				distDown = leftWall.platPosition.Top - playerPos.Bottom;
				distUp = playerPos.Top - leftWall.platPosition.Bottom;
				distRight = leftWall.platPosition.Left - playerPos.Right;
				distLeft = playerPos.Left - leftWall.platPosition.Right;
			}
			else if (platformIndex == 3)
			{
				distDown = ceiling.platPosition.Top - playerPos.Bottom;
				distUp = playerPos.Top - ceiling.platPosition.Bottom;
				distRight = ceiling.platPosition.Left - playerPos.Right;
				distLeft = playerPos.Left - ceiling.platPosition.Right;
			}
			else
			{
				distDown = lPlatforms[platformIndex - 4].platPosition.Top - playerPos.Bottom;
				distUp = playerPos.Top - lPlatforms[platformIndex - 4].platPosition.Bottom;
				distRight = lPlatforms[platformIndex - 4].platPosition.Left - playerPos.Right;
				distLeft = playerPos.Left - lPlatforms[platformIndex - 4].platPosition.Right;
			}

			if (distDown > 0)
			{
				leastDist = distDown;
				dir = CollisionDir.Bottom;

				if (distRight > 0)
				{
					if (distRight < leastDist)
					{
						dir = CollisionDir.Right;
					}
				}
				else if (distLeft > 0)
				{
					if (distLeft < leastDist)
					{
						dir = CollisionDir.Left;
					}
				}
			}
			else if (distUp > 0)
			{
				leastDist = distUp;
				dir = CollisionDir.Top;

				if (distRight > 0)
				{
					if (distRight < leastDist)
					{
						dir = CollisionDir.Right;
					}
				}
				else if (distLeft > 0)
				{
					if (distLeft < leastDist)
					{
						dir = CollisionDir.Left;
					}
				}
			}
			else if (distRight > 0)
			{
				dir = CollisionDir.Right;
			}
			else if (distLeft > 0)
			{
				dir = CollisionDir.Left;
			}

			return dir;
		}

		public Vector2 NewVelocity(int platformIndex, Level.CollisionDir dir, Rectangle playerPos, Vector2 velocity)
		{
			Vector2 newVelocity = velocity;
			if (dir == CollisionDir.Bottom)
			{
				if (platformIndex == 0)
				{
					newVelocity.Y = floor.platPosition.Top - playerPos.Bottom - 1;
				}
				else if (platformIndex == 1)
				{
					newVelocity.Y = rightWall.platPosition.Top - playerPos.Bottom - 1;
				}
				else if (platformIndex == 2)
				{
					newVelocity.Y = leftWall.platPosition.Top - playerPos.Bottom - 1;
				}
				else if (platformIndex == 3)
				{
					newVelocity.Y = ceiling.platPosition.Top - playerPos.Bottom - 1;
				}
				else
				{
					newVelocity.Y = lPlatforms[platformIndex - 4].platPosition.Top - playerPos.Bottom - 1;
				}
			}
			else if (dir == CollisionDir.Top)
			{
				if (platformIndex == 0)
				{
					newVelocity.Y = floor.platPosition.Bottom - playerPos.Top + 1;
				}
				else if (platformIndex == 1)
				{
					newVelocity.Y = rightWall.platPosition.Bottom - playerPos.Top + 1;
				}
				else if (platformIndex == 2)
				{
					newVelocity.Y = leftWall.platPosition.Bottom - playerPos.Top + 1;
				}
				else if (platformIndex == 3)
				{
					newVelocity.Y = ceiling.platPosition.Bottom - playerPos.Top + 1;
				}
				else
				{
					newVelocity.Y = lPlatforms[platformIndex - 4].platPosition.Bottom - playerPos.Top + 1;
				}
			}
			else if (dir == CollisionDir.Right)
			{
				if (platformIndex == 0)
				{
					newVelocity.X = floor.platPosition.Left - playerPos.Right - 1;
				}
				else if (platformIndex == 1)
				{
					newVelocity.X = rightWall.platPosition.Left - playerPos.Right - 1;
				}
				else if (platformIndex == 2)
				{
					newVelocity.X = leftWall.platPosition.Left - playerPos.Right - 1;
				}
				else if (platformIndex == 3)
				{
					newVelocity.X = ceiling.platPosition.Left - playerPos.Right - 1;
				}
				else
				{
					newVelocity.X = lPlatforms[platformIndex - 4].platPosition.Left - playerPos.Right - 1;
				}
			}
			else if (dir == CollisionDir.Left)
			{
				if (platformIndex == 0)
				{
					newVelocity.X = floor.platPosition.Right - playerPos.Left + 1;
				}
				else if (platformIndex == 1)
				{
					newVelocity.X = rightWall.platPosition.Right - playerPos.Left + 1;
				}
				else if (platformIndex == 2)
				{
					newVelocity.X = leftWall.platPosition.Right - playerPos.Left + 1;
				}
				else if (platformIndex == 3)
				{
					newVelocity.X = ceiling.platPosition.Right - playerPos.Left + 1;
				}
				else
				{
					newVelocity.X = lPlatforms[platformIndex - 4].platPosition.Right - playerPos.Left + 1;
				}
			}
			return newVelocity;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(background, backgroundPosition, Color.White);
			if (floor != null)
			{
				floor.Draw(spriteBatch);
			}
			if (ceiling != null)
			{
				ceiling.Draw(spriteBatch);
			}
			if (rightWall != null)
			{
				rightWall.Draw(spriteBatch);
			}
			if (leftWall != null)
			{
				leftWall.Draw(spriteBatch);
			}
			foreach (var plat in lPlatforms)
			{
				plat.Draw(spriteBatch);
			}
		}
	}
}
