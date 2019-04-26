using System;
using System.Collections.Generic;

using Xamarin.Forms;

using System.Globalization;
using System.Diagnostics;
using SwipeCards.Controls;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;

namespace Timbr
{
    public partial class MatchStack : ContentPage
    {

        ObservableCollection<TreeProfile> favorites;

        public MatchStack(ObservableCollection<TreeProfile> faves)
        {
            InitializeComponent();
            Title = "Browse";

            favorites = faves;

            CosmosDBService.GetTrees().ContinueWith((task) =>
            {
                if (!task.IsCompleted)
                {
                    // Usually would display something to indicate error
                    return;
                }
                List < TreeProfile > trees = task.Result;
                if (trees.Count == 0)
                {
                    // Display alert
                    return;
                }
                Debug.Print(trees.Count.ToString());
                CardStackView stack = new CardStackView();
                stack.VerticalOptions = LayoutOptions.FillAndExpand;

                stack.ItemTemplate = new DataTemplate(() => {
                    Frame frame = new Frame();
                    RelativeLayout layout = new RelativeLayout();
                    frame.Content = layout;
                    frame.CornerRadius = 15;
                    frame.Padding = 0;

                    ImageSource source = new UriImageSource();
                    source.SetBinding(UriImageSource.UriProperty, "ProfilePic");

                    Label nameLabel = new Label() { FontAttributes = FontAttributes.Bold, 
                        FontSize = 24, TextColor=Color.White };
                    nameLabel.SetBinding(Label.TextProperty, "Name");

                    Label description = new Label() {FontSize = 18, 
                        TextColor = Color.GhostWhite };
                    description.SetBinding(Label.TextProperty, "Description");

                    Label age = new Label() { FontSize = 24,  
                        TextColor = Color.White };
                    age.SetBinding(Label.TextProperty, "Age", BindingMode.Default, new IntToString());

                    Image profilePic = new Image() { Aspect = Aspect.AspectFill };
                    profilePic.Source = source;

                    Image likeStamp = new Image() { Aspect = Aspect.AspectFit };
                    likeStamp.Source = ImageSource.FromResource("Timbr.like.png");
                    likeStamp.SetBinding(Image.IsVisibleProperty, "Status");

                    Image dislikeStamp = new Image() { Aspect = Aspect.AspectFit };
                    dislikeStamp.Source = ImageSource.FromResource("Timbr.nope.png");
                    dislikeStamp.SetBinding(Image.IsVisibleProperty, "Status");

                    layout.Children.Add(profilePic,
                         Constraint.Constant(0),
                         Constraint.Constant(0),
                         Constraint.RelativeToParent((parent) => { return parent.Width; }),
                         Constraint.RelativeToParent((parent) => { return parent.Height; })
                         );
                    layout.Children.Add(description,
                        Constraint.RelativeToParent((parent) => { return parent.X + 10; }),
                        Constraint.RelativeToParent((parent) => { return parent.Y + parent.Height - 
                            (parent.Height * 0.15); })
                        );
                    layout.Children.Add(nameLabel,
                        Constraint.RelativeToView(description, (parent, sibling) => { return sibling.X; }),
                        Constraint.RelativeToView(description, (parent, sibling) => { return sibling.Y - 
                            sibling.Height - 5; })
                        );
                    layout.Children.Add(age,
                        Constraint.RelativeToView(nameLabel, (parent, sibling) => { return sibling.X + 
                            sibling.Width + 10; }),
                        Constraint.RelativeToView(nameLabel, (parent, sibling) => { return sibling.Y; })
                        );
                    layout.Children.Add(dislikeStamp,
                        Constraint.RelativeToParent((parent) => { return parent.Width * 0.6; }),
                        Constraint.RelativeToParent((parent) => { return parent.Height * 0.1; }),
                        Constraint.RelativeToParent((parent) => { return parent.Width * 0.4; }),
                        Constraint.RelativeToParent((parent) => { return parent.Height * 0.5; })
                        );
                    layout.Children.Add(likeStamp,
                        Constraint.RelativeToParent((parent) => { return parent.Width * 0.1; }),
                        Constraint.RelativeToParent((parent) => { return parent.Height * 0.1; }),
                        Constraint.RelativeToParent((parent) => { return parent.Width * 0.4; }),
                        Constraint.RelativeToParent((parent) => { return parent.Height * 0.5; })
                        );
                    return frame;
                });
                stack.ItemsSource = trees;
                stack.Swiped += processSwipe;
                stack.StartedDragging += processDragStart;
                stack.FinishedDragging += processDragEnd;
                Device.BeginInvokeOnMainThread(() => Content = stack);
            });
        }

        void processDragStart(object sender, SwipeCards.Controls.Arguments.DraggingEventArgs args)
        {
            (args.Item as TreeProfile).Status = true;
        }

        void processDragEnd(object sender, SwipeCards.Controls.Arguments.DraggingEventArgs args)
        {
            (args.Item as TreeProfile).Status = false;
        }

        async void processSwipe(object sender, SwipeCards.Controls.Arguments.SwipedEventArgs e2)
        {
            if (e2.Direction == SwipeCards.Controls.Arguments.SwipeDirection.Right)
            {
                TreeProfile tree = e2.Item as TreeProfile;
                favorites.Add(tree);
                var assembly = typeof(App).GetTypeInfo().Assembly;
                Stream audioStream = assembly.GetManifestResourceStream("Timbr." + "twig.wav");
                var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                player.Load(audioStream);
                player.Play();
                await Task.Run(() =>
                {
                    string currPref = Preferences.Get("favorites", null);
                    List<TreeProfile> faves;
                    if (currPref == null || currPref.Equals(""))
                    {
                        faves = new List<TreeProfile>();
                    }
                    else
                    {
                        faves = JsonConvert.DeserializeObject<List<TreeProfile>>(currPref);
                    }
                    faves.Add(tree);
                    currPref = JsonConvert.SerializeObject(faves);
                    Preferences.Set("favorites", currPref);
                });
            } else
            {
                var assembly = typeof(App).GetTypeInfo().Assembly;
                Stream audioStream = assembly.GetManifestResourceStream("Timbr." + "gasp.wav");
                var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                player.Load(audioStream);
                player.Play();
            }
        }
    }

    class IntToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var nullable = value as int?;
            var result = string.Empty;

            if (nullable.HasValue)
            {
                result = nullable.Value.ToString();
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            int intValue;
            int? result = null;

            if (int.TryParse(stringValue, out intValue))
            {
                result = new Nullable<int>(intValue);
            }

            return result;
        }
    }
   
}
