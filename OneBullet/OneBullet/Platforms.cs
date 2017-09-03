using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Platforms
    {
        Texture2D texture;
        Vector2 position;
        public Rectangle rectangle;



        public Platforms(Texture2D stageTexture, Vector2 newPosition)
        {
            texture = stageTexture;
            position = newPosition;
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

       /*  public void Update(GameTime gameTime)
        {
            if (platform1Pos.Contains(player1.pPosition) && !player1.jumping)
            {
                player1.pVelocity.Y -= 30;
                player1.jumping = true;
            }
        } */

        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Add your drawing code here
            spriteBatch.Draw(texture, rectangle, Color.White);


        }


    }
}