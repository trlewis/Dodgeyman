namespace Dodgeyman.Code
{
    using System;

    public static class EnumHelper<T>
    {
        /// <summary>
        /// Attempts to convert the given name to the enum value with the same name, ignores case. An exception
        /// is thrown if no enum value exists with the passed in name.
        /// </summary>
        /// <param name="name">The name of the enum value to get.</param>
        /// <returns>The enum value for the given name.</returns>
        public static T Parse(string name)
        {
            return (T) Enum.Parse(typeof (T), name, true);
        }
    }
}
