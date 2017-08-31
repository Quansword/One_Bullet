using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneBullet
{

    public class GunManager : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D megaManXR;
        Texture2D megaManXL;
        Texture2D p1GunR;
        Texture2D p1GunL;
        Texture2D p1GunSprite;
        Texture2D p1Sprite;
        Texture2D p1TempDirection;
        Texture2D bullet;
        Texture2D platform;

        Rectangle bulletPosition;
        Rectangle p1Position;
        Rectangle p1GunPosition;
        Rectangle platformPosition;
        Vector2 p1Velocity;

        int p1LevelOffset;
        int bulletSpeed = 20;
        const int p1Acceleration = 3;
        bool onGround, jumping;
        double charSize;
        bool p1BulletMoving;
        bool p1HasBullet;
        bool p2BulletMoving;
        bool p2HasBullet;

        KeyboardState kState;
        private KeyboardState oldKState;

        enum GunLevel
        {
            High,
            Mid,
            Low
        };

        GunLevel level = GunLevel.Mid;

        //speed and direction bullet is moving when fired
        public void bulletMovement()
        {
            if(p1BulletMoving == true && p1TempDirection == megaManXR)
            {
                p1HasBullet = false;
                bulletPosition.X += bulletSpeed;
            }

            else if(p1BulletMoving == true && p1TempDirection == megaManXL)
            {
                p1HasBullet = false;
                bulletPosition.X -= bulletSpeed;
            }

            else if(p1BulletMoving == false)
            {
                p1HasBullet = true;
                bulletPosition = p1GunPosition;

            }

        }


        public void platformChecker()
        {
            if (p1Position.Intersects(platformPosition))
            {
                p1Position.Y = platformPosition.Y;
                p1Velocity.Y = 0;

            }
        }

        public GunManager()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            charSize = GraphicsDevice.Viewport.Height / 7;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            p1Position = new Rectangle(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - (int)charSize, (int)(charSize * 1.04), (int)charSize);
            platformPosition = new Rectangle(400, 450,600,256);
            p1GunPosition = p1Position;
            p1GunPosition.X += (int)charSize / 2;
            p1Velocity = new Vector2(0, 0);
            onGround = true;
            jumping = false;
            p1LevelOffset = 0;
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            megaManXR = Content.Load<Texture2D>("MegaManXR");
            megaManXL = Content.Load<Texture2D>("MegaManXL");
            p1GunR = Content.Load<Texture2D>("gunR");
            p1GunL = Content.Load<Texture2D>("gunL");
            bullet = Content.Load<Texture2D>("shot_poulpi");
            platform = Content.Load<Texture2D>("square");
            p1GunSprite = p1GunR;
            p1Sprite = megaManXR;
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {

            //checks to see if bullet was fired and what direction the player was facing
            if (kState.IsKeyDown(Keys.F) && p1HasBullet == true) // move bullet
            {
                p1BulletMoving = true;
                p1TempDirection = p1Sprite;
            }

            bulletMovement();
            platformChecker();
            //////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // ------------------------------------------ Keyboard inputs
            kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.A)) // Move left
            {
                p1Velocity.X -= 10;
                p1Sprite = megaManXL;
                p1GunSprite = p1GunL;
                p1GunPosition.X = p1Position.X - (int)charSize / 2;
            }
            if (kState.IsKeyDown(Keys.D)) // Move right
            {
                p1Velocity.X += 10;
                p1Sprite = megaManXR;
                p1GunSprite = p1GunR;
                p1GunPosition.X = p1Position.X + (int)charSize / 2;
            }
            if (kState.IsKeyDown(Keys.G)) // Jump
            {
                if (onGround && !jumping)
                {
                    p1Velocity.Y -= 30;
                    jumping = true;
                }
                else if (!onGround && jumping && p1Velocity.Y < 0)
                {
                    p1Velocity.Y -= 1;
                }
            }
            if (kState.IsKeyUp(Keys.G) && oldKState.IsKeyDown(Keys.G))
            {
                jumping = false;
            }
            if (kState.IsKeyDown(Keys.W) && oldKState.IsKeyUp(Keys.W)) // Change gun level
            {
                if (level == GunLevel.Low)
                {
                    level = GunLevel.Mid;
                    p1LevelOffset = 0;
                }
                else if (level == GunLevel.Mid)
                {
                    level = GunLevel.High;
                    p1LevelOffset = -((int)charSize / 3);
                }
            }
            if (kState.IsKeyDown(Keys.S) && oldKState.IsKeyUp(Keys.S)) // Change gun level
            {
                if (level == GunLevel.High)
                {
                    level = GunLevel.Mid;
                    p1LevelOffset = 0;
                }
                else if (level == GunLevel.Mid)
                {
                    level = GunLevel.Low;
                    p1LevelOffset = (int)charSize / 3;
                }
            }

                oldKState = kState;

            // ------------------------------------------ Falling parameters
            if (p1Position.Y < GraphicsDevice.Viewport.Height - charSize)
            {
                onGround = false;
                p1Velocity.Y += p1Acceleration;
            }
            else if (!onGround)
            {
                onGround = true;
                p1Velocity.Y = 0;
            }

            // ------------------------------------------ Calculating velocity
            p1Position.X += (int)p1Velocity.X;
            if (p1Position.X > GraphicsDevice.Viewport.Width - (int)charSize)
            {
                p1Position.X = GraphicsDevice.Viewport.Width - (int)charSize;
            }
            else if (p1Position.X < 0)
            {
                p1Position.X = 0;
            }
            p1Position.Y += (int)p1Velocity.Y;
            if (p1Position.Y > GraphicsDevice.Viewport.Height - (int)charSize)
                p1Position.Y = GraphicsDevice.Viewport.Height - (int)charSize;

            p1GunPosition.X += (int)p1Velocity.X;
            p1GunPosition.Y = p1Position.Y + p1LevelOffset;

            // ------------------------------------------ Resetting values
            p1Velocity.X = 0;

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(platform, platformPosition, Color.White);
            spriteBatch.Draw(p1Sprite, p1Position, Color.White);
            spriteBatch.Draw(p1GunSprite, p1GunPosition, Color.White);
            spriteBatch.Draw(bullet, bulletPosition, Color.White);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
