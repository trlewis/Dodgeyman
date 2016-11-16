namespace Dodgeyman.Code.Extensions
{
    using System;

    public static class EventHandlerExtensions
    {
        public static void SafeInvoke(this EventHandler handler, object sender, EventArgs args)
        {
            if (handler != null)
                handler(sender, args);
        }

        public static void SafeInvoke<T>(this EventHandler<T> handler, object sender, T args)
        {
            if (handler != null)
            {
                handler(sender, args);
            }
        }
    }
}
