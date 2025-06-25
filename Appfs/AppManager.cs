using Microsoft.Win32;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Appfs
{
    /// <summary>
    /// Общий класс для управления приложением, реализующий MVVM, модели и репозиторий
    /// </summary>
    public class AppManager : INotifyPropertyChanged
    {
        private readonly IDataFactory _dataFactory;
        private readonly string _connectionString = "Host=localhost;Port=5432;Database=bdnew;Username=postgres;Password=12345";
        private ObservableCollection<UserModel> _users = new ObservableCollection<UserModel>();
        private ObservableCollection<PostModel> _posts = new ObservableCollection<PostModel>();
        private ObservableCollection<GroupModel> _groups = new ObservableCollection<GroupModel>();
        private ObservableCollection<UserModel> _friends = new ObservableCollection<UserModel>();
        private ObservableCollection<string> _tags = new ObservableCollection<string>();
        private UserModel _selectedUser;
        private PostModel _selectedPost;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<UserModel> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public ObservableCollection<PostModel> Posts
        {
            get => _posts;
            set
            {
                _posts = value;
                OnPropertyChanged(nameof(Posts));
            }
        }

        public ObservableCollection<GroupModel> Groups
        {
            get => _groups;
            set
            {
                _groups = value;
                OnPropertyChanged(nameof(Groups));
            }
        }

        public ObservableCollection<UserModel> Friends
        {
            get => _friends;
            set
            {
                _friends = value;
                OnPropertyChanged(nameof(Friends));
            }
        }

        public ObservableCollection<string> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }

        public UserModel SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                if (_selectedUser != null) LoadUserDetails(_selectedUser);
            }
        }

        public PostModel SelectedPost
        {
            get => _selectedPost;
            set
            {
                _selectedPost = value;
                OnPropertyChanged(nameof(SelectedPost));
            }
        }

        public AppManager(IDataFactory dataFactory)
        {
            _dataFactory = dataFactory;
            LoadUsers();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT user_id, first_name, last_name, email FROM \"Пользователи\" ORDER BY first_name", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(_dataFactory.CreateUser(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.IsDBNull(2) ? "" : reader.GetString(2),
                            reader.IsDBNull(3) ? "" : reader.GetString(3)));
                    }
                }
            }
            return users;
        }

        private void AddUser(UserModel user)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                    "INSERT INTO \"Пользователи\" (first_name, last_name, email, hashed_password, registration_date) " +
                    "VALUES (@firstName, @lastName, @email, @hashedPassword, CURRENT_DATE) RETURNING user_id",
                    conn);
                cmd.Parameters.AddWithValue("firstName", user.FirstName);
                cmd.Parameters.AddWithValue("lastName", user.LastName);
                string uniqueEmail = $"user{DateTime.Now.Ticks}@example.com";
                cmd.Parameters.AddWithValue("email", uniqueEmail);
                cmd.Parameters.AddWithValue("hashedPassword", "hashedpass");
                user.UserId = (int)cmd.ExecuteScalar();
                user.Email = uniqueEmail; // Сохраняем email в модели
            }
        }

        private void DeleteUser(int userId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("DELETE FROM \"Пользователи\" WHERE user_id = @userId", conn);
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateUser(UserModel user)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("UPDATE \"Пользователи\" SET first_name = @firstName, last_name = @lastName, email = @email WHERE user_id = @userId", conn);
                cmd.Parameters.AddWithValue("firstName", user.FirstName);
                cmd.Parameters.AddWithValue("lastName", user.LastName);
                cmd.Parameters.AddWithValue("email", user.Email);
                cmd.Parameters.AddWithValue("userId", user.UserId);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdatePost(PostModel post)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("UPDATE \"Публикации\" SET content = @content WHERE post_id = @postId", conn);
                cmd.Parameters.AddWithValue("content", post.Content);
                cmd.Parameters.AddWithValue("postId", post.PostId);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateGroup(GroupModel group)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("UPDATE \"Группы\" SET group_name = @groupName WHERE group_id = @groupId", conn);
                cmd.Parameters.AddWithValue("groupName", group.GroupName);
                cmd.Parameters.AddWithValue("groupId", group.GroupId);
                cmd.ExecuteNonQuery();

                cmd = new NpgsqlCommand("UPDATE \"Участники_групп\" SET role = @role WHERE group_id = @groupId AND user_id = @userId", conn);
                cmd.Parameters.AddWithValue("role", group.Role);
                cmd.Parameters.AddWithValue("groupId", group.GroupId);
                cmd.Parameters.AddWithValue("userId", SelectedUser?.UserId ?? 0);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateFriend(UserModel friend)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("UPDATE \"Пользователи\" SET first_name = @firstName, last_name = @lastName, email = @email WHERE user_id = @userId", conn);
                cmd.Parameters.AddWithValue("firstName", friend.FirstName);
                cmd.Parameters.AddWithValue("lastName", friend.LastName);
                cmd.Parameters.AddWithValue("email", friend.Email);
                cmd.Parameters.AddWithValue("userId", friend.UserId);
                cmd.ExecuteNonQuery();
            }
        }

        #region Методы управления
        public void AddUser()
        {
            var user = _dataFactory.CreateUser(0, "Новый", "Пользователь", "");
            AddUser(user);
            LoadUserDetails(user);
            Users.Add(user);
        }

        public void DeleteUser()
        {
            if (SelectedUser != null)
            {
                DeleteUser(SelectedUser.UserId);
                Users.Remove(SelectedUser);
                SelectedUser = null;
                Posts.Clear();
                Groups.Clear();
                Friends.Clear();
                Tags.Clear();
            }
        }

        public void UpdateUser()
        {
            if (SelectedUser != null)
            {
                UpdateUser(SelectedUser);
            }
        }

        public void AddPost()
        {
            if (SelectedUser != null)
            {
                var post = _dataFactory.CreatePost(0, "Новая публикация", DateTime.Now, SelectedUser.UserId);
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand("INSERT INTO \"Публикации\" (user_id, content, created_date) VALUES (@userId, @content, CURRENT_TIMESTAMP) RETURNING post_id", conn);
                    cmd.Parameters.AddWithValue("userId", post.UserId);
                    cmd.Parameters.AddWithValue("content", post.Content);
                    post.PostId = (int)cmd.ExecuteScalar();
                    Posts.Add(post);
                    SelectedUser.Posts.Add(post);
                }
            }
        }

        public void DeletePost()
        {
            if (SelectedPost != null)
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand("DELETE FROM \"Публикации\" WHERE post_id = @postId", conn);
                    cmd.Parameters.AddWithValue("postId", SelectedPost.PostId);
                    cmd.ExecuteNonQuery();
                    Posts.Remove(SelectedPost);
                    if (SelectedUser != null) SelectedUser.Posts.Remove(SelectedPost);
                    SelectedPost = null;
                }
            }
        }

        private void LoadUsers()
        {
            try
            {
                Users.Clear();
                var users = GetAllUsers();
                if (users != null && users.Count > 0)
                {
                    foreach (var user in users)
                    {
                        LoadUserDetails(user);
                        Users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}");
            }
        }

        private void LoadUserDetails(UserModel user)
        {
            if (user != null)
            {
                LoadUserPosts(user);
                LoadUserGroups(user);
                LoadUserFriends(user);
            }
        }

        private void LoadUserPosts(UserModel user)
        {
            try
            {
                user.Posts.Clear();
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT post_id, content, created_date FROM \"Публикации\" WHERE user_id = @userId ORDER BY created_date DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("userId", user.UserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var values = new object[reader.FieldCount];
                                reader.GetValues(values);
                                var post = _dataFactory.CreatePost(
                                    (int)values[0],
                                    values[1].ToString(),
                                    (DateTime)values[2],
                                    user.UserId);
                                user.Posts.Add(post);
                                Posts.Add(post);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки публикаций для пользователя {user.FirstName}: {ex.Message}");
            }
        }

        private void LoadUserGroups(UserModel user)
        {
            try
            {
                user.Groups.Clear();
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(
                        "SELECT g.group_id, g.group_name, m.role " +
                        "FROM \"Группы\" g " +
                        "JOIN \"Участники_групп\" m ON g.group_id = m.group_id " +
                        "WHERE m.user_id = @userId", conn))
                    {
                        cmd.Parameters.AddWithValue("userId", user.UserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var values = new object[reader.FieldCount];
                                reader.GetValues(values);
                                user.Groups.Add(_dataFactory.CreateGroup(
                                    (int)values[0],
                                    values[1].ToString(),
                                    values[2].ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки групп для пользователя {user.FirstName}: {ex.Message}");
            }
        }

        private void LoadUserFriends(UserModel user)
        {
            try
            {
                user.Friends.Clear();
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT friend_id, first_name, last_name, email " +
                        "FROM get_active_friends(@userId) f " +
                        "JOIN \"Пользователи\" u ON f.friend_id = u.user_id", conn))
                    {
                        cmd.Parameters.AddWithValue("userId", user.UserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var values = new object[reader.FieldCount];
                                reader.GetValues(values);
                                user.Friends.Add(_dataFactory.CreateUser(
                                    (int)values[0],
                                    values[1].ToString(),
                                    reader.IsDBNull(2) ? "" : values[2].ToString(),
                                    reader.IsDBNull(3) ? "" : values[3].ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки друзей для пользователя {user.FirstName}: {ex.Message}");
            }
        }

        public void ImportJson()
        {
            var openFileDialog = new OpenFileDialog { Filter = "JSON files (*.json)|*.json" };
            if (openFileDialog.ShowDialog() == true)
            {
                var json = File.ReadAllText(openFileDialog.FileName);
                var users = JsonConvert.DeserializeObject<List<UserModel>>(json);
                if (users != null)
                {
                    Users.Clear();
                    foreach (var user in users)
                    {
                        AddUser(user);
                        LoadUserDetails(user);
                        Users.Add(user);
                    }
                }
            }
        }

        public void ExportJson()
        {
            var saveFileDialog = new SaveFileDialog { Filter = "JSON files (*.json)|*.json", FileName = "users.json" };
            if (saveFileDialog.ShowDialog() == true)
            {
                if (Users == null || Users.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта.", "Ошибка");
                    return;
                }

                var json = JsonConvert.SerializeObject(Users.ToList(), Formatting.Indented,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                File.WriteAllText(saveFileDialog.FileName, json);
                MessageBox.Show($"Данные успешно экспортированы в {saveFileDialog.FileName}.", "Успех");
            }
        }

        public void BackupDatabase()
        {
            var saveFileDialog = new SaveFileDialog { Filter = "Backup files (*.backup)|*.backup", FileName = "database.backup" };
            if (saveFileDialog.ShowDialog() == true)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Program Files\PostgreSQL\17\bin\pg_dump.exe",
                        Arguments = $"-h localhost -U postgres -F c -f \"{saveFileDialog.FileName}\" bdnew",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                try
                {
                    process.Start();
                    process.WaitForExit();
                    MessageBox.Show("Резервное копирование успешно выполнено.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при резервном копировании: {ex.Message}");
                }
            }
        }

        public void RestoreDatabase()
        {
            var openFileDialog = new OpenFileDialog { Filter = "Backup files (*.backup)|*.backup" };
            if (openFileDialog.ShowDialog() == true)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Program Files\PostgreSQL\17\bin\pg_restore.exe",
                        Arguments = $"-h localhost -U postgres -d bdnew --verbose \"{openFileDialog.FileName}\"",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                try
                {
                    process.Start();
                    process.WaitForExit();
                    MessageBox.Show("Восстановление успешно выполнено.");
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при восстановлении: {ex.Message}");
                }
            }
        }

        public void ExecuteSql()
        {
        }

        public void ExecuteDdlQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Введите DDL-запрос.", "Ошибка");
                return;
            }

            if (query.ToUpper().Contains("DROP") || query.ToUpper().Contains("DELETE") || query.ToUpper().Contains("TRUNCATE"))
            {
                if (MessageBox.Show("Вы уверены, что хотите выполнить эту операцию? Это может привести к потере данных.", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("DDL-команда выполнена успешно.", "Успех");
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения DDL: {ex.Message}", "Ошибка");
            }
        }

        public void ExecuteSqlQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Введите SQL-запрос.", "Ошибка");
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand(query, conn);
                    var stopwatch = Stopwatch.StartNew();
                    using (var reader = cmd.ExecuteReader())
                    {
                        stopwatch.Stop();
                        var result = $"Запрос выполнен за {stopwatch.ElapsedMilliseconds} мс\n";
                        while (reader.Read())
                        {
                            var values = new object[reader.FieldCount];
                            reader.GetValues(values);
                            result += string.Join(", ", values) + "\n";
                        }
                        MessageBox.Show(result, "Результат SQL-запроса");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка SQL-запроса");
            }
        }
        #endregion
    }
}