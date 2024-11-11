﻿using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.IO;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
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
    }
}