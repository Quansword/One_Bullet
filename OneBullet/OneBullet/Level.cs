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
		public Platforms floor, rightWall, leftWall;
		public Platforms[] lPlatforms;
		int platNum;

		public void Initialize(Texture2D bg, Rectangle bgPos, int pNum, Platforms[] lPlat, Platforms fPlat = null, Platforms rWall = null, Platforms lWall = null)
		{
			background = bg;
			backgroundPosition = bgPos;
			platNum = pNum;
			lPlatforms = new Platforms[platNum];
			lPlatforms = lPlat;
			floor = fPlat;
			rightWall = rWall;
			leftWall = lWall;
			Level.curLevel = this;
		}

		public bool PlatformCollision(Rectangle playerCollision)
		{
			if (floor != null)
			{
				if (floor.platPosition.Intersects(playerCollision))
					return false;
			}
			if (rightWall != null)
			{
				if (rightWall.platPosition.Intersects(playerCollision))
					return false;
			}
			if (leftWall != null)
			{
				if (rightWall.platPosition.Intersects(playerCollision))
					return false;
			}
			foreach (var plat in lPlatforms)
			{
				if (plat.platPosition.Intersects(playerCollision))
					return false;
			}
			return true;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(background, backgroundPosition, Color.White);
			if (floor != null)
			{
				floor.Draw(spriteBatch);
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
