namespace WpfValidation
{
    using System.Windows;
    using System.Windows.Input;
    using WpfValidation.Commands;

    public class MainWindowViewModel : ValidatableViewModelBase
    {
        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (Set(ref _firstName, value))
                {
                    ValidateFirstName();
                }
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (Set(ref _lastName, value))
                {
                    ValidateLastName();
                }
            }
        }

        private ICommand _submitCommand;

        public ICommand SubmitCommand
        {
            get { return _submitCommand ?? (_submitCommand = new DelegateCommand(Submit)); }
        }

        private void Submit()
        {
            Validate();

            if (HasErrors)
            {
                return;
            }

            MessageBox.Show(Application.Current.MainWindow, "Data submitted...", "WPF Validation",
                MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        protected override void DoValidate()
        {
            ValidateFirstName();
            ValidateLastName();
        }

        private void ValidateFirstName()
        {
            ValidateRequired(FirstName, nameof(FirstName), "First name");
        }

        private void ValidateLastName()
        {
            ValidateRequired(LastName, nameof(LastName), "Last name");
        }
    }
}