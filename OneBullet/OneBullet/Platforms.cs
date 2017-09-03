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
        Texture2D platform1;
        Player player1;
        public Rectangle platform1Pos = new Rectangle(400, 500, 200, 200);



        public void Initialize(Texture2D stageTexture)
        {
            platform1 = stageTexture;
           // player1 = new Player();
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
            spriteBatch.Draw(platform1, platform1Pos, Color.White);


        }


    }
}