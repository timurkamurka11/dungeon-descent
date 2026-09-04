using DungeonDescent.Audio;
using DungeonDescent.Core;
using DungeonDescent.Player;
using UnityEngine;

namespace DungeonDescent.World
{
    public sealed class ZoneTrigger : MonoBehaviour
    {
        private int floor;private MusicState music;private string ambience;private bool fired;
        public void Configure(int floorIndex,MusicState state,string ambienceName){floor=floorIndex;music=state;ambience=ambienceName;}
        private void OnTriggerEnter(Collider other){if(other.GetComponentInParent<PlayerController>()==null)return;AudioManager.Instance?.SetMusic(music);AudioManager.Instance?.SetAmbience(ambience);if(!fired&&floor>0){fired=true;GameSession.Instance?.MarkFloor(floor);}}
    }
}
