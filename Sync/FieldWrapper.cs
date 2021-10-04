using System.Reflection;

namespace GameCore.Net
{
    public class FieldWrapper : ISettable
    {
        private readonly FieldInfo m_field;

        private readonly object m_target;

        public bool Set(object value)
        {
            m_field.SetValue(m_target, value);
            return true;
        }
    }
}
