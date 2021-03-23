using System;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweens
{
    public class ActionTween : ITween
    {
        private readonly Action Callback;

        public ActionTween(Action callback)
        {
            Callback = callback;
        }

        public void Act() => Callback();
    }
}