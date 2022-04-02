﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Util;
using Microsoft.Xna.Framework.Input;

namespace BreakoutTest
{
    class PaddleController
    {
        InputHandler input;
        Ball ball; //may should delgate to parent
        public Vector2 Direction { get; private set; }

        public PaddleController(Game game, Ball ball)
        {
            input = (InputHandler)game.Services.GetService(typeof(IInputHandler));
            this.Direction = Vector2.Zero;
            this.ball = ball;   //need refernce to ball to be able to lanch ball could possibly use delegate here
        }

        public void HandleInput(GameTime gametime)
        {
            this.Direction = Vector2.Zero;  //Start with no direction on each new upafet

            //No need to sum input only uses left and right
            if (input.KeyboardState.IsKeyDown(Keys.Left))
            {
                this.Direction = new Vector2(-1, 0);
            }
            if (input.KeyboardState.IsKeyDown(Keys.Right))
            {
                this.Direction = new Vector2(1, 0);
            }
            //TODO add mouse controll?

            //Up launches ball
            if (input.KeyboardState.WasKeyPressed(Keys.Up))
            {
                if (ball.State == BallState.OnPaddleStart) //Only Launch Ball is it's on paddle
                {
                    if (this.Direction.X == 0)
                    {
                        Random r = new Random();
                        float randomX = r.Next(-1, 1);
                        if (randomX >= 0)
                        {
                            randomX = 1;
                        }
                        this.ball.LaunchBall(gametime, new Vector2(randomX, this.Direction.Y));
                    }
                    else
                    this.ball.LaunchBall(gametime, this.Direction); 
                }
            }
        }
    }
}
