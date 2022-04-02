using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGameLibrary.Sprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;


namespace BreakoutTest
{
    class ScoreManager : DrawableGameComponent
    {

        SpriteFont font;
        public static int Lives;
        public static int Level;
        public static int Score;
        int lilpaddlesX;
        int lilpaddlesY;
        Texture2D paddle;

        SpriteBatch sb;
        Vector2 scoreLoc, livesLoc, levelLoc;
        
        
        public ScoreManager(Game game)
            : base(game)
        {
            SetupNewGame();
        }

        public static void SetupNewGame()
        {
            Lives = 3;
            Level = 1;
            Score = 0;
        }

        protected override void LoadContent()
        {
            sb = new SpriteBatch(this.Game.GraphicsDevice);
            font = this.Game.Content.Load<SpriteFont>("Arial");
            paddle = this.Game.Content.Load<Texture2D>("paddleSmall");
            livesLoc = new Vector2(this.GraphicsDevice.Viewport.Width / 25, this.GraphicsDevice.Viewport.Width / 50);
            levelLoc = new Vector2(this.GraphicsDevice.Viewport.Width / 2.5f, this.GraphicsDevice.Viewport.Width / 50);
            scoreLoc = new Vector2(this.GraphicsDevice.Viewport.Width / 1.3f, this.GraphicsDevice.Viewport.Width / 50);
            lilpaddlesX = this.GraphicsDevice.Viewport.Width / 8;
            lilpaddlesY = this.GraphicsDevice.Viewport.Width / 45;
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            sb.Begin();
            for (int i = 0; i < Lives; i++)
            {
                sb.Draw(paddle, new Rectangle((65 * i) + 100, 15, paddle.Width / 2, paddle.Height / 2), Color.White);
            }
            sb.DrawString(font, "Lives: " + Lives, livesLoc, Color.White);
            sb.DrawString(font, "Score: " + Score, scoreLoc, Color.White);
            sb.DrawString(font, "Level: " + Level, levelLoc, Color.White); 
            sb.End();
            base.Draw(gameTime);
        }
    }
}
