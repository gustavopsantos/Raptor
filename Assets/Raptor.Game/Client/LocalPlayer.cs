using System;
using System.Collections.Generic;
using System.Linq;
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

        public void IncrementInput(int tick, Input input) // Called when a fixed input is processed by client timer
        {
            if (_unconfirmedInputs.Count(ui => ui.Tick.Equals((int) tick)) != 0)
            {
                // Debug.LogError($"deu ruim {tick}");
            }
            
            var tickedInput = new Ticked<Input>(tick, input);
            _unconfirmedInputs.Add(tickedInput);
            ApplyInput(tickedInput, this);
            
            //throw new System.NotImplementedException();
        }
        
        private void ApplyInput(Ticked<Input> tickedInput, LocalPlayer localPlayer)
        {
            var input = new Vector2(tickedInput.Value.Horizontal, tickedInput.Value.Vertical);
            var translation = (float) Configuration.TickInterval.TotalSeconds * Configuration.PlayerSpeed * input;
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

            var normal = (float)(tickRender - Math.Truncate(tickRender));
            var tickLeft = (int)tickRender;
            var tickRight = tickLeft + 1;
            
            // Apply snapshot hard
            var leftPosition = GetPositionAtTick(tickLeft);
            var rightPosition = GetPositionAtTick(tickRight);
            transform.position = Vector2.Lerp(leftPosition, rightPosition, normal);
        }

        private Vector2 GetPositionAtTick(int tick)
        {
            if (tick == _lastConfirmedSnapshot.Position.Tick)
            {
                return new Vector2(
                    _lastConfirmedSnapshot.Position.Value.Item1,
                    _lastConfirmedSnapshot.Position.Value.Item2);
            }
            
            if (tick < _lastConfirmedSnapshot.Position.Tick)
            {
                Debug.LogError($"Tick is to low {tick} {_lastConfirmedSnapshot.Position.Tick}");
            }
            
            // if (tick < _lastConfirmedSnapshot.Position.Tick || _unconfirmedInputs.Count == 0 || tick > _unconfirmedInputs.Last().Tick)
            // {
            //     Debug.LogError("Something bad is going on");
            // }
            
            if (_unconfirmedInputs.Count != 0 && tick > _unconfirmedInputs.Last().Tick)
            {
                Debug.LogError($"Tick is too high {tick} {_unconfirmedInputs.Last().Tick}");
            }

            var position = new Vector2(
                _lastConfirmedSnapshot.Position.Value.Item1,
                _lastConfirmedSnapshot.Position.Value.Item2);

            foreach (var unconfirmedInput in _unconfirmedInputs.Where(t => t.Tick <= tick))
            {
                var input = new Vector2(unconfirmedInput.Value.Horizontal, unconfirmedInput.Value.Vertical);
                var translation = (float) Configuration.TickInterval.TotalSeconds * Configuration.PlayerSpeed * input;
                position += translation;
            }
            
            return position;
        }
        
        public void EnqueueSnapshot(Snapshot snapshot) // FixedUpdate
        {
            _lastConfirmedSnapshot = snapshot;
            _unconfirmedInputs.RemoveAll(ui => ui.Tick <= snapshot.Position.Tick);
            
            // // Apply snapshot hard
            // var positionTuple = snapshot.Position.Value;
            // var position = new Vector2(positionTuple.Item1, positionTuple.Item2);
            // transform.position = position;
            
            // // Reapply all pending inputs
            // foreach (var unconfirmedInput in _unconfirmedInputs)
            // {
            //     ApplyInput(unconfirmedInput, this);
            // }
        }
    }
}