﻿namespace CustomerManager.Controllers
{
    using CustomerManager.Services;
    using CustomerManager.ViewModels;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using CustomerManager.Utils;

    public class CustomerC : ViewModelBase
    {
        private ICustomerService customerService;
        private INavigationService navigationService;
        private ObservableCollection<CustomerVM> customers;
        private ObservableCollection<AddressTypeVM> addressTypes;
        private CustomerVM selectedCustomer;
        private EmailAddressVM selectedEmailAddress;
        private bool isProcessing;

        public CustomerC(
            ICustomerService customerService,
            INavigationService navigationService)
        {
            this.customerService = customerService;
            this.navigationService = navigationService;

            this.LoadCustomers();
            this.LoadAddressTypes();

            this.NavigateToCustomerDetailCommand =
                new RelayCommand<CustomerVM>(this.NavigateToCustomerDetail);
            this.NavigateToCustomerEditCommand =
                new RelayCommand<CustomerVM>(this.NavigateToCustomerEdit);
            this.NavigateToCustomerListCommand =
                new RelayCommand(this.NavigateToCustomerList);
            this.DeleteCommand = new RelayCommand<CustomerVM>(this.Delete, this.CanDelete);
            this.NavigateToEmailAddressDetailCommand =
                new RelayCommand<EmailAddressVM>(this.NavigateToEmailAddressDetail);
            this.NavigateToEmailAddressEditCommand =
                new RelayCommand<EmailAddressVM>(this.NavigateToEmailAddressEdit);

            this.AddNewCustomerCommand = new RelayCommand(this.AddNewCustomer);
            this.SaveCommand = new RelayCommand(this.Save);
            this.RefreshCommand = new RelayCommand(this.LoadCustomers);
            this.DeleteEmailAddressCommand = new RelayCommand(this.DeleteEmailAddress);
        }

        public ICommand NavigateToCustomerDetailCommand { get; private set; }

        public ICommand NavigateToCustomerEditCommand { get; private set; }

        public ICommand NavigateToCustomerListCommand { get; private set; }

        public ICommand NavigateToEmailAddressDetailCommand { get; private set; }

        public ICommand AddNewCustomerCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public ICommand RefreshCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand DeleteEmailAddressCommand { get; private set; }

        public ICommand NavigateToEmailAddressEditCommand { get; private set; }

        public ObservableCollection<CustomerVM> Customers
        {
            get
            {
                return this.customers;
            }

            private set
            {
                this.Set(() => this.Customers, ref this.customers, value);
            }
        }

        public ObservableCollection<AddressTypeVM> AddressTypes
        {
            get
            {
                return this.addressTypes;
            }

            private set
            {
                this.Set(() => this.AddressTypes, ref this.addressTypes, value);
            }
        }

        public CustomerVM SelectedCustomer
        {
            get
            {
                return this.selectedCustomer;
            }

            private set
            {
                this.Set(() => this.SelectedCustomer, ref this.selectedCustomer, value);
            }
        }

        public EmailAddressVM SelectedEmailAddress
        {
            get
            {
                return this.selectedEmailAddress;
            }

            private set
            {
                this.Set(() => this.SelectedEmailAddress, ref this.selectedEmailAddress, value);
            }
        }

        public bool IsProcessing
        {
            get
            {
                return this.isProcessing;
            }

            private set
            {
                this.Set(() => this.IsProcessing, ref this.isProcessing, value);
            }
        }

        private void NavigateToCustomerDetail(CustomerVM customer)
        {
            if (customer == null)
            {
                throw new System.ArgumentNullException("customer");
            }

            this.selectedCustomer = customer;
            this.navigationService.NavigateToCustomerDetail();
        }

        private void AddNewCustomer()
        {
            var customer = new CustomerVM();
            this.customers.Add(customer);
            this.SelectedCustomer = customer;
            this.navigationService.NavigateToCustomerEdit();
        }

        private async void LoadCustomers()
        {
            try
            {
                this.IsProcessing = true;
                var list = await this.customerService.List();

                this.SelectedCustomer = list.FirstOrDefault();

                if (this.SelectedCustomer != null)
                {
                    this.SelectedEmailAddress =
                        this.SelectedCustomer.EmailAddresses.FirstOrDefault();
                }

                this.Customers = new ObservableCollection<CustomerVM>(list);
            }
            finally
            {
                this.IsProcessing = false;
            }
        }

        private void NavigateToCustomerEdit(CustomerVM customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            this.selectedCustomer = customer;
            this.navigationService.NavigateToCustomerEdit();
        }

        private void NavigateToCustomerList()
        {
            this.navigationService.NavigateToCustomerList();
        }

        private async void Save()
        {
            try
            {
                this.IsProcessing = true;

                foreach (var customer in this.customers)
                {
                    await this.customerService.Save(customer);
                }

                this.navigationService.NavigateToCustomerList();
            }
            finally
            {
                this.IsProcessing = false;
            }
        }

        private async void Delete(CustomerVM customer)
        {
            try
            {
                this.IsProcessing = true;

                await this.customerService.Delete(customer);
                this.SelectedCustomer = null;
                this.Customers.Remove(customer);

                this.navigationService.NavigateToCustomerList();
            }
            finally
            {
                this.IsProcessing = false;
            }
        }

        private bool CanDelete(CustomerVM customer)
        {
            return customer != null;
        }

        private void NavigateToEmailAddressDetail(EmailAddressVM emailAddress)
        {
            this.SelectedEmailAddress = emailAddress;
            this.navigationService.NavigateToEmailAddressDetail();
        }

        private void DeleteEmailAddress()
        {
            this.SelectedCustomer.EmailAddresses.Remove(this.SelectedEmailAddress);
            this.SelectedEmailAddress = null;
            this.navigationService.NavigateToCustomerList();
        }

        private void NavigateToEmailAddressEdit(EmailAddressVM emailAddress)
        {
            this.SelectedEmailAddress = emailAddress;
            this.navigationService.NavigateToEmailAddressEdit();
        }

        private async void LoadAddressTypes()
        {
            try
            {
                this.IsProcessing = true;

                var types = await this.customerService.ListAddressTypes();
                this.addressTypes = types.ToObservableCollection();
            }
            finally
            {
                this.IsProcessing = false;
            }
        }
    }
}