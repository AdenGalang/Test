using PassManaAlpha.Core;
using PassManaAlpha.Core.Scurity;
using PassManaAlpha.MVVM.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace PassManaAlpha.MVVM.ViewModel
{
    public class PasswordViewModel : ForkObject
    {
        public string? InputTitle { get; set; }
        public string? InputUsername { get; set; }
        public string? InputPassword { get; set; }
        public string MasterKey { get; set; } = "deliciouslolis";

        private string? _consoleLog;
        public string? ConsoleLog
        {
            get => _consoleLog;
            set
            {
                _consoleLog = value;
                OnPropertyChanged(nameof(ConsoleLog));
            }
        }

        public void Log(string message)
        {
            ConsoleLog += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
        }

        private bool _isLoadEnabled = true;
        public bool IsLoadEnabled
        {
            get => _isLoadEnabled;
            set
            {
                _isLoadEnabled = value;
                OnPropertyChanged(nameof(IsLoadEnabled));
            }
        }

        public ObservableCollection<PasswordEntry> Entries { get; set; }

        public PasswordViewModel()
        {
            Entries = new ObservableCollection<PasswordEntry>();
            IsLoadEnabled = true;
        }

        public ICommand ReloadCommand => new RelayCommand(o =>
        {
            Entries.Clear();
            IsLoadEnabled = true;
            Log("Vault Entries cleared.");
        });

        public ICommand ClearConsoleCommand => new RelayCommand(o => ConsoleLog = string.Empty);

        public ICommand SaveCommand => new RelayCommand(o =>
        {
            if (string.IsNullOrWhiteSpace(InputTitle) ||
                string.IsNullOrWhiteSpace(InputUsername) ||
                string.IsNullOrWhiteSpace(InputPassword))
            {
                Log("Please fill in all fields before saving.");
                return;
            }

            var entry = new PasswordEntry
            {
                Title = InputTitle,
                Username = InputUsername,
                Password = InputPassword
            };

            string json = JsonSerializer.Serialize(entry);
            string encrypted;

            try
            {
                encrypted = HakoHelper.Encrypt(json, MasterKey);
            }
            catch (Exception ex)
            {
                Log($"Encryption failed: {ex.Message}");
                return;
            }

            try
            {
                File.AppendAllText("vault.dat", encrypted + Environment.NewLine);
                Entries.Add(entry);
                Log("Entry saved successfully.");
            }
            catch (Exception ex)
            {
                Log($"Failed to save entry: {ex.Message}");
            }
        });

        public ICommand LoadCommand => new RelayCommand(o =>
        {
            if (!File.Exists("vault.dat"))
            {
                Log("Vault file not found.");
                return;
            }

            var lines = File.ReadAllLines("vault.dat");
            if (lines.Length == 0)
            {
                Log("Vault is empty.");
                return;
            }

            Entries.Clear();
            int successCount = 0;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    string decrypted = HakoHelper.Decrypt(line, MasterKey);
                    if (string.IsNullOrWhiteSpace(decrypted))
                        throw new Exception("Decryption returned empty or null");

                    var entry = JsonSerializer.Deserialize<PasswordEntry>(decrypted);
                    if (entry == null)
                        throw new Exception("Deserialization returned null");

                    Entries.Add(entry);
                    successCount++;
                }
                catch (Exception ex)
                {
                    Log($"Failed to load entry: {ex.Message}");
                }
            }

            Log($"Successfully loaded {successCount} entr{(successCount == 1 ? "y" : "ies")} from vault.");
        });

    }
}
