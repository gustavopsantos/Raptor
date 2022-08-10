using JetBrains.Annotations;
using UnityEngine;

namespace Raptor.Game.Server
{
    public class GameServer : MonoBehaviour
    {
        private IGameServerState _current;
        public RaptorClient Client { get; set; }

        private void Start()
        {
            SwitchState(new GameServerStoppedState());
        }

        private void OnDestroy()
        {
            SwitchState(null);
        }

        private void OnGUI()
        {
            using (new GUILayout.AreaScope(new Rect(Screen.width - 8 - 200, 8, 200, 100)))
            {
                using (new GUILayout.VerticalScope("box", GUILayout.Width(200)))
                {
                    GUILayout.Label("Game Server");
                    _current.Present(this);
                }
            }
        }

        public void SwitchState([CanBeNull] IGameServerState state)
        {
            _current?.OnExit(this);
            _current = state;
            _current?.OnEnter(this);
        }
    }
}