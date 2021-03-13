using System.Linq;

namespace Assets.Utils.JBehavior
{
    public static class JCurve
    {
        public static readonly float[] FibbonacciUpDown = { 1, 1, 2, 3, 5, 5, 3, 2, 1, 1 };
        public static readonly float[] FibbonacciUp = { 1, 1, 2, 3, 5 };
        public static readonly float[] Linear = { 1.0f };

        public static float CurveRatio(float realRatio, float[] curve)
        {
            if (curve.Length == 1) return realRatio;
            var relativeRatio = 0.0f;
            var sumOfFactors = curve.Sum();
            var ratioPerFactor = 1.0f / curve.Length;
            foreach (var factor in curve.Select((f, index) => new { f, index = index + 1 }))
            {
                var setPoint = ratioPerFactor * factor.index;
                if (realRatio <= setPoint)
                {
                    relativeRatio += factor.f * (1.0f - (setPoint - realRatio) / ratioPerFactor);
                    break;
                }

                relativeRatio += factor.f;
            }

            return relativeRatio / sumOfFactors;
        }
    }
}