using EcsJobs.Components;
using Unity.Entities;
using UnityEngine;

namespace EcsJobs.Spawn
{
    public class ArmySpawnerAuthorization : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int count = 50;
        [SerializeField] private float spacing = 1.5f;
        private class Baker : Baker<ArmySpawnerAuthorization>
        {
            public override void Bake(ArmySpawnerAuthorization authoring)
            {
                var spawnerEntity = GetEntity(TransformUsageFlags.None);
                var prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);

                AddComponent(spawnerEntity, new ArmySpawner
                {
                    Prefab = prefabEntity,
                    Count = authoring.count,
                    Spacing = authoring.spacing
                });
            }
        }
    }
}