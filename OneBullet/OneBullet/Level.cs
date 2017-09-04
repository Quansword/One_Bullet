using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
	class Level
	{
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
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(background, backgroundPosition, Color.White);
			if (floor != null)
			{
				spriteBatch.Draw(floor.platTexture, floor.platPosition, Color.White);
			}
			if (rightWall != null)
			{
				spriteBatch.Draw(rightWall.platTexture, rightWall.platPosition, Color.White);
			}
			if (leftWall != null)
			{
				spriteBatch.Draw(leftWall.platTexture, leftWall.platPosition, Color.White);
			}
			foreach (var plat in lPlatforms)
			{
				plat.Draw(spriteBatch);
			}
		}
	}
}
