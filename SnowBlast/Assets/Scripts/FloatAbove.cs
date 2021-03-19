using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class FloatAbove : MonoBehaviour
    {
        public GameObject Target;

        public void FixedUpdate()
        {
            if (Target == null) return;

            var parentObjectHeight = Target.GetMaxBounds().max.y;
            var myHeight = gameObject.GetComponent<Canvas>().GetComponent<RectTransform>().rect.height;

            var targetPosition = Target.transform.position;

            gameObject.transform.position = new Vector3(targetPosition.x, targetPosition.y + parentObjectHeight + myHeight + 1, 
                targetPosition.z);
        }
    }
}