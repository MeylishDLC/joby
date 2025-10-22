using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int objectCount = 500;
    [SerializeField] private float radius = 10f;
    [SerializeField] private float speed = 1f;

    private TransformAccessArray _transforms;
    private NativeArray<float> _angles;

    private void Start()
    {
        Spawn();
    }
    private void Update()
    {
        InitJob();
    }
    private void InitJob()
    {
        var job = new RotateAroundJob
        {
            DeltaTime = Time.deltaTime,
            Speed = speed,
            Radius = radius,
            Angles = _angles
        };

        var handle = job.Schedule(_transforms);
        handle.Complete();
    }
    private void Spawn()
    {
        var spawned = new Transform[objectCount];
        _angles = new NativeArray<float>(objectCount, Allocator.Persistent);

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