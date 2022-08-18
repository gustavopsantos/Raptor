using System;
using System.Collections.Generic;
using UnityEngine;
using Raptor.Game.Shared;
using Raptor.Game.Shared.Timing;
using Input = Raptor.Game.Shared.GameInput.Input;

namespace Raptor.Game.Client
{
    public class LocalPlayer : MonoBehaviour
    {
        public string Id;
        private Timer _timer;
        private Snapshot _lastConfirmedSnapshot;
        private readonly List<Ticked<Input>> _unconfirmedInputs = new();
        
        private Vector2 _position;

        public Vector2 Position
        {
            set
            {
                _position = value;
                UnityMainThreadDispatcher.Instance().Enqueue(() => { transform.position = value; });
            }
            get
            {
                return _position;
            }
        }

        public void Setup(PlayerInfo info)
        {
            Id = info.PlayerId.ToString();
        }
        
        public void SetupTimer(Timer timer)
        {
            _timer = timer;
        }

        public void IncrementInput(double tick, Input input) // Called when a fixed input is processed by client timer
        {
            // Assert.IsTrue(_unconfirmedInputs.Count(ui => ui.Tick.Equals((int) tick)) == 0);
            var tickedInput = new Ticked<Input>((int) tick, input);
            _unconfirmedInputs.Add(tickedInput);
            ApplyInput(tickedInput, this);
            
            //throw new System.NotImplementedException();
        }
        
        private void ApplyInput(Ticked<Input> tickedInput, LocalPlayer localPlayer)
        {
            var input = new Vector2(tickedInput.Value.Horizontal, tickedInput.Value.Vertical);
            var translation = (float) Configuration.TickInterval.TotalSeconds * 4 * input;
            var position = localPlayer.Position + translation;
            localPlayer.Position = position;
        }

        public void Update()
        {
            if (_timer == null)
            {
                return;
            }
            
            var tickNow = _timer.CalculateTickNow(TimeSpan.Zero);
            var tickRender = _timer.CalculateTickNow(-Configuration.InterpolationWindow);
            
            Debug.Log($"now '{tickNow}' render '{tickRender}'");
        }

        public void EnqueueSnapshot(Snapshot snapshot) // FixedUpdate
        {
            _lastConfirmedSnapshot = snapshot;
            _unconfirmedInputs.RemoveAll(ui => ui.Tick <= snapshot.Position.Tick);
            
            // Apply snapshot hard
            var positionTuple = snapshot.Position.Value;
            var position = new Vector2(positionTuple.Item1, positionTuple.Item2);
            transform.position = position;
            
            // // Reapply all pending inputs
            foreach (var unconfirmedInput in _unconfirmedInputs)
            {
                ApplyInput(unconfirmedInput, this);
            }
        }
    }
}