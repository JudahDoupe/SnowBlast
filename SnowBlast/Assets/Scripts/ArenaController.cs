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
            Start,
            Wave1,
        }

        private ArenaState State = ArenaState.Start;

        void Update()
        {
            switch (State)
            {
                case ArenaState.Start:
                    var announcement = FindObjectOfType<WaveAnnouncement>(true);
                    announcement.Say($"Wave 1 - Start!");
                    State = ArenaState.Wave1;
                    break;
            }
        }
    }
}
