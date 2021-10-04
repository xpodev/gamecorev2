namespace GameCore.Net
{
    public class Command<T> where T : struct
    {
        public CommandType m_opType;

        public T m_operandId;

        public Command(CommandType type, T operand)
        {
            m_opType = type;
            m_operandId = operand;
        }
    }
}
