using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Utils;

namespace Assets.Scripts
{
    class ArenaController: MonoBehaviour
    {
        enum ArenaState
        {
            ShowWaveAnnouncement,
            StartWave,
            InWave
        }

        public GameObject[] SpawnablePrefabs;
        private ArenaState State = ArenaState.ShowWaveAnnouncement;
        private float WaveStartTime;
        private int Wave = 1;

        void Update()
        {
            switch (State)
            {
                case ArenaState.ShowWaveAnnouncement:
                    var announcement = FindObjectOfType<WaveAnnouncement>(true);
                    announcement.Say($"Wave {Wave} - Start!");
                    State = ArenaState.StartWave;
                    WaveStartTime = Time.fixedTime + announcement.TotalTime;
                    break;
                case ArenaState.StartWave:
                    if (Time.fixedTime >= WaveStartTime)
                    {
                        var player = FindObjectOfType<Player>();
                        var turret = Instantiate(SpawnablePrefabs[0], player.gameObject.transform.position + new Vector3(10, 0, 10),
                            Quaternion.identity);
                        State = ArenaState.InWave;
                    }
                    break;
                case ArenaState.InWave:
                    break;
            }
        }
    }
}
