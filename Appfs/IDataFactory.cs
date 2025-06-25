namespace Appfs
{
    /// <summary>
    /// Интерфейс для абстрактной фабрики создания данных
    /// </summary>
    public interface IDataFactory
    {
        UserModel CreateUser(int userId, string firstName, string lastName, string email);
        PostModel CreatePost(int postId, string content, DateTime createdDate, int userId);
        GroupModel CreateGroup(int groupId, string groupName, string role);
    }
}