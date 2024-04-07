#if DevConsole
using Serializable = System.SerializableAttribute;

using SerializeField = UnityEngine.SerializeField;

namespace Game.DevConsole
{
    [Serializable]
    internal sealed class TypeCommands
    {
        [field: SerializeField] public string TypeName { get; private set; }

        [field: SerializeField] public string AssemblyName { get; private set; }

        [field: SerializeField] public string[] InstanceMethodsNames { get; private set; }

        [field: SerializeField] public string[] StaticMethodsNames { get; private set; }

        public TypeCommands(string typeName, string assemblyName, string[] typeInstanceMethodsNames, string[] typeStaticMethodsNames)
        {
            TypeName = typeName;

            AssemblyName = assemblyName;

            InstanceMethodsNames = typeInstanceMethodsNames;

            StaticMethodsNames = typeStaticMethodsNames;
        }
    }
}
#endif