using System.Collections.Generic;
using Unity.Entities;

namespace Survivors
{
    public class EnemyListComponent : IComponentData
    {
        public List<EnemyData> Enemies;
    }
}
