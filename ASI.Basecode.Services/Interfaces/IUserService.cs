using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string userName, string password, ref User user);
        void AddUser(UserViewModel model);
        UserViewModel RetrieveUser(int user);
        bool ChangePassword(int userId, string oldPassword, string newPassword);
        bool ResetPassword(string newPassword, string token); 
        Task<bool> SendPasswordResetEmailAsync(string email);
        Task SendEmailAsync(string email, string subject, string message);
    }
}
