namespace WpfValidation.Behaviors
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using System.Windows.Media;

    /// <summary>
    ///     Inspired by <a href="http://stackoverflow.com/a/12056846/65060">This StackOverflow answer</a>.
    /// </summary>
    public sealed class SetFocusOnValidationErrorBehavior : Behavior<FrameworkElement>
    {
        private object _dataContext;

        private object DataContext
        {
            get { return _dataContext; }
            set
            {
                UnhookDataContextEvents();
                _dataContext = value;
                HookDataContextEvents();
            }
        }

        protected override void OnAttached()
        {
            DataContext = AssociatedObject.DataContext;
            AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
        }

        protected override void OnDetaching()
        {
            DataContext = null;
        }

        private void AssociatedObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataContext = AssociatedObject.DataContext;
        }

        private void HookDataContextEvents()
        {
            var validatableViewModel = DataContext as IValidatableViewModel;
            if (validatableViewModel != null)
            {
                validatableViewModel.ValidationTriggered += NotifyDataErrorInfo_ValidationTriggered;
            }
        }

        private void UnhookDataContextEvents()
        {
            var validatableViewModel = DataContext as IValidatableViewModel;
            if (validatableViewModel != null)
            {
                validatableViewModel.ValidationTriggered -= NotifyDataErrorInfo_ValidationTriggered;
            }
        }

        private void NotifyDataErrorInfo_ValidationTriggered(object sender, EventArgs e)
        {
            var notifyDataErrorInfo = sender as INotifyDataErrorInfo;
            if (notifyDataErrorInfo != null && notifyDataErrorInfo.HasErrors)
            {
                SetFocusToFirstControlWithValidationError(AssociatedObject);
            }
        }

        private bool SetFocusToFirstControlWithValidationError(DependencyObject parent)
        {
            // VisualTreeHelper.GetChildrenCount() for TabControl will always return 0, so we need to 
            // do this branch of code
            var tabControl = parent as TabControl;
            if (tabControl != null)
            {
                foreach (TabItem tabItem in tabControl.Items)
                {
                    if (tabItem.Content != null)
                    {
                        if (SetFocusToFirstControlWithValidationError(tabItem.Content as UIElement))
                        {
                            return true;
                        }
                    }
                }
            }

            // if element has children
            int count = VisualTreeHelper.GetChildrenCount(parent);
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    var control = child as Control;
                    if (control != null)
                    {
                        // if control have error - we found first control, set focus to it and 
                        // return true
                        if ((bool)control.GetValue(Validation.HasErrorProperty))
                        {
                            control.Focus();

                            return true;
                        }
                    }

                    if (SetFocusToFirstControlWithValidationError(child))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}