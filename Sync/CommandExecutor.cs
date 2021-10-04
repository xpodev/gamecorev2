using System.Collections.Generic;

namespace GameCore.Net
{
    public class CommandExecutor<T> where T : struct
    {
        public bool Execute(T id)
        {
            return true;
            //switch (command)
            //{
            //    case CommandType.Set:
            //        if (m_settables.TryGetValue(id, out ISettable settable))
            //        {
            //            // TODO: fix
            //            object value = default;
            //            return settable.Set(value);
            //        }
            //        break;
            //    case CommandType.Invoke:
            //        if (m_invocables.TryGetValue(id, out IInvocable invocable))
            //        {
            //            // TODO: fix
            //            object[] args = default;
            //            return invocable.Invoke(args);
            //        }
            //        break;
            //    default:
            //        break;
            //}
            //return false;
        }
    }
}
