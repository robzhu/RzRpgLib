using System;
using RzAspects;

namespace RzRpgLib
{
    /// <summary>
    /// Effect that performs an action periodically.
    /// </summary>
    public class PeriodicEffect : GameEntityEffect
    {
        /// <summary>
        /// This event is raised once every {periodSpan}.  After the total span has elapsed, this event will be set to null.
        /// </summary>
        public event Action<int> Tick;

        /// <summary>
        /// This event is raised when the total span has elapsed.  Once raised, it will automatically the event will be set to null.
        /// </summary>
        public event Action Completed;

        public Action<int> OnTick
        {
            set { Tick += value; }
        }

        private double _totalSpan;
        private double _periodSpan;
        private bool _callInitially;

        public PeriodicEffect( double totalSpan, double periodSpan, bool callInitially = false) : base( null, null )
        {
            _totalSpan = totalSpan;
            _periodSpan = periodSpan;
            _callInitially = callInitially;
        }

        protected override void ApplyPersistInternal( IGameEntity target )
        {
            if( target == null ) return;
            if( !target.HasPersistentEffect( this ) )
            {
                target.AddPersistentEffect( this );
            }
            else
            {
                Duration = RzAspects.Duration.CreateWithCustomUpdateGroup( UpdateGroupIds.GameLogic, _totalSpan, true, _periodSpan );
                Duration.OnPeriodicDurationElapsed += ( periodCount ) =>
                    {
                        RaiseTick( periodCount );
                    };
                Duration.OnTotalDurationElapsed += () =>
                    {
                        RaiseCompleted();
                    };

                if( _callInitially )
                    RaiseTick( 0 );
            }
        }

        protected override void ApplyInstantInternal( IGameEntity target )
        {
            throw new NotImplementedException();
        }

        protected override void UnapplyInternal( IGameEntity target )
        {
            if( target == null ) return;
            if( target.HasPersistentEffect( this ) )
            {
                target.RemovePersistentEffect( this );
            }
        }

        private void RaiseTick( int periodCount )
        {
            if (Tick != null) Tick( periodCount );
        }

        private void RaiseCompleted()
        {
            if (Completed != null) Completed();
            Tick = null;
            Completed = null;
        }
    }
}
