using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerState : MonoBehaviour
    {
        public bool WeaponsFree = true;
        public readonly BlockerStack MoveBlocker = new BlockerStack();
        public readonly BlockerStack RotateBlocker = new BlockerStack();
        public readonly BlockerStack InputBlocker = new BlockerStack();

        public bool AttackAllowed => WeaponsFree && !InputBlocker.Blocked;

        public IDisposable BlockAll() =>
            MoveBlocker.Block()
                .Then(RotateBlocker.Block())
                .Then(InputBlocker.Block());
    }

    public class BlockerStack
    {
        private int Next = 0;
        public bool Blocked => Blockers.Any();

        private readonly HashSet<int> Blockers = new HashSet<int>();

        public IDisposable Block()
        {
            var item = Next++;
            Blockers.Add(item);
            return new ActionDisposer(() => Blockers.Remove(item));
        }
    }
}