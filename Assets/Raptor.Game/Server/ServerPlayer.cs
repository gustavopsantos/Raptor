using System.Net;
using UnityEngine;
using Raptor.Game.Shared;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Raptor.Game.Shared.Generics;
using Input = Raptor.Game.Shared.GameInput.Input;

namespace Raptor.Game.Server
{
    public class ServerPlayer : MonoBehaviour
    {
        public string Id;
        public IPEndPoint EndPoint;
        public ConcurrentList<Ticked<Input>> CommandBuffer;

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
    }
}