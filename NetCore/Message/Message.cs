using System.IO;
using GameCore.Serialization;
using GameCore.Net.Sync;


namespace GameCore.Net
{
    /// <summary>
    /// This class represents a message object. It implements the <c>GameCore.Net.IMessage&lt;<typeparamref name="T"/>></c> interface, so
    /// it can be used with <c>GameCore.Net.Connection</c> for sending and receiving messages.
    /// </summary>
    /// <typeparam name="T">An <c>enum</c> that specifies all messages types.</typeparam>
    public class Message<T> : IMessage<T> where T : struct//, Enum
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public T Id { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public uint Length
        {
            get
            {
                return (uint)buffer.Length;
            }
        }

        /// <summary>
        /// Internal buffer holds the message bytes.
        /// </summary>
        private readonly MemoryStream buffer;

        /// <summary>
        /// Internal reader for easily reading primitives.
        /// </summary>
        private readonly BinaryReader reader;

        /// <summary>
        /// Internal writer for easily writing primitives.
        /// </summary>
        private readonly BinaryWriter writer;

        /// <summary>
        /// Constructs a new message ready for reading/writing data.
        /// </summary>
        public Message()
        {
            buffer = new MemoryStream();
            reader = new BinaryReader(buffer);
            writer = new BinaryWriter(buffer);
        }

        /// <summary>
        /// Constructs a new message ready for reading/writing data.
        /// </summary>
        /// <param name="id">The id to initialize the message with</param>
        public Message(T id) : this()
        {
            Id = id;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="bytes"><inheritdoc/></param>
        public void FromBytes(byte[] bytes)
        {
            buffer.Seek(0, SeekOrigin.Begin);
            buffer.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public byte[] ToBytes()
        {
            return buffer.ToArray();
        }

        /// <summary>
        /// Insert a <c>byte[]</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(byte[]), Direct = true, Strict= true)]
        public Message<T> Insert(byte[] data)
        {
            buffer.Write(data, 0, data.Length);
            return this;
        }

        /// <summary>
        /// Insert a <c>bool</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(bool), Direct = true, Strict = true)]
        public Message<T> Insert(bool data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert a <c>char</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(char), Direct = true, Strict = true)]
        public Message<T> Insert(char data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert a <c>byte</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(byte), Direct = true, Strict = true)]
        public Message<T> Insert(byte data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert a <c>sbyte</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(sbyte), Direct = true, Strict = true)]
        public Message<T> Insert(sbyte data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert a <c>short</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(short), Direct = true, Strict = true)]
        public Message<T> Insert(short data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert a <c>ushort</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(ushort), Direct = true, Strict = true)]
        public Message<T> Insert(ushort data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert an <c>int</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(int), Direct = true, Strict = true)]
        public Message<T> Insert(int data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert a <c>uint</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(uint), Direct = true, Strict = true)]
        public Message<T> Insert(uint data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert a <c>long</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(long), Direct = true, Strict = true)]
        public Message<T> Insert(long data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert a <c>ulong</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(ulong), Direct = true, Strict = true)]
        public Message<T> Insert(ulong data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert a <c>float</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(float), Direct = true, Strict = true)]
        public Message<T> Insert(float data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert a <c>double</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(double), Direct = true, Strict = true)]
        public Message<T> Insert(double data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert a <c>string</c> to the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        [TypeSerializer(typeof(string), Direct = true, Strict = true)]
        public Message<T> Insert(string data)
        {
            writer.Write(data);
            return this;
        }

        /// <summary>
        /// Insert an <c>ISerialiazble&lt;Message&lt;T>></c> object to the message.
        /// </summary>
        /// <param name="obj">The object to insert.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Insert(ISerializable<Message<T>> obj)
        {
            obj.SerializeTo(this);
            return this;
        }

        /// <summary>
        /// Extracts bytes to a <c>byte[]</c>.
        /// </summary>
        /// <param name="buffer">The buffer to extract into.</param>
        /// <param name="offset">The offset in the buffer to start writing from.</param>
        /// <param name="count">The max amount of bytes to extract.</param>
        /// <returns><c>this</c> message.</returns>
        //[TypeDeserializer(typeof(byte[]), Direct = true, Strict = true)]
        public Message<T> Extract(byte[] buffer, int offset, int count)
        {
            reader.Read(buffer, offset, count);
            return this;
        }

        /// <summary>
        /// Extracts a <c>bool</c> from the message.
        /// </summary>
        /// <param name="data">The data to extract into.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out bool data)
        {
            data = reader.ReadBoolean();
            return this;
        }

        /// <summary>
        /// Extracts a <c>char</c> from the message.
        /// </summary>
        /// <param name="data">The data to extract into.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out char data)
        {
            data = reader.ReadChar();
            return this;
        }

        /// <summary>
        /// Extracts a <c>byte</c> from the message.
        /// </summary>
        /// <param name="data">The data to extract into.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out byte data)
        {
            data = reader.ReadByte();
            return this;
        }

        /// <summary>
        /// Extracts a <c>sbyte</c> from the message.
        /// </summary>
        /// <param name="data">The data to extract into.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out sbyte data)
        {
            data = reader.ReadSByte();
            return this;
        }

        /// <summary>
        /// Extracts a <c>short</c> from the message.
        /// </summary>
        /// <param name="data">The data to extract into.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out short data)
        {
            data = reader.ReadInt16();
            return this;
        }

        /// <summary>
        /// Extracts a <c>ushort</c> from the message.
        /// </summary>
        /// <param name="data">The data to extract into.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out ushort data)
        {
            data = reader.ReadUInt16();
            return this;
        }

        /// <summary>
        /// Extracts an <c>int</c> from the message.
        /// </summary>
        /// <param name="data">The data to extract into.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out int data)
        {
            data = reader.ReadInt32();
            return this;
        }

        /// <summary>
        /// Extracts a <c>uint</c> from the message.
        /// </summary>
        /// <param name="data">The data to extract into.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out uint data)
        {
            data = reader.ReadUInt32();
            return this;
        }

        /// <summary>
        /// Extracts a <c>long</c> from the message.
        /// </summary>
        /// <param name="data">The data to extract into.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out long data)
        {
            data = reader.ReadInt64();
            return this;
        }

        /// <summary>
        /// Extracts a <c>ulong</c> from the message.
        /// </summary>
        /// <param name="data">The data to extract into.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out ulong data)
        {
            data = reader.ReadUInt64();
            return this;
        }

        /// <summary>
        /// Extracts a <c>float</c> from the message.
        /// </summary>
        /// <param name="data">The data to extract into.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out float data)
        {
            data = reader.ReadSingle();
            return this;
        }

        /// <summary>
        /// Extracts a <c>double</c> from the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out double data)
        {
            data = reader.ReadDouble();
            return this;
        }

        /// <summary>
        /// Extracts a <c>string</c> from the message.
        /// </summary>
        /// <param name="data">The data to insert.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(out string data)
        {
            data = reader.ReadString();
            return this;
        }

        /// <summary>
        /// Extracts a <c>ISerializable&lt;byte[]></c> object from the message.
        /// </summary>
        /// <param name="obj">The object to extract into.</param>
        /// <returns><c>this</c> message.</returns>
        public Message<T> Extract(ISerializable<Message<T>> obj)
        {
            obj.DeserializeFrom(this);
            return this;
        }
    }
}
