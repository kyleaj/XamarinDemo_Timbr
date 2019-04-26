using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Timbr
{
    public partial class MatchView : Xamarin.Forms.TabbedPage
    {
        public ObservableCollection<TreeProfile> favorites;

        public MatchView()
        {
            InitializeComponent();

            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);

            favorites = new ObservableCollection<TreeProfile>();

            Title = "Tindr";

            Children.Add(new MatchStack(favorites));
            Children.Add(new Liked(favorites));
            string currPref = Preferences.Get("favorites", null);
            List<TreeProfile> faves;
            if (currPref != null && !currPref.Equals(""))
            {
                faves = JsonConvert.DeserializeObject<List<TreeProfile>>(currPref);
                foreach (TreeProfile tree in faves)
                {
                    favorites.Add(tree);
                }
            }
        }
    }
}
