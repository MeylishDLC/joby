using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _currentSafeArea;
    private ScreenOrientation _currentOrientation;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }
    private void OnEnable()
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        ApplySafeArea();
    }
    private void OnRectTransformDimensionsChange()
    {
        if (_rectTransform == null)
        {
            return;
        }
        if (_currentSafeArea != Screen.safeArea || _currentOrientation != Screen.orientation)
        {
            ApplySafeArea();
        }
    }
    private void ApplySafeArea()
    {
        _currentSafeArea = Screen.safeArea;
        _currentOrientation = Screen.orientation;

        var anchorMin = _currentSafeArea.position;
        var anchorMax = _currentSafeArea.position + _currentSafeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }
}