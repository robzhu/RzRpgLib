using System;
using RzAspects;

namespace RzRpgLib
{
    /// <summary>
    /// Tracks how often something can be used.
    /// </summary>
    /// <example>HP, Mana, experience, rage, energy, global cooldown, ammo, heat, gold</example>
    public interface ICooldown : IUpdatable
    {
        event Action<ICooldown> IsAvailableChanged;

        /// <summary>
        /// Gets or sets whether the cooldown is available for use.
        /// </summary>
        bool IsAvailable { get; set; }

        /// <summary>
        /// The time, in milliseconds, until this cooldown becomes available for use again.
        /// </summary>
        double RemainingCooldown { get; }

        /// <summary>
        /// The RemainingCooldown, expressed on a ratio scale.  1.0 indicates that the entire cooldown remains.  0 means the cooldown has ended.
        /// </summary>
        float RemainingCooldownRatio { get; }

        /// <summary>
        /// The recharge period for using this resource, in milliseconds.
        /// </summary>
        IStatistic Period { get; }

        /// <summary>
        /// Depletes the cool resource and places it on cooldown.
        /// </summary>
        bool Use();
    }

    public class Cooldown : UpdatableModel, ICooldown
    {
        public event Action<ICooldown> IsAvailableChanged;

        public string PropertyIsAvailable { get { return "IsAvailable"; } }
        private bool _IsAvailable = true;
        public bool IsAvailable
        {
            get { return _IsAvailable; }
            set { SetProperty( PropertyIsAvailable, ref _IsAvailable, value ); }
        }

        public string PropertyRemainingCooldown { get { return "RemainingCooldown"; } }
        private double _RemainingCooldown;
        public double RemainingCooldown
        {
            get { return _RemainingCooldown; }
            set { SetProperty( PropertyRemainingCooldown, ref _RemainingCooldown, value ); }
        }

        public string PropertyRemainingCooldownRatio { get { return "RemainingCooldownRatio"; } }
        private float _RemainingCooldownRatio;
        public float RemainingCooldownRatio
        {
            get { return _RemainingCooldownRatio; }
            set { SetProperty( PropertyRemainingCooldownRatio, ref _RemainingCooldownRatio, value ); }
        }

        public string PropertyPeriod { get { return "Period"; } }
        private IStatistic _period = new Statistic();
        public IStatistic Period{ get { return _period; } }

        private object _sync = new object();

        public Cooldown(float cooldownInMilliseconds = 0) 
        {
            Period.BaseValue = cooldownInMilliseconds;
        }

        public bool Use()
        {
            lock (_sync)
            {
                if (IsAvailable)
                {
                    RemainingCooldown = Period.Value;
                    if( RemainingCooldown > 0 )
                    {
                        IsAvailable = false;
                        RaiseIsAvailableChanged();
                    }

                    return true;
                }
                return false;
            }
        }

        protected override void UpdateInternal(UpdateTime gameTime)
        {
            lock (_sync)
            {
                if( !IsAvailable )
                {
                    RemainingCooldown -= gameTime.ElapsedTime;
                    RemainingCooldownRatio = (float)RemainingCooldown / Period.Value;

                    if( RemainingCooldown <= 0 )
                    {
                        RemainingCooldown = 0;
                        RemainingCooldownRatio = 0;
                        IsAvailable = true;
                        RaiseIsAvailableChanged();
                    }
                }
            }
        }

        private void RaiseIsAvailableChanged()
        {
            if( IsAvailableChanged != null ) IsAvailableChanged( this );
        }
    }
}
