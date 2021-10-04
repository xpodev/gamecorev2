using System;

namespace GameCore.Net.Wrappers
{
    public class DelegateWrapper : IInvocable
    {
        private readonly Delegate m_delegate;

        public DelegateWrapper(Delegate del)
        {
            m_delegate = del;
        }

        public bool Invoke(object[] args)
        {
            m_delegate.Method.Invoke(m_delegate.Target, args);
            return true;
        }
    }
}
