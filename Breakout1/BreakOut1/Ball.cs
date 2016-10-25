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


namespace BreakOut1
{

    enum BallState { OnPaddle, Launched };

    class Ball  : DrawableSprite2
    {
        
        GameConsole console;

        protected BallState _state;
        public BallState State
        {
            get { return _state; }
            protected set
            {
                if(this._state != value)
                {
                    this._state = value;
                }
            }
        }
        
        public Ball(Game game) : base (game)
        {
            this.State = BallState.OnPaddle;

            console = (GameConsole)this.Game.Services.GetService(typeof(IGameConsole));
            if (console == null) //ohh no no console
            {
                console = new GameConsole(this.Game);   
                this.Game.Components.Add(console);  //add a new game console to Game
            }
            this.ShowMarkers = true;
        }

        public void LaunchBall(GameTime gameTime)
        {
            this.Speed = 190;
            this.Direction = new Vector2(1, 1);
            this.State = BallState.Launched;
            this.console.GameConsoleWrite("Ball Launched " + gameTime.TotalGameTime.ToString());
        }

        protected override void LoadContent()
        {
            this.spriteTexture = this.Game.Content.Load<Texture2D>("ballSmall");
            base.LoadContent();
        }

        private void resetBall(GameTime gameTime)
        {
            this.Speed = 0;
            this.State = BallState.OnPaddle;
            this.console.GameConsoleWrite("Ball Reset " + gameTime.TotalGameTime.ToString());
            
        }

        public override void Update(GameTime gameTime)
        {
            if (this.State == BallState.OnPaddle)
            {
                base.Update(gameTime);
                return;
            }
            this.Location += this.Direction * (this.Speed * gameTime.ElapsedGameTime.Milliseconds / 1000);

            //bounce off wall
            //Left and Right
            if ((this.Location.X + this.spriteTexture.Width > this.Game.GraphicsDevice.Viewport.Width)
                ||
                (this.Location.X < 0))
            {
                this.Direction.X *= -1;
            }
            //bottom Miss
            if (this.Location.Y + this.spriteTexture.Height > this.Game.GraphicsDevice.Viewport.Height)
              
            {
                this.resetBall(gameTime);
            }

            //Top
            if  (this.Location.Y < 0)
            {
                this.Direction.Y *= -1;
            }

            base.Update(gameTime);
        }
    }
}
