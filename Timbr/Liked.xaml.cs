using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Timbr
{
    public partial class Liked : ContentPage
    {

        ObservableCollection<TreeProfile> favorites;

        public Liked(ObservableCollection<TreeProfile> faves)
        {
            InitializeComponent();
            Title = "Liked";
            favorites = faves;
            FavList.ItemsSource = favorites;
        }
    }
}
