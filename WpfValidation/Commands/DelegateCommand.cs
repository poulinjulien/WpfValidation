namespace WpfValidation.Commands
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Implementation of ICommand using delegates.
    /// From: http://www.wpftutorial.net/delegatecommand.html.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action _executeMethod;
        private readonly Func<bool> _canExecuteMethod;

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod = null)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod));
            }

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        public bool CanExecute()
        {
            if (_canExecuteMethod != null)
            {
                return _canExecuteMethod();
            }

            return true;
        }

        public void Execute()
        {
            _executeMethod?.Invoke();
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            Execute();
        }
    }

}