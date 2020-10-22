using Microsoft.Scripting.Hosting;


namespace GameCore.Scripting.Python
{
    public class PythonScripting
    {
        public ScriptEngine PythonEnvironment
        {
            get;
            private set;
        }

        public ScriptScope PythonScriptScope
        {
            get;
            private set;
        }

        public PythonScripting()
        {
            ResetEnvironment();
        }

        public void SetupEnvironment()
        {
            PythonScriptScope = PythonEnvironment.CreateScope();
        }

        public void ResetEnvironment()
        {
            PythonScriptScope = null;
            PythonEnvironment = IronPython.Hosting.Python.CreateEngine();
        }

        public void Execute(string source)
        {
            ScriptSource script = PythonEnvironment.CreateScriptSourceFromString(source);
            script.Execute(PythonScriptScope);
        }
    }
}
