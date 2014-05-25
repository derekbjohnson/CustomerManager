﻿namespace CustomerManager.ViewModels
{
    using CustomerManager.Models;
    using GalaSoft.MvvmLight;
    using System;
    using System.Linq;
    using System.Collections.ObjectModel;
    using CustomerManager.Utils;

    public class CustomerVM : ViewModelBase
    {
        private Guid id;
        private string firstName;
        private string lastName;
        private DateTime birthday;
        private ObservableCollection<EmailAddressVM> emailAddresses;

        public CustomerVM()
        {
            this.Id = Guid.NewGuid();
            this.firstName = string.Empty;
            this.lastName = string.Empty;
            this.birthday = DateTime.Now.AddYears(-25);
        }

        public Guid Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.Set(() => this.Id, ref this.id, value);
            }
        }

        public string FirstName
        {
            get
            {
                return this.firstName;
            }

            set
            {
                this.Set(() => this.FirstName, ref this.firstName, value);
            }
        }

        public string LastName
        {
            get
            {
                return this.lastName;
            }

            set
            {
                this.Set(() => this.LastName, ref this.lastName, value);
            }
        }

        public DateTime Birthday
        {
            get
            {
                return this.birthday;
            }

            set
            {
                this.Set(() => this.Birthday, ref this.birthday, value);
            }
        }

        public ObservableCollection<EmailAddressVM> EmailAddresses
        {
            get
            {
                return this.emailAddresses;
            }

            private set
            {
                this.Set(() => this.EmailAddresses, ref this.emailAddresses, value);
            }
        }

        public static CustomerVM FromModel(CustomerM model)
        {
            return new CustomerVM()
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Birthday = model.Birthday,
                EmailAddresses = model.EmailAddresses
                    .Select(x => EmailAddressVM.FromModel(x))
                    .ToObservableCollection(),
            };
        }

        public CustomerM ToModel()
        {
            return new CustomerM
            {
                Id = this.Id,
                FirstName = this.firstName,
                LastName = this.LastName,
                Birthday = this.Birthday,
                EmailAddresses = this.EmailAddresses
                    .Select(x => x.ToModel())
                    .ToList(),
            };
        }

        public override string ToString()
        {
            return this.GetFullName();
        }

        private string GetFullName()
        {
            return this.FirstName + " " + this.LastName;
        }
    }
}