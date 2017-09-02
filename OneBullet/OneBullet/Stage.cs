using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    class Stage
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background, backgroundElements;
        Player player1, player2;


        /*Stage()
        {
            if (platformPosition.Contains(player1.pPosition) && !player1.jumping)
            {
                player1.pVelocity.Y -= 30;
                player1.jumping = true;
            }
        } */

        public void Initialize(Texture2D texture)
        {
            background = texture;
            backgroundElements = texture;

        }


        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Add your drawing code here
            spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.Draw(backgroundElements, new Rectangle(0, 0, 1280, 720), Color.White);
     

        }




    }
}