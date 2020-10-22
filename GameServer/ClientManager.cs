using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    using SerialType = Int64;

    public class ClientManager
    {
        private readonly Dictionary<SerialType, ClientHandler> clients = new Dictionary<SerialType, ClientHandler>();

        public static ClientManager CurrentManager
        {
            get;
            private set;
        }

        public ClientManager()
        {
            CurrentManager = this;
        }

        public ClientHandler AddClient(ClientInfo info)
        {
            info.SetSerial(GenerateUniqueSerial(info));
            ClientHandler handler = new ClientHandler(info);
            if (!info.IsValid())
            {
                throw new Exception($"Error: client joined, but SerialGenerator returned an invalid serial: {info.Serial} ({info.ClientEndPoint.Address}:{info.ClientEndPoint.Port})");
            }
            if (clients.ContainsKey(info.Serial))
            {
                throw new Exception($"A client tried to join with duplicate serial: {info.Serial}");
            }
            clients.Add(info.Serial, handler);
            return handler;
        }

        public void RemoveClient(ClientInfo info)
        {
            clients.Remove(info.Serial);
        }

        private static SerialType GenerateUniqueSerial(ClientInfo info)
        {
            byte[] serial = new byte[8];
            IPEndPoint ep = info.ClientEndPoint;
            byte[] ipBytes = ep.Address.GetAddressBytes();
            byte[] portBytes = BitConverter.GetBytes(ep.Port);
            if (ipBytes.Length != portBytes.Length)
            {
                throw new Exception("Ip was not 4 bytes");
            }
            for (int i = 0; i < 4; i++)
            {
                serial[i] = (byte)(ipBytes[i] + portBytes[i]);
                serial[i + 4] = (byte)(ipBytes[i] * portBytes[i]);
            }
            return BitConverter.ToInt64(serial, 0);
        }
    }
}
