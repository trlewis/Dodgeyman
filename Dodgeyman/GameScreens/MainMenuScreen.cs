namespace Dodgeyman.GameScreens
{
    using System;
    using BitmapFont;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    internal class MainMenuScreen : GameScreen
    {
        private const float BounceDistance = 15f;
        private const float BounceTime = 1.1f; //bounces per second
        private const string QuitString = "Quit";
        private const string StartString = "Start";
        private const string StatsString = "Stats";

        private readonly String[] _menuOptions = {StartString, QuitString};
        private readonly Vector2f _titleScale = new Vector2f(4, 4);

        private float _bounceOffset;
        private BitmapFont _font;
        private int _selectedOption;
        private Vector2u _windowSize;

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

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        //IDisposable
        public override void Dispose()
        {
            this._font.Dispose();
            GameScreenManager.RenderWindow.KeyPressed -= this.HandleKeyPress;
        }

        //ActiveEntity
        protected override void Activate()
        {
            this.RegisterEvents();
        }

        //ActiveEntity
        protected override void Deactivate()
        {
            this.DeregisterEvents();
        }

        //GameScreen
        public override void Draw(RenderTarget target)
        {
            // draw title
            this._font.RenderText("Dodgeyman");
            this._font.StringSprite.Scale = this._titleScale;
            Sprite titleSprite = this._font.StringSprite;
            var titleWidth = titleSprite.TextureRect.Width*this._titleScale.X;
            titleSprite.Position = new Vector2f((this._windowSize.X - titleWidth)/2, 20);
            target.Draw(titleSprite);

            // draw menu options
            this._font.StringSprite.Scale = new Vector2f(1, 1);
            const float itemX = 50;
            float itemY = 300;
            for (int i = 0; i < this._menuOptions.Length; i++)
            {
                this._font.RenderText(this._menuOptions[i]);
                Sprite sp = this._font.StringSprite;
                var xpos = (i == this._selectedOption) ? this._bounceOffset + itemX : itemX;
                sp.Position = new Vector2f(xpos, itemY);
                target.Draw(sp);
                itemY += 10 + (sp.TextureRect.Height*sp.Scale.Y);
            }
        }

        //GameScreen
        public override void Initialize()
        {
            this._windowSize = GameScreenManager.RenderWindow.Size;
            this._font = new BitmapFont("Assets/monochromeSimple.png");
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

        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        private void DeregisterEvents()
        {
            GameScreenManager.RenderWindow.KeyPressed -= this.HandleKeyPress;
        }

        private void RegisterEvents()
        {
            GameScreenManager.RenderWindow.KeyPressed += this.HandleKeyPress;
        }

        private void ChoiceSelected()
        {
            if (!this.IsActive)
                return; //may not be needed since this method is only called by HandleKeyPress which also checks it

            var choiceText = this._menuOptions[this._selectedOption];
            if(choiceText.Equals(QuitString))
                GameScreenManager.ShutDown();
            if(choiceText.Equals(StartString))
                GameScreenManager.PushScreen(new ArenaScreen.ArenaScreen());
        }
    }
}
