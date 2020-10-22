using System.Diagnostics;


namespace GameCore.Systems
{
    /// <summary>
    /// This system is responsible to output the messages from varius places.
    /// </summary>
    public sealed class LoggingSystem : SystemBase
    {
        /// <summary>
        /// This class is used internally by <c>LoggingSystem</c> to interface with the <c>System.Diagnostics.Debug</c> class.
        /// </summary>
        private class InternalListener : TraceListener
        {
            public LoggingSystem LoggingSystem
            {
                get;
                set;
            }

            public override void Write(string message)
            {
                LoggingSystem.Write(message);
            }

            public override void WriteLine(string message)
            {
                LoggingSystem.WriteLine(message);
            }
        }

        /// <summary>
        /// This member acts as a proxy for the <c>TraceListener</c> class.
        /// </summary>
        private readonly InternalListener proxyListener = new InternalListener();

        /// <summary>
        /// This delegates describes the callback type for Write/WriteLine events.
        /// </summary>
        /// <param name="message">The message to output</param>
        public delegate void OnMessageWriteCallback(string message);

        /// <summary>
        /// This event is invoked when Debug.Write is called.
        /// </summary>
        public event OnMessageWriteCallback OnMessageWrite;

        /// <summary>
        /// This event is invoked when Debug.WriteLine is called.
        /// </summary>
        public event OnMessageWriteCallback OnMessageWriteLine;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Name => "Logging System";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="manager"></param>
        protected override void OnRegister(SystemsManager manager)
        {
            proxyListener.LoggingSystem = this;
            proxyListener.Name = Name;
            Debug.Listeners.Add(proxyListener);
        }

        /// <summary>
        /// Add a listener that inherits <c>TraceListener</c> to the logging system.
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(TraceListener listener)
        {
            Debug.Listeners.Add(listener);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        public void Write(string message)
        {
            if (OnMessageWrite is null)
            {
                return;
            }
            OnMessageWrite.Invoke(message);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        public void WriteLine(string message)
        {
            if (OnMessageWriteLine is null)
            {
                return;
            }
            OnMessageWriteLine.Invoke(message);
        }
    }
}
