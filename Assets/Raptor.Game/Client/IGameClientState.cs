namespace Raptor.Game.Client
{
    public interface IGameClientState
    {
        public void OnEnter(GameClient gameClient);
        public void Present(GameClient gameClient);
        public void OnExit(GameClient gameClient);
    }
}