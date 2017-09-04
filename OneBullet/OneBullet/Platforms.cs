using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
	class Platforms
	{
		public Texture2D platTexture;
		public Rectangle platPosition;

		public void Initialize(Texture2D texture, Rectangle position)
		{
			platTexture = texture;
			platPosition = position;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(platTexture, platPosition, Color.White);
		}
	}
}
