using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class ObjectPool: MonoBehaviour
    {
        public static ObjectPool Instance;

        [SerializeField] 
        private GameObject[] ObjectReferences;

        [HideInInspector]
        public Dictionary<string, GameObject> Objects = new Dictionary<string, GameObject>();

        void Start()
        {
            Instance = this;
            Objects = ObjectReferences.ToDictionary(t => t.name, t => t);
        }
    }
}