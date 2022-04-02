using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BreakoutTest
{
    class BlockManager : DrawableGameComponent
    {

        public List<MonogameBlock> Blocks { get; private set; } //List of Blocks the are managed by Block Manager
        protected int blockcount;
        protected int w;
        protected int h;
        //Dependancy on Ball
        protected Ball ball;
        public static bool nomoreblocks; // I made this static because I didn't want to make another instance of block manager


        List<MonogameBlock> blocksToRemove; //list of block to remove probably because they were hit

        /// <summary>
        /// BlockManager hold a list of blocks and handles updating, drawing a block collision
        /// </summary>
        /// <param name="game">Reference to Game</param>
        /// <param name="ball">Refernce to Ball for collision</param>
        public BlockManager(Game game, Ball b)
            : base(game)
        {
            this.Blocks = new List<MonogameBlock>();
            this.blocksToRemove = new List<MonogameBlock>();
            
            this.ball = b;
        }

        public override void Initialize()
        {
            LoadLevel();
            base.Initialize();
        }

        /// <summary>
        /// Replacable Method to Load a Level by filling the Blocks List with Blocks
        /// </summary>
        public void NextLevel()
        {
            ScoreManager.Level++;
            LoadLevel();
        }
        public void ResetLevels(GameTime gameTime)
        {
            foreach (MonogameBlock b in Blocks)
            {
                b.BlockState = BlockState.Broken;
                blocksToRemove.Add(b);

            }
            foreach (var block in blocksToRemove)
            {
                Blocks.Remove(block);
            }
            blocksToRemove.Clear();
            ScoreManager.SetupNewGame();
            LoadLevel();
        }

        protected virtual void LoadLevel()
        {

            if (ScoreManager.Level <= 10)
            {
                w = 24;
                h = ScoreManager.Level;
            }

            blockcount = w * h;
            CreateBlockArrayByWidthAndHeight(w, h, 1);
        }

        /// <summary>
        /// Simple Level lays out multiple levels of blocks
        /// </summary>
        /// <param name="width">Number of blocks wide</param>
        /// <param name="height">Number of blocks high</param>
        /// <param name="margin">space between blocks</param>
        private void CreateBlockArrayByWidthAndHeight(int width, int height, int margin)
        {

            MonogameBlock b;
            //Create Block Array based on with and hieght
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    b = new MonogameBlock(this.Game);
                    b.Initialize();
                    b.Location = new Vector2(5 + (w * b.SpriteTexture.Width + (w * margin)), 50 + (h * b.SpriteTexture.Height + (h * margin)));
                    Blocks.Add(b);
                }
            }
        }
        bool reflected; //the ball should only reflect once even if it hits two bricks
        public override void Update(GameTime gameTime)
        {
            CheckBlockCount();

            this.reflected = false; //only reflect once per update
            this.blockhit = false;  // only hit once per frame

            UpdateCheckBlocksForCollision(gameTime);
            UpdateBlocks(gameTime);
            UpdateRemoveDisabledBlocks();

            base.Update(gameTime);
        }
        private void CheckBlockCount() //Just a simple checker to reveal at the score manager that all blocks are dead
        {
            if (blockcount == 0)
            {
                nomoreblocks = true;
            }
            else if (blockcount > 0)
            {
                nomoreblocks = false;
            }
        }
        private void UpdateBlocks(GameTime gameTime)
        {
            foreach (var block in Blocks)
            {
                block.Update(gameTime);
            }
        }
        /// <summary>
        /// Removes disabled blocks from list
        /// </summary>
        private void UpdateRemoveDisabledBlocks()
        {
            //remove disabled blocks
            foreach (var block in blocksToRemove)
            {
                Blocks.Remove(block);
                ScoreManager.Score++;
                blockcount--;
            }
            blocksToRemove.Clear();
        }
        bool blockhit = false;
        private void UpdateCheckBlocksForCollision(GameTime gameTime)
        {
            foreach (MonogameBlock b in Blocks)
            {
                if (b.Enabled) //Only chack active blocks
                {
                    b.Update(gameTime); //Update Block
                    //Ball Collision
                    if (b.Intersects(ball) && blockhit == false) //chek rectagle collision between ball and current block 
                    {
                        blockhit = true;
                        //hit
                        b.HitByBall(ball, gameTime);
                        if (b.BlockState == BlockState.Broken)
                            blocksToRemove.Add(b);  //Ball is hit add it to remove list
                        
                        if (!reflected) //only reflect once
                        {
                            ball.Reflect(b);
                            this.reflected = true;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Block Manager Draws blocks they don't draw themselves
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            foreach (var block in this.Blocks)
            {
                if (block.Visible)   //respect block visible property
                    block.Draw(gameTime);
            }
            base.Draw(gameTime);
        }
    }
}
