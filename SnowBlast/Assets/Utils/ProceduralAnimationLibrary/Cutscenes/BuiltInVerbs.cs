#nullable enable
using System;
using Assets.Scripts.Scene;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using UnityEngine;

namespace Assets.Utils.ProceduralAnimationLibrary.Cutscenes
{
    public static class BuiltInVerbs
    {
        [CutsceneVerb]
        public static ITween MoveTo(GameObject subject, GameObject directObject, float? speed)
        {
            var velocity = speed ?? 5f;

            return new SerialTweener().Computed(
                () => new
                {
                    position = subject.transform.position,
                    target = directObject.transform.position.ReplaceY(subject.transform.position.y)
                },
                initial => Vector3.Distance(initial.position, initial.target) / velocity,
                (initial, ratio) =>
                {
                    subject.transform.position = initial.position + ratio * (initial.target - initial.position);
                });
        }

        [CutsceneVerb]
        public static ITween Face(GameObject subject, GameObject directObject, float? degreesPerSecond)
        {
            var velocity = degreesPerSecond ?? 360f;

            return new SerialTweener().Computed(
                () =>
                {
                    // Rotate subject to face directObject
                    var vec = subject.transform.forward;
                    var dir = directObject.transform.position - subject.transform.position;
                    var cross = Vector3.Cross(vec, dir);
                    var angle = Vector3.Angle(vec, dir);

                    return new
                    {
                        facing = subject.transform.rotation,
                        angle,
                        sign = Mathf.Sign(cross.y)
                    };
                },
                initial => initial.angle / velocity,
                (initial, ratio) =>
                {
                    subject.transform.rotation = initial.facing *
                                                 Quaternion.AngleAxis(ratio * initial.angle * initial.sign, Vector3.up);
                });
        }

        [CutsceneVerb]
        public static ITween Say(GameObject subject, string directObject)
        {
            var done = false;
            Action? unsay = null;
            return new SerialTweener().Then(() =>
            {
                unsay = Sayer.Say(subject, directObject, true);

                Find.SceneState.SetInteraction(new InteractionAction(() =>
                {
                    done = true;
                }), false);
            }).While(() => !done).Then(() => unsay?.Invoke());
        }

        [CutsceneVerb]
        public static ITween LoadScene(string directObject)
        {
            return new ActionTween(() => { Utils.TransitionToScene(directObject); });
        }

        [CutsceneVerb]
        public static ITween Disable(GameObject directObject)
        {
            return new ActionTween(() => directObject.SetActive(false));
        }
    }
}