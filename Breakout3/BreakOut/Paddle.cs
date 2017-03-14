using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGameLibrary.Sprite2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;

namespace BreakOut
{
    class Paddle : DrawableSprite2
    {
        GameConsole console;
        PaddleController controller;

        Ball ball;      //Need refernce to ball for collision
        Rectangle top;  //Regatngle for paddle collision we are using a smaller rectangle on the top of the paddle for collision

        public Paddle(Game game, Ball b)
            : base(game)
        {
            this.Speed = 200;
            this.ball = b;
            controller = new PaddleController(game, ball);

            console = (GameConsole)this.Game.Services.GetService(typeof(IGameConsole));
            if (console == null) //ohh no no console
            {
                console = new GameConsole(this.Game);
                this.Game.Components.Add(console);  //add a new game console to Game

            }
            this.ShowMarkers = true;
        }

        protected override void LoadContent()
        {
            this.spriteTexture = this.Game.Content.Load<Texture2D>("paddleSmall");      //default paddle is small may subclass for differnt paddles
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            //Movement from controller
            controller.HandleInput(gameTime);
            this.Direction = controller.Direction;

            //Collision Rect
            top = new Rectangle((int)this.Location.X, (int)this.Location.Y, this.spriteTexture.Width, 1);

            if (ball.State == BallState.OnPaddle)
            {
                //Move the ball with the paddle until launch
                ball.Speed = 0;
                ball.Direction = Vector2.Zero;
                ball.Location = new Vector2(this.Location.X + this.LocationRect.Width / 2, this.Location.Y - ball.SpriteTexture.Height);
            }
            else
            {
                //Ball Collsion
                //Very simple collision with ball only uses rectangles
                if (top.Intersects(ball.LocationRect))
                {
                    //TODO Change angle based on location of collision or direction of paddle
                    ball.Direction.Y *= -1;
                    console.GameConsoleWrite("Paddle collision ballLoc:" + ball.Location + " paddleLoc:" + this.Location.ToString());

                }
            }

            this.Location += this.Direction * (this.Speed * gameTime.ElapsedGameTime.Milliseconds / 1000);
            KeepPaddleOnScreen();
            base.Update(gameTime);
        }

        private void KeepPaddleOnScreen()
        {
            this.Location.X = MathHelper.Clamp(this.Location.X, 0, this.Game.GraphicsDevice.Viewport.Width - this.spriteTexture.Width);
        }
    }
}
