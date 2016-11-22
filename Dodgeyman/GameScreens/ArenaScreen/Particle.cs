namespace Dodgeyman.GameScreens.ArenaScreen
{
    using System;
    using Code.Extensions;
    using SFML.Graphics;
    using SFML.System;

    /// <summary>
    /// A particle that starts with the Color and position given, and moves with the velocity given
    /// while fading the color towards black.
    /// </summary>
    class Particle
    {
        private const int FadeSpeed = 2;
        //if all RGB values are below this, then the particle is "finished"
        private const int MinColor = 10;
        //just a "singleton array", don't want to create one every time Draw() gets called
        private readonly Vertex[] _pointArray; 
        private readonly Vector2f _velocity;

        /// <summary>
        /// What to do when the particle has faded to the point where it's no longer visible. Does
        /// not pass EventArgs.
        /// </summary>
        public event EventHandler ParticleFinished;

        public Particle(Color startColor, Vector2f position, Vector2f velocity)
        {
            this._pointArray = new[] { new Vertex(position, startColor) };
            this._velocity = velocity;
        }

        public void Draw(RenderTarget target)
        {
            target.Draw(this._pointArray, 0, 1, PrimitiveType.Points);
        }

        public void Update()
        {
            this._pointArray[0].Position += this._velocity;
            var currentColor = this._pointArray[0].Color;
            var r = (byte)Math.Max(currentColor.R - FadeSpeed, 0);
            var g = (byte)Math.Max(currentColor.G - FadeSpeed, 0);
            var b = (byte)Math.Max(currentColor.B - FadeSpeed, 0);

            if(r <= MinColor && g <= MinColor && b <= MinColor)
                this.ParticleFinished.SafeInvoke(this, EventArgs.Empty);

            this._pointArray[0].Color = new Color(r, g, b, 0xFF);
        }

    }
}
