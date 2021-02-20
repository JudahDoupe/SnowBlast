using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class ArenaController: MonoBehaviour
    {
        void Start()
        {
            var announcement = GameObject.FindObjectOfType<WaveAnnouncement>(true);
            announcement.gameObject.SetActive(true);
        }
    }
}
