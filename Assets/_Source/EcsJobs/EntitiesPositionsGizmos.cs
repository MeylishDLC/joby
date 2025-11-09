using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EcsJobs
{
    [ExecuteAlways]
    public class EntitiesPositionsGizmos: MonoBehaviour
    {
        [SerializeField] private Color gizmoColor = Color.cyan;
        [SerializeField] private int limit;

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
            {
                return;
            }
            var entityManager = world.EntityManager;
            Gizmos.color = gizmoColor;
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<LocalTransform>());
            using var transforms = query.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            
            int count;
            if (limit > 0)
            {
                count = math.min(limit, transforms.Length);
            }
            else
            {
                count = transforms.Length;
            }
            for (var i = 0; i < count; i++)
            {
                Gizmos.DrawSphere(transforms[i].Position, 0.1f);
            }
        }
    }
}