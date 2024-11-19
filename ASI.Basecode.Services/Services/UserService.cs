using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly ITokenRepository _tokenrepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, ITokenRepository tokenRepository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _tokenrepository = tokenRepository;
        }

        public LoginResult AuthenticateUser(string userName, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.Username == userName &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public void AddUser(UserViewModel model)
        {
            var user = new User();
            if (!_repository.UserExists(model.Username))
            {
                _mapper.Map(model, user);
                user.Password = PasswordManager.EncryptPassword(model.Password);
                user.DateCreated = DateTime.Now;
                user.DateUpdated = DateTime.Now;
                user.CreatedBy = System.Environment.UserName;
                user.UpdatedBy = System.Environment.UserName;
                user.DarkMode = false;
                _repository.AddUser(user);
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }
        }

        public UserViewModel RetrieveUser(int user)
        {
            var current_user = _repository.GetUsers().Where(x=>x.UserId == user)
                .Select(e=>new UserViewModel
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Username = e.Username
                }).FirstOrDefault();

            if (current_user is null)
            {
                new UserViewModel();
            }

            return current_user;
        }

        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _repository.GetUsers().Where(x => x.UserId == userId).FirstOrDefault();

            if (user == null)
            {
                return false;
                throw new InvalidDataException(Resources.Messages.Errors.UserNotFound);
            }

            var encryptedOldPassword = PasswordManager.EncryptPassword(oldPassword);
            if (user.Password != encryptedOldPassword)
            {
                return false;
                throw new InvalidDataException(Resources.Messages.Errors.InvalidOldPassword);
            }

            var encryptedNewPassword = PasswordManager.EncryptPassword(newPassword);

            user.Password = encryptedNewPassword;
            user.DateUpdated = DateTime.Now;
            user.UpdatedBy = System.Environment.UserName;

            _repository.ChangePassword(user);
            return true;
        }
        //FOR EMAIL SENDING

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("gastue.official@gmail.com", "qouh hmui nbcu xyuk"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("gastue.official@gmail.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);
            await smtpClient.SendMailAsync(mailMessage);    
        }

        //FORGOT PASSWORD

        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            var user = _repository.GetUsers().Where(x => x.Email == email).FirstOrDefault();
            if (user == null)
                return false;

            var generate_token = Guid.NewGuid().ToString();
            // Construct the reset password link
            var resetLink = $"https://youtube.com";
            var emailBody = $"Click <a href='{generate_token}'>here</a> to reset your password.";

            // Send the email
            await SendEmailAsync(email, "Password Reset Request", emailBody);
            
            //If email is sent successfully, add token to db 
            var token = new Token();
            token.Token1 = generate_token;
            token.ExpirationDate = DateTime.UtcNow.AddMinutes(5);
            token.Email = email;

            // Store the token and expiration in your database (implement this method)
            _tokenrepository.AddToken(token);
            return true;
        }

        //Forgot Password
        public bool ChangePassword(string email, string newPassword)
        {

            var user  = _repository.GetUsers().Where(x => x.Email == email).FirstOrDefault();
            if (user == null)
            {
                return false;
                throw new InvalidDataException(Resources.Messages.Errors.UserNotFound);
            }
            var encryptedNewPassword = PasswordManager.EncryptPassword(newPassword);

            user.Password = encryptedNewPassword;
            user.DateUpdated = DateTime.Now;
            user.UpdatedBy = System.Environment.UserName;

            _repository.ChangePassword(user);
            return true;
        }


    }
}