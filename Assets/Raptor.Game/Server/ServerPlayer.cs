using System.Collections.Generic;
using System.Net;
using Raptor.Game.Shared;
using UnityEngine;
using Input = Raptor.Game.Shared.GameInput.Input;

namespace Raptor.Game.Server
{
    public class ServerPlayer : MonoBehaviour
    {
        public string Id;
        public IPEndPoint EndPoint;
        public List<Ticked<Input>> CommandBuffer;
    }
}