using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace CodeZash.NPC {
    [CreateAssetMenu(menuName = "NPC/Actions/SitAction")]
    public class SitAction : NpcAction {
        public Seat[] seats;
        public UnityEvent onArriveAtTakenSeat;

        public override void Execute(NavMeshAgent agent, NPCCivilianController controller) {
            Seat seat = ChooseRandomSeat();
            if (seat != null) {
                seat.IsTaken = true; // Mark seat as taken immediately to avoid race conditions
                seat.NPCCivilianController = controller;
                //Debug.Log($"{GetType().Name} - usedBy {controller.name} - {seat.IsTaken}");
                agent.SetDestination(seat.transform.position);
                controller.CurrentSeat = seat.transform;
                controller.StartCoroutine(WaitForSeatArrival(agent, controller, seat));
            } else {
                onArriveAtTakenSeat?.Invoke();
                controller.ChooseNextAction();
            }
        }

        void SearchSeats() {
            seats = FindObjectOfType<SeatList>().seatList.ToArray();
        }

        private Seat ChooseRandomSeat() {
            SearchSeats();
            List<Seat> availableSeats = new List<Seat>();
            foreach (Seat seat in seats) {
                if (!seat.GetComponent<Seat>().IsTaken) {
                    availableSeats.Add(seat);
                }
            }
            if (availableSeats.Count > 0) {
                return availableSeats[Random.Range(0, availableSeats.Count)];
            }
            return null;
        }

        private IEnumerator WaitForSeatArrival(NavMeshAgent agent, NPCCivilianController controller, Seat seat) {
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance) {
                yield return null;
            }

            // Double-check if the seat is still available after arriving
            //Debug.Log($"{GetType().Name} - seat [{seat != null}] - isTaken [{seat.IsTaken == false}] - controller [{seat.NPCCivilianController?.name == controller.name}]");

            if (seat != null && seat.IsTaken && seat.NPCCivilianController == controller) {
                seat.IsTaken = true;

                // Disable the NavMeshAgent to prevent it from overriding the position and rotation
                agent.enabled = false;

                controller.transform.position = seat.sitAnchor.position;
                controller.transform.rotation = seat.sitAnchor.rotation;

                //Debug.Log($"Seat taken by {controller.name}. Snapping to position {seat.sitAnchor.position} and rotation {seat.sitAnchor.rotation.eulerAngles}");

                // If the NPC has an animator, set the "sitting" parameter to true
                if (controller.npcAnimator != null) {
                    controller.npcAnimator.Play("sitting");
                }

                // Re-enable the NavMeshAgent if needed
                agent.enabled = true;
            } else {
                // If the seat is taken, invoke the event and choose the next action
                onArriveAtTakenSeat?.Invoke();
                seat.IsTaken = false; // Release the seat
                controller.CurrentSeat = null;
                controller.ChooseNextAction();
            }
        }


    }
}
