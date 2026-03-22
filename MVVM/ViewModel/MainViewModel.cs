using PassManaAlpha.Core;
using PassManaAlpha.Core.Scurity;
using PassManaAlpha.MVVM.Model;
using PassManaAlpha.MVVM.View;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;

namespace PassManaAlpha.MVVM.ViewModel
{
    class MainViewModel : ForkObject
    {
        public ObservableCollection<PasswordEntry> Entries { get; set; } = new();
        public string InputTitle { get; set; }
        public string InputUsername { get; set; }
        public string InputPassword { get; set; }
        public string MasterKey { get; set; } = "deliciouslolis";

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand PasswordViewCommand { get; set; }
        public RelayCommand SettingsViewCommand { get; set; }

        public UserControl HomeView { get; set; }
        public UserControl PasswordView { get; set; }
        public UserControl SettingsView { get; set; }

        public HomeViewModel HomeVM { get; set; }
        public PasswordViewModel PasswordVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            HomeView = new HomeViewModel();
            PasswordView = new PasswordView();
            SettingsView = new SettingsView();

         
            CurrentView = HomeView;

            HomeViewCommand = new RelayCommand(o => CurrentView = HomeView);
            PasswordViewCommand = new RelayCommand(o => CurrentView = PasswordView);
            SettingsViewCommand = new RelayCommand(o => CurrentView = SettingsView);
        }

    }
}
