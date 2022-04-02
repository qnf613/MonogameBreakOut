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
    public enum BallState {  OnPaddleStart, Playing }

    public class Ball : DrawableSprite
    {
        
        public BallState State { get; private set; }

        GameConsole console;

        public Ball(Game game)
            : base(game)
        {
            this.State = BallState.OnPaddleStart;

            console = (GameConsole)this.Game.Services.GetService(typeof(IGameConsole));
            if (console == null) //ohh no no console
            {
                console = new GameConsole(this.Game);
                this.Game.Components.Add(console);  //add a new game console to Game
            }
#if DEBUG
            this.ShowMarkers = true;
#endif
        }

        public void SetInitialLocation()
        {
            this.Location = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 10);
        }

        public void LaunchBall(GameTime gameTime, Vector2 d)
        {
            d += Vector2.UnitY;
            this.Speed = 150 + (ScoreManager.Level + 1) * 10;
            this.Direction = d;
            this.State =  BallState.Playing;
            this.console.GameConsoleWrite("Ball Launched " + gameTime.TotalGameTime.ToString());
        }

        protected override void LoadContent()
        {
            this.spriteTexture = this.Game.Content.Load<Texture2D>("ballSmall");
            SetInitialLocation();
            base.LoadContent();
        }

        public void resetBall(GameTime gameTime)
        {
            this.Speed = 0;
            this.State =  BallState.OnPaddleStart;
            this.console.GameConsoleWrite("Ball Reset " + gameTime.TotalGameTime.ToString());
        }

        public override void Update(GameTime gameTime)
        {
            switch(this.State)
            {
                case BallState.OnPaddleStart:
                    break;

                case BallState.Playing:
                    UpdateBall(gameTime);
                    break;
            }
            
            base.Update(gameTime);
        }

        private void UpdateBall(GameTime gameTime)
        {
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
                ScoreManager.Lives--;
            }

            //Top
            if (this.Location.Y < 0)
            {
                this.Direction.Y *= -1;
            }
        }

        public void Reflect(MonogameBlock block)
        {
            if (Rectagle.Intersects(block.LocationRect))
            {
                //TODO check for side collision with block
                if (this.Location.X < block.Location.X)
                {
                    this.Direction.X *= -1;
                    console.GameConsoleWrite("left");
                }
                if (this.Location.X >= block.Location.X + block.spriteTexture.Width / 4)
                {
                    this.Direction.X *= -1;
                    console.GameConsoleWrite("right");
                }
                if(this.Location.Y < block.Location.Y)
                { 
                    this.Direction.Y *= -1; console.GameConsoleWrite("up"); 
                }
                if (this.Location.Y >= block.Location.Y + block.spriteTexture.Height / 4)
                {
                    this.Direction.Y *= -1;
                    console.GameConsoleWrite("below");
                }
            }
        }

    }
}
