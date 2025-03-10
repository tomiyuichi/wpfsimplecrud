using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

public class MainViewModel
{
    private const string CsvFilePath = "data.csv";
    public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

    public string Username { get; set; }
    public string Email { get; set; }
    public User SelectedUser { get; set; }

    public ICommand AddCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }

    public MainViewModel()
    {
        LoadData();
        AddCommand = new RelayCommand(AddUser);
        UpdateCommand = new RelayCommand(UpdateUser, () => SelectedUser != null);
        DeleteCommand = new RelayCommand(DeleteUser, () => SelectedUser != null);
    }

    private void LoadData()
    {
        if (!File.Exists(CsvFilePath)) return;

        foreach (var line in File.ReadAllLines(CsvFilePath).Skip(1)) // ヘッダーをスキップ
        {
            var parts = line.Split(',');
            Users.Add(new User
            {
                Id = int.Parse(parts[0]),
                Username = parts[1],
                Email = parts[2]
            });
        }
    }

    private void SaveData()
    {
        var lines = Users.Select(u => $"{u.Id},{u.Username},{u.Email}");
        File.WriteAllLines(CsvFilePath, new[] { "Id,Username,Email" }.Concat(lines));
    }

    private void AddUser()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email)) return;

        int newId = Users.Count > 0 ? Users.Max(u => u.Id) + 1 : 1;
        Users.Add(new User { Id = newId, Username = Username, Email = Email });

        SaveData();
        Username = Email = string.Empty;
    }

    private void UpdateUser()
    {
        if (SelectedUser == null) return;

        SaveData();
    }

    private void DeleteUser()
    {
        if (SelectedUser == null) return;

        Users.Remove(SelectedUser);
        SaveData();
    }
}