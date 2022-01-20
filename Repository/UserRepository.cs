using AuthLearning.Models;

namespace AuthLearning.Repository
{
	public class UserRepository
	{
		private User[] users = new User[] { 
			new User { Id = 1, Name = "Harry", Email = "harry@gmail.com", House = "Gryffindor", Password = "HarryPW", Role = "Student" }, 
			new User  { Id = 2, Name = "Albus", Email = "albus@gmail.com", House = "Gryffindor", Password = "AlbusPW", Role = "Principal" } };

		public User? GetUserByUsernameAndPassword(string username, string password)
		{
			var user = users.First(user => user.Name == username);
			if(user!=null && user.Password.Equals(password))
			{
				return user;
			}
			return null;
		}
	}
}
