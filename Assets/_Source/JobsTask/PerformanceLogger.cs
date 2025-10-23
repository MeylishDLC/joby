using UnityEngine;

namespace JobsTask
{
    public class PerformanceLogger : MonoBehaviour
    {
        private float _timer;
        private float _averageFPS;

        private void Update()
        {
            _timer += Time.deltaTime;
            _averageFPS += (1f / Time.deltaTime);

            if (_timer >= 2f)
            {
                var fps = _averageFPS / (_timer / Time.deltaTime);
                Debug.Log($"Average FPS: {fps:F1}");
                _timer = 0;
                _averageFPS = 0;
            }
        }
        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 200, 20), $"FPS: {1f / Time.deltaTime:F1}");
        }
    }
}