using System;
using Assets.Scripts.Scene;
using UnityEngine;

namespace Assets.Utils
{
    public static class Utils
    {
        public static void TransitionToScene(string directObject)
        {
            GameObject.FindGameObjectWithTag("Transition")
                .GetComponent<Transition>()
                .StartTransition(directObject);
        }
    }
}