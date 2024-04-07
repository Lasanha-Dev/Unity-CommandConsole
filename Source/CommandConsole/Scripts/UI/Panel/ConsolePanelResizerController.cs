#if DevConsole
using UnityEngine;

using UnityEngine.EventSystems;

namespace Game.DevConsole
{
    internal sealed class ConsolePanelResizerController : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform _panelToResize;

        [SerializeField] private ResizeDirection _resizeDirection;

        [SerializeField] private Texture2D _resizeWindowMouseCursor;

        [SerializeField] private Vector2 _minSize;

        [SerializeField] private Vector2 _maxSize;

        [SerializeField] private bool _useInverseXDirection;

        [SerializeField] private bool _useInverseYDirection;

        private Vector2 _currentPointerPosition;

        private Vector2 _previousPointerPosition;

        private static bool _isDragging;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isDragging is false)
            {
                SetCursorTexture(_resizeWindowMouseCursor);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isDragging is false)
            {
                SetCursorTexture(null);
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            SetCursorTexture(_resizeWindowMouseCursor);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_panelToResize, data.position, data.pressEventCamera, out _previousPointerPosition);

            _isDragging = true;
        }

        public void OnDrag(PointerEventData data)
        {
            Vector2 currentSizeDelta = _panelToResize.sizeDelta;

            Vector2 targetSizeDelta = currentSizeDelta;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_panelToResize, data.position, data.pressEventCamera, out _currentPointerPosition);

            Vector2 resizeValue = _currentPointerPosition - _previousPointerPosition;

            CalculateTargetSizeDelta();

            targetSizeDelta = new Vector2(
            Mathf.Clamp(targetSizeDelta.x, _minSize.x, _maxSize.x),
            Mathf.Clamp(targetSizeDelta.y, _minSize.y, _maxSize.y)
            );

            AdjustTargetSizeDirectionValue();

            _panelToResize.sizeDelta = targetSizeDelta;

            _previousPointerPosition = _currentPointerPosition;

            void CalculateTargetSizeDelta()
            {
                if (_useInverseXDirection)
                {
                    resizeValue.x = -resizeValue.x;
                }

                if(_useInverseYDirection)
                {
                    resizeValue.y = -resizeValue.y;
                }

                targetSizeDelta += 2 * GetComponentInParent<Canvas>().scaleFactor * resizeValue;
            }

            void AdjustTargetSizeDirectionValue()
            {
                if (_resizeDirection == ResizeDirection.Vertical)
                {
                    targetSizeDelta.x = currentSizeDelta.x;
                }

                if (_resizeDirection == ResizeDirection.Horizontal)
                {
                    targetSizeDelta.y = currentSizeDelta.y;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            SetCursorTexture(null);

            _isDragging = false;
        }

        private void SetCursorTexture(Texture2D texture)
        {
            Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
        }

        private enum ResizeDirection
        {
            Horizontal,
            Vertical,
            Diagonal
        }
    }
}
#endif