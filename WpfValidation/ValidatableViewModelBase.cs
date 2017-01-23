namespace WpfValidation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class ValidatableViewModelBase : ObservableObject, IValidatableViewModel
    {
        private readonly Dictionary<string, ICollection<string>> _validationErrors =
            new Dictionary<string, ICollection<string>>();

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_validationErrors.ContainsKey(propertyName))
            {
                return null;
            }

            return _validationErrors[propertyName];
        }

        public bool HasErrors => _validationErrors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public event EventHandler ValidationTriggered;

        protected virtual void OnValidationTriggered()
        {
            ValidationTriggered?.Invoke(this, EventArgs.Empty);
        }

        public void Validate()
        {
            DoValidate();
            OnValidationTriggered();
        }

        protected virtual void DoValidate()
        {
            // NOP
        }

        protected virtual void RemoveValidationErrors(string propertyName)
        {
            if (_validationErrors.ContainsKey(propertyName))
            {
                _validationErrors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        protected virtual void AddValidationErrors(string propertyName, ICollection<string> validationErrors)
        {
            _validationErrors[propertyName] = validationErrors;
            OnErrorsChanged(propertyName);
        }

        protected void ValidateRequired(string propertyValue, string propertyName, string propertyPrettyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyValue))
            {
                AddValidationErrors(propertyName,
                    new List<string> { (propertyPrettyName ?? propertyName) + " is required." });
            }
            else
            {
                RemoveValidationErrors(propertyName);
            }
        }
    }
}