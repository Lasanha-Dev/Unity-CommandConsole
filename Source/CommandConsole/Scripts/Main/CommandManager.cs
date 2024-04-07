#if DevConsole
using MethodInfo = System.Reflection.MethodInfo;

using BindingFlags = System.Reflection.BindingFlags;

using Assembly = System.Reflection.Assembly;

using Type = System.Type;

using InputAction = UnityEngine.InputSystem.InputAction;

using System.Collections.Generic;

using UnityEngine;

namespace Game.DevConsole
{
    internal static class CommandManager
    {
        private static readonly Dictionary<string, CommandMethodDefinition> _commandDefinitions = new Dictionary<string, CommandMethodDefinition>();

        public static HashSet<string> CommmandLines { get; private set; } = new HashSet<string>();

        private static GameObject _consoleInstance;

        private static bool _isConsoleEnabled;

        private const string CONSOLE_RESOURCE_PATH = "DevConsole/DeveloperConsoleCanvas";

        private const string COMMANDS_CONTAINER_RESOURCES_PATH = "DevConsole/TypeCommandsContainer";

        private const string UNKNOWN_COMMAND_DEFAULT_LOG = "Unknown Command:";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeCommandManager()
        {
            CommandConsoleInputManager.ToggleEnableDisableInputAction.performed += OnPerformedToggleEnableDisable;

            _consoleInstance = (GameObject)GameObject.Instantiate(Resources.Load(CONSOLE_RESOURCE_PATH));

            _consoleInstance.SetActive(false);

            GameObject.DontDestroyOnLoad(_consoleInstance);
        }

        private static void OnPerformedToggleEnableDisable(InputAction.CallbackContext context)
        {
            _isConsoleEnabled = !_isConsoleEnabled;

            _consoleInstance.SetActive(_isConsoleEnabled);
        }

        public static void ExecuteCommand(string commandName, object commandParamemeter = null)
        {
            if (_commandDefinitions.TryGetValue(commandName, out CommandMethodDefinition commandDefinition))
            {
                commandDefinition.Invoke(commandParamemeter);

                return;
            }

            Debug.LogWarning($"{UNKNOWN_COMMAND_DEFAULT_LOG} {commandName}");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeCommands()
        {
            TypeCommandsContainer typeCommandsContainer = (TypeCommandsContainer)Resources.Load(COMMANDS_CONTAINER_RESOURCES_PATH);

            Dictionary<string, Assembly> commandsAssemblies = new Dictionary<string, Assembly>();

            foreach (TypeCommands typeCommand in typeCommandsContainer.TypeCommands)
            {
                Type commandType = GetCommandType(typeCommand);

                HashSet<MethodInfo> instanceMethods = new HashSet<MethodInfo>();

                HashSet<MethodInfo> staticMethods = new HashSet<MethodInfo>();

                if (typeCommand.InstanceMethodsNames.Length > 0)
                {
                    GatherCommandMethods(commandType, typeCommand.InstanceMethodsNames, BindingFlags.Instance, instanceMethods);

                    InitializeMethodsCommands(instanceMethods, MethodType.Instance);
                }

                if (typeCommand.StaticMethodsNames.Length > 0)
                {
                    GatherCommandMethods(commandType, typeCommand.StaticMethodsNames, BindingFlags.Static, staticMethods);

                    InitializeMethodsCommands(staticMethods, MethodType.Static);
                }

                Type GetCommandType(TypeCommands typeCommand)
                {
                    if (commandsAssemblies.TryGetValue(typeCommand.AssemblyName, out Assembly commandAssembly))
                    {
                        return commandAssembly.GetType(typeCommand.TypeName, true, true);
                    }

                    Assembly assembly = Assembly.Load(typeCommand.AssemblyName);

                    commandsAssemblies.Add(typeCommand.AssemblyName, assembly);

                    return assembly.GetType(typeCommand.TypeName, true, true);
                }

                void GatherCommandMethods(Type type, string[] methodsNames, BindingFlags methodBindingFlag, HashSet<MethodInfo> listToFill)
                {
                    BindingFlags defaultBindingFlags = BindingFlags.NonPublic | BindingFlags.Public;

                    foreach (string methodName in methodsNames)
                    {
                        listToFill.Add(type.GetMethod(methodName, defaultBindingFlags | methodBindingFlag));
                    }
                }

                void InitializeMethodsCommands(HashSet<MethodInfo> methods, MethodType methodType)
                {
                    const string commandLineParametersPattern = "<[^>]*>";

                    foreach (MethodInfo method in methods)
                    {
                        string commandLine = GetCommandLine(method);

                        CommmandLines.Add(commandLine);

                        CommandMethodDefinition commandDefinition = new CommandMethodDefinition(methodType, method);

                        commandLine = System.Text.RegularExpressions.Regex.Replace(commandLine, commandLineParametersPattern, "");

                        commandLine = commandLine.Replace(" ", "");

                        _commandDefinitions.Add(commandLine, commandDefinition);
                    }
                }

                string GetCommandLine(MethodInfo commandMethod)
                {
                    string parameterTypeName = GetCommandParameterTypeName(commandMethod);

                    if (parameterTypeName == string.Empty)
                    {
                        return $"{commandMethod.Name}";
                    }

                    return $"{commandMethod.Name} {parameterTypeName}";
                }

                string GetCommandParameterTypeName(MethodInfo method)
                {
                    if (method.GetParameters().Length == 0)
                    {
                        return string.Empty;
                    }

                    if (method.GetParameters()[0].ParameterType == typeof(int))
                    {
                        return "<int>";
                    }

                    if (method.GetParameters()[0].ParameterType == typeof(bool))
                    {
                        return "<bool>";
                    }

                    if (method.GetParameters()[0].ParameterType == typeof(float))
                    {
                        return "<float>";
                    }

                    if (method.GetParameters()[0].ParameterType == typeof(string))
                    {
                        return "<string>";
                    }

                    return string.Empty;
                }
            }
        }
    }
}
#endif