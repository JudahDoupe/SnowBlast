using System.Collections;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweens
{
    public interface IEnumeratorTween : ITween
    {
        IEnumerator Start();
    }
}