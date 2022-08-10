namespace Raptor.Game.Server
{
    public interface IGameServerState
    {
        public void OnEnter(GameServer gameServer);
        public void Present(GameServer gameServer);
        public void OnExit(GameServer gameServer);
    }
}