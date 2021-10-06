using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using GameCore.Net.Sync;
using GameCore.Net.Sync.Processors;
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
                    if (method.IsConstructor)
                    {
                        return Target.All;
                    }
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

        static void MakeSynced(string assemblyPath, SynchronizationSettings settings, Authority authority)
        {
            using (Stream assemblyStream = File.Open(assemblyPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyStream))
                {
                    settings.NetworkManager = GetNetworkManagerTypeFrom(assembly);
                    settings.MessageSettings = GetMessageSettingsFrom(settings.NetworkManager);

                    settings.MessageSettings.MessageConstructor = settings.NetworkManager.Module.ImportReference(settings.MessageSettings.MessageConstructor);
                    settings.MessageSettings.MessageType = settings.NetworkManager.Module.ImportReference(settings.MessageSettings.MessageType);
                    settings.MessageSettings.MessageIDGetter = settings.NetworkManager.Module.ImportReference(settings.MessageSettings.MessageIDGetter);
                    settings.MessageSettings.MessageSenderMethod = settings.NetworkManager.Module.ImportReference(settings.MessageSettings.MessageSenderMethod);

                    settings.Authority = authority;
                    if (!new AssemblyProcessor(assembly).Process(settings))
                    {
                        Console.WriteLine($"{authority} assembly synchronization failed.");
                    }
                    else
                        Console.WriteLine($"{authority} assembly synchronization succeeded.");

                    assembly.Write();
                }
            }
        }

        static void PrepareAssembly(string inputPath)
        {
            using (AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(inputPath, new ReaderParameters() { ReadWrite = true }))
            {
                AssemblyPreProcessor.ProcessAssembly(assemblyDefinition);
                assemblyDefinition.Write();
            }
        }

        static (string, string, string) PrepareSyncWithShared(string inputPath, string outputPath)
        {
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
            }

            return (clientPath, serverPath, targetPath);
        }

        static (string, string, string) PrepareSyncWithoutShared(string inputPath, string outputPath)
        {
            string clientPath = string.Format(outputPath, "Client");
            string serverPath = string.Format(outputPath, "Server");

            Assembly assembly = new Assembly(inputPath);

            assembly.Repack(clientPath);
            assembly.Repack(serverPath);

            return (clientPath, serverPath, null);
        }

        static TypeDefinition GetNetworkManagerTypeFrom(AssemblyDefinition assembly)
        {
            if (assembly.GetCustomAttribute(typeof(NetworkConfigAttribute)) is CustomAttribute configAttribute)
            {
                TypeDefinition networkConfigurationType = (configAttribute.ConstructorArguments[0].Value as TypeReference).Resolve();
                if (networkConfigurationType.GetCustomAttribute(typeof(NetworkManagerAttribute)) is CustomAttribute networkManagerAttribute)
                {
                    return networkConfigurationType;
                }
            }
            return null;
        }

        static MessageSettings DefaultSettingsFor(TypeDefinition messageType)
        {
            return GetMessageSettingsFrom(messageType, ".ctor", "Id");
        }

        static MessageSettings ReadMessageSettingsFrom(TypeDefinition messageType)
        {
            if (messageType.GetAttribute(typeof(MessageTypeAttribute)) is MessageTypeAttribute messageSettings)
            {
                TypeReference messageRedirectedType = null;
                if (messageType.GetCustomAttribute(typeof(ForwardedMessageTypeAttribute)) is CustomAttribute forwardedMessageType)
                {
                    messageRedirectedType = forwardedMessageType.ConstructorArguments[0].Value as TypeReference;
                }
                return GetMessageSettingsFrom(
                    messageType, 
                    messageSettings.ConstructorName ?? ".ctor", 
                    messageSettings.IdPropertyName ?? "Id", 
                    messageRedirectedType
                    );
            }
            return DefaultSettingsFor(messageType);
        }

        static MessageSettings GetMessageSettingsFrom(TypeDefinition configType, string constructorName, string idPropertyName, TypeReference messageType = null)
        {
            messageType = messageType ?? configType;

            MethodDefinition messageConstructor = configType.GetMethod(constructorName) ?? messageType?.Resolve().GetMethod(constructorName);
            TypeReference declaringType;
            MethodReference idMethod;

            {
                PropertyDefinition idProperty;
                (idProperty, declaringType) = configType.GetPropertyInHierarchy(idPropertyName);
                if (idProperty == null) (idProperty, declaringType) = messageType.GetPropertyInHierarchy(idPropertyName);
                idMethod = idProperty?.GetMethod?.MakeGeneric(declaringType);
            }

            if (idMethod == null)
            {
                (idMethod, declaringType) = configType.GetMethodInHierarchy(idPropertyName);
                if (idMethod == null) (idMethod, declaringType) = messageType.GetMethodInHierarchy(idPropertyName);
                idMethod = idMethod?.MakeGeneric(declaringType);

                if (idMethod == null)
                    throw new Exception($"Couldn't find member {idPropertyName} in type {configType.FullName}");
            }

            if (messageConstructor.GetAttribute(typeof(CustomFunctionCallAttribute)) is CustomFunctionCallAttribute customCall)
            {
                if (customCall.Args.Length != messageConstructor.Parameters.Count)
                    throw new Exception($"Message constructor {messageConstructor.FullName} has a custom call with a different amount than its parameter");
            }
            else if (!messageConstructor.IsConstructor)
            {
                if (!messageConstructor.IsStatic)
                    throw new Exception($"Message constructor {messageConstructor.FullName} must be static");

                if (messageConstructor.Parameters.Count != 1 || messageConstructor.Parameters[0].ParameterType != configType)
                    throw new Exception($"Message constructor {messageConstructor.FullName} must get a single parameter of type {configType.FullName}");
                else if (messageConstructor.ReturnType.Resolve() != configType)
                    throw new Exception($"Message constructor {messageConstructor.FullName} must return an object of type {configType.FullName}");
            }

            return new MessageSettings()
            {
                MessageConstructor = messageConstructor,
                MessageType = messageType,
                MessageIDGetter = idMethod
            };
        }

        static MessageSettings GetMessageSettingsFrom(TypeDefinition networkConfigurationType)
        {
            // todo: make this work with reflection instead of Mono.Cecil
            MessageSettings settings = null;

            if (networkConfigurationType.GetCustomAttribute(typeof(NetworkManagerAttribute)) is CustomAttribute networkManagerAttribute)
            {
                settings = ReadMessageSettingsFrom((networkManagerAttribute.ConstructorArguments[0].Value as TypeReference).Resolve());

                settings.MessageSenderMethod = networkConfigurationType.GetMethod(
                    networkManagerAttribute.Properties.First(
                        item => item.Name == nameof(NetworkManagerAttribute.MessageSenderName)
                        ).Argument.Value as string);
                if (!settings.MessageSenderMethod.Resolve().IsStatic)
                    throw new Exception($"Message sending method {settings.MessageSenderMethod.FullName} must be static");
            }

            return settings;
        }

        static void Main()
        {
            // todo: make shared only if static method

            bool shared = false;
            string path = shared ? "Generated-Shared/" : "Generated-NotShared";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "NetTest.Generated.dll");
            string targetPath = Path.Combine(Directory.GetCurrentDirectory(), path, "NetTest.Generated.{0}.dll");

            string clientLibraryPath, serverLibraryPath, sharedLibraryPath;

            PrepareAssembly(assemblyPath);

            (clientLibraryPath, serverLibraryPath, sharedLibraryPath) = shared ? PrepareSyncWithShared(assemblyPath, targetPath) : PrepareSyncWithoutShared(assemblyPath, targetPath);

            if (shared)
            {
                using (Stream assemblyStream = File.OpenRead(sharedLibraryPath))
                {
                    byte[] data = new byte[assemblyStream.Length];
                    assemblyStream.Read(data, 0, data.Length);
                    System.Reflection.Assembly.Load(data);
                }
            }

            SynchronizationSettings settings = new SynchronizationSettings()
            {
                SerializationTable = new SerializationTable(),
                MessageSettings = null,
                IncludeNonAuthorityClasses = true,
                IncludeNonAuthorityMethods = true,
                IncludeNonAuthorityProperties = true,
                IncludeNonAuthorityNestedClasses = true,
                CreateSharedLibrary = shared,
                SharedLibraryPath = sharedLibraryPath,
                ClientLibraryPath = clientLibraryPath,
                ServerLibraryPath = serverLibraryPath
            };

            MakeSynced(clientLibraryPath, settings, Authority.Client);
            MakeSynced(serverLibraryPath, settings, Authority.Server);
        }
    }
}
