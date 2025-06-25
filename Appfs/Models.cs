using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Appfs
{
    /// <summary>
    /// Модель данных для пользователя
    /// </summary>
    public class UserModel : INotifyPropertyChanged
    {
        private int _userId;
        private string _firstName;
        private string _lastName;
        private string _email;

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public ObservableCollection<PostModel> Posts { get; set; } = new ObservableCollection<PostModel>();
        public ObservableCollection<GroupModel> Groups { get; set; } = new ObservableCollection<GroupModel>();
        public ObservableCollection<UserModel> Friends { get; set; } = new ObservableCollection<UserModel>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Модель данных для публикации
    /// </summary>
    public class PostModel : INotifyPropertyChanged
    {
        private int _postId;
        private string _content;
        private DateTime _createdDate;
        private int _userId;

        public int PostId
        {
            get => _postId;
            set
            {
                _postId = value;
                OnPropertyChanged(nameof(PostId));
            }
        }

        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set
            {
                _createdDate = value;
                OnPropertyChanged(nameof(CreatedDate));
            }
        }

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        public ObservableCollection<string> Tags { get; set; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Модель данных для группы
    /// </summary>
    public class GroupModel : INotifyPropertyChanged
    {
        private int _groupId;
        private string _groupName;
        private string _role;

        public int GroupId
        {
            get => _groupId;
            set
            {
                _groupId = value;
                OnPropertyChanged(nameof(GroupId));
            }
        }

        public string GroupName
        {
            get => _groupName;
            set
            {
                _groupName = value;
                OnPropertyChanged(nameof(GroupName));
            }
        }

        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged(nameof(Role));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Модель данных для тега
    /// </summary>
    public class TagModel : INotifyPropertyChanged
    {
        private int _tagId;
        private string _tagName;

        public int TagId
        {
            get => _tagId;
            set
            {
                _tagId = value;
                OnPropertyChanged(nameof(TagId));
            }
        }

        public string TagName
        {
            get => _tagName;
            set
            {
                _tagName = value;
                OnPropertyChanged(nameof(TagName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}