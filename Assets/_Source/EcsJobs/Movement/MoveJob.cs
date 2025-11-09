using EcsJobs.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace EcsJobs.Movement
{
    [BurstCompile]
    public partial struct MoveJob : IJobEntity
    {
        public float DeltaTime { get; set; }
        private void Execute(ref LocalTransform transform, ref Angle angle, in MoveSpeed speed, in Radius radius, in Origin origin)
        {
            angle.Value += speed.Value * DeltaTime;

            var c = math.cos(angle.Value);
            var s = math.sin(angle.Value);

            var offset = new float3(c * radius.Value, 0f, s * radius.Value);
            var newPos = origin.Value + offset;

            transform = transform.WithPosition(newPos);
        }
    }
}