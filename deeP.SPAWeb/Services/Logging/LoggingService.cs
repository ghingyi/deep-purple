namespace deeP.SPAWeb.Services
{
    using System;
    using System.Diagnostics;
    using System.Web;
    using Elmah;

    /// <summary>
    /// Log <see cref="Exception"/> objects.
    /// </summary>
    public sealed class LoggingService : ILoggingService
    {
        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Error(Exception exception)
        {
            // Log to Tracing.
            Trace.TraceError(exception.ToString());
            // Log to Elmah.
            ErrorSignal.FromCurrentContext().Raise(exception, HttpContext.Current);
        }

        /// <summary>
        /// Logs the specified message as warning.
        /// </summary>
        /// <param name="format">Format string to use for message composition.</param>
        /// <param name="args">Arguments to include in the message.</param>
        public void Warning(string format, params object[] args)
        {
            // Log to Tracing.
            Trace.TraceWarning(format, args);
        }

        /// <summary>
        /// Logs the specified message as information.
        /// </summary>
        /// <param name="format">Format string to use for message composition.</param>
        /// <param name="args">Arguments to include in the message.</param>
        public void Info(string format, params object[] args)
        {
            // Log to Tracing.
            Trace.TraceInformation(format, args);
        }

        /// <summary>
        /// Logs the specified message as verbose information.
        /// </summary>
        /// <param name="format">Format string to use for message composition.</param>
        /// <param name="args">Arguments to include in the message.</param>
        public void Verbose(string format, params object[] args)
        {
            // Log to Tracing.
            Trace.WriteLine(string.Format(format, args));
        }
    }
}