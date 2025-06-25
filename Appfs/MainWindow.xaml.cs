using System.Windows;

namespace Appfs
{
    public partial class MainWindow : Window
    {
        private readonly AppManager _appManager;

        public MainWindow()
        {
            InitializeComponent();
            _appManager = new AppManager(new DatabaseDataFactory());
            DataContext = _appManager;
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            _appManager.AddUser();
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            _appManager.DeleteUser();
        }

        private void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            _appManager.UpdateUser();
        }

        private void UsersListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _appManager.SelectedUser = UsersListBox.SelectedItem as UserModel;
            if (_appManager.SelectedUser != null)
            {
                _appManager.Posts.Clear();
                _appManager.Posts.AddRange(_appManager.SelectedUser.Posts);
                _appManager.Groups.Clear();
                _appManager.Groups.AddRange(_appManager.SelectedUser.Groups);
                _appManager.Friends.Clear();
                _appManager.Friends.AddRange(_appManager.SelectedUser.Friends);
                _appManager.Tags.Clear();
            }
        }

        private void AddPost_Click(object sender, RoutedEventArgs e)
        {
            _appManager.AddPost();
        }

        private void DeletePost_Click(object sender, RoutedEventArgs e)
        {
            _appManager.DeletePost();
        }

        private void ImportJson_Click(object sender, RoutedEventArgs e)
        {
            _appManager.ImportJson();
        }

        private void ExportJson_Click(object sender, RoutedEventArgs e)
        {
            _appManager.ExportJson();
        }

        private void BackupDatabase_Click(object sender, RoutedEventArgs e)
        {
            _appManager.BackupDatabase();
        }

        private void RestoreDatabase_Click(object sender, RoutedEventArgs e)
        {
            _appManager.RestoreDatabase();
        }

        private void ShowSqlPanel_Click(object sender, RoutedEventArgs e)
        {
            SqlPanel.Visibility = SqlPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ExecuteSqlQuery_Click(object sender, RoutedEventArgs e)
        {
            string query = SqlQueryTextBoxInMenu.Text;
            _appManager.ExecuteSqlQuery(query);
        }

        private void ExecuteDdlQuery_Click(object sender, RoutedEventArgs e)
        {
            string query = SqlQueryTextBoxInMenu.Text;
            _appManager.ExecuteDdlQuery(query);
        }

        private void UpdatePosts_Click(object sender, RoutedEventArgs e)
        {
            if (_appManager.SelectedUser != null)
            {
                foreach (var post in _appManager.Posts)
                {
                    _appManager.UpdatePost(post);
                }
                MessageBox.Show("Публикации обновлены.");
            }
        }

        private void UpdateGroups_Click(object sender, RoutedEventArgs e)
        {
            if (_appManager.SelectedUser != null)
            {
                foreach (var group in _appManager.Groups)
                {
                    _appManager.UpdateGroup(group);
                }
                MessageBox.Show("Группы обновлены.");
            }
        }

        private void UpdateFriends_Click(object sender, RoutedEventArgs e)
        {
            if (_appManager.SelectedUser != null)
            {
                foreach (var friend in _appManager.Friends)
                {
                    _appManager.UpdateFriend(friend);
                }
                MessageBox.Show("Друзья обновлены.");
            }
        }
    }
}