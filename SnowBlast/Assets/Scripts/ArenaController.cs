using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    class ArenaController: MonoBehaviour
    {
        [Range(1,3)]
        public int MaxWaves = 2;
        public GameObject[] SpawnablePrefabs;
        public GameObject HealthBarPrefab;

        private readonly List<GameObject> SpawnedEnemies = new List<GameObject>();

        private enum ArenaState
        {
            ShowWaveAnnouncement,
            StartWave,
            InWave,
            GameOver,
            Done,
            WinState
        }

        private readonly IReadOnlyDictionary<ArenaState, Func<ArenaState>> StateFunctions;

        private ArenaState State = ArenaState.ShowWaveAnnouncement;
        private float WaveStartTime;
        private int Wave;
        public IEnumerable<GameObject> Enemies => SpawnedEnemies.Where(it => it.activeSelf);

        public ArenaController()
        {
            StateFunctions = new Dictionary<ArenaState, Func<ArenaState>>
            {
                {ArenaState.ShowWaveAnnouncement, () => ShowWaveAnnouncement()},
                {ArenaState.StartWave, () => StartWave()},
                {ArenaState.InWave, () => InWave()},
                {ArenaState.GameOver, () => GameOver()},
                {ArenaState.Done, () => ArenaState.Done},
                {ArenaState.WinState, () => WinState()}
            };
        }

        void Update()
        {
            try
            {
                State = StateFunctions[State]();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                State = ArenaState.Done;
            }
        }

        private ArenaState ShowWaveAnnouncement()
        {
            Wave += 1;
            SpawnedEnemies.Clear();
            var announcement = FindObjectOfType<WaveAnnouncement>(true);
            announcement.Say($"Wave {Wave} - Start!");
            WaveStartTime = Time.fixedTime + announcement.TotalTime;
            return ArenaState.StartWave;
        }

        private ArenaState StartWave()
        {
            if (Time.fixedTime >= WaveStartTime)
            {
                var player = FindObjectOfType<Player.Player>();
                for (var i = 0; i < Wave; ++i)
                {
                    var angleFromPlayer = Random.Range(0.0f, 359.0f) * Mathf.Deg2Rad;
                    var distanceFromPlayer = Random.Range(5.0f, 10.0f);
                    var x = Mathf.Cos(angleFromPlayer) * distanceFromPlayer;
                    var z = Mathf.Sin(angleFromPlayer) * distanceFromPlayer;
                    var whichPrefab = Random.Range(0, SpawnablePrefabs.Length);
                    var enemy = Instantiate(SpawnablePrefabs[whichPrefab], player.gameObject.transform.position + new Vector3(x, 0, z),
                        Quaternion.identity);
                    var healthBar = Instantiate(HealthBarPrefab);
                    healthBar.GetComponent<EnemyInfoDisplay>().SetParent(enemy);
                    SpawnedEnemies.Add(enemy);
                }
                
                return ArenaState.InWave;
            }

            return State;
        }

        private ArenaState InWave()
        {
            var player = FindObjectOfType<Player.Player>();
            if (player == null)
            {
                return ArenaState.GameOver;
            }
            if (SpawnedEnemies.All(enemy => !enemy.gameObject.activeSelf))
            {
                if (Wave >= MaxWaves) return ArenaState.WinState;
                return ArenaState.ShowWaveAnnouncement;
            }

            return State;
        }

        private ArenaState GameOver()
        {
            var gameOver = GameObject.Find("UILayer").transform.Find("GameOver");
            gameOver.gameObject.SetActive(true);
            return ArenaState.Done;
        }

        private ArenaState WinState()
        {
            var gameOver = GameObject.Find("UILayer").transform.Find("GameOver");
            gameOver.GetComponent<Text>().text = "VICTORY!";
            gameOver.gameObject.SetActive(true);
            GameState.ArenaVictory = true;
            this.BeginSerial()
                .Wait(1.0f)
                .Then(() => Utils.Utils.TransitionToScene("Hallway"))
                .Start();
            return ArenaState.Done;
        }
    }
}
