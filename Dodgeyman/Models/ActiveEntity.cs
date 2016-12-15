namespace Dodgeyman.Models
{
    /// <summary>
    /// An object that can be "turned on and off". This is meant for objects that can be
    /// activated and deactivated multiple times, and perform some actions when 
    /// activated and deactivated. It is not meant for objects that require a one-time
    /// initialization procedure.
    /// </summary>
    internal abstract class ActiveEntity
    {
        private bool _isActive;

        /// <summary>
        /// Gets or sets whether the object is active or inactive.
        /// </summary>
        public bool IsActive
        {
            get { return this._isActive; }
            set
            {
                if (value == this._isActive)
                    return;
                this._isActive = value;
                if(this._isActive)
                    this.Activate();
                else
                    this.Deactivate();
            }
        }

        /// <summary>
        /// What to do when this object is activated.
        /// </summary>
        protected abstract void Activate();

        /// <summary>
        /// What to do when an object is deactivated.
        /// </summary>
        protected abstract void Deactivate();
    }
}
