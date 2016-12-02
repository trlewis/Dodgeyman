namespace Dodgeyman.GameScreens
{
    using System;
    using Models;
    using SFML.Graphics;
    using SFML.System;

    internal abstract class GameScreen : ActiveEntity, IDisposable
    {
        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------

        /// <summary>
        /// Whether or not the GameScreen has already been initialized. Initialize() should not be called
        /// more than once.
        /// </summary>
        protected bool IsInitialized { get; private set; }

        /// <summary>
        /// The size of the RenderTarget that will be passed to the Draw() method.
        /// </summary>
        protected Vector2u TargetSize { get; private set; }

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        public abstract void Dispose();

        public abstract void Draw(RenderTarget target);

        /// <summary>
        /// Inializes the game screen, telling it to use the given size as the drawable area.
        /// </summary>
        /// <param name="targetSize">The size of the RenderTarget that will be passed to Draw()</param>
        //public abstract void Initialize(Vector2u targetSize);
        public virtual void Initialize(Vector2u targetSize)
        {
            this.TargetSize = targetSize;
            this.IsInitialized = true;
        }

        //may need to include a Time parameter at some point
        public abstract void Update(Time time);
    }
}
