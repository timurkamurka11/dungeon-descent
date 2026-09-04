using System.Collections;
using System.Collections.Generic;
using DungeonDescent.Audio;
using DungeonDescent.Core;
using DungeonDescent.Enemies;
using DungeonDescent.Player;
using UnityEngine;

namespace DungeonDescent.World
{
    public sealed class EncounterController : MonoBehaviour
    {
        private readonly List<EnemyHealth> enemies=new List<EnemyHealth>();
        private EnemyArchetype[] types;
        private Vector3[] offsets;
        private GameObject gate;
        private bool triggered,cleared;
        private string roomId;
        public void Configure(string id,EnemyArchetype[] spawnTypes,Vector3[] spawnOffsets,GameObject lockGate=null){roomId=id;types=spawnTypes;offsets=spawnOffsets;gate=lockGate;}
        private void OnTriggerEnter(Collider other){if(triggered||other.GetComponentInParent<PlayerController>()==null)return;triggered=true;StartCoroutine(Begin());}
        private IEnumerator Begin()
        {
            if(gate!=null)gate.SetActive(true);AudioManager.Instance?.SetMusic(MusicState.Combat,.65f);yield return new WaitForSeconds(.35f);
            for(int i=0;i<types.Length;i++){var e=EnemyFactory.Spawn(types[i],transform.position+(i<offsets.Length?offsets[i]:Vector3.zero),DungeonWorldBuilder.Instance.EnemyRoot);var h=e.GetComponent<EnemyHealth>();enemies.Add(h);yield return new WaitForSeconds(.12f);}
        }
        private void Update()
        {
            if(!triggered||cleared||enemies.Count==0)return;for(int i=0;i<enemies.Count;i++)if(enemies[i]!=null&&!enemies[i].IsDead)return;
            cleared=true;if(gate!=null)gate.SetActive(false);GameEvents.RaiseRoomCleared(roomId);AudioManager.Instance?.SetMusic(MusicState.Exploration,1.1f);
        }
    }
}
