#if DevConsole
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System;
#endif

using System.Collections.Generic;

using UnityEngine;

namespace Game.DevConsole
{
    [CreateAssetMenu(fileName = "TypeCommandsContainer")]
    internal sealed class TypeCommandsContainer : ScriptableObject
    {
        [field: SerializeField] public List<TypeCommands> TypeCommands { get; private set; } = new List<TypeCommands>();

#if UNITY_EDITOR
        [SerializeField] private bool _compileCommandsOnDomainReload;

        private const string COMMANDS_CONTAINER_RESOURCES_PATH = "DevConsole/TypeCommandsContainer";

        private static readonly Type _commandAttributeType = typeof(CommandAttribute);

        private static readonly List<string> NotAllowedCommandAssembliesNames = new List<string>()
        {
            "System",
            "Unity",
            "Bee",
            "com.",
            "Mono",
            "mscorlib",
            "Microsoft",
            "AndroidPlayerBuildProgram",
            "NiceIO",
            "Anonymously",
            "netstandard",
            "nunit",
            "Newtonsoft",
            "WebGLPlayerBuildProgram",
            "ScriptCompilationBuildProgram",
            "ExCSS",
            "PPv2URPConverters",
            "ReportGeneratorMerged",
            "PlayerBuildProgramLibrary",
            "log4net,",
            "Cinemachine",
        };

        [InitializeOnLoadMethod]
        private static void TryToExecuteCommandsCompilation()
        {
            TypeCommandsContainer commandAssemblies = (TypeCommandsContainer)Resources.Load(COMMANDS_CONTAINER_RESOURCES_PATH);

            if (commandAssemblies._compileCommandsOnDomainReload)
            {
                commandAssemblies.CompileCommands();
            }
        }

        private void CompileCommands()
        {
            TypeCommands.Clear();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                if (IsAssemblyNotAllowed(assembly))
                {
                    continue;
                }

                foreach (Type type in assembly.GetTypes())
                {
                    List<string> instanceMethodsNames = new List<string>();

                    List<string> staticMethodsNames = new List<string>();

                    BindingFlags defaultBindingFlags = BindingFlags.NonPublic | BindingFlags.Public;

                    GatherCommandMethods(type, defaultBindingFlags | BindingFlags.Instance, instanceMethodsNames);

                    GatherCommandMethods(type, defaultBindingFlags | BindingFlags.Static, staticMethodsNames);

                    if (instanceMethodsNames.Count > 0 || staticMethodsNames.Count > 0)
                    {
                        TypeCommands.Add(new TypeCommands(type.FullName, assembly.FullName, instanceMethodsNames.ToArray(), staticMethodsNames.ToArray()));
                    }
                }

                void GatherCommandMethods(Type type, BindingFlags methodsFlags, List<string> listToFill)
                {
                    MethodInfo[] methods = type.GetMethods(methodsFlags);

                    foreach (MethodInfo method in methods)
                    {
                        if (Attribute.IsDefined(method, _commandAttributeType) is false)
                        {
                            continue;
                        }

                        listToFill.Add(method.Name);
                    }
                }
            }

            Debug.Log($"<color=#17FF32>Successfully Compiled Commands</color>");
        }

        private static bool IsAssemblyNotAllowed(Assembly assembly)
        {
            foreach (string prefix in NotAllowedCommandAssembliesNames)
            {
                if (assembly.FullName.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
#endif
    }
}
#endif