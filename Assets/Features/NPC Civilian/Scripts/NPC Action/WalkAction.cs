using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;

namespace CodeZash.NPC {
    [CreateAssetMenu(menuName = "NPC/Actions/WalkAction")]
    public class WalkAction : NpcAction {
        public Vector3 areaSize;
        public UnityEvent onArriveAtDestination;
        //public Animator animator;

        public override void Execute(NavMeshAgent agent, NPCCivilianController controller) {
            Vector3 randomPosition = GetRandomPosition(controller.transform.position, areaSize);
            agent.SetDestination(randomPosition);

            controller.StartCoroutine(WaitForArrival(agent, controller));
        }

        private Vector3 GetRandomPosition(Vector3 center, Vector3 size) {
            Vector3 randomPosition = center;
            randomPosition.x += Random.Range(-size.x * 0.5f, size.x * 0.5f);
            randomPosition.z += Random.Range(-size.z * 0.5f, size.z * 0.5f);
            randomPosition.y += Random.Range(0f, size.y);

            Vector3 groundPos = Vector3.zero;
            while (groundPos == Vector3.zero) {
                RaycastHit[] raycastHits = Physics.RaycastAll(randomPosition, Vector3.down);
                if (raycastHits.Length <= 0) {
                    continue;
                }

                groundPos = raycastHits[0].point;
            }

            return groundPos;
        }

        private IEnumerator WaitForArrival(NavMeshAgent agent, NPCCivilianController controller) {
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance) {
                yield return null;
            }

            //if (animator != null) {
            //    animator.SetTrigger("WalkArrive");
            //}

            onArriveAtDestination?.Invoke();
        }
    }
}
