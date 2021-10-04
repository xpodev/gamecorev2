using System;
using System.IO;
using GameCore;
using GameCore.Systems;
using GameCore.ModLoader;


namespace SystemT
{
    class Program
    {
        //static SystemsManager manager;
        //private static ModdingSystem moddingSystem;

        static void Main()
        {
            Console.WriteLine(NetTest.Generated.SyncThis.NetworkUpdateRate);
            //manager = new SystemsManager(AppDomain.CurrentDomain);
            //moddingSystem = new ModdingSystem(Directory.GetCurrentDirectory());

            ////manager.RegisterSystem(mdpSystem);
            //manager.RegisterSystem(moddingSystem);

            //while (true)
            //{
            //    string cmd = Console.ReadLine();
            //    switch (cmd.ToLower())
            //    {
            //        case "exit":
            //            return;
            //        case "load":
            //            Load();
            //            break;
            //        case "run":
            //            Run();
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        //static void Run()
        //{
        //    string type = Console.ReadLine();
        //    switch (type.ToLower())
        //    {
        //        case "all":
        //            manager.RunAll();
        //            break;
        //        default:
        //            return;
        //    }
        //}

        //static void Load()
        //{
        //    string type = Console.ReadLine();
        //    switch (type.ToLower())
        //    {
        //        case "mods":
        //            moddingSystem.RunOnThread();
        //            break;
        //        default:
        //            return;
        //    }
        //}
    }
}
