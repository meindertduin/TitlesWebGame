namespace TitlesWebGame.WebUi.Models
{
    public class ViewModelPropertyChangedEventArgs
    {
        public string PropertyName { get; private set; }
        
        public ViewModelPropertyChangedEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}