using System;
using System.Threading;

namespace GameCore.Systems
{
    /// <summary>
    /// This exception is thrown whenever there is an exception in a system thread
    /// </summary>
    public class SystemException : Exception
    {
        /// <summary>
        /// Constructs a new <c>SystemException</c>.
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="system">The system that caused the exception</param>
        public SystemException(string message = "", SystemBase system = null) : base(message)
        {
            if (system == null)
            {
                system = SystemsManager.CurrentManager.GetSystem(Thread.CurrentThread.Name);
            }
            System = system;
        }

        /// <summary>
        /// Constructs a new <c>SystemException</c> as a wrapper.
        /// </summary>
        /// <param name="e">The inner exception</param>
        /// <param name="message">The message to display</param>
        /// <param name="system">The system that caused the exception</param>
        public SystemException(Exception e, string message = "", SystemBase system = null) : base(message, e)
        {
            if (system == null)
            {
                system = SystemsManager.CurrentManager.GetSystem(Thread.CurrentThread.Name);
            }
            System = system;
        }

        /// <summary>
        /// The system that created the thread
        /// </summary>
        public SystemBase System
        {
            get;
            private set;
        }
    }
}
