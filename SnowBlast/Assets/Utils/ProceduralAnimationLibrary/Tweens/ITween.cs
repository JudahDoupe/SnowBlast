namespace Assets.Utils.ProceduralAnimationLibrary.Tweens
{
    public interface ITween
    {
        public void Instant();
    }

    public static class TweenExtensions
    {
        public static ITween ToInstant(this ITween self)
        {
            return new ActionTween(() => self.Instant());
        }
    }
}