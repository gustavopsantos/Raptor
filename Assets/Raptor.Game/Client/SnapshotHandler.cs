using System.Linq;
using Raptor.Game.Shared;
using Raptor.Interface;
using UnityEngine;

namespace Raptor.Game.Client
{
    public class SnapshotHandler : IMessageHandler<Snapshot>
    {
        public void Handle(Message<Snapshot> message)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                var player = Object.FindObjectsOfType<LocalPlayer>().Single();
                var positionTuple = message.Payload.Position.Value;
                var position = new Vector2(positionTuple.Item1, positionTuple.Item2);
                player.transform.position = position;
            });
        }
    }
}