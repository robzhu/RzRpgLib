using System;
using System.ComponentModel;
using RzAspects;

namespace RzRpgLib
{
    //Properties- values that describe the state and characteristics of the entity.  
    //  Examples: Eye color = blue, hitpoints = 100, faction = greek

    //Accessories- physical components, equipment, or accoutrements
    public interface IGameEntityProperty : ICompositeProperty
    {
        string Name { get; }
        string DisplayName { get; set; }
        string Description { get; set; }
        object Parent { get; set; }
        dynamic Value { get; set; }
        object Tag { get; set; }

        Func<IGameEntityProperty, string> UpdateDescription { get; set; }

        ObservableCollectionEx<IPropertyModifierEffect> Modifiers { get; }

        void Reset();
    }

    public class GameEntityProperty : ModelBase, IGameEntityProperty
    {
        public event PropertyChangedEventHandler ValuePropertyChanged;

        private string _name = null;
        public string Name { get { return _name; } }

        public string PropertyDisplayName { get { return "DisplayName"; } }
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { SetProperty( PropertyDisplayName, ref _DisplayName, value ); }
        }

        public string PropertyDescription { get { return "Description"; } }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { SetProperty( PropertyDescription, ref _Description, value ); }
        }

        public string PropertyParent { get { return "Parent"; } }
        private object _Parent;
        public object Parent
        {
            get { return _Parent; }
            set { SetProperty( PropertyParent, ref _Parent, value ); }
        }

        public string PropertyValue { get { return "Value"; } }
        private dynamic _value;
        public dynamic Value
        {
            get { return _value; }
            set { SetProperty( PropertyValue, ref _value, value ); }
        }

        public string PropertyTag { get { return "Tag"; } }
        private object _Tag;
        public object Tag
        {
            get { return _Tag; }
            set { SetProperty( PropertyTag, ref _Tag, value ); }
        }

        public string PropertyUpdateDescription { get { return "UpdateDescription"; } }
        private Func<IGameEntityProperty,string> _UpdateDescription;
        public Func<IGameEntityProperty,string> UpdateDescription
        {
            get { return _UpdateDescription; }
            set { SetProperty( PropertyUpdateDescription, ref _UpdateDescription, value, RefreshDescription ); }
        }

        public ObservableCollectionEx<IPropertyModifierEffect> Modifiers { get; private set; }

        public GameEntityProperty( string name, dynamic value )
        {
            Modifiers = new ObservableCollectionEx<IPropertyModifierEffect>( OnModifierAdded, OnModifierRemoved );

            _name = name;
            DisplayName = name;
            Value = value;

            DescendentPropertyChanged += OnDescendentPropertyChanged;
        }

        public virtual void Reset()
        {
            Parent = null;
            Modifiers.Clear();
        }

        private void OnModifierAdded( IPropertyModifierEffect modifier )
        {
            if( null == modifier ) throw new Exception( "cannot process null effect" );
            modifier.Modify( this );
            modifier.ValueChanging += BeforeModifierChanged;
            modifier.ValueChanged += AfterModifierChanged;
        }

        private void OnModifierRemoved( IPropertyModifierEffect modifier )
        {
            if( null == modifier ) throw new Exception( "cannot process null effect" );
            modifier.Unmodify( this );
            modifier.ValueChanging -= BeforeModifierChanged;
            modifier.ValueChanged -= AfterModifierChanged;
        }

        private void BeforeModifierChanged( IPropertyModifierEffect effect )
        {
            effect.UnmodifyOnly( this );
        }

        private void AfterModifierChanged( IPropertyModifierEffect effect )
        {
            effect.ModifyOnly( this );
        } 

        private void OnDescendentPropertyChanged( object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.StartsWith( PropertyValue ))
            {
                RaiseValuePropertyChanged( sender, e );
                RefreshDescription();
            }
        }

        protected void RaiseValuePropertyChanged( object sender, PropertyChangedEventArgs e)
        {
            if (ValuePropertyChanged != null)
            {
                ValuePropertyChanged( sender, e );
            }
        }

        private void RefreshDescription()
        {
            if( UpdateDescription != null )
            {
                Description = UpdateDescription( this );
            }
        }
    }
}
