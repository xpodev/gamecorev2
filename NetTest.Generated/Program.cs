//#define CUSTOM_CONDITION_TEST
//#define AUTO_SYNC_TEST

using System;
using GameCore.Net;
using GameCore.Net.Sync;

[assembly: NetworkConfig(typeof(NetTest.Generated.NetworkManager))]

namespace NetTest.Generated
{
    [SynchronizeClass(MaxUpdateRate = 10)]
    public class SyncThis
    {
        //const string a = "MyString";

        //// set is invalid because it tries to call client-only method
        //[SetFromClient]
        //public int ANumber { get => 0; set => Something(); }

        //[ServerSide]
        //public void Something() { }

        //[CustomFunctionCall("Object", null)]
        //[TypeSerializer(typeof(string), Strict = true)]
        //static int SerializeObjectA<T>(T a, string b)
        //{
        //    return 0;
        //}

        //[TypeSerializer(typeof(object))]
        //static int SerializeObjectB(object a)
        //{
        //    return 0;
        //}

        [RunOnServer]
        public void DoOnServer_CallFromClient()
        {
            Console.WriteLine("This is called from the client and runs on the server");
            ClientLog0();
            ClientLog1();
            ClientLog2();
            ClientLog3();
        }

        [ServerSide]
        public void DoOnServerOnly()
        {
            Console.WriteLine("This runs on the server");
            WriteWorks();
        }

        [RunOnClient]
        public static void ClientLog0()
        {
            Console.WriteLine("WriteOnClient0: nothing yet");
        }

        [RunOnClient]
        public static bool ClientLog1()
        {
            Console.WriteLine("WriteOnClient1: nothing yet");
            return true;
        }

        [RunOnClient]
        public static int ClientLog2()
        {
            Console.WriteLine("WriteOnClient2: nothing yet");
            return 15;
        }

        [RunOnClient]
        public static void ClientLog3()
        {
            Console.WriteLine("WriteOnClient3: nothing yet");
        }

        public static void WriteWorks()
        {
            Console.WriteLine("Works!");
        }

        #region AUTO_SYNC_TEST

#if AUTO_SYNC_TEST

[ClientSide]
        [SetFromServer]
        public int Client_X { get; set; }

        [ClientSide]
        [SetFromServer]
        public int Client_SetOnly
        {
            set
            {
                Console.WriteLine("Called from server: this is common code");
            }
        }

        [ServerSide]
        [SetFromClient]
        public int Server_X { get; set; }

        [SetFromClient]
        public int Server_SetOnly
        {
            set
            {
                Console.WriteLine("Called from server: this is common code");
            }
        }

        [SetFromServer(false)]
        public int ClientOnly_X { get; set; }

        [SetFromServer(false)]
        public int ClientOnly_SetOnly
        {
            set
            {
                Console.WriteLine("Called from server: this is client code");
            }
        }

        [SetFromClient(false)]
        public int ServerOnly_X { get; set; }

        [ServerSide]
        [SetFromClient(false)]
        public int ServerOnly_SetOnly
        {
            set
            {
                Console.WriteLine("Called from server: this is server code");
            }
        }

        [RunOnClient]
        public void Client_OnlyClient()
        {
            Console.WriteLine("Called from server: runs on client only");
        }

        [ServerSide]
        [RunOnServer]
        public void Server_OnlyServer()
        {
            Console.WriteLine("Called from client: runs on server only");
        }

        [RunOnClient(true)]
        public void Client_Both()
        {
            Console.WriteLine("Called from server: runs on both server & client");
        }

        [RunOnServer]
        public void Server_Both()
        {
            Console.WriteLine("Called from client: runs on both server & client");
        }

#endif

        #endregion

        #region CUSTOM_CONDITION_TEST
#if CUSTOM_CONDITION_TEST

        [SetFromServer]
        private int Condition_None { get; set; }

        [SetFromServer(ConditionFunction = nameof(Cond_This))]
        private int Condition_This { get; set; }

        [SetFromServer(ConditionFunction = nameof(Cond_ThisNew))]
        private int Condition_ThisNew { get; set; }

        [SetFromServer(ConditionFunction = nameof(Cond_ThisNewOld))]
        private int Condition_ThisNewOld { get; set; }

        [SetFromServer(ConditionFunction = nameof(SCond_This))]
        private int SCondition_This { get; set; }

        [SetFromServer(ConditionFunction = nameof(SCond_ThisNew))]
        private int SCondition_ThisNew { get; set; }

        [SetFromServer(ConditionFunction = nameof(SCond_ThisNewOld))]
        private int SCondition_ThisNewOld { get; set; }

        private bool Cond_This()
        {
            return this != null;
        }

        private bool Cond_ThisNew(int n)
        {
            return n != 0 && this != null;
        }

        private bool Cond_ThisNewOld<T>(T n, T o)
        {
            return !n.Equals(o) && this != null;
        }

        private static bool SCond_This()
        {
            return true;
        }

        private static bool SCond_ThisNew(ref int n)
        {
            return n != 0;
        }

        private static bool SCond_ThisNewOld(in int n, ref int o)
        {
            return n != o;
        }

#endif

        #endregion
    }

    [MessageType("Id")]
    public class CustomMessage : Message<int>
    {
        public CustomMessage(int id) : base(id)
        {

        }
    }

    [NetworkManager(typeof(CustomMessage), MessageSenderName = "SendMessage")]
    public class NetworkManager
    {
        [CustomFunctionCall("Message")]
        public static void SendMessage(CustomMessage myMessage)
        {
            Console.WriteLine("Sending message: " + myMessage.ToString());
        }
    }

    class Program
    {
        //static int Main(string[] args)
        //{
        //    return 0;
        //}
    }
}
