using System;
using System.Collections.Generic;
using Assets.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

namespace Assets.Scripts.Player
{
    public class PlayerState : MonoBehaviour
    {
        public bool WeaponsFree = true;
        public readonly BlockerStack MoveBlocker = new BlockerStack();
        public readonly BlockerStack RotateBlocker = new BlockerStack();
        public readonly BlockerStack AimBlocker = new BlockerStack();
        private readonly BlockerStack InputBlocker = new BlockerStack();

        void Start()
        {
            var input = GetComponent<PlayerInput>();

            InputBlocker.Subscribe(blockState =>
            {
                if (blockState == BlockerStack.BlockState.Blocked)
                {
                    input.DeactivateInput();
                }
                else
                {
                    input.ActivateInput();
                }
            });
        }

        public Action BlockAll()
        {
            foreach (var blocker in AllBlockers)
            {
                blocker.Block(this);
            }

            return () =>
            {
                foreach (var blocker in AllBlockers)
                {
                    blocker.Unblock(this);
                }
            };
        }

        private IEnumerable<BlockerStack> AllBlockers => new[]
        {
            MoveBlocker, InputBlocker, AimBlocker, RotateBlocker
        };
    }
}