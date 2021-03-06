﻿namespace Dodgeyman.GameScreens.ArenaScreen.Lines
{
    using System;
    using Code.Extensions;
    using Models;
    using SFML.Graphics;
    using SFML.System;

    abstract class DodgeLine : ActiveEntity, IDisposable
    {
        protected const float LineThickness = 2f;

        /// <summary>
        /// Event that fires if the player crosses the line. May or may not mean that
        /// the player was killed.
        /// </summary>
        public event EventHandler<LineCrossedEventArgs> Crossed;

        /// <summary>
        /// Event that fires when the line is no longer needed.
        /// </summary>
        public event EventHandler Finished;

        /// <summary>
        /// Call this method from any constructor of a class that inherits from this one.
        /// </summary>
        /// <param name="player">The Player that is trying to dodge the line.</param>
        /// <param name="targetSize">The size of the screen or are this line should occupy.</param>
        protected DodgeLine(Player player, Vector2u targetSize)
        {
            this.Player = player;
            this.TargetSize = targetSize;
            this.IsActive = true;
        }

        // ---------------------------------------------
        // PROPERTIES
        // ---------------------------------------------

        /// <summary>
        /// The Player that is trying to dodge the line. Used for collision detection.
        /// </summary>
        protected Player Player { get; private set; }

        /// <summary>
        /// Whether or not the player has crossed the line yet. A true value does not mean
        /// a killing collision occurred, but it does mean the opportunity to score a point has passed.
        /// </summary>
        protected bool IsCrossed { get; set; }

        /// <summary>
        /// The size of the screen or area that this DodgeLine will be drawn on.
        /// </summary>
        protected Vector2u TargetSize { get; private set; }

        // ---------------------------------------------
        // INHERITED MEMBERS
        // ---------------------------------------------

        #region Inherited members

        // IDisposable
        public abstract void Dispose();

        //DodgeLines don't subscribe to any events, so these do nothing. But
        //they still need to know if they're active or not for their update methods
        protected override void Activate() { } // ActiveEntity
        protected override void Deactivate() { } // ActiveEntity
        
        #endregion Inherited members

        // ---------------------------------------------
        // METHODS
        // ---------------------------------------------

        /// <summary>
        /// Draws the line to the specified target.
        /// </summary>
        /// <param name="target">The target to draw the line onto.</param>
        public abstract void Draw(RenderTarget target);

        /// <summary>
        /// Moves the line and checks if it collided with the player.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Dims the fill color of the given Shape
        /// </summary>
        protected void DimShape(Shape shape)
        {
            const uint dimFactor = 3;
            var color = shape.FillColor;
            var r = (byte)(color.R/dimFactor);
            var g = (byte)(color.G/dimFactor);
            var b = (byte)(color.B/dimFactor);
            shape.FillColor = new Color(r, g, b, 0xFF);
        }

        protected void OnCrossed(LineCrossedEventArgs args)
        {
            this.Crossed.SafeInvoke(this, args);
        }

        protected void OnFinished()
        {
            this.Finished.SafeInvoke(this, EventArgs.Empty);
        }
    }
}
