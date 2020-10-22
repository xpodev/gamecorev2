using System;
using System.Collections.Generic;
using System.Net;


namespace GameServer
{
    public class ClientManager
    {
        private readonly Dictionary<long, ClientInfo> clients = new Dictionary<long, ClientInfo>();

        public static ClientManager CurrentManager
        {
            get;
            private set;
        }

        internal ClientManager()
        {
            CurrentManager = this;
        }

        public void AddClient(ClientInfo info)
        {
            info.SetSerial(GenerateUniqueSerial(info));
            if (!info.IsValid())
            {
                throw new Exception($"Error: client joined, but SerialGenerator returned an invalid serial: {info.Serial} ({info.ClientEndPoint.Address}:{info.ClientEndPoint.Port})");
            }
            if (clients.ContainsKey(info.Serial))
            {
                throw new Exception($"A client tried to join with duplicate serial: {info.Serial}");
            }
            clients.Add(info.Serial, info);
        }

        public void RemoveClient(ClientInfo info)
        {
            clients.Remove(info.Serial);
        }

        private static long GenerateUniqueSerial(ClientInfo info)
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
