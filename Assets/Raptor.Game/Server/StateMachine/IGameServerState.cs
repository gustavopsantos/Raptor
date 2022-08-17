namespace Raptor.Game.Server.StateMachine
{
    public interface IGameServerState
    {
        public void OnEnter(GameServer gameServer);
        public void Present(GameServer gameServer);
        public void OnExit(GameServer gameServer);
    }
}