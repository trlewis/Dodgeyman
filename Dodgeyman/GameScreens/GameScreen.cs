namespace Dodgeyman.GameScreens
{
    using System;
    using Models;
    using SFML.Graphics;
    using SFML.System;

    internal abstract class GameScreen : ActiveEntity, IDisposable
    {
        public abstract void Dispose();

        public abstract void Draw(RenderTarget target);

        public abstract void Initialize();

        //may need to include a Time parameter at some point
        public abstract void Update(Time time);
    }
}
