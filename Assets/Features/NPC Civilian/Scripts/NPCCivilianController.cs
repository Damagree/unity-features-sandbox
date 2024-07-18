using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CodeZash.NPC {
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCCivilianController : MonoBehaviour {

        [Header("Main Settings")]
        [Tooltip("Time (in seconds) before choosing next action.")]
        [SerializeField] private float timeToThink = 30f;

        [Header("Animator Settings")]
        [Tooltip("Animator for the NPC (Walk when moving, idle when reached destination).")]
        public Animator npcAnimator;

        public List<string> idleAnimationName = new List<string> { "idle" };
        public List<string> walkAnimationName = new List<string> { "walk" };

        [SerializeField] private List<NpcStateAction> actions;

        private NavMeshAgent agent;
        private float timer;

        public Transform CurrentSeat { get; set; }

        [System.Serializable]
        public struct NpcStateAction {
            public float probability;
            public NpcAction action;
        }

        void Start() {
            agent = GetComponent<NavMeshAgent>();
            timer = timeToThink;
            //Debug.Log("NPC started. Initializing first action.");
            ChooseNextAction();
        }

        void Update() {
            // Check if the agent has reached its destination
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending) {
                timer -= Time.deltaTime;
                if (timer <= 0f) {
                    //Debug.Log("Time to think is up. Choosing next action.");
                    ChooseNextAction();
                    timer = timeToThink;
                } else {
                    // Set animator to idle if the agent has reached its destination
                    DoIdle();
                }
            } else {
                // Set animator to walking if the agent is moving
                DoWalking();
            }
        }

        bool isIdle = true;
        public void DoWalking() {
            if (!isIdle) return;

            isIdle = false;
            npcAnimator?.Play(walkAnimationName[Random.Range(0, walkAnimationName.Count)]);
        }

        public void DoIdle() {
            if (isIdle) return;

            isIdle = true;
            npcAnimator?.Play(idleAnimationName[Random.Range(0, idleAnimationName.Count)]);
        }

        public void ChooseNextAction() {
            float totalProbability = 0f;
            foreach (NpcStateAction stateAction in actions) {
                totalProbability += stateAction.probability;
            }

            float randomValue = Random.Range(0, totalProbability);
            float cumulativeProbability = 0f;
            foreach (NpcStateAction stateAction in actions) {
                cumulativeProbability += stateAction.probability;
                if (randomValue <= cumulativeProbability) {
                    //Debug.Log($"Chosen action: {stateAction.action.GetType().Name} with probability {stateAction.probability}");
                    Reset();
                    stateAction.action.Execute(agent, this);
                    break;
                }
            }
        }

        private void Reset() {
            //Debug.Log($"Reset {GetType().Name} here");
            if (CurrentSeat != null) {
                CurrentSeat.GetComponent<Seat>()?.Reset();
                CurrentSeat = null;
                if (npcAnimator != null) {
                    npcAnimator.SetBool("sitting", false);
                }
            }
        }
    }
}
