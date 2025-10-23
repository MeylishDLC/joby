using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace JobsTask
{
    public class JobMovement : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int objectCount = 500;
        [SerializeField] private float radius = 10f;
        [SerializeField] private float speed = 1f;
        [SerializeField] private float computeInterval = 2f;

        private TransformAccessArray _transforms;
        private NativeArray<float> _angles;
        private NativeArray<float> _results;
        private float _timer;
        private void Start()
        {
            Spawn();
        }
        private void Update()
        {
            InitJobs();
        }
        private void InitJobs()
        {
            var moveJob = new RotateAroundJob()
            {
                DeltaTime = Time.deltaTime,
                Speed = speed,
                Radius = radius,
                Angles = _angles
            };

            var moveHandle = moveJob.Schedule(_transforms);

            _timer += Time.deltaTime;
            if (_timer >= computeInterval)
            {
                _timer = 0;
                var logJob = new LogarithmJob
                {
                    Results = _results
                };

                var logHandle = logJob.Schedule(moveHandle);
                logHandle.Complete();

                Debug.Log($"Sample log result: {_results[0]:F3}, {_results[1]:F3}, {_results[2]:F3}");
            }
            else
            {
                moveHandle.Complete();
            }
        }
        private void Spawn()
        {
            var spawned = new Transform[objectCount];
            _angles = new NativeArray<float>(objectCount, Allocator.Persistent);
            _results = new NativeArray<float>(objectCount, Allocator.Persistent);

            for (var i = 0; i < objectCount; i++)
            {
                var pos = new Vector3(Mathf.Cos(i) * radius, 0, Mathf.Sin(i) * radius);
                var obj = Instantiate(prefab, pos, Quaternion.identity);
                spawned[i] = obj.transform;
                _angles[i] = i;
            }
            _transforms = new TransformAccessArray(spawned);
        }
        private void OnDestroy()
        {
            if (_transforms.isCreated)
            {
                _transforms.Dispose();
            }
            if (_angles.IsCreated)
            {
                _angles.Dispose();
            }
            if (_results.IsCreated)
            {
                _results.Dispose();
            }
        }
        private struct LogarithmJob : IJob
        {
            public NativeArray<float> Results;
            public void Execute()
            {
                var random = new Unity.Mathematics.Random((uint)(System.DateTime.Now.Ticks & 0x0000FFFF));
                for (var i = 0; i < Results.Length; i++)
                {
                    var value = random.NextFloat(1f, 100f);
                    Results[i] = Mathf.Log(value);
                }
            }
        }
        private struct RotateAroundJob : IJobParallelForTransform
        {
            public float DeltaTime;
            public float Speed;
            public float Radius;
            public NativeArray<float> Angles;

            public void Execute(int index, TransformAccess transform)
            {
                Angles[index] += Speed * DeltaTime;

                var x = Mathf.Cos(Angles[index]) * Radius;
                var z = Mathf.Sin(Angles[index]) * Radius;

                transform.position = new Vector3(x, 0, z);
            }
        }
        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 200, 20), $"FPS: {1f / Time.deltaTime:F1}");
        }
    }
}