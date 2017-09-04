using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Platform
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle rectangle;



        public Platform(Texture2D stageTexture, Vector2 newPosition)
        {
            texture = stageTexture;
            position = newPosition;
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }



        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Add your drawing code here
            spriteBatch.Draw(texture, rectangle, Color.White);


        }


    }
}