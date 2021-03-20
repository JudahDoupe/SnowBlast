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
        public readonly LogicalOrSet WeaponsBlocked = new LogicalOrSet();
        public readonly LogicalOrSet MoveBlocked = new LogicalOrSet();
        public readonly LogicalOrSet RotateBlocked = new LogicalOrSet();
        public readonly LogicalOrSet AimBlocked = new LogicalOrSet();
        private readonly LogicalOrSet InputBlocked = new LogicalOrSet();

        void Start()
        {
            var input = GetComponent<PlayerInput>();
            InputBlocked.Subscribe(inputBlocked =>
            {
                if (inputBlocked)
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
                blocker.Add(this);
            }

            return () =>
            {
                foreach (var blocker in AllBlockers)
                {
                    blocker.Remove(this);
                }
            };
        }

        private IEnumerable<LogicalOrSet> AllBlockers => new[]
        {
            MoveBlocked, InputBlocked, AimBlocked, RotateBlocked, WeaponsBlocked
        };
    }
}