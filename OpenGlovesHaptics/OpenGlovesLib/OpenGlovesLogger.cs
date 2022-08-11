using System;

namespace OpenGlovesLib
{
    public static class OpenGlovesLogger
    {
        private static Action<string> callback;
        private static string header = "[OpenGloves] ";
        private static string errorheader = "[OpenGloves][ERROR] ";

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        /// <param name="logCallback"></param>
        public static void Init(Action<string> logCallback)
        {
            callback = logCallback;
        }

        /// <summary>
        /// Logs a message to the callback.
        /// </summary>
        /// <param name="message"></param>
        internal static void Log(string message)
        {
            callback?.Invoke(header + message);
        }

        /// <summary>
        /// Logs an error to the callback.
        /// </summary>
        /// <param name="message"></param>
        internal static void Error(string message)
        {
            callback?.Invoke(errorheader + message);
        }
    }
}
