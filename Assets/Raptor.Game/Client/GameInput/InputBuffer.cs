using UnityEngine;

namespace Raptor.Game.Client.GameInput
{
    public class InputBuffer : MonoBehaviour
    {
        private Shared.GameInput.Input _buffered;

        private void Update()
        {
            var input = SampleInput.Sample();
            _buffered.Horizontal = input.Horizontal != 0 ? input.Horizontal : _buffered.Horizontal;
            _buffered.Vertical = input.Vertical != 0 ? input.Vertical : _buffered.Vertical;
        }

        public Shared.GameInput.Input Consume()
        {
            var copy = _buffered;
            _buffered.Clear();
            return copy;
        }
    }
}