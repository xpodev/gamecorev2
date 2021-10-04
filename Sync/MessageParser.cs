namespace GameCore.Net
{
    public class MessageParser<T> where T : struct
    {
        private CommandExecutor<T> m_executor;

        public CommandExecutor<T> Executor
        {
            get => m_executor;
            set => m_executor = value;
        }

        public MessageParser(CommandExecutor<T> executor)
        {
            m_executor = executor;
        }

        public void Parse(ISyncMessage<T> message)
        {
            for (int i = 0, bitIndex = 7; bitIndex >= 0 && i < message.ArgumentIds.Length; bitIndex--, i++)
            {
                if (!m_executor.Execute(message.ArgumentIds[i]))
                {
                    // TODO: maybe throw an error since that means the id was not valid for current command
                }
            }
        }
    }
}
