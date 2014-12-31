
namespace RzRpgLib
{
    public enum GameEntityPropertyCollectionChangedAction
    {
        Add,
        Remove
    }

    public struct GameEntityPropertyCollectionChangedEventArgs
    {
        public GameEntityPropertyCollectionChangedAction Action;
        public IGameEntityProperty ChangedProperty;

        public GameEntityPropertyCollectionChangedEventArgs( GameEntityPropertyCollectionChangedAction action, IGameEntityProperty changedProperty )
        {
            Action = action;
            ChangedProperty = changedProperty;
        }
    }
}
