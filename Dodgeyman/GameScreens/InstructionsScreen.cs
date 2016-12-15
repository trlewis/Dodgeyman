namespace Dodgeyman.GameScreens
{
    using Code;
    using Font;
    using SFML.Graphics;
    using SFML.System;
    using SFML.Window;

    internal class InstructionsScreen : GameScreen
    {
        private const float LeftSide = 20f;
        private const string MovementInstructions = "ARROW KEYS: MOVE DODGEYMAN";
        private const string ColorSwitchInstruction = "Z: CHANGE COLOR";
        private const string ObjectiveInstructions = "MATCH LINE COLORS TO PASS THROUGH AND GAIN A POINT";
        private BitmapFont _bf;

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        //IDisposable
        public override void Dispose()
        {
            this._bf.Dispose();
        }

        //GameScreen
        public override void Draw(RenderTarget target)
        {
            this._bf.RenderText(MovementInstructions);
            float y = LeftSide; //start so top/left are same distance from edge of window
            this._bf.StringSprite.Position = new Vector2f(LeftSide, y);
            target.Draw(this._bf.StringSprite);

            y += this._bf.ScreenLineHeight;
            this._bf.RenderText(ColorSwitchInstruction);
            this._bf.StringSprite.Position = new Vector2f(LeftSide, y);
            target.Draw(this._bf.StringSprite);

            y += this._bf.ScreenLineHeight*2;
            this._bf.RenderText(ObjectiveInstructions);
            this._bf.StringSprite.Position = new Vector2f(LeftSide, y);
            target.Draw(this._bf.StringSprite);
        }

        //GameScreen
        public override void Update(Time time) { /*nothing to do here*/ }

        //ActiveEntity
        protected override void Activate()
        {
            GameScreenManager.RenderWindow.KeyPressed += this.HandleKeyPress;
        }

        //ActiveEntity
        protected override void Deactivate()
        {
            GameScreenManager.RenderWindow.KeyPressed -= this.HandleKeyPress;
        }

        //GameScreen
        protected override void OnInitialize()
        {
            this._bf = new BitmapFont(ConfigHelper.GeneralFontLocation);
            this._bf.StringSprite.Scale = new Vector2f(2, 2);
        }

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        private void HandleKeyPress(object sender, KeyEventArgs args)
        {
            if(args.Code == Keyboard.Key.Escape)
                GameScreenManager.PopScreen();
        }
    }
}
