using System;
using System.Net.Sockets;


namespace GameCore.Net
{
    /// <summary>
    /// This class acts as a wrapper for the builtin socket, and implements the <c>GameCore.Net.IAsyncByteIO</c>
    /// </summary>
    public sealed class AsyncSocket : Socket, IAsyncByteIO
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="socketType"><inheritdoc/></param>
        /// <param name="protocolType"><inheritdoc/></param>
        public AsyncSocket(SocketType socketType, ProtocolType protocolType) : base(socketType, protocolType) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="addressFamily"><inheritdoc/></param>
        /// <param name="socketType"><inheritdoc/></param>
        /// <param name="protocolType"><inheritdoc/></param>
        public AsyncSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"><inheritdoc/></param>
        /// <param name="offset"><inheritdoc/></param>
        /// <param name="size"><inheritdoc/></param>
        /// <param name="callback"><inheritdoc/></param>
        public void ReadAsync(byte[] buffer, int offset, int size, Action<int> callback)
        {
            BeginReceive(buffer, offset, size, SocketFlags.None, (result) =>
            {
                callback?.Invoke(EndReceive(result));
            }, null);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"><inheritdoc/></param>
        /// <param name="offset"><inheritdoc/></param>
        /// <param name="size"><inheritdoc/></param>
        /// <param name="callback"><inheritdoc/></param>
        public void WriteAsync(byte[] buffer, int offset, int size, Action<int> callback)
        {
            BeginSend(buffer, offset, size, SocketFlags.None, (result) =>
            {
                callback?.Invoke(EndSend(result));
            }, null);
        }
    }
}
