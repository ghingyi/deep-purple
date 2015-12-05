namespace deeP.SPAWeb.Services
{
    using System;

    /// <summary>
    /// Log <see cref="Exception"/> objects.
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Logs the specified exception as error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Error(Exception exception);

        /// <summary>
        /// Logs the specified message as warning.
        /// </summary>
        /// <param name="format">Format string to use for message composition.</param>
        /// <param name="args">Arguments to include in the message.</param>
        void Warning(string format, params object[] args);

        /// <summary>
        /// Logs the specified message as information.
        /// </summary>
        /// <param name="format">Format string to use for message composition.</param>
        /// <param name="args">Arguments to include in the message.</param>
        void Info(string format, params object[] args);

        /// <summary>
        /// Logs the specified message as verbose information.
        /// </summary>
        /// <param name="format">Format string to use for message composition.</param>
        /// <param name="args">Arguments to include in the message.</param>
        void Verbose(string format, params object[] args);
    }
}
