#if DevConsole
using UnityEngine;

using Button = UnityEngine.UI.Button;

namespace Game.DevConsole
{
    internal sealed class ConsoleClearLogsButtonController : MonoBehaviour
    {
        [SerializeField] private Transform _logsParentTransform;

        private Button _clearLogsButton;

        private void Awake()
        {
            _clearLogsButton = GetComponent<Button>();

            _clearLogsButton.onClick.AddListener(OnClickClearLogsButton);
        }

        private void OnClickClearLogsButton()
        {
            int childCount = _logsParentTransform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(_logsParentTransform.GetChild(i).gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_clearLogsButton != null)
            {
                _clearLogsButton.onClick.RemoveAllListeners();
            }
        }
    }
}
#endif