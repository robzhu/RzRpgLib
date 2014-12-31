using System;
using RzAspects;

namespace RzRpgLib
{
    public interface IMessageLog
    {
        event Action<IMessageLogEntry> EntryAdded;
        void AddEntry( IMessageLogEntry entry );
        void AddString( string message );
    }

    public class MessageLog : IMessageLog
    {
        public event Action<IMessageLogEntry> EntryAdded;
        public ObservableCollectionEx<IMessageLogEntry> Messages { get; private set; }

        public MessageLog()
        {
            Messages = new ObservableCollectionEx<IMessageLogEntry>();
        }

        public void AddEntry( IMessageLogEntry entry )
        {
            Messages.Add( entry );
            if( EntryAdded != null )
            {
                EntryAdded( entry );
            }
        }

        public void AddString( string message )
        {
            AddEntry( new StringLogEntry( message ) );
        }
    }
}
