﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public class User
    {
        private DateTime? birth;

        public User() { }
        public User( string login, string password, string firstName, string lastName, string patronymic, string phone, string email, DateTime dateBirth, string role, int id)
        {
            
            Login = login;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            Phone = phone;
            Email = email;
            DateBirth = dateBirth;
            Role = role;
            Id = id;
        }

        public User(string login, string password, string firstName, string lastName, string patronymic, string phone, string email, DateTime? birth, string role, int id)
        {
            Login = login;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            Phone = phone;
            Email = email;
            this.birth = birth;
            Role = role;
            Id = id;
        }

        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime DateBirth { get; set; }
        public string Role { get; set; }
    }
}
