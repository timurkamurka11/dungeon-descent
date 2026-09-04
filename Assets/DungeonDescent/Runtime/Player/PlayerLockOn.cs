using DungeonDescent.Enemies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonDescent.Player
{
    public sealed class PlayerLockOn:MonoBehaviour
    {
        public Transform Target{get;private set;}public bool Locked=>Target!=null;
        private void Update(){if(Keyboard.current!=null&&Keyboard.current.qKey.wasPressedThisFrame){if(Target!=null){Target=null;return;}Acquire();}if(Target!=null&&(Vector3.Distance(transform.position,Target.position)>18f||Target.GetComponent<EnemyHealth>()==null||Target.GetComponent<EnemyHealth>().IsDead))Target=null;}
        private void Acquire(){var enemies=Object.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);float best=float.MaxValue;foreach(var e in enemies){if(e==null||e.IsDead)continue;var d=Vector3.SqrMagnitude(e.transform.position-transform.position);if(d<best&&d<225f){best=d;Target=e.transform;}}}
    }
}
