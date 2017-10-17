using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BreakOut
{
    class BlockManager : GameComponent
    {

        public List<Block> Blocks { get; private set; }     //Blocks managed by block manager
        Ball ball;                                          //Reference to ball for collision
        List<Block> blocksToRemove;                         //List of blocks to remove after they have been hit ny the ball
        bool reflected;                                     //Bool to make sure ball is only reflected by a one block per update. The ball may hit two blocks at once

        /// <summary>
        /// BlockManager hold a list of blocks and handles updating, drawing a block collision
        /// </summary>
        /// <param name="game">Reference to Game</param>
        /// <param name="ball">Refernce to Ball for collision</param>
        public BlockManager(Game game, Ball ball)
            : base(game)
        {
            this.Blocks = new List<Block>();
            this.blocksToRemove = new List<Block>();
            this.ball = ball;
        }

        public override void Initialize()
        {
            LoadLevel();        //Load a simple level

            base.Initialize();
        }

        /// <summary>
        /// Replacable Method to Load a Level by filling the Blocks List with Blocks
        /// </summary>
        protected virtual void LoadLevel()
        {
            CreateBlockArrayByWidthAndHeight(24, 2, 1);
        }

        /// <summary>
        /// Simple Level lays out multiple levels of blocks
        /// </summary>
        /// <param name="width">Number of blocks wide</param>
        /// <param name="height">Number of blocks high</param>
        /// <param name="margin">space between blocks</param>
        private void CreateBlockArrayByWidthAndHeight(int width, int height, int margin)
        {
            Block b;
            //Create Block Array based on with and hieght
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    b = new Block(this.Game);
                    b.Initialize();
                    b.Location = new Vector2(5 + (w * b.SpriteTexture.Width + (w * margin)), 50 + (h * b.SpriteTexture.Height + (h * margin)));
                    Blocks.Add(b);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            this.reflected = false;     //Only Reflect the Ball once per update
            foreach (Block b in Blocks)
            {
                if (b.Enabled)  //Only chack active blocks
                {
                    b.Update(gameTime);//Update Block
                    //Ball Collision
                    if (b.Intersects(ball)) //Check for collision with current block
                    {
                        b.HitByBall(ball);
                        blocksToRemove.Add(b); //Can't remove blocks from the current Array as we are interating on that collection
                                               //Add the current block to another array to be removed after the look finishes
                        if (!reflected) //Only reflect once but can break multiple blocks
                        {
                            ball.Reflect(b);
                            this.reflected = true;
                        }
                    }
                }
            }

            //remove disabled blocks
            foreach (var block in blocksToRemove)
            {
                Blocks.Remove(block);

            }
            blocksToRemove.Clear(); //empty broken blocks array

            base.Update(gameTime);
        }

        public void DrawBlocks(SpriteBatch sb)
        {
            foreach (var block in Blocks)
            {
                //Only draw visible blocks
                if (block.Visible)
                {
                    block.Draw(sb);
                }
            }
        }
    }
}
