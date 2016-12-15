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
        /// The size of the RenderTarget that will be passed to the Draw() method.
        /// </summary>
        protected Vector2u TargetSize { get; private set; }

        /// <summary>
        /// Whether or not the GameScreen has already been initialized. Initialize() should not be called
        /// more than once.
        /// </summary>
        private bool IsInitialized { get; set; }

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        public abstract void Dispose();

        /// <summary>
        /// Draws the contents of the GameScreen onto the RenderTarget. The RenderTarget's size should be
        /// the same as the targetSize passed into the Initialize() function
        /// </summary>
        /// <param name="target">The target to draw the GameScreen on to.</param>
        public abstract void Draw(RenderTarget target);

        /// <summary>
        /// Inializes the game screen, telling it to use the given size as the drawable area.
        /// </summary>
        /// <param name="targetSize">The size of the RenderTarget that will be passed to Draw()</param>
        public void Initialize(Vector2u targetSize)
        {
            if (this.IsInitialized)
                return;

            this.TargetSize = targetSize;
            this.IsInitialized = true;
            this.OnInitialize();
        }

        //may need to include a Time parameter at some point
        public abstract void Update(Time time);

        /// <summary>
        /// This is where the actual process of initialization happens
        /// </summary>
        protected abstract void OnInitialize();
    }
}
