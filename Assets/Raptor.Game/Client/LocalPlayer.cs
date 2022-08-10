using Raptor.Game.Shared;
using UnityEngine;

namespace Raptor.Game.Client
{
    public class LocalPlayer : MonoBehaviour
    {
        public string Id;
        
        public void Setup(PlayerInfo info)
        {
            Id = info.PlayerId.ToString();
        }
    }
}