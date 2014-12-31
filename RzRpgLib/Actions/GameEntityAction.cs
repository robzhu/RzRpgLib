using System;
using RzAspects;

namespace RzRpgLib
{
    public enum GameEntityActionState
    {
        OnCooldown, //Action is not ready.
        Ready,      //Action is ready to be used.
        Targetting, //Action is awaiting target selection.
        Casting,    //Action is currently being cast.
    }

    public enum CastActionResult
    {
        Cancelled,
        Interrupted,
        Completed,
    }

    public interface IGameEntityAction : IBoundEvaluationComponent
    {
        GameEntityActionState State { get; }

        string DisplayName { get; set; }
        string Name { get; set; }
        object Glyph { get; set; }
        string ExecutionId { get; set; }
        string Description { get; set; }

        /// <summary>
        /// Whether this action can be executed. True when all the requirements are met. False otherwise.
        /// </summary>
        bool CanExecute { get; }

        /// <summary>
        /// The requirements for this action to begin execution.  
        /// </summary>
        ObservableCollectionEx<IActionRequirement> Requirements { get; }

        /// <summary>
        /// The list of effects this action applies when it begins execution.
        /// </summary>
        ObservableCollectionEx<IActionEffect> BeginEffects { get; }

        /// <summary>
        /// The list of effects this action applies each period, as defined by CastTime
        /// </summary>
        ObservableCollectionEx<IActionEffect> PeriodicEffects { get; }

        /// <summary>
        /// The list of effects this action applies when it finished execution.
        /// </summary>
        ObservableCollectionEx<IActionEffect> EndEffects { get; }

        /// <summary>
        /// The frequency with which this action can be executed.
        /// </summary>
        ICooldown InternalCooldown { get; }
        //TODO: add cooldown modifiers to this thing.

        /// <summary>
        /// The cast duration of this action. Null means this action is instant-cast.
        /// </summary>
        IDuration ExecuteDuration { get; }

        /// <summary>
        /// The time it takes to cast this skill. Null means this action is instant-cast.
        /// </summary>
        IStatistic CastTime { get; set; }

        /// <summary>
        /// The time of a period for period effects to be applied. Null means this action has no periodic effects.
        /// </summary>
        IStatistic PeriodTime { get; set; }

        void SetCooldown( float periodInMilliseconds );

        void Cancel( bool isInterrupt = false );

        /// <summary>
        /// Call this method to begin the execution timer if this is an action with a duration. Note that this method does not actually
        /// apply any of the Action's effects. It just starts the timer.
        /// </summary>
        /// <returns>True if casting was successfully begun. False otherwise. </returns>
        bool BeginExecuteDuration();

        event Action<CastActionResult> CastFinished;
        event Action<int> CastPeriodElapsed;
    }

    /// <summary>
    /// Actions are performed by GameEntities that can affect any game entity or the game environment.
    /// Examples: Attack, Heal, Buff, Debuff, Knockback, Move
    /// </summary>
    public class GameEntityAction : ModelBase, IGameEntityAction
    {
        public event Action<CastActionResult> CastFinished;
        public event Action<int> CastPeriodElapsed;

        public string PropertyState { get { return "State"; } }
        private GameEntityActionState _State = GameEntityActionState.OnCooldown;
        public GameEntityActionState State
        {
            get { return _State; }
            private set { SetProperty( PropertyState, ref _State, value, RefreshCanExecute ); }
        }

        public string PropertyDisplayName { get { return "DisplayName"; } }
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { SetProperty( PropertyDisplayName, ref _DisplayName, value ); }
        }

        public string PropertyName { get { return "Name"; } }
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set 
            {
                SetProperty( PropertyName, ref _Name, value );
                if( DisplayName == null ) DisplayName = value;
            }
        }

        public string PropertyGlyph { get { return "Glyph"; } }
        private object _Glyph;
        public object Glyph
        {
            get { return _Glyph; }
            set { SetProperty( PropertyGlyph, ref _Glyph, value ); }
        }

        public string PropertyExecutionId { get { return "ExecutionId"; } }
        private string _ExecutionId;
        public string ExecutionId
        {
            get { return _ExecutionId; }
            set { SetProperty( PropertyExecutionId, ref _ExecutionId, value ); }
        }

        public string PropertyDescription { get { return "Description"; } }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { SetProperty( PropertyDescription, ref _Description, value ); }
        }

        public static string PropertyCanExecute { get { return "CanExecute"; } }
        private bool _CanExecute;
        public bool CanExecute
        {
            get { return _CanExecute; }
            private set { SetProperty( PropertyCanExecute, ref _CanExecute, value ); }
        }

        public string PropertyInternalCooldown { get { return "InternalCooldown"; } }
        private ICooldown _InternalCooldown = new Cooldown();
        public ICooldown InternalCooldown
        {
            get { return _InternalCooldown; }
            private set { SetProperty( PropertyInternalCooldown, ref _InternalCooldown, value ); }
        }

        public string PropertyExecuteDuration { get { return "ExecuteDuration"; } }
        private IDuration _executeDuration = null;
        public IDuration ExecuteDuration
        {
            get { return _executeDuration; }
            private set { SetProperty( PropertyExecuteDuration, ref _executeDuration, value ); }
        }

        public string PropertyCastTime { get { return "CastTime"; } }
        private IStatistic _CastTime = null;
        public IStatistic CastTime
        {
            get { return _CastTime; }
            set { SetProperty( PropertyCastTime, ref _CastTime, value, RefreshExecuteDuration ); }
        }

        public string PropertyPeriodTime { get { return "PeriodTime"; } }
        private IStatistic _PeriodTime;
        public IStatistic PeriodTime
        {
            get { return _PeriodTime; }
            set { SetProperty( PropertyPeriodTime, ref _PeriodTime, value, RefreshExecuteDuration ); }
        }

        public ObservableCollectionEx<IActionRequirement> Requirements { get; private set; }
        public ObservableCollectionEx<IActionEffect> BeginEffects { get; private set; }
        public ObservableCollectionEx<IActionEffect> PeriodicEffects { get; private set; }
        public ObservableCollectionEx<IActionEffect> EndEffects { get; private set; }

        private CooldownRequirement _internalCooldownRequirement;

        public GameEntityAction()
        {
            _internalCooldownRequirement = new CooldownRequirement( InternalCooldown );

            Requirements = new ObservableCollectionEx<IActionRequirement>();
            Requirements.Add( _internalCooldownRequirement );
            Requirements.ItemPropertyChanged += ( o, e ) => RefreshCanExecute();
            Requirements.CollectionChanged += ( o, e ) => RefreshCanExecute();

            RefreshCanExecute();

            BeginEffects = new ObservableCollectionEx<IActionEffect>();
            PeriodicEffects = new ObservableCollectionEx<IActionEffect>();
            EndEffects = new ObservableCollectionEx<IActionEffect>();
        }

        public void SetCooldown( float periodInMilliseconds )
        {
            if (periodInMilliseconds < 0) throw new Exception( "cooldown period cannot be negative" );
            InternalCooldown.Period.BaseValue = periodInMilliseconds;

            if( !Requirements.Contains( _internalCooldownRequirement ) )
            {
                Requirements.Add( _internalCooldownRequirement );
            }
        }

        public void Cancel( bool isInterrupt = false )
        {
            if( State == GameEntityActionState.Casting )
            {
                State = GameEntityActionState.OnCooldown;
                if( ExecuteDuration != null )
                {
                    ExecuteDuration.Pause();
                }

                if( CastFinished != null )
                {
                    if( isInterrupt )
                        CastFinished( CastActionResult.Interrupted );
                    else 
                        CastFinished( CastActionResult.Cancelled );
                }
            }
        }

        public bool BeginExecuteDuration()
        {
            if( CastTime == null ) return false;

            State = GameEntityActionState.Casting;

            if( PeriodTime == null )
            {
                ExecuteDuration.Reset( CastTime.Value );
            }
            else//( PeriodTime != null )
            {
                ExecuteDuration.Reset( CastTime.Value, PeriodTime.Value );
            }

            return true;
        }

        private void RefreshExecuteDuration()
        {
            if( ( CastTime != null ) && ( CastTime.Value > 0 ) )
            {
                if( ExecuteDuration == null )
                {
                    ExecuteDuration = Duration.CreateWithCustomUpdateGroup( UpdateGroupIds.GameLogic, CastTime.Value, false );

                    ExecuteDuration.OnPeriodicDurationElapsed += ( n ) =>
                        {
                            if ( CastPeriodElapsed != null ) CastPeriodElapsed( n );
                        };
                    ExecuteDuration.OnTotalDurationElapsed += () =>
                        {
                            if ( CastFinished != null ) CastFinished( CastActionResult.Completed );
                            State = GameEntityActionState.OnCooldown;
                        };
                }
            }
        }

        protected void RefreshCanExecute()
        {
            if ( State == GameEntityActionState.Casting || State == GameEntityActionState.Targetting )
            {
                CanExecute = false;
                return;
            }

            foreach( IActionRequirement requirement in Requirements )
            {
                if( !requirement.RequirementMet )
                {
                    CanExecute = false;
                    return;
                }
            }

            CanExecute = true;
            if( State == GameEntityActionState.OnCooldown ) State = GameEntityActionState.Ready;
        }

        public void Bind( IGameEntity parent )
        {
            if( parent == null ) return;

            //TODO: change IActionRequirement to be an IBoundEvaluationComponent
            foreach( IActionRequirement requirement in Requirements )
            {
                //TODO: perhaps this code should be pushed into a Bind() method no the IActionRequirement.
                IResourceRequirement resourceReq = requirement as IResourceRequirement;
                if( resourceReq != null )
                {
                    IResourceProperty resourceProperty = parent.GetProperty( resourceReq.ResourceName ) as IResourceProperty;
                    if( resourceProperty != null )
                    {
                        resourceReq.Resource = resourceProperty;
                    }
                }

                IPropertyStateRequirement propertyStateRequirement = requirement as IPropertyStateRequirement;
                if( propertyStateRequirement != null )
                {
                    IGameEntityProperty property = parent.GetProperty( propertyStateRequirement.PropertyName );
                    if( property != null )
                    {
                        propertyStateRequirement.TargetProperty = property;
                    }
                }
            }

            foreach( IActionEffect effect in EndEffects )
            {
                var bindEffect = effect as IBoundEvaluationComponent;
                if( bindEffect != null )
                {
                    bindEffect.Bind( parent );
                }
            }
        }
    }
}
