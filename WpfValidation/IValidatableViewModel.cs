namespace WpfValidation
{
    using System;
    using System.ComponentModel;

    public interface IValidatableViewModel : INotifyDataErrorInfo
    {
        event EventHandler ValidationTriggered;
    }
}