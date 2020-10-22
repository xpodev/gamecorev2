using UE = UnityEngine;


namespace PythonEnvironment
{
    public static class Debug
    {
        public static void Log(object message)
        {
            UE.Debug.Log(message);
        }

        public static void Log(object message, UE.Object context)
        {
            UE.Debug.Log(message, context);
        }

        public static void LogWarning(object message)
        {
            UE.Debug.LogWarning(message);
        }

        public static void LogWarning(object message, UE.Object context)
        {
            UE.Debug.LogWarning(message, context);
        }

        public static void LogError(object message)
        {
            UE.Debug.LogError(message);
        }

        public static void LogError(object message, UE.Object context)
        {
            UE.Debug.LogError(message, context);
        }
    }
}
