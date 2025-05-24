using AdministrationDataBase.Context;
using AdministrationDataBaseData.Models;
using System.Security.Claims;

namespace AdministrationDataBase.Helpers
{
    public class UserHelper
    {
        public static User GetLoggedUser(BDContext db, ClaimsPrincipal User)
        {
            User usuario = db.Users
                .Where(m => m.Id == GetUserId(User))
                .FirstOrDefault();

            return usuario;
        }

        public static List<User> GetUsers(BDContext db, ClaimsPrincipal User)
        {
            List<User> usuarios = db.Users
				    .ToList();
            
            return usuarios;
        }

        public static User GetUser(BDContext db, int userId)
        {
            User usuario = db.Users
                .FirstOrDefault(m => m.Id == userId);

            return usuario;
        }

        public static User GetUserByEmailAndPassword(BDContext db, string email, string password)
        {
            User usuario = db.Users
                    .FirstOrDefault(m => m.Email == email && m.Password == password);

            return usuario;
        }

        public static User GetUserByEmail(BDContext db, string email)
        {
            return db.Users.FirstOrDefault(u => u.Email == email);
		    }

        public static int GetUserId(ClaimsPrincipal User)
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
		    }

		    public static bool IsMe(ClaimsPrincipal User, int userId)
        {
            return (GetUserId(User) == userId);
		    }
    }
}