#if DevConsole
using UnityEngine;

namespace Game.DevConsole
{
    internal sealed class ConsolePanelScreenFitAdjusterController : MonoBehaviour
    {
        [SerializeField] private float _adjustmentTimerDelay = 1f;

        private RectTransform _consoleCanvasRectTransform;

        private Vector2 _consolePosition;

        private RectTransform _consolePanelRectTransform;

        private void Awake()
        {
            _consolePanelRectTransform = GetComponent<RectTransform>();

            _consoleCanvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            InvokeRepeating(nameof(AdjustPanelLocation), _adjustmentTimerDelay, _adjustmentTimerDelay);
        }

        private void AdjustPanelLocation()
        {
            _consolePosition = _consolePanelRectTransform.anchoredPosition;

            CalculatePositionBounds();

            _consolePanelRectTransform.anchoredPosition = _consolePosition;
        }

        private void CalculatePositionBounds()
        {
            Vector2 positionBounds;

            positionBounds.x = (-_consoleCanvasRectTransform.rect.width * 0.5f) + (_consolePanelRectTransform.rect.width * 0.5f);

            positionBounds.y = (-_consoleCanvasRectTransform.rect.height * 0.5f) + (_consolePanelRectTransform.rect.height * 0.5f);

            _consolePosition.x = Mathf.Clamp(_consolePosition.x, positionBounds.x, -positionBounds.x);

            _consolePosition.y = Mathf.Clamp(_consolePosition.y, positionBounds.y, -positionBounds.y);
        }

        private void OnDisable()
        {
            CancelInvoke();
        }
    }
}
#endif