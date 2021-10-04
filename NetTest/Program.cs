using System.IO;
using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using GameCore.Net.Sync;
using GameCore.Net.Sync.Generators;
using GameCore.Net.Sync.Extensions;
using GameCore;

namespace NetTest
{
    class Program
    {
        [Flags]
        enum Target
        {
            None = 0,
            Shared = 0b001,
            Client = 0b010,
            Server = 0b100,
            NetCode = Client | Server,
            All = Shared | Client | Server
        }

        static void ReplaceReferences(MethodDefinition method, TypeDefinition type)
        {
            ModuleDefinition module = type.Module;

            if (method.Module == module) return;

            if (method.ReturnType.Module == module)
            {
                method.ReturnType = method.Module.ImportReference(module.GetType(method.ReturnType.FullName));
            }

            foreach (Instruction instruction in method.Body.Instructions)
            {
                if (instruction.Operand is MemberReference member)
                {
                    if (member.Module == null || member.Module == module)
                    {
                        var m = member.RelativeTo(type);
                        instruction.Operand = method.Module.ImportReference(m);
                    }
                }
            }
        }

        static void MoveItemsByAuthority<T>(Collection<T> items, Collection<T> client, Collection<T> server, Func<T, Target> predicate)
            where T : MemberReference, ICustomAttributeProvider
        {
            foreach ((T item, T c, T s) in new Zip<T, T, T>(items.Copy(), client.Copy(), server.Copy()))
            {
                Target target = predicate(item);

                bool isShared = (target & Target.Shared) == Target.Shared;

                if (!isShared)
                {
                    items.Remove(item);
                }

                if ((target & Target.Client) != Target.Client)
                {
                    client.Remove(c);
                }

                if ((target & Target.Server) != Target.Server)
                {
                    server.Remove(s);
                }
            }
        }

        static void MoveItemsByAuthority<T>(Collection<T> items, Collection<T> client, Collection<T> server) 
            where T : MemberReference, ICustomAttributeProvider
        {
            MoveItemsByAuthority(items, client, server, item =>
            {
                if (item.GetAttribute(typeof(AuthorityCodeAttribute)) is AuthorityCodeAttribute code)
                {
                    if (code.Authority == Authority.Server) return Target.Server;
                    if (code.Authority == Authority.Client) return Target.Client;
                }
                if (item.GetAttribute(typeof(SynchronizeAttribute)) != null)
                {
                    return Target.NetCode;
                }
                // todo: check if code references authority code

                if (item is MethodDefinition method)
                {
                    if (!method.IsStatic && method.DeclaringType.HasAttribute(typeof(SynchronizeClassAttribute)))
                        throw new Exception($"Shared method in a synchronized class must be static. ({method})");

                    foreach (Instruction instruction in method.Body.Instructions)
                    {
                        if (instruction.Operand is MethodDefinition methodDefinition)
                        {
                            if (methodDefinition.GetAttribute(typeof(AuthorityCodeAttribute)) is AuthorityCodeAttribute refCode)
                            {
                                throw new Exception(
                                    "Shared method can't reference authority item.\n" +
                                    $"{method} references {methodDefinition} with authority {refCode.Authority}");
                            }

                            if (methodDefinition.GetAttribute(typeof(SynchronizeCallAttribute)) is SynchronizeCallAttribute sync)
                            {
                                throw new Exception(
                                    "Shared method can't reference synced item.\n" +
                                    $"{method} references {methodDefinition} with authority {sync.Authority}");
                            }
                        }
                    }
                }

                return Target.Shared;
            });
        }

        static void MoveItemsByAuthority(TypeDefinition source, TypeDefinition client, TypeDefinition server)
        {
            MoveItemsByAuthority(source.NestedTypes, client.NestedTypes, server.NestedTypes);
            MoveItemsByAuthority(source.Fields, client.Fields, server.Fields);
            MoveItemsByAuthority(source.Properties, client.Properties, server.Properties);
            MoveItemsByAuthority(source.Methods, client.Methods, server.Methods);
        }

        static void MoveItemsByAuthority(AssemblyDefinition source, AssemblyDefinition client, AssemblyDefinition server)
        {
            foreach (TypeDefinition type in source.MainModule.Types)
            {
                if (type.Name == "<Module>") continue;

                TypeDefinition
                    clientType = client.MainModule.GetType(type.Namespace, type.Name),
                    serverType = server.MainModule.GetType(type.Namespace, type.Name);
                //TypeDefinition
                //    clientType = new TypeDefinition(type.Namespace, type.Name, type.Attributes, type.BaseType),
                //    serverType = new TypeDefinition(type.Namespace, type.Name, type.Attributes, type.BaseType);
                //client.MainModule.Types.Add(clientType);
                //server.MainModule.Types.Add(serverType);

                //type.Interfaces.CopyTo(clientType.Interfaces, i =>
                //{
                //    client.MainModule.ImportReference(i.InterfaceType);
                //    return i;
                //});
                //type.Interfaces.CopyTo(serverType.Interfaces, i =>
                //{
                //    client.MainModule.ImportReference(i.InterfaceType);
                //    return i;
                //});

                MoveItemsByAuthority(type, clientType, serverType);
            }
        }

        static void MakeSyncedWithShared(string inputPath, string outputPath)
        {
            Version version;
            ModuleKind moduleKind;

            using (Stream s = File.Open(inputPath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(s))
                {
                    version = assemblyDefinition.Name.Version;
                    moduleKind = assemblyDefinition.MainModule.Kind;

                    AssemblyProcessor.ProcessAssembly(assemblyDefinition);
                    assemblyDefinition.Write();
                }
            }

            string targetPath = string.Format(outputPath, "Shared");
            string clientPath = string.Format(outputPath, "Client");
            string serverPath = string.Format(outputPath, "Server");

            Assembly assembly = new Assembly(inputPath);

            assembly.Repack(targetPath);

            assembly.Repack(clientPath);
            assembly.Repack(serverPath);

            using (AssemblyDefinition
                    sourceAssembly = AssemblyDefinition.ReadAssembly(targetPath, new ReaderParameters() { ReadWrite = true }),
                    clientAssembly = AssemblyDefinition.ReadAssembly(clientPath, new ReaderParameters() { ReadWrite = true }),
                    serverAssembly = AssemblyDefinition.ReadAssembly(serverPath, new ReaderParameters() { ReadWrite = true })
                    )
            {
                MoveItemsByAuthority(sourceAssembly, clientAssembly, serverAssembly);

                ModuleDefinition sharedModule = sourceAssembly.MainModule;

                foreach (TypeDefinition type in clientAssembly.MainModule.Types)
                {
                    foreach (MethodDefinition method in type.Methods)
                    {
                        ReplaceReferences(method, sharedModule.GetType(type.FullName));
                    }
                }

                foreach (TypeDefinition type in serverAssembly.MainModule.Types)
                {
                    foreach (MethodDefinition method in type.Methods)
                    {
                        ReplaceReferences(method, sharedModule.GetType(type.FullName));
                    }
                }

                sourceAssembly.Write();

                AssemblyGenerator clientGenerator = new AssemblyGenerator(clientAssembly);
                AssemblyGenerator serverGenerator = new AssemblyGenerator(serverAssembly);

                clientGenerator.MakeSynced(Authority.Client);
                serverGenerator.MakeSynced(Authority.Server);

                clientAssembly.Write();
                serverAssembly.Write();
            }
        }

        static void MakeSyncedWithoutShared(string inputPath, string outputPath)
        {
            string clientPath = string.Format(outputPath, "Client");
            string serverPath = string.Format(outputPath, "Server");

            using (AssemblyDefinition 
                assemblyDefinition = AssemblyDefinition.ReadAssembly(inputPath, new ReaderParameters() { ReadWrite = true })
                )
            {
                AssemblyProcessor.ProcessAssembly(assemblyDefinition);
                assemblyDefinition.Write();
            }

            Assembly assembly = new Assembly(inputPath);

            assembly.Repack(clientPath);
            assembly.Repack(serverPath);

            using (AssemblyDefinition
                clientAssembly = AssemblyDefinition.ReadAssembly(clientPath, new ReaderParameters() { ReadWrite = true }),
                serverAssembly = AssemblyDefinition.ReadAssembly(serverPath, new ReaderParameters() { ReadWrite = true })
                )
            {
                AssemblyGenerator clientGenerator = new AssemblyGenerator(clientAssembly);
                AssemblyGenerator serverGenerator = new AssemblyGenerator(serverAssembly);

                clientGenerator.MakeSynced(Authority.Client);
                serverGenerator.MakeSynced(Authority.Server);

                clientAssembly.Write();
                serverAssembly.Write();
            }
        }

        static void Main()
        {
            // todo: make shared only if static method

            bool shared = true;
            string path = shared ? "Generated-Shared/" : "Generated-NotShared";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "NetTest.Generated.dll");
            string targetPath = Path.Combine(Directory.GetCurrentDirectory(), path, "NetTest.Generated.{0}.dll");

            if (shared)
            {
                MakeSyncedWithShared(assemblyPath, targetPath);
            } else
            {
                MakeSyncedWithoutShared(assemblyPath, targetPath);
            }
        }
    }
}
