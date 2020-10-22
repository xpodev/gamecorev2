using System;
using System.Threading;


namespace GameCore.Systems
{
    /// <summary>
    /// The base class for all the systems
    /// </summary>
    public abstract class SystemBase
    {
        /// <summary>
        /// This member holds the thread that the system runs on.
        /// </summary>
        /// See <see cref="RunOnThread"/>
        private Thread CurrentThread = null;

        /// <summary>
        /// A function that is called when the system fails.
        /// </summary>
        public delegate void OnSystemFailureCallback();

        /// <summary>
        /// This event is fired when the system fails (on exception).
        /// </summary>
        /// See <see cref="DiagnosticsSystem.OnSystemCrash"/>
        public event OnSystemFailureCallback OnSystemFailure = () => { };

        /// <summary>
        /// The id of the system.
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// The name of the system. This is used in logging.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// A boolean that represents whether the system should stop or run.
        /// </summary>
        public bool SignalRunning
        {
            get;
            private set;
        }

        /// <summary>
        /// A boolean that represents wheter the system is running or not.
        /// </summary>
        public bool IsRunning
        {
            get;
            protected set;
        }

        /// <summary>
        /// This member is used when the system crashes (on exception). 
        /// </summary>
        /// See <see cref="DiagnosticsSystem.CatchException(object, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs)"/>
        public bool RestartOnCrash
        {
            get;
            protected set;
        }

        /// <summary>
        /// This method will raise an exception in the system thread causing it to stop.
        /// </summary>
        public void Abort()
        {
            SignalRunning = false;
            if (CurrentThread == null)
            {
                return;
            }
            if (CurrentThread.IsAlive)
            {
                CurrentThread.Abort();
            }
        }

        /// <summary>
        /// This method is called when the system is registered.
        /// </summary>
        /// <param name="id">The new id of the system</param>
        /// <param name="manager">The manager that registered the system</param>
        /// See <see cref="SystemsManager.RegisterSystem(SystemBase)"/>
        internal void SetupSystem(int id, SystemsManager manager)
        {
            Id = id;
            OnRegister(manager);
        }

        /// <summary>
        /// This method is called when the system is registered.
        /// </summary>
        /// <param name="manager">The manager that registered the system</param>
        /// See <see cref="SystemsManager.RegisterSystem(SystemBase)"/>
        protected virtual void OnRegister(SystemsManager manager) { }

        /// <summary>
        /// This method turns on the system in a separate thread.
        /// </summary>
        /// See <see cref="SystemsManager.RunAll"/>
        public void RunOnThread()
        {
            if (IsRunning && CurrentThread != null)
            {
                return;
            }
            else
            {
                TurnOff();
            }
            CurrentThread = new Thread(BeginSystemLoop);
            SignalRunning = true;
            CurrentThread.Name = Name;
            CurrentThread.Start();
        }

        /// <summary>
        /// Signals the system to restart by turning it off and on again.
        /// </summary>
        public void Restart()
        {
            TurnOff();
            RunOnThread();
        }

        /// <summary>
        /// Signal the system to turn off.
        /// </summary>
        public virtual void TurnOff()
        {
            SignalRunning = false;
            while (IsRunning) ;
            CurrentThread = null;
        }

        /// <summary>
        /// Private method that wraps exception handling.
        /// </summary>
        private void BeginSystemLoop()
        {
            try
            {
                IsRunning = true;
                TurnOn();
                IsRunning = false;
            } catch (Exception e)
            {
                OnSystemFailure.Invoke();
                IsRunning = false;
                try
                {
                    throw new SystemException(e, "", this);
                }
                catch
                {
                    if (RestartOnCrash)
                    {
                        Restart();
                    }
                }
            }
        }

        /// <summary>
        /// Signal the system to turn on.
        /// </summary>
        protected virtual void TurnOn() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns>A string that represents the system in the form of "System(Id=SystemId, Name=SystemName)".</returns>
        public override string ToString()
        {
            return $"System(Id={Id}, Name=\"{Name}\")";
        }
    }
}
