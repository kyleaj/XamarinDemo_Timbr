using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Timbr
{

    public class TreeProfile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string _id;
        [JsonProperty("id")]
        public string Id
        {
            get => _id;
            set
            {
                if (_id == value)
                    return;

                _id = value;

                HandlePropertyChanged();
            }
        }

        string _image_url;
        [JsonProperty("image")]
        public string ProfilePic
        {
            get => _image_url;
            set
            {
                if (_image_url == value)
                    return;

                _image_url = value;

                HandlePropertyChanged();
            }
        }

        string _name;
        [JsonProperty("name")]
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;

                _name = value;

                HandlePropertyChanged();
            }
        }

        string _description;
        [JsonProperty("description")]
        public string Description
        {
            get => _description;
            set
            {
                if (_description == value)
                    return;

                _description = value;

                HandlePropertyChanged();
            }
        }

        int _age;
        [JsonProperty("age")]
        public int Age
        {
            get => _age;
            set
            {
                if (_age == value)
                    return;

                _age = value;

                HandlePropertyChanged();
            }
        }

        bool _status = false;
        public bool Status
        {
            get => _status;
            set
            {
                if (_status == value)
                    return;

                _status = value;

                HandlePropertyChanged();
            }
        }

        void HandlePropertyChanged([CallerMemberName]string propertyName = "")
        {
            var eventArgs = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, eventArgs);
        }
    }
}
