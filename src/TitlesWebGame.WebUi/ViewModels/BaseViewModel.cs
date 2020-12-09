using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TitlesWebGame.WebUi.Models;

namespace TitlesWebGame.WebUi.ViewModels
{
    public abstract class BaseViewModel
    {
        private bool isBusy = false; 
        
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                SetValue(ref isBusy, value);
            }
        }
 
        public event EventHandler<ViewModelPropertyChangedEventArgs> PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new ViewModelPropertyChangedEventArgs(propertyName));
        }
        
        protected bool SetValue<T>(ref T backingFiled, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingFiled, value)) return false;  
            backingFiled = value;
            OnPropertyChanged(propertyName);
            
            return true;
        }
    }
}