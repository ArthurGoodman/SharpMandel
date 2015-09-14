using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mandel {
    public class Game : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Effect mandel;
        Texture2D texture;

        Vector2 resolution, defaultResolution;

        bool fullScreen;

        float prevWheelValue;
        float currWheelValue;

        Vector2 offset;
        float scale;
        int maxIt, samples;

        Vector2 seed;

        KeyboardState keyboardState, oldKeyboardState;
        MouseState oldMouseState;

        int pass;

        public Game() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            defaultResolution = new Vector2(1280, 720);

            graphics.PreferredBackBufferWidth = (int)defaultResolution.X;
            graphics.PreferredBackBufferHeight = (int)defaultResolution.Y;
            graphics.ApplyChanges();

            IsMouseVisible = true;

            resolution = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

            fullScreen = false;

            defaults();

            seed = Vector2.Zero;

            pass = 0;

            base.Initialize();
        }

        private void defaults() {
            maxIt = 500;
            samples = 1;
            offset = Vector2.Zero;
            scale = 1.0f / 3;
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mandel = Content.Load<Effect>("Fractal");
            texture = new Texture2D(GraphicsDevice, (int)resolution.X, (int)resolution.Y);
        }

        protected override void UnloadContent() {
        }

        private bool keyTyped(Keys key) {
            return keyboardState.IsKeyDown(key) && !oldKeyboardState.IsKeyDown(key);
        }

        protected override void Update(GameTime gameTime) {
            keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyTyped(Keys.Q))
                if (maxIt > 100) maxIt -= 100;
            if (keyTyped(Keys.W))
                maxIt += 100;

            if (keyTyped(Keys.A))
                if (samples > 1) samples--;
            if (keyTyped(Keys.S))
                samples++;

            if (keyTyped(Keys.Back))
                defaults();

            if (keyTyped(Keys.M)) pass = 0;
            if (keyTyped(Keys.J)) pass = 1;
            if (keyTyped(Keys.C)) pass = 2;

            if (keyTyped(Keys.F11)) {
                if (!fullScreen) {
                    fullScreen = true;

                    resolution.X = graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    resolution.Y = graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                } else {
                    fullScreen = false;

                    resolution.X = graphics.PreferredBackBufferWidth = (int)defaultResolution.X;
                    resolution.Y = graphics.PreferredBackBufferHeight = (int)defaultResolution.Y;
                }

                texture = new Texture2D(GraphicsDevice, (int)resolution.X, (int)resolution.Y);

                graphics.IsFullScreen = fullScreen;
                graphics.ApplyChanges();
            }

            oldKeyboardState = keyboardState;

            MouseState mouseState = Mouse.GetState();

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                seed = (Mouse.GetState().Position.ToVector2() / resolution - new Vector2(0.5f, 0.5f)) / scale * new Vector2(1, resolution.Y / resolution.X) - offset;

            else if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Pressed) {
                float x = (mouseState.Position.X - oldMouseState.Position.X) / (float)GraphicsDevice.PresentationParameters.BackBufferWidth;
                float y = (mouseState.Position.Y - oldMouseState.Position.Y) / (float)GraphicsDevice.PresentationParameters.BackBufferHeight;
                offset += new Vector2(x, y * resolution.Y / resolution.X) / scale;
            }

            prevWheelValue = currWheelValue;
            currWheelValue = mouseState.ScrollWheelValue;

            if (prevWheelValue < currWheelValue)
                scale *= 1.1f;
            else if (prevWheelValue > currWheelValue)
                scale /= 1.1f;

            oldMouseState = mouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            mandel.Parameters["resolution"].SetValue(resolution);

            mandel.Parameters["maxIt"].SetValue(maxIt);
            mandel.Parameters["samples"].SetValue(samples);
            mandel.Parameters["offset"].SetValue(offset);
            mandel.Parameters["scale"].SetValue(scale);

            mandel.Parameters["seed"].SetValue(seed);

            spriteBatch.Begin(SpriteSortMode.Immediate);
            mandel.CurrentTechnique.Passes[pass].Apply();
            spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
