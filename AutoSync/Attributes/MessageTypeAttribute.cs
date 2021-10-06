using System;

namespace GameCore.Net.Sync
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageTypeAttribute : CustomFunctionCallAttribute
    {
        /// <summary>
        /// Doesn't have to be the real construtor. This is the function that gets called in order
        /// to create a message object. The method with the specified name must return the same type
        /// as passed to the constructor.
        /// </summary>
        public string ConstructorName { get; }

        /// <summary>
        /// The name of the property (or method) that returns the id of the message. default is <c>Id</c>
        /// </summary>
        public string IdPropertyName { get; }

        public MessageTypeAttribute(params object[] args) : base(args.Length == 0 ? new object[] { "Id" } : args)
        {

        }
    }
}
