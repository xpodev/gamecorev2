using System;
using System.IO;

using GameCore.Systems;
using GameCore.Mdp;

namespace SystemT
{
    class MdpSystem : SystemBase
    {
        public override string Name => "Mdp System";

        public readonly MdpClient MdpClient = new MdpClient();

        protected override void TurnOn()
        {
            MdpClient.Run(Directory.GetCurrentDirectory());
        }
    }
}
