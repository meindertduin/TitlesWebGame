using System;

namespace TitlesWebGame.WebUi.ViewModels
{
    public class ApplicationViewModel : BaseViewModel
    {
        public event Action OnErrorOccured;

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetValue(ref _errorMessage, value))
                {
                    NotifyOfErrorMessageChange();
                }
            }
        }

        private void NotifyOfErrorMessageChange()
        {
            OnErrorOccured?.Invoke();
        }
    }
}