using System;


namespace GameCore.Net
{
    /// <summary>
    /// A common interface for messages that can be sent by a <c>GameCore.Net.Connection</c> object.
    /// </summary>
    /// <typeparam name="T">An <c>enum</c> that specifies all messages types.</typeparam>
    public interface IMessage<T> where T : struct, Enum
    {
        /// <summary>
        /// The type of the message.
        /// </summary>
        T Id
        {
            get;
            set;
        }

        /// <summary>
        /// The length of the body of the message.
        /// </summary>
        uint Length
        {
            get;
        }

        /// <summary>
        /// Get the bytes that represents the message contents.
        /// </summary>
        /// <returns>A <c>byte[]</c> that represents the whole message.</returns>
        byte[] ToBytes();

        /// <summary>
        /// Load the message data from the byte array.
        /// </summary>
        /// <param name="bytes"><c>byte[]</c> that represents a message object.</param>
        void FromBytes(byte[] bytes);
    }
}
