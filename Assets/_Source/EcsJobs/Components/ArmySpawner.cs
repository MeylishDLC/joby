using Unity.Entities;

namespace EcsJobs.Components
{
    public struct ArmySpawner : IComponentData
    {
        public Entity Prefab;
        public int Count;
        public float Spacing;
    }
}