using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace MonoGameLibrary.Sprite
{
    /// <summary>
    /// This is and Extension of Drawable sprite. Each Sprite has
    /// an animation adapter that can manage animations
    /// </summary>
    public class DrawableAnimatableSprite : DrawableSprite
    {

        protected SpriteAnimationAdapter spriteAnimationAdapter;

        protected Texture2D currentTexture;

        public DrawableAnimatableSprite(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            //content = game.Content;
            spriteAnimationAdapter = new SpriteAnimationAdapter(game);
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            //graphics = (GraphicsDeviceManager)Game.Services.GetService(typeof(IGraphicsDeviceManager));
            base.Initialize();
        }

        protected override void LoadContent()
        {

            //Set Sprite Texture
            this.spriteTexture = spriteAnimationAdapter.CurrentTexture;

            base.LoadContent();
        }

        

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            //Elapsed time since last update
            lastUpdateTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //GamePad1
            SpriteEffects = SpriteEffects.None;       //Default Sprite Effects
            this.spriteTexture = this.spriteAnimationAdapter.CurrentTexture;   //update texture for collision

            Rectangle currentTextureRect = spriteAnimationAdapter.GetCurrentDrawRect(lastUpdateTime);

            this.locationRect = new Rectangle((int)Location.X - (int)this.Orgin.X,
                (int)Location.Y - (int)this.Orgin.Y,
                currentTextureRect.Width,
                currentTextureRect.Height);

            base.Update(gameTime);
        }

        //Override to use the correct rectagle
        protected override void SetTranformAndRect()
        {
            try
            {
                // Build the block's transform
                spriteTransform =
                    Matrix.CreateTranslation(new Vector3(this.Orgin, 0.0f)) *
                    Matrix.CreateScale(this.Scale) *
                    Matrix.CreateRotationZ(0.0f) *
                    Matrix.CreateTranslation(new Vector3(this.Location, 0.0f));

                // Calculate the bounding rectangle of this block in world space
                this.locationRect = CalculateBoundingRectangle(
                         new Rectangle(0, 0, this.spriteAnimationAdapter.CurrentTexture.Width,
                             this.spriteAnimationAdapter.CurrentTexture.Height),
                         spriteTransform);
            }
            catch (NullReferenceException nu)
            {
                //nothing
                if (this.spriteTexture == null)
                {
                    //first time this will fail because load content hasn't been called yet

                }
                else
                {
                    throw nu;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Rectangle currentTextureRect = spriteAnimationAdapter.GetCurrentDrawRect(lastUpdateTime);

            this.locationRect = new Rectangle((int)Location.X - (int)this.Orgin.X,
                (int)Location.Y - (int)this.Orgin.Y,
                currentTextureRect.Width,
                currentTextureRect.Height);
            
            spriteBatch.Begin();
            spriteBatch.Draw(spriteAnimationAdapter.CurrentTexture,
                new Rectangle(
                    (int)Location.X,
                    (int)Location.Y,
                    currentTextureRect.Width * (int)this.Scale,
                    currentTextureRect.Height *(int)this.Scale),
                currentTextureRect,
                Color.White,
                MathHelper.ToRadians(Rotate),
                this.Orgin,
                SpriteEffects,
                0);

            this.DrawMarkers(spriteBatch);
            spriteBatch.End();
            //base.Draw(gameTime);
        }

        protected override void DrawMarkers(SpriteBatch sb)
        {
            //Show markers on the location and rect of a sprite
            if (showMarkers)
            {
                Rectangle markerRect = new Rectangle((int)(this.locationRect.X  * scale),
                    (int)(this.locationRect.Y   * scale),
                    (int)(this.spriteAnimationAdapter.CurrentLocationRect.Width * this.Scale),
                    (int)(this.spriteAnimationAdapter.CurrentLocationRect.Height * this.Scale));

                //Rectangle markerRect = this.locationRect;

                //Rect Top Left
                sb.Draw(this.SpriteMarkersTexture,
                    new Rectangle(markerRect.Left - this.SpriteMarkersTexture.Width / 2,
                        markerRect.Top - this.SpriteMarkersTexture.Height / 2,
                        SpriteMarkersTexture.Width, SpriteMarkersTexture.Height),
                    Color.Red);
                //Rect Top Right
                sb.Draw(this.SpriteMarkersTexture,
                   new Rectangle(markerRect.Right - this.SpriteMarkersTexture.Width / 2,
                       markerRect.Top, SpriteMarkersTexture.Width, SpriteMarkersTexture.Height),
                   Color.Red);
                //Rect Bottom Left
                sb.Draw(this.SpriteMarkersTexture,
                   new Rectangle(markerRect.Left - this.SpriteMarkersTexture.Width / 2,
                       markerRect.Bottom - this.SpriteMarkersTexture.Height / 2,
                       SpriteMarkersTexture.Width, SpriteMarkersTexture.Height),
                   Color.Red);
                //Rect Bottom Right
                sb.Draw(this.SpriteMarkersTexture,
                   new Rectangle(markerRect.Right - this.SpriteMarkersTexture.Width / 2,
                       markerRect.Bottom - this.SpriteMarkersTexture.Height / 2,
                       SpriteMarkersTexture.Width, SpriteMarkersTexture.Height),
                   Color.Red);

                //location Marker
                sb.Draw(this.SpriteMarkersTexture,
                    new Rectangle((int)this.Location.X - this.SpriteMarkersTexture.Width / 2,
                        (int)this.Location.Y - this.SpriteMarkersTexture.Height / 2,
                        SpriteMarkersTexture.Width, SpriteMarkersTexture.Height),
                    Color.Yellow);


            }
        }
    }




    public class SpriteAnimationAdapter
    {
        List<SpriteAnimation> spriteAnimations;
        protected SpriteAnimation currentAnimation;
        protected CelAnimationManager celAnimationManger;

        public Rectangle CurrentLocationRect
        {
            get
            {
                return this.GetCurrentDrawRect();
            }
        }

        public CelAnimationManager CelAnimationManager { get { return celAnimationManger;}}
        public SpriteAnimation CurrentAnimation {
            get { return currentAnimation; }
            set {
                    if(!(spriteAnimations.Contains(value)))
                    {
                        this.spriteAnimations.Add(value);
                    }
                        this.currentAnimation = value;
            }
        }
              
        public SpriteAnimationAdapter(Game game)
        {
            spriteAnimations = new List<SpriteAnimation>();
            
            celAnimationManger = (CelAnimationManager)game.Services.GetService(typeof(ICelAnimationManager));
            if (celAnimationManger == null)
            {
                throw new Exception("To use a DrawableAnimatedSprite you must a CelAnimationManager to the game as a service!");
            }
            
        }

        public Texture2D CurrentTexture
        {
            get { return celAnimationManger.GetTexture(currentAnimation.TextureName); }
        }

        public void AddAnimation(SpriteAnimation s)
        {
            this.spriteAnimations.Add(s);
            this.celAnimationManger.AddAnimation(s.AnimationName, s.TextureName, s.CellCount, s.FPS);
            this.celAnimationManger.ToggleAnimation(s.AnimationName, false);
            if (spriteAnimations.Count == 1)
            {
                currentAnimation = s;
            }
            
        }

        public void ResetAnimation(SpriteAnimation s)
        {

            this.celAnimationManger.ResetAnimation(s.AnimationName);
        }

        public void RemoveAnimation(SpriteAnimation s)
        {
            this.spriteAnimations.Remove(s);
            this.celAnimationManger.Animations.Remove(s.AnimationName);
        }

        public void PauseAnimation(SpriteAnimation s)
        {
            this.celAnimationManger.ToggleAnimation(s.AnimationName, true);
        }

        public void GotToFrame(SpriteAnimation s, int frame)
        {
            //TODO
        }

        public void ResumeAmination(SpriteAnimation s)
        {
            this.celAnimationManger.ToggleAnimation(s.AnimationName, false);
        }

        public Rectangle GetCurrentDrawRect(float elapsedTime)
        {
            return this.CelAnimationManager.GetCurrentDrawRect(elapsedTime, currentAnimation.AnimationName);
        }

        public Rectangle GetCurrentDrawRect()
        {
            return GetCurrentDrawRect(0.0f);
        }

        public int GetLoopCount()
        {
            return this.celAnimationManger.Animations[currentAnimation.AnimationName].LoopCount;
        }


    }

    public class SpriteAnimation 
    {

        public string AnimationName;
        public int FPS;
        public string TextureName;
        public CelCount CellCount;

        protected bool isPaused;
        public bool IsPaused { get { return isPaused;} set { isPaused = value;} }

        public SpriteAnimation(string animationName, string textureName,
            int fps,  int numberOfCols, int numberOfRows  )
        {
            this.AnimationName = animationName;
            this.FPS = fps;
            this.TextureName = textureName;
            this.CellCount = new CelCount(numberOfCols,numberOfRows);
            isPaused = true;
        }

    }
}