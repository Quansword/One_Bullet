using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
	class Bullet
	{
		Texture2D bTexture;
		public Rectangle bPosition;
		public const int bSpeed = 15;
		public bool bDirRight, bMoving, bIsLoaded, bHasRicocheted;

		public void Initialize(Texture2D texture, Rectangle position)
		{
			bTexture = texture;
			bPosition = position;
			bDirRight = true;
			bMoving = false;
			bIsLoaded = true;
			bHasRicocheted = false;
		}

		public void Update()
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

		public void Wall()
		{
			// When the bullet hits a wall
			// bMoving = false;
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
