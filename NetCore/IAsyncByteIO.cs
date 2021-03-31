using System;
using System.Net;


namespace GameCore.Net
{
    /// <summary>
    /// A common interface for asynchonous <c>byte[]</c> IO.
    /// </summary>
    public interface IAsyncByteIO
    {
        /// <summary>
        /// The local end point.
        /// </summary>
        EndPoint LocalEndPoint
        {
            get;
        }

        /// <summary>
        /// The remote end point.
        /// </summary>
        EndPoint RemoteEndPoint
        {
            get;
        }

        /// <summary>
        /// A boolean that tells whether the IO is open and can be used for reading/writing data.
        /// </summary>
        bool Connected
        { 
            get;
        }

        /// <summary>
        /// Asynchronosuly read data of max <c>count</c> bytes into the buffer, starting from the <c>offset</c>.
        /// </summary>
        /// <param name="buffer">The buffer to write the received data into.</param>
        /// <param name="offset">The offset to start writing data from.</param>
        /// <param name="count">The max amount of bytes to read.</param>
        /// <param name="callback">A callback to call after the data was read.</param>
        void ReadAsync(byte[] buffer, int offset, int count, Action<int> callback);

        /// <summary>
        /// Asynchronosuly write <c>count</c> bytes from the buffer, starting from the <c>offset</c>.
        /// </summary>
        /// <param name="buffer">The buffer to read the data to send.</param>
        /// <param name="offset">The offset to start reading data from.</param>
        /// <param name="count">The amount of bytes to write.</param>
        /// <param name="callback">A callback to call after the data was sent.</param>
        void WriteAsync(byte[] buffer, int offset, int count, Action<int> callback);
    }
}
