namespace YouthUnion
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    internal abstract class ViewModel : INotifyPropertyChanged
    {
        private const string DEFAULT_PROPERTY_NAME = "";
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = DEFAULT_PROPERTY_NAME)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T newValue,
            [CallerMemberName] string propertyName = DEFAULT_PROPERTY_NAME)
        {
            if (Equals(field, newValue))
                return false;

            field = newValue;
            OnPropertyChanged(propertyName);
            return true;

        }
    }
}