using UnityEngine;
using UnityEngine.AI;

namespace CodeZash.NPC {
    public abstract class NpcAction : ScriptableObject {
        public abstract void Execute(NavMeshAgent agent, NPCCivilianController controller);
    }
}
