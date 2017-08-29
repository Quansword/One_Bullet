using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GunManager : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D gunLeft;
        Texture2D gunRight;
        Texture2D bullet;

        int bulletX;
        int yAxis;
        int yAxisMax = 0;
        int yAxisMin = 50;
        int yAxisMid = 20;
        int low = 1;
        int mid = 2;
        int high = 3;
        int currentLevel = 2;

        private Texture2D bulletTexture;
        private Vector2 bulletTarget;
        public Vector2 bulletPosition;
        private Vector2 bulletVelocity;
        public bool isBulletActive;
        public Rectangle bulletRectangle;

 
        private void SetVelocity()
        {
            bulletVelocity = -(bulletPosition - bulletTarget);
            bulletVelocity.Normalize();
        }


        public GunManager()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

 
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gunLeft = Content.Load<Texture2D>("gun");
            gunRight = Content.Load<Texture2D>("gun");
            bullet = Content.Load<Texture2D>("shot_poulpi");



            // TODO: use this.Content to load your game content here
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {



            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState state = Keyboard.GetState();

            // If they hit esc, exit
            if (state.IsKeyDown(Keys.Escape))
                Exit();

            if (state.IsKeyDown(Keys.F))
            {
                isBulletActive = true;

            }

            if(isBulletActive == true)
            {
                bulletX += 10;
            }


            // Move our sprite based on arrow keys being pressed:
            //move gun up unless it has reached maxium height
            if (state.IsKeyDown(Keys.Up))
            {
                if(currentLevel == low)
                {
                    currentLevel = mid;
                }

                if(currentLevel == mid)
                {
                    currentLevel = high;
                }

                if (currentLevel == high)
                {
                    currentLevel = high;
                }
            }

            //move gun down unless it has reached minimum height
            if (state.IsKeyDown(Keys.Down))
            {
                if (currentLevel == high)
                {
                    currentLevel = mid;
                }

                if (currentLevel == mid)
                {
                    currentLevel = low;
                }

                if (currentLevel == low)
                {
                    currentLevel = low;
                }
            }

            //move gun to one of the three leveles based on where it currently is
            if(currentLevel == low)
            {
                yAxis = yAxisMin;
            }

            if(currentLevel == mid)
            {
                yAxis = yAxisMid;
            }

            if(currentLevel == high)
            {
                yAxis = yAxisMax;
            }

            base.Update(gameTime);

            //previousState = state;
       
        }

    
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SpriteEffects s = SpriteEffects.FlipHorizontally;
            spriteBatch.Begin();
            spriteBatch.Draw(gunLeft, new Rectangle(20, yAxis, 100, 148), null, Color.White, 0, new Vector2(50,50), s, 0f);
            spriteBatch.Draw(gunRight, new Rectangle(700, 20, 100, 148), null, Color.White);
            spriteBatch.Draw(bullet, new Rectangle(bulletX, yAxis, 100, 148), null, Color.White, 0, new Vector2(50, 50), s, 0f);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}