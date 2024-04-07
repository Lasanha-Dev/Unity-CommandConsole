#if DevConsole
using TMP_InputField = TMPro.TMP_InputField;

using IPointerEnterHandler = UnityEngine.EventSystems.IPointerEnterHandler;

using PointerEventData = UnityEngine.EventSystems.PointerEventData;

using UnityEngine;

namespace Game.DevConsole
{
    internal sealed class CommandInputFieldController : MonoBehaviour, IPointerEnterHandler
    {
        private TMP_InputField _commandInputField;

        private void Awake()
        {
            _commandInputField = GetComponent<TMP_InputField>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_commandInputField.text.Length > 0)
            {
                _commandInputField.Select();
            }
        }
    }
}
#endif