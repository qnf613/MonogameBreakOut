using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace MonoGameLibrary.Util
{
    /// <summary>
    /// This is a game component that implements DrawableGameComponent
    /// and defines the IGameConsole interface so that GameConsole
    /// may be uses as a game service
    /// </summary>

    public interface IGameConsole
    {
        string FontName { get; set;}
        string DebugText { get; set; }
        Dictionary<string, string> DebugTextOutput { get; set; }

        string GetGameConsoleText();
        void GameConsoleWrite(string s);

    }

    public class GameConsole : Microsoft.Xna.Framework.DrawableGameComponent, IGameConsole
    {
        protected string fontName;
        public string FontName { get { return fontName; } set { fontName = value; } }

        protected string debugText;
        public string DebugText { get { return debugText; } set { debugText = value; } }

        protected Dictionary<string, string> debugTextOutput;
        public Dictionary<string, string> DebugTextOutput { get { return debugTextOutput; } set { debugTextOutput = value; } }
        protected string debugTextOutString;
        float debugTextStartX, debugTextStartY;


        protected int maxLines;
        public int MaxLines { get { return maxLines; } set { maxLines = value; } }

        SpriteFont font;
        SpriteBatch spriteBatch;
        
        protected List<string> gameConsoleText;
        protected GameConsoleState gameConsoleState;

        public Keys ToggleConsoleKey;

        InputHandler input;     //GameConsole depends on InputHandler
        
        public GameConsole(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            this.fontName = "Arial";
            this.gameConsoleText = new List<string>();
            this.maxLines = 20;
            this.debugText = "Console default \ndebug text";
            this.ToggleConsoleKey = Keys.OemTilde;
            this.debugTextStartX = 400;
            this.debugTextStartY = 0;
            

            this.debugTextOutput = new Dictionary<string, string>();

            this.gameConsoleState = GameConsoleState.Open;

           
            input = (InputHandler)game.Services.GetService(typeof(IInputHandler));

            //Make sure input service exsists
            if (input == null)
            {
                //try to add one
                input= new InputHandler(game);
                game.Components.Add(input);

                //Check again
                if (input == null)
                {
                    throw new Exception("GameConsole Depends on Input service please add input service before you add GameConsole.");
                }
            }
            
            game.Services.AddService(typeof(IGameConsole), this);
        }

        protected override void LoadContent()
        {

            try
            {
                font = this.Game.Content.Load<SpriteFont>("content/Arial");
            }
            catch
            {
                try
                {
                    font = this.Game.Content.Load<SpriteFont>("Arial");
                }
                catch
                {
                    font = this.Game.Content.Load<SpriteFont>("SpriteFont1");
                }
            }
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.gameConsoleText.Add("Console Initalized");
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            
            base.Initialize();
        }

        protected override void UnloadContent()
        {
            

            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            
            if(input.KeyboardState.HasReleasedKey(ToggleConsoleKey))
            {
                this.ToggleConsole();   
            }
            
            base.Update(gameTime);
        }

        public void ToggleConsole()
        {
            if (this.gameConsoleState == GameConsoleState.Closed)
            {
                this.gameConsoleState = GameConsoleState.Open;
            }
            else
            {
                this.gameConsoleState = GameConsoleState.Closed;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.gameConsoleState == GameConsoleState.Open)
            {
                //spriteBatch.Begin();
                //4.0 change
                //spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteS2rtMode.Deferred, SaveStateMode.SaveState);
                spriteBatch.Begin();
                spriteBatch.DrawString(font, GetGameConsoleText(), Vector2.Zero, Color.Wheat);
                
                
                
                debugTextOutString = String.Empty;
                foreach (var item in debugTextOutput)
                {
                    debugTextOutString += "\n" +  item.Key + " : " + item.Value;
                    
                }
                
                spriteBatch.DrawString(font, debugText + debugTextOutString, new Vector2(debugTextStartX, debugTextStartY), Color.Wheat);

                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public string GetGameConsoleText()
        {
            string Text = "";

            string[] current = new string[Math.Min(gameConsoleText.Count, MaxLines)];
            int offsetLines = (gameConsoleText.Count / maxLines) * maxLines;
            
            int offest = gameConsoleText.Count - offsetLines;

            int indexStart = offsetLines - (maxLines - offest);
            if (indexStart < 0)
                indexStart = 0;
            /*
            this.debugText = string.Format(
                "offesetLines:{0}\noffset:{1}\ngameConsoleText.Count:{2}\nIndexStart:{3}",
                offsetLines.ToString(),
                offest.ToString(),
                gameConsoleText.Count.ToString(),
                indexStart);
            */
            
            gameConsoleText.CopyTo(
                indexStart, current, 0 , Math.Min(gameConsoleText.Count, MaxLines));

            foreach (string s in current)
            {
                Text += s;
                Text += "\n";
            }
            return Text;
        }

        public void Log(string DebugKey, string DebugValue)
        {
            if(this.debugTextOutput.ContainsKey(DebugKey))
            {
                debugTextOutput[DebugKey] = DebugValue;
                return;
            }
            debugTextOutput.Add(DebugKey, DebugValue); 
        }

        public void GameConsoleWrite(string s)
        {
            gameConsoleText.Add(s);
        }

        //Console State
        public enum GameConsoleState { Closed, Open};
    }
}