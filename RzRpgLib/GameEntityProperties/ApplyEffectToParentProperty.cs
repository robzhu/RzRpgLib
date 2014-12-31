
namespace RzRpgLib
{
    public interface IApplyEffectToParentProperty : IGameEntityProperty
    {
        IGameEntityEffect Effect { get; }
    }

    /// <summary>
    /// This property contains an effect that is applied to the parent whenever the owner of this property is equipped as an accessory.
    /// </summary>
    public class ApplyEffectToParentProperty : GameEntityProperty, IApplyEffectToParentProperty
    {
        public IGameEntityEffect Effect { get; private set; }

        public ApplyEffectToParentProperty(string name, IGameEntityEffect effect)
            : base( name, effect ) 
        {
            Effect = effect;
        }
    }
}
