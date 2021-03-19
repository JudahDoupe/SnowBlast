using Assets.Utils.JBehavior;
using UnityEngine;

#nullable enable
namespace Assets.Scripts.Shoggoth
{
    public class ShoggothAttackArm : MonoBehaviour
    {
        public float ExtensionTime = 1.0f;
        public float SwingTime = 1.0f;
        private GameObject Arm = null!;

        public bool InProgress => Animation?.InProgress == true;

        private readonly JBehaviorSet Animation;

        public ShoggothAttackArm()
        {
            Animation = new JBehaviorSet()
                .Then(() =>
                {
                    Arm.GetComponent<AttackArmCollider>().HitDetected = false;
                    transform.rotation = Quaternion.AngleAxis(180.0f, Vector3.up);
                    Arm.gameObject.SetActive(true);
                })
                .Then(ExtensionTime, ExtendArm, JCurve.FibbonacciUp)
                .Then(SwingTime, SwingArm, JCurve.FibbonacciUpDown)
                .Then(ExtensionTime, RetractArm, JCurve.FibbonacciUp)
                .Then(() => Arm!.gameObject.SetActive(false));
        }

        void Start()
        {
            Arm = transform.Find("AttackArm").gameObject;
        }

        public void SwingArm()
        {
            StartCoroutine(Animation.Start());
        }

        private void SwingArm(float ratio)
        {
            transform.rotation = Quaternion.AngleAxis(180 - 180 * ratio,
                Vector3.up);
        }

        private void ExtendArm(float ratio)
        {
            var localScale = Arm.transform.localScale;
            Arm.transform.localScale = new Vector3(localScale.x,
                ratio, localScale.z);
        }

        private void RetractArm(float ratio)
        {
            var localScale = Arm.transform.localScale;
            Arm.transform.localScale = new Vector3(localScale.x,
                1.0f - ratio, localScale.z);
        }
    }
}
