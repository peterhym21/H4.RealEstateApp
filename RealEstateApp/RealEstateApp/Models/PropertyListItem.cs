using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RealEstateApp.Models
{
    // Her kunne vi også have benyttet Fody og blot tilføjet følgende property: [AddINotifyPropertyChangedInterface]
    public class PropertyListItem : INotifyPropertyChanged
    {
        public PropertyListItem(Property property, double distance)
        {
            Property = property;
            Distance = distance;
        }

        private Property _property;
        private double _distance;

        public Property Property
        {
            get => _property;
            set
            {
                _property = value;
                RaisePropertyChanged();
            }
        }
        public double Distance
        {
            get => _distance;
            set
            {
                _distance = value;
                RaisePropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
