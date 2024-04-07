#if DevConsole
using UnityEngine;

using InputAction = UnityEngine.InputSystem.InputAction;

namespace Game.DevConsole
{
    internal static class CommandConsoleInputManager
    {
        private static CommandConsoleInputActions _commandConsoleInputActions;

        public static InputAction ToggleEnableDisableInputAction { get; private set; }

        public static InputAction ScrollInputAction { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeCommandConsoleInputActions()
        {
            _commandConsoleInputActions = new CommandConsoleInputActions();

            _commandConsoleInputActions.Enable();

            ToggleEnableDisableInputAction = _commandConsoleInputActions.CommandConsole.ToggleEnableDisable;

            ScrollInputAction = _commandConsoleInputActions.CommandConsole.Scroll;
        }
    }
}
#endif