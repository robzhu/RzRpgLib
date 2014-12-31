using System;
using System.Collections.Generic;

namespace RzRpgLib
{
    public interface IActionExecutor
    {
        ActionResult Execute( IGameEntityAction action, IGameEntityActor actor, object targetContext );
    }

    /// <summary>
    /// Action executors are handlers for actions.  They check requirements, deduct resource costs, enforce/apply game rules, and request the resultant effects 
    /// to be processed.
    /// </summary>
    public abstract class ActionExecutor : IActionExecutor
    {
        protected IMessageLog CombatLog { get; private set; }
        protected IEffectProcessor EffectHandler { get; private set; }

        public ActionExecutor( IMessageLog combatLog, IEffectProcessor effectProcessor )
        {
            if( combatLog == null ) throw new ArgumentNullException( "combatLog cannot be null" );
            if (effectProcessor == null) throw new ArgumentNullException( "effectProcessor cannot be null" );
            CombatLog = combatLog;
            EffectHandler = effectProcessor;
        }

        public ActionResult Execute( IGameEntityAction action, IGameEntityActor actor, object targetContext )
        {
            if( ( action == null ) || ( actor == null ) || ( targetContext == null ) ) return new ActionResult( "Action, actor, or targetContext is null" );

            //Check to see if the action has enough resources.
            foreach( IActionRequirement requirement in action.Requirements )
            {
                if( !requirement.RequirementMet )
                {
                    //Example error string: "Cannot execute {Herioc Strike}, {not enough rage}"
                    string error = string.Format( "Cannot execute {0}, {1}", action.DisplayName, requirement.ErrorString );
                    return new ActionResult( error );
                }
            }

            //Give the implementor of this class a chance to check if the action can be executed.
            string canExecuteError = CanExecuteAction( action, actor, targetContext );
            if( canExecuteError != null ) return new ActionResult( canExecuteError );

            //Use all the requirements
            foreach( IActionRequirement requirement in action.Requirements )
            {
                requirement.Use();
            }

            return ExecuteInternal( action, actor, targetContext );
        }

        protected virtual string CanExecuteAction( IGameEntityAction action, IGameEntityActor actor, object targetContext )
        {
            return null;
        }

        protected abstract ActionResult ExecuteInternal( IGameEntityAction action, IGameEntityActor actor, object targetContext );

        protected void ProcessActionBeginEffects( IGameEntityAction action, IGameEntityActor actor, object targetContext )
        {
            ProcessActionEffects( action.BeginEffects, action, actor, targetContext );
        }

        protected void ProcessActionPeriodictEffects( IGameEntityAction action, IGameEntityActor actor, object targetContext )
        {
            ProcessActionEffects( action.PeriodicEffects, action, actor, targetContext );
        }

        protected void ProcessActionEndEffects( IGameEntityAction action, IGameEntityActor actor, object targetContext )
        {
            ProcessActionEffects( action.EndEffects, action, actor, targetContext );
        }

        private void ProcessActionEffects( IEnumerable<IActionEffect> effects, IGameEntityAction action, IGameEntityActor actor, object targetContext )
        {
            if( effects == null ) return;
            foreach( IActionEffect actionEffect in effects )
            {
                EffectHandler.Process( action, actionEffect, actor, targetContext );
            }
        }
    }
}
