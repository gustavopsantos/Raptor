using System;
using Raptor.Game.Shared;
using UnityEngine;

namespace Raptor.Game.Server
{
    public class ServerPlayer : MonoBehaviour
    {
        public string Id;

        public void Setup(PlayerInfo info)
        {
            Id = info.PlayerId.ToString();
        }
    }
}