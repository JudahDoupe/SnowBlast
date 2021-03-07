using UnityEngine;

namespace Assets.Scripts
{
    public class RotateTowardsCamera: MonoBehaviour
    {
        public void Update()
        {
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        }
    }
}