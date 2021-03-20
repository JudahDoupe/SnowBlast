using Assets.Utils.JBehavior;
using UnityEngine;

namespace Assets.Utils
{
    public abstract class MyMonoBehaviour : MonoBehaviour
    {
        public JStartableBehavior BeginBehavior => new JStartableBehavior(this);
    }
}