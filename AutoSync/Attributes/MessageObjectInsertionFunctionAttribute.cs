using System;

namespace GameCore.Net.Sync
{
    /// <summary>
    /// Put this attribute on a NetworkConfig class, the Message type or on the method itself
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class MessageObjectInsertionFunctionAttribute : CustomFunctionCallAttribute
    {
        public string MethodName { get; set; }

        /// <summary>
        /// <list type="table">
        /// <listheader>
        /// <term>Argument Name</term>
        /// <term>Description</term>
        /// </listheader>
        /// <item>
        /// <term>Id</term>
        /// <term>the id of the object to update</term>
        /// </item>
        /// <item>
        /// <term>Object</term>
        /// <term>the object to insert</term>
        /// </item>
        /// <item>
        /// <term>IsReliable</term>
        /// <term>whether or not the message is reliable</term>
        /// </item>
        /// <item>
        /// <term>Priority</term>
        /// <term>the priority of the message</term>
        /// </item>
        /// <item>
        /// <term>CommandType</term>
        /// <term>the type of the command</term>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="args">The argument to pass to the function, in order</param>
        public MessageObjectInsertionFunctionAttribute(params object[] args) : base(args.Length == 0 ? new object[] { "Object" } : args)
        {

        }
    }
}
