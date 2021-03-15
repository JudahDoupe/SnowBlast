using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts.Turret
{
    public class TurretPivot : MonoBehaviour
    {
        public float PivotSpeed;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var player = Find.ThePlayer;
            if (player != null)
            {
                var lookQuat = Quaternion.LookRotation(player.transform.position - gameObject.transform.position, Vector3.up);
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, lookQuat, PivotSpeed * Time.deltaTime);
            }
        }
    }
}
