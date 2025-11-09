using Unity.Entities;
using Unity.Mathematics;

namespace EcsJobs.Components
{
    public struct Origin: IComponentData
    {
        public float3 Value;
    }
}