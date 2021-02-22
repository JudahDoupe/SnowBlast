using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    class ArenaController: MonoBehaviour
    {
        public GameObject[] SpawnablePrefabs;

        private enum ArenaState
        {
            ShowWaveAnnouncement,
            StartWave,
            InWave,
            GameOver,
            Done
        }

        private readonly IReadOnlyDictionary<ArenaState, Func<ArenaState>> StateFunctions;

        private ArenaState State = ArenaState.ShowWaveAnnouncement;
        private float WaveStartTime;
        private int Wave = 1;

        public ArenaController()
        {
            StateFunctions = new Dictionary<ArenaState, Func<ArenaState>>
            {
                {ArenaState.ShowWaveAnnouncement, () => ShowWaveAnnouncement()},
                {ArenaState.StartWave, () => StartWave()},
                {ArenaState.InWave, () => InWave()},
                {ArenaState.GameOver, () => GameOver()},
                {ArenaState.Done, () => ArenaState.Done}
            };
        }

        void Update()
        {
            State = StateFunctions[State]();
        }

        private ArenaState ShowWaveAnnouncement()
        {
            var announcement = FindObjectOfType<WaveAnnouncement>(true);
            announcement.Say($"Wave {Wave} - Start!");
            WaveStartTime = Time.fixedTime + announcement.TotalTime;
            return ArenaState.StartWave;
        }

        private ArenaState StartWave()
        {
            if (Time.fixedTime >= WaveStartTime)
            {
                var player = FindObjectOfType<Player>();
                var turret = Instantiate(SpawnablePrefabs[0], player.gameObject.transform.position + new Vector3(10, 0, 10),
                    Quaternion.identity);
                return ArenaState.InWave;
            }

            return State;
        }

        private ArenaState InWave()
        {
            var player = FindObjectOfType<Player>();
            if (player == null)
            {
                return ArenaState.GameOver;
            }

            return State;
        }

        private ArenaState GameOver()
        {
            var gameOver = GameObject.Find("UILayer").transform.Find("GameOver");
            gameOver.gameObject.SetActive(true);
            return ArenaState.Done;
        }
    }
}
