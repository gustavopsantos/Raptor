using System.Net;
using Raptor.Interface;
using Raptor.Game.Shared;
using System.Collections.Generic;
using Raptor.Game.Shared.GameInput;

namespace Raptor.Game.Server.GameInput
{
    public class InputMessageHandler : IMessageHandler<Ticked<Input>>
    {
        private readonly Dictionary<IPEndPoint, ServerPlayer> _players;

        public InputMessageHandler(Dictionary<IPEndPoint, ServerPlayer> players)
        {
            _players = players;
        }

        public void Handle(Message<Ticked<Input>> message)
        {
            var serverPlayer = _players[message.Source];
            serverPlayer.CommandBuffer.Add(message.Payload);
        }
    }
}