using EcsJobs.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace EcsJobs.Movement
{
    [BurstCompile]
    public partial struct MoveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var job = new MoveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            job.ScheduleParallel();
        }
    }
}