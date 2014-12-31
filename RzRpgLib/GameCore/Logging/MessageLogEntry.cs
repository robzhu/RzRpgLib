using System;
using RzAspects;

namespace RzRpgLib
{
    public interface IMessageLogEntry
    {
        DateTime Timestamp { get; }
        string Message { get; }
    }

    public abstract class LogEntryBase : IMessageLogEntry
    {
        public DateTime Timestamp { get; private set; }
        public virtual string Message { get; protected set; }

        public LogEntryBase()
        {
            Timestamp = DateTime.Now;
        }
    }

    public class StringLogEntry : LogEntryBase
    {
        public StringLogEntry( string message ) : base()
        {
            Message = message;
        }
    }

    public class EffectLogEntry : LogEntryBase
    {
        public IGameEntityActor Actor { get; set; }
        public IGameEntityAction Action { get; set; }
        public IGameEntityActor Target { get; set; }
        public IActionEffect ActionEffect { get; set; }
        public IGameEntityEffect EntityEffect { get; set; }
        public float EffectValue { get; set; }

        // ToString Examples:
        // [Actor]'s [Action] hits [Target] for [EffectValue] Damage.
        // [Actor]'s [Action] crits [Target] for [EffectValue] Damage.
        // [Actor]'s [Action] heals [Actor] for [EffectValue] Health.
        // [Hero]'s [Warcry] buffs [Hero] with [Bravery].
        private string _message = null;
        public override string Message
        {
            get
            {
                if( _message == null )
                {
                    RefreshMessage();
                }
                return _message;
            }
        }

        public void RefreshMessage()
        {
            _message = ActionEffect.GetMessageLogString( this );
        }
    }
}
