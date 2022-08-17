using Raptor.Game.Shared.GameInput;

namespace Raptor.Game.Client.GameInput
{
    public static class SampleInput
    {
        public static Input Sample()
        {
            return new Input
            {
                Horizontal = (sbyte) UnityEngine.Input.GetAxisRaw("Horizontal"),
                Vertical = (sbyte) UnityEngine.Input.GetAxisRaw("Vertical"),
            };
        }
    }
}