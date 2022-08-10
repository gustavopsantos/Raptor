using UnityEngine;
using JetBrains.Annotations;

namespace Raptor.Game.Client
{
    public class GameClient : MonoBehaviour
    {
        private IGameClientState _current;
        public RaptorClient Client { get; set; }

        private void Start()
        {
            SwitchState(new GameClientDisconnectedState());
        }

        private void OnDestroy()
        {
            SwitchState(null);
        }

        private void OnGUI()
        {
            using (new GUILayout.AreaScope(new Rect(8, 8, Screen.width - 16, Screen.height - 16)))
            {
                using (new GUILayout.VerticalScope("box", GUILayout.Width(240)))
                {
                    GUILayout.Label("Game Client");
                    _current.Present(this);
                }
            }
        }

        public void SwitchState([CanBeNull] IGameClientState state)
        {
            _current?.OnExit(this);
            _current = state;
            _current?.OnEnter(this);
        }
    }
}