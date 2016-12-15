namespace Dodgeyman.GameScreens
{
    using System;
    using Code;
    using Font;
    using Models.Stats;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;
    using Stats;

    internal class MainMenuScreen : GameScreen
    {
        private const string AuthorText = "CREATED BY: TRAVIS LEWIS";
        private const float BounceDistance = 15f;
        private const float BounceTime = 1.1f; //bounces per second
        private const string InstructionsString = "INSTRUCTIONS";
        private const string QuitString = "QUIT";
        private const string StartString = "START";
        private const string StatsString = "STATS";
        private static readonly Vector2f TitleScale = new Vector2f(8, 8);
        private static readonly Vector2f OtherScale = new Vector2f(3, 3);
        private static readonly string VersionText = string.Format("V{0}", ConfigHelper.VersionNumber);
        
        private float _bounceOffset;
        private BitmapFont _font;
        private String[] _menuOptions;
        private int _selectedOption;

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        //IDisposable
        public override void Dispose()
        {
            this._font.Dispose();
        }

        //GameScreen
        public override void Draw(RenderTarget target)
        {
            // draw title
            this._font.RenderText("DODGEYMAN");
            this._font.StringSprite.Scale = TitleScale;
            Sprite titleSprite = this._font.StringSprite;
            var titleWidth = titleSprite.TextureRect.Width*TitleScale.X;
            titleSprite.Position = new Vector2f((int)((this.TargetSize.X - titleWidth)/2), 20);
            target.Draw(titleSprite);

            // draw high score
            if (GameStats.Instance.HasStats)
            {
                this._font.StringSprite.Scale = OtherScale;
                this._font.RenderText(String.Format("HIGH SCORE: {0}", GameStats.Instance.HighScore));
                var scoreWidth = this._font.StringSprite.TextureRect.Width*this._font.StringSprite.Scale.X;
                var scoreY = (int)(titleSprite.Position.Y + (titleSprite.TextureRect.Height*TitleScale.Y) + 30);
                var scoreX = (int) ((this.TargetSize.X - scoreWidth)/2);
                this._font.StringSprite.Position = new Vector2f(scoreX, scoreY);
                target.Draw(this._font.StringSprite);
            }

            // draw menu options
            this._font.StringSprite.Scale = OtherScale;
            const float itemX = 50;
            float itemY = 300;
            for (int i = 0; i < this._menuOptions.Length; i++)
            {
                this._font.RenderText(this._menuOptions[i]);
                Sprite sp = this._font.StringSprite;
                var xpos = (i == this._selectedOption) ? this._bounceOffset + itemX : itemX;
                sp.Position = new Vector2f((int)xpos, itemY);
                target.Draw(sp);
                itemY += 10 + (sp.TextureRect.Height*sp.Scale.Y);
            }

            // draw version/about
            this._font.StringSprite.Scale = new Vector2f(1, 1);
            this._font.RenderText(AuthorText);
            var x = this.TargetSize.X - this._font.StringSprite.TextureRect.Width - 10;
            var y = this.TargetSize.Y - this._font.StringSprite.TextureRect.Height - 10;
            this._font.StringSprite.Position = new Vector2f(x, y);
            target.Draw(this._font.StringSprite);

            this._font.RenderText(VersionText);
            x = this.TargetSize.X - this._font.StringSprite.TextureRect.Width - 10;
            y -= this._font.ScreenLineHeight;
            this._font.StringSprite.Position = new Vector2f(x, y);
            target.Draw(this._font.StringSprite);
        }

        //GameScreen
        public override void Update(Time time)
        {
            if (!this.IsActive)
                return; //don't bother with math if this screen isn't even active

            //0.5 because the abs causes one sine-wave to be two bounces
            var angle = time.AsSeconds()*2*Math.PI*BounceTime*0.5; 
            var offset = Math.Sin(angle);
            this._bounceOffset = (float) Math.Abs(offset) * BounceDistance;
        }

        //ActiveEntity
        protected override void Activate()
        {
            GameScreenManager.RenderWindow.KeyPressed += this.HandleKeyPress;
            //this is here so the Stats option can be added after the first game
            if (GameStats.Instance.HasStats)
                this._menuOptions = new[] { StartString, InstructionsString, StatsString, QuitString };
            else
                this._menuOptions = new[] { StartString, InstructionsString, QuitString };
        }

        //ActiveEntity
        protected override void Deactivate()
        {
            GameScreenManager.RenderWindow.KeyPressed -= this.HandleKeyPress;
        }

        //GameScreen
        protected override void OnInitialize()
        {
            this._font = new BitmapFont(ConfigHelper.GeneralFontLocation);
            GameStats.Initialize();
        }

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        private void HandleKeyPress(object sender, KeyEventArgs args)
        {
            if (!this.IsActive)
                return;

            if (args.Code == Keyboard.Key.Down && this._selectedOption < this._menuOptions.Length - 1)
                this._selectedOption++;
            if (args.Code == Keyboard.Key.Up && this._selectedOption > 0)
                this._selectedOption--;
            if (args.Code == Keyboard.Key.Return)
                this.ChoiceSelected();
            if(args.Code == Keyboard.Key.Escape)
                GameScreenManager.PopScreen();
        }

        private void ChoiceSelected()
        {
            if (!this.IsActive)
                return; //may not be needed since this method is only called by HandleKeyPress which also checks it

            var choiceText = this._menuOptions[this._selectedOption];
            if(choiceText.Equals(InstructionsString))
                GameScreenManager.PushScreen(new InstructionsScreen());
            if(choiceText.Equals(QuitString))
                GameScreenManager.ShutDown();
            if(choiceText.Equals(StartString))
                GameScreenManager.PushScreen(new ArenaScreen.ArenaScreen());
            if(choiceText.Equals(StatsString))
                GameScreenManager.PushScreen(new StatsScreen());
        }
    }
}
