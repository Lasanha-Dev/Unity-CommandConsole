#if DevConsole
using PointerEventData = UnityEngine.EventSystems.PointerEventData;

using IDragHandler = UnityEngine.EventSystems.IDragHandler;

using IEndDragHandler = UnityEngine.EventSystems.IEndDragHandler;

using UnityEngine;

namespace Game.DevConsole
{
    internal sealed class ConsolePanelDragPanel : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform _consolePanelRectTransform;

        private RectTransform _consoleCanvasRectTransform;

        private Canvas _consoleCanvas;

        private Vector2 _targetPosition;

        private Vector2 _positionBounds;

        private void Awake()
        {
            _consoleCanvas = GetComponentInParent<Canvas>();

            _consoleCanvasRectTransform = _consoleCanvas.GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Cursor.lockState = CursorLockMode.Confined;

            _targetPosition += eventData.delta / _consoleCanvas.scaleFactor;

            AdjustPanelLocation();
        }

        private void AdjustPanelLocation()
        {
            CalculateDragPositionBounds();

            _targetPosition.x = Mathf.Clamp(_targetPosition.x, _positionBounds.x, -_positionBounds.x);

            _targetPosition.y = Mathf.Clamp(_targetPosition.y, _positionBounds.y, -_positionBounds.y);

            _consolePanelRectTransform.anchoredPosition = _targetPosition;
        }

        private void CalculateDragPositionBounds()
        {
            _positionBounds.x = -_consoleCanvasRectTransform.rect.width * 0.5f + _consolePanelRectTransform.rect.width * 0.5f;

            _positionBounds.y = -_consoleCanvasRectTransform.rect.height * 0.5f + _consolePanelRectTransform.rect.height * 0.5f;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
#endif