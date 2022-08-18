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
                var players = Object.FindObjectsOfType<LocalPlayer>();

                if (players.Length == 0)
                {
                    return;
                }

                var player = players.Single();
                player.EnqueueSnapshot(message.Payload);
                
                
      
            });
        }
    }
}