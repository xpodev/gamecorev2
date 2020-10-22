using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace GameCore.Systems
{
    /// <summary>
    /// This system is responsible for making sure the other systems run properly.
    /// </summary>
    public sealed class DiagnosticsSystem : SystemBase
    {
        /// <summary>
        /// An <c>OnSystemCrash</c> callback structure.
        /// </summary>
        /// <param name="system">The system that crashed</param>
        /// <param name="exception">The exception that caused the system to crash</param>
        public delegate void OnSystemCrashCallback(SystemBase system, Exception exception);

        /// <summary>
        /// This event is fired whenever a system crashed due to an exception.
        /// </summary>
        public event OnSystemCrashCallback OnSystemCrash;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Name => "Diagnostics System";

        /// <summary>
        /// In case where there is an exception in one of the systems, if RestartOnCrash = true,
        /// it will restart the crashed system and save a crash report using <c>System.Diagnostics.Debug</c>.
        /// </summary>
        /// <param name="sender">The <c>AppDomain</c> that sent the exception</param>
        /// <param name="e">Exception event args</param>
        private void CatchException(object sender, FirstChanceExceptionEventArgs e)
        {
            if (e.Exception is SystemException systemException)
            {
                SystemBase system = systemException.System;
                Debug.WriteLine(
                    $"System Failure:\n" +
                    $"{system}\n" +
                    $"{e.Exception}\n");
                OnSystemCrash.Invoke(system, systemException.InnerException);
            }
            else
            {
                Debug.WriteLine($"{((AppDomain)sender).FriendlyName}: \n{e.Exception}");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="manager"></param>
        protected override void OnRegister(SystemsManager manager)
        {
            manager.HostAppDomain.FirstChanceException += CatchException;
        }
    }
}
