using UnityEngine;

namespace CodeZash.NPC {
    public class Seat : MonoBehaviour {

        public string npcTag = "NPC";
        public Transform sitAnchor;

        public bool IsTaken { get; set; }

        public NPCCivilianController NPCCivilianController { get; set; }

        public void Reset() {
            IsTaken = false;
            NPCCivilianController = null;
        }
    }
}
