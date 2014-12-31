using System;

namespace RzRpgLib
{
    public interface IEffectProcessor
    {
        void Process( IGameEntityAction action, IActionEffect effect, IGameEntityActor actor, object effectTargetContext = null );
    }

    /// <summary>
    /// EffectProcessor are handlers for applying effects.  They enforce/apply game rules, and apply effects to the target(s).
    /// Example: different EffectProcessor could exist for processing Damage and Healing effects.
    /// </summary>
    public abstract class EffectProcessor : IEffectProcessor
    {
        protected IMessageLog CombatLog { get; private set; }

        public EffectProcessor(IMessageLog combatLog)
        {
            if( combatLog == null ) throw new ArgumentNullException( "combatLog cannot be null" );
            CombatLog = combatLog;
        }

        public void Process( IGameEntityAction action, IActionEffect effect, IGameEntityActor actor, object effectTargetContext = null )
        {
            ProcessInternal( action, effect, actor, effectTargetContext );
        }

        protected abstract void ProcessInternal( IGameEntityAction action, IActionEffect effect, IGameEntityActor actor, object effectTargetContext = null );
    }
}
