#if DevConsole
using MethodInfo = System.Reflection.MethodInfo;

using ParameterInfo = System.Reflection.ParameterInfo;

using Object = UnityEngine.Object;

using Type = System.Type;

namespace Game.DevConsole
{
    internal sealed class CommandMethodDefinition
    {
        private readonly MethodInfo _commandMethod;

        private readonly MethodType _commandMethodType;

        private readonly Type _requiredMethodParameterType = null;

        private readonly string _failedCommandInvocationErrorText;

        public CommandMethodDefinition(MethodType commandMethodType, MethodInfo commandMethod)
        {
            _commandMethod = commandMethod;

            _commandMethodType = commandMethodType;

            _failedCommandInvocationErrorText = $"Failed To Execute Command: {_commandMethod.Name}. The Invocation Parameters Was Incorrect";

            ParameterInfo[] parameterInfo = _commandMethod.GetParameters();

            if (parameterInfo.Length > 0)
            {
                _requiredMethodParameterType = parameterInfo[0].ParameterType;
            }
        }

        public void Invoke(object commandParamameter = null)
        {
            object[] parameter = GetInvocationParameter(commandParamameter);

            if(parameter.Length > 0 && parameter[0].GetType() != _requiredMethodParameterType)
            {
                UnityEngine.Debug.LogWarning(_failedCommandInvocationErrorText);

                return;
            }

            if (_commandMethodType == MethodType.Static)
            {
                _commandMethod.Invoke(null, parameter);

                return;
            }

            Object[] targetObjects = Object.FindObjectsByType(_commandMethod.DeclaringType, UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None); ;

            foreach (Object @object in targetObjects)
            {
                _commandMethod.Invoke(@object, parameter);
            }
        }

        private object[] GetInvocationParameter(object parameter = null)
        {
            if (parameter == null)
            {
                return new object[0];
            }

            return new object[1] { parameter };
        }
    }
}
#endif