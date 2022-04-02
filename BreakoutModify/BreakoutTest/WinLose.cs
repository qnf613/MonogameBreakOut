using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace BreakoutTest
{
    public enum WLstate { neutral, win, lose };
    
    class WinLose : BlockManager
    { 
        private bool buttondown;
        SpriteBatch sb;
        SpriteFont font;       
        
      
        public WLstate State { get; set; }
        public WinLose(Game game, Ball b) : base(game, b)
        {
            this.State = WLstate.neutral;
            ball = b;
        }
        public void CheckWin()
        {
            if (nomoreblocks == true && ScoreManager.Lives > 0)
            {
                this.State = WLstate.win; 
            }              
        }
        public void CheckLose()
        {
            if (ScoreManager.Lives == 0)
            {
                this.State = WLstate.lose;   
            }         
        }
        void CheckNeutral()
        {
            if (ScoreManager.Lives > 0 && blockcount >0)
            {
                this.State = WLstate.neutral;
            }
        }
        public override void Update(GameTime gameTime)
        {

            CheckLose();
            CheckWin();
            CheckNeutral();
            
            base.Update(gameTime);
        }
        
        private void ResettoNeutral() { this.State = WLstate.neutral; }
        protected override void LoadContent()
        {
            
            sb = new SpriteBatch(this.Game.GraphicsDevice);
            font = this.Game.Content.Load<SpriteFont>("Arial");
            
            base.LoadContent();
        }
        public override void Draw(GameTime gameTime)
        {
            sb.Begin();
           

            if (this.State == WLstate.lose)
            {
                ball.resetBall(gameTime);
                sb.DrawString(font, "Press 'R' to play again", new Vector2(GraphicsDevice.Viewport.Width / 2, 250), Color.White);
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    buttondown = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.R) && buttondown == true)
                {
                    ResettoNeutral();
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    ResetLevels(gameTime);
                    ball.resetBall(gameTime);
                    buttondown = false;
                }
            }
           else if (this.State == WLstate.win && nomoreblocks == true)
            {
                ball.resetBall(gameTime);
                if (ScoreManager.Level >= 10)
                {
                    sb.DrawString(font, "You won the game! Thanks for play!", new Vector2(GraphicsDevice.Viewport.Width / 2, 250), Color.White);
                }
                else
                {
                    sb.DrawString(font, "Press 'Space Bar' to try the next level", new Vector2(GraphicsDevice.Viewport.Width / 2, 250), Color.White);
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        buttondown = true;
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Space) && buttondown == true)
                    {
                        ResettoNeutral();
                        GraphicsDevice.Clear(Color.CornflowerBlue);
                        if (ScoreManager.Lives < 3)
                        {
                            ScoreManager.Lives++;
                        }
                        NextLevel();

                        buttondown = false;
                    }
                }
                
            }
           
            //sb.DrawString(font, nomoreblocks.ToString(), new Vector2(300, 200), Color.Red);
            //sb.DrawString(font, ball.Speed.ToString(), new Vector2(300, 300), Color.Red);
            //sb.DrawString(font, ball.State.ToString(), new Vector2(300, 350), Color.Red);
            //sb.DrawString(font, State.ToString(), new Vector2(300, 400), Color.Red);
            //sb.DrawString(font, blockcount.ToString(), new Vector2(300, 250), Color.Red);
            sb.End();
        
           
            base.Draw(gameTime);
        }

    }
}
