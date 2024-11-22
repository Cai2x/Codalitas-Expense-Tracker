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
using ASI.Basecode.Data.Repositories;

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
            var existingEmail = _repository.EmailExists(model.Email);

            if (existingEmail)
            {
                throw new InvalidDataException("Email already in use");
            }

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

        public EditProfileModel RetrieveUser(int user)
        {
            var current_user = _repository.GetUsers().Where(x=>x.UserId == user)
                .Select(e=>new EditProfileModel
                {
                    UserId = e.UserId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Username = e.Username,
                    Email = e.Email,
                }).FirstOrDefault();

            if (current_user is null)
            {
                new EditProfileModel();
            }

            return current_user;
        }

        public UserViewModel GetUserPass(int id)
        {
            var current_user = _repository.GetUsers().Where(x => x.UserId == id)
                .Select(e => new UserViewModel
                {
                    Username = e.Username,
                    Password = PasswordManager.DecryptPassword(e.Password),
                }).FirstOrDefault();

            return current_user;
        }

        public void UpdateUser(EditProfileModel model)
        {
            var profile = _repository.GetUsers().Where(x => x.UserId == model.UserId).FirstOrDefault();

            if (profile is null)
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserNotFound);
            }

            _mapper.Map(model, profile);
            profile.DateUpdated = DateTime.Now;
            profile.UpdatedBy = System.Environment.UserName;

            try
            {
                _repository.UpdateUser(profile);
            }

            catch (Exception)
            {
                throw new InvalidDataException(Resources.Messages.Errors.ServerError);
            }
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

            _repository.UpdateUser(user);
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
                return true; 

            var generate_token = Guid.NewGuid().ToString();
            var resetLink = $"https://localhost:62290/Account/ResetPassword?token={generate_token}";
            var emailBody = $"Click <a href='{resetLink}'>here</a> to reset your password.";

            await SendEmailAsync(email, "Password Reset Request", emailBody);

            var token = new Token
            {
                Token1 = generate_token,
                ExpirationDate = DateTime.UtcNow.AddMinutes(2),
                Email = email
            };

            _tokenrepository.AddToken(token);
            return true;
        }


        //Reset Password
        public bool ResetPassword(string newPassword, string token)
        {
            var lateToken = _tokenrepository.RetrieveTokens().Where(x => x.ExpirationDate < DateTime.UtcNow).ToList();
            foreach(var tokens in lateToken)
            {
                _tokenrepository.DeleteToken(tokens);
            }

            var validToken = _tokenrepository.RetrieveTokens().FirstOrDefault(x => x.Token1 == token);

            if (validToken == null)
            {
                return false;
            }
            
            var user  = _repository.GetUsers().Where(x => x.Email == validToken.Email).FirstOrDefault();
            if (user == null)
            {
                _tokenrepository.DeleteToken(validToken);
                return false;
                throw new InvalidDataException(Resources.Messages.Errors.UserNotFound);
            }

            var encryptedNewPassword = PasswordManager.EncryptPassword(newPassword);

            user.Password = encryptedNewPassword;
            user.DateUpdated = DateTime.Now;
            user.UpdatedBy = System.Environment.UserName;

            _repository.UpdateUser(user);
            _tokenrepository.DeleteToken(validToken);
            return true;
        }
    }
}