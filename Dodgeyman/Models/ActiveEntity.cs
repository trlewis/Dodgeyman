namespace Dodgeyman.Models
{
    internal abstract class ActiveEntity
    {
        private bool _isActive;

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

        protected abstract void Activate();
        protected abstract void Deactivate();
    }
}
