using Assets.Utils;
using Assets.Utils.JBehavior;
using UnityEngine;

namespace Assets.Scripts.Shoggoth
{
    public class Shoggoth : MonoBehaviour
    {
        public float LurchStartSpeed = 1;
        public float LurchMinSpeed = 0.5f;
        public float LurchDuration = 2f;

        private ParticleSystem SlimeTrail;

        private JBehaviorSet LurchBehavior;
        private ShoggothAttackArm AttackArm;

        public Shoggoth()
        {
            LurchBehavior = new JBehaviorSet().Then(() =>
                {
                    var slimeTrailEmission = SlimeTrail.emission;
                    slimeTrailEmission.enabled = true;
                    SetVelocity(LurchStartSpeed);
                })
                .Then(LurchDuration, ratio => SetVelocity(LurchStartSpeed - (LurchStartSpeed - LurchMinSpeed) * ratio));
        }

        // Start is called before the first frame update
        void Start()
        {
            SlimeTrail = transform.Find("SlimeTrail").GetComponent<ParticleSystem>();
            AttackArm = GetComponentInChildren<ShoggothAttackArm>();
        }

        void Update()
        {
            var player = Find.ThePlayer;
        
            if (AttackArm.InProgress || LurchBehavior.InProgress || player == null) return;
        
            // Rotate toward player
            var vec = transform.forward;
            var dir = player.transform.position - transform.position;
            var cross = Vector3.Cross(vec, dir);
            var angle = Vector3.Angle(vec, dir);

            // Rotate toward player
            transform.rotation = transform.rotation *
                                 Quaternion.AngleAxis(Mathf.Min(angle, 1) * Mathf.Sign(cross.y), Vector3.up);

            if (angle > 20)
            {
                StopMovement();
                return;
            }

            if (Vector3.Distance(player.transform.position, transform.position) <= 7)
            {
                StopMovement();
                AttackArm.SwingArm();
                return;
            }
            StartLurch();
        }

        public void StartLurch()
        {
            StartCoroutine(LurchBehavior.Start());
        }

        public void StopMovement()
        {
            StopAllCoroutines();
            SetVelocity(0);
            var slimeTrailEmission = SlimeTrail.emission;
            slimeTrailEmission.enabled = false;
        }

        public void SetVelocity(float f)
        {
            gameObject.GetComponent<Rigidbody>()
                .velocity = transform.forward * f;
        }
    }
}