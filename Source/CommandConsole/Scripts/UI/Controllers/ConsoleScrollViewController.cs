#if DevConsole
using UnityEngine;

using UnityEngine.InputSystem;

namespace Game.DevConsole
{
    internal sealed class ConsoleScrollViewController : MonoBehaviour
    {
        [SerializeField] private float _scrollSensitivity;

        [SerializeField] private RectTransform _scrollArea;

        private RectTransform _contentRectTransform;

        private void Awake()
        {
            _contentRectTransform = GetComponent<RectTransform>();

            CommandConsoleInputManager.ScrollInputAction.performed += OnScroll;
        }

        private void OnScroll(InputAction.CallbackContext context)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_scrollArea, Mouse.current.position.value))
            {
                _contentRectTransform.localPosition -= (Vector3)context.ReadValue<Vector2>() * _scrollSensitivity;
            }
        }

        private void OnDestroy()
        {
            CommandConsoleInputManager.ScrollInputAction.performed -= OnScroll;
        }
    }
}
#endif