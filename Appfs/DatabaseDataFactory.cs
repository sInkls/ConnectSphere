using System;

namespace Appfs
{
    /// <summary>
    /// Конкретная реализация фабрики для работы с базой данных
    /// </summary>
    public class DatabaseDataFactory : IDataFactory
    {
        public UserModel CreateUser(int userId, string firstName, string lastName, string email)
        {
            return new UserModel
            {
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };
        }

        public PostModel CreatePost(int postId, string content, DateTime createdDate, int userId)
        {
            return new PostModel
            {
                PostId = postId,
                Content = content,
                CreatedDate = createdDate,
                UserId = userId
            };
        }

        public GroupModel CreateGroup(int groupId, string groupName, string role)
        {
            return new GroupModel
            {
                GroupId = groupId,
                GroupName = groupName,
                Role = role
            };
        }
    }
}