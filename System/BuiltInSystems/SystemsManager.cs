using System;
using System.Collections.Generic;

namespace GameCore.Systems
{

    /// <summary>
    /// <c>SystemManager</c> is a system that manages the other systems that are registered.
    /// </summary>
    /// See <see cref="SystemBase"/> to create a custom system.
    /// See <see cref="SystemsManager.RegisterSystem(SystemBase)"/> to register systems.
    public sealed class SystemsManager : SystemBase
    {
        /// <summary>
        /// This is a static member so you can get the last manager that was created.
        /// </summary>
        public static SystemsManager CurrentManager
        {
            get;
            private set;
        }

        /// <summary>
        /// This list contains all the registered systems.
        /// </summary>
        private readonly List<SystemBase> Systems = new List<SystemBase>();

        /// <summary>
        /// The application domain that created this <c>SystemManager</c>.
        /// </summary>
        internal AppDomain HostAppDomain
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a <c>new SystemManager</c> to manage your systems.
        /// </summary>
        /// <param name="host">The <c>AppDomain</c> object that represents the host domain that created this <c>SystemManager</c></param>
        public SystemsManager(AppDomain host)
        {
            HostAppDomain = host;
            CurrentManager = this;
            RegisterSystem(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Name => "Systems Manager";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void TurnOn()
        {
            if (SignalRunning)
            {
                RunAll();
            }
        }

        /// <summary>
        /// Register a system with this manager.
        /// </summary>
        /// <param name="system">The system to register</param>
        public void RegisterSystem(SystemBase system)
        {
            int systemId = Systems.Count;
            Systems.Add(system);
            system.SetupSystem(systemId, this);
        }

        /// <summary>
        /// This method turns on all the systems that are registered.
        /// </summary>
        public void RunAll()
        {
            foreach (SystemBase system in Systems)
            {
                system.RunOnThread();
            }
        }

        /// <summary>
        /// Get a registered system by its id.
        /// </summary>
        /// <param name="systemId">The id of the system</param>
        /// <returns>The <c>SystemBase</c> object with Id = id, or null if no system was found</returns>
        public SystemBase GetSystem(int systemId)
        {
            if (systemId < 0 || systemId >= Systems.Count)
            {
                return null;
            }
            return Systems[systemId];
        }

        /// <summary>
        /// Get a registered system by its name.
        /// </summary>
        /// <param name="name">The id of the system</param>
        /// <returns>The <c>SystemBase</c> object with Name = name, or null if no system was found</returns>
        public SystemBase GetSystem(string name)
        {
            foreach (SystemBase system in Systems)
            {
                if (system.Name == name)
                {
                    return system;
                }
            }
            return null;
        }
    }
}
