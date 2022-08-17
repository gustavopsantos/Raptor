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