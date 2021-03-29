using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class ObjectPool: MonoBehaviour
    {
        [SerializeField] 
        private GameObject[] ObjectReferences;

        [HideInInspector]
        private Dictionary<string, GameObject> Objects = new Dictionary<string, GameObject>();

        private void Initialize()
        {
            if (!Objects.Any())
            {
                Objects = ObjectReferences.ToDictionary(t => t.name, t => t);
            }
        }

        public GameObject Get(string name)
        {
            Initialize();
            return Objects[name];
        }
    }
}