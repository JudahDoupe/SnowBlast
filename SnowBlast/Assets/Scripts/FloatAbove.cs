using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class FloatAbove : MonoBehaviour
    {
        private GameObject Target;

        public void SetTarget(GameObject target)
        {
            Target = target;
            MoveToPosition();
        }

        public void FixedUpdate()
        {
            MoveToPosition();
        }

        private void MoveToPosition()
        {
            if (Target == null) return;

            var parentObjectHeight = Target.GetBounds().max.y;
            var myBounds = this.GetBounds();

            var targetPosition = Target.transform.position;

            gameObject.transform.position = new Vector3(targetPosition.x, parentObjectHeight + myBounds.extents.y,
                targetPosition.z);
        }
    }
}