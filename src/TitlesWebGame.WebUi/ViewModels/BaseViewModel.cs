using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TitlesWebGame.WebUi.Annotations;
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
        
        protected void SetValue<T>(ref T backingFiled, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingFiled, value)) return; 
            backingFiled = value;
            Console.WriteLine(propertyName);
            OnPropertyChanged(propertyName);
        }
    }
}