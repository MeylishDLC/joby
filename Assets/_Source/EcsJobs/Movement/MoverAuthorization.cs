using EcsJobs.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EcsJobs.Movement
{
    public class MoverAuthorization: MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float radius = 1f;
        [SerializeField] private float initialAngle;
        private class Baker : Baker<MoverAuthorization>
        {
            public override void Bake(MoverAuthorization authoring)
            {
                var ent = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                AddComponent(ent, new MoveSpeed { Value = authoring.moveSpeed });
                AddComponent(ent, new Radius    { Value = authoring.radius });
                AddComponent(ent, new Angle { Value = authoring.initialAngle });
                    
                float3 pos = GetComponent<Transform>().position;
                AddComponent(ent, new Origin    { Value = pos });
                
                AddComponent<LocalTransform>(ent);
            }
        }
    }
}