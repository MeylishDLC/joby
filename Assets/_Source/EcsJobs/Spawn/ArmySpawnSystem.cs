using EcsJobs.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace EcsJobs.Spawn
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct ArmySpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ArmySpawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (spawner, spawnerEntity) in SystemAPI.Query<RefRO<ArmySpawner>>().WithEntityAccess())
            {
                if (state.EntityManager.HasComponent<SpawnedTag>(spawnerEntity))
                {
                    continue;
                }

                var prefab = spawner.ValueRO.Prefab;
                var count = spawner.ValueRO.Count;
                var spacing = spawner.ValueRO.Spacing;

                var gridSize = (int)math.ceil(math.sqrt(count));

                for (var x = 0; x < gridSize; x++)
                {
                    for (var z = 0; z < gridSize; z++)
                    {
                        var index = x * gridSize + z;
                        if (index >= count)
                        {
                            break;
                        }

                        var instance = ecb.Instantiate(prefab);

                        var pos = new float3(x * spacing, 0, z * spacing);
                        ecb.SetComponent(instance, LocalTransform.FromPosition(pos));
                    }
                }
                ecb.AddComponent<SpawnedTag>(spawnerEntity);
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}