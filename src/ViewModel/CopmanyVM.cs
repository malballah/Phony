﻿using MahApps.Metro.Controls.Dialogs;
using Phony.Kernel;
using Phony.Model;
using Phony.Persistence;
using Phony.Utility;
using Phony.View;
using System;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Phony.ViewModel
{
    public class CopmanyVM : CommonBase
    {
        int _companyId;
        string _name;
        string _site;
        string _email;
        string _searchText;
        string _phone;
        string _notes;
        static string _companiesCount;
        static string _companiesPurchasePrice;
        static string _companiesSalePrice;
        static string _companiesProfit;
        byte[] _image;
        decimal _balance;
        bool _isCompanyFlyoutOpen;
        Company _dataGridSelectedCompany;
        ObservableCollection<Company> _companies;

        public int CompanyId
        {
            get => _companyId;
            set
            {
                if (value != _companyId)
                {
                    _companyId = value;
                    RaisePropertyChanged(nameof(CompanyId));
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        public string Site
        {
            get => _site;
            set
            {
                if (value != _site)
                {
                    _site = value;
                    RaisePropertyChanged(nameof(Site));
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (value != _email)
                {
                    _email = value;
                    RaisePropertyChanged(nameof(Email));
                }
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (value != _phone)
                {
                    _phone = value;
                    RaisePropertyChanged(nameof(Phone));
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value != _searchText)
                {
                    _searchText = value;
                    RaisePropertyChanged(nameof(SearchText));
                }
            }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                if (value != _notes)
                {
                    _notes = value;
                    RaisePropertyChanged(nameof(Notes));
                }
            }
        }

        public string CompaniesCount
        {
            get => _companiesCount;
            set
            {
                if (value != _companiesCount)
                {
                    _companiesCount = value;
                    RaisePropertyChanged(nameof(CompaniesCount));
                }
            }
        }

        public string CompaniesPurchasePrice
        {
            get => _companiesPurchasePrice;
            set
            {
                if (value != _companiesPurchasePrice)
                {
                    _companiesPurchasePrice = value;
                    RaisePropertyChanged(nameof(CompaniesPurchasePrice));
                }
            }
        }

        public string CompaniesSalePrice
        {
            get => _companiesSalePrice;
            set
            {
                if (value != _companiesSalePrice)
                {
                    _companiesSalePrice = value;
                    RaisePropertyChanged(nameof(CompaniesSalePrice));
                }
            }
        }

        public string CompaniesProfit
        {
            get => _companiesProfit;
            set
            {
                if (value != _companiesProfit)
                {
                    _companiesProfit = value;
                    RaisePropertyChanged(nameof(CompaniesProfit));
                }
            }
        }

        public byte[] Image
        {
            get => _image;
            set
            {
                if (value != _image)
                {
                    _image = value;
                    RaisePropertyChanged(nameof(Image));
                }
            }
        }

        public decimal Balance
        {
            get => _balance;
            set
            {
                if (value != _balance)
                {
                    _balance = value;
                    RaisePropertyChanged(nameof(Balance));
                }
            }
        }

        public Company DataGridSelectedCompany
        {
            get => _dataGridSelectedCompany;
            set
            {
                if (value != _dataGridSelectedCompany)
                {
                    _dataGridSelectedCompany = value;
                    RaisePropertyChanged(nameof(DataGridSelectedCompany));
                }
            }
        }

        public ObservableCollection<Company> Companies
        {
            get => _companies;
            set
            {
                if (value != _companies)
                {
                    _companies = value;
                    RaisePropertyChanged(nameof(Companies));
                }
            }
        }

        public ObservableCollection<User> Users { get; set; }

        public bool IsAddCompanyFlyoutOpen
        {
            get => _isCompanyFlyoutOpen;
            set
            {
                if (value != _isCompanyFlyoutOpen)
                {
                    _isCompanyFlyoutOpen = value;
                    RaisePropertyChanged(nameof(IsAddCompanyFlyoutOpen));
                }
            }
        }

        public ICommand OpenAddCompanyFlyout { get; set; }
        public ICommand SelectImage { get; set; }
        public ICommand FillUI { get; set; }
        public ICommand DeleteCompany { get; set; }
        public ICommand ReloadAllCompanies { get; set; }
        public ICommand Search { get; set; }
        public ICommand AddCompany { get; set; }
        public ICommand EditCompany { get; set; }
        public ICommand AddBalance { get; set; }

        Users.LoginVM CurrentUser = new Users.LoginVM();

        Companies CompaniesMassage = Application.Current.Windows.OfType<Companies>().FirstOrDefault();

        public CopmanyVM()
        {
            LoadCommands();
            using (var db = new PhonyDbContext())
            {
                Companies = new ObservableCollection<Company>(db.Companies);
                Users = new ObservableCollection<User>(db.Users);
            }
            new Thread(() =>
            {
                CompaniesCount = $"إجمالى الشركات: {Companies.Count().ToString()}";
                CompaniesPurchasePrice = $"اجمالى لينا: {decimal.Round(Companies.Where(c => c.Balance > 0).Sum(i => i.Balance), 2).ToString()}";
                CompaniesSalePrice = $"اجمالى علينا: {decimal.Round(Companies.Where(c => c.Balance < 0).Sum(i => i.Balance), 2).ToString()}";
                CompaniesProfit = $"تقدير لصافى لينا: {decimal.Round((Companies.Where(c => c.Balance > 0).Sum(i => i.Balance) + Companies.Where(c => c.Balance < 0).Sum(i => i.Balance)), 2).ToString()}";
                Thread.CurrentThread.Abort();
            }).Start();
        }

        public void LoadCommands()
        {
            OpenAddCompanyFlyout = new CustomCommand(DoOpenAddCompanyFlyout, CanOpenAddCompanyFlyout);
            SelectImage = new CustomCommand(DoSelectImage, CanSelectImage);
            FillUI = new CustomCommand(DoFillUI, CanFillUI);
            DeleteCompany = new CustomCommand(DoDeleteCompany, CanDeleteCompany);
            ReloadAllCompanies = new CustomCommand(DoReloadAllCompanies, CanReloadAllCompanies);
            Search = new CustomCommand(DoSearch, CanSearch);
            AddCompany = new CustomCommand(DoAddCompany, CanAddCompany);
            EditCompany = new CustomCommand(DoEditCompany, CanEditCompany);
            AddBalance = new CustomCommand(DoAddBalance, CanAddBalance);
        }

        private bool CanAddBalance(object obj)
        {
            if (DataGridSelectedCompany == null)
            {
                return false;
            }
            return true;
        }

        private async void DoAddBalance(object obj)
        {
            var result = await CompaniesMassage.ShowInputAsync("تدفيع", $"ادخل المبلغ الذى تريد تدفيعه للشركة {DataGridSelectedCompany.Name}");
            if (string.IsNullOrWhiteSpace(result))
            {
                await CompaniesMassage.ShowMessageAsync("ادخل مبلغ", "لم تقم بادخال اى مبلغ لاضافته للرصيد");
            }
            else
            {
                decimal compantpaymentamount;
                bool isvalidmoney = decimal.TryParse(result, out compantpaymentamount);
                if (isvalidmoney)
                {
                    using (var db = new UnitOfWork(new PhonyDbContext()))
                    {
                        var s = db.Companies.Get(DataGridSelectedCompany.Id);
                        s.Balance -= compantpaymentamount;
                        s.EditDate = DateTime.Now;
                        s.EditById = CurrentUser.Id;
                        var sm = new CompanyMove
                        {
                            CompanyId = DataGridSelectedCompany.Id,
                            Amount = compantpaymentamount,
                            CreateDate = DateTime.Now,
                            CreatedById = CurrentUser.Id,
                            EditDate = null,
                            EditById = null
                        };
                        db.CompaniesMoves.Add(sm);
                        db.Complete();
                        await CompaniesMassage.ShowMessageAsync("تمت العملية", $"تم شحن الشركة {DataGridSelectedCompany.Name} مبلغ {compantpaymentamount} جنية بنجاح");
                        DataGridSelectedCompany = null;
                        CompanyId = 0;
                        Companies.Remove(DataGridSelectedCompany);
                        Companies.Add(s);
                    }
                }
                else
                {
                    await CompaniesMassage.ShowMessageAsync("خطاء فى المبلغ", "ادخل مبلغ صحيح بعلامه عشرية واحدة");
                }
            }
        }

        private bool CanEditCompany(object obj)
        {
            if (string.IsNullOrWhiteSpace(Name) || CompanyId == 0 || DataGridSelectedCompany == null)
            {
                return false;
            }
            return true;
        }

        private void DoEditCompany(object obj)
        {
            using (var db = new UnitOfWork(new PhonyDbContext()))
            {
                var s = db.Companies.Get(DataGridSelectedCompany.Id);
                s.Name = Name;
                s.Balance = Balance;
                s.Site = Site;
                s.Email = Email;
                s.Phone = Phone;
                s.Image = Image;
                s.Notes = Notes;
                s.EditDate = DateTime.Now;
                s.EditById = CurrentUser.Id;
                db.Complete();
                CompanyId = 0;
                Companies.Remove(DataGridSelectedCompany);
                Companies.Add(s);
                DataGridSelectedCompany = null;
                CompaniesMassage.ShowMessageAsync("تمت العملية", "تم تعديل الشركة بنجاح");
            }
        }

        private bool CanAddCompany(object obj)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }
            return true;
        }

        private void DoAddCompany(object obj)
        {
            using (var db = new UnitOfWork(new PhonyDbContext()))
            {
                var s = new Company
                {
                    Name = Name,
                    Balance = Balance,
                    Site = Site,
                    Email = Email,
                    Phone = Phone,
                    Image = Image,
                    Notes = Notes,
                    CreateDate = DateTime.Now,
                    CreatedById = CurrentUser.Id,
                    EditDate = null,
                    EditById = null
                };
                db.Companies.Add(s);
                db.Complete();
                Companies.Add(s);
                CompaniesMassage.ShowMessageAsync("تمت العملية", "تم اضافة الشركة بنجاح");
            }
        }

        private bool CanSearch(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                return false;
            }
            return true;
        }

        private void DoSearch(object obj)
        {
            using (var db = new PhonyDbContext())
            {
                Companies = new ObservableCollection<Company>(db.Companies);
            }
        }

        private bool CanReloadAllCompanies(object obj)
        {
            return true;
        }

        private void DoReloadAllCompanies(object obj)
        {
            using (var db = new PhonyDbContext())
            {
                Companies = new ObservableCollection<Company>(db.Companies);
            }
        }

        private bool CanDeleteCompany(object obj)
        {
            if (DataGridSelectedCompany == null)
            {
                return false;
            }
            return true;
        }

        private async void DoDeleteCompany(object obj)
        {
            var result = await CompaniesMassage.ShowMessageAsync("حذف الخدمة", $"هل انت متاكد من حذف الشركة {DataGridSelectedCompany.Name}", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                using (var db = new UnitOfWork(new PhonyDbContext()))
                {
                    db.Companies.Remove(db.Companies.Get(DataGridSelectedCompany.Id));
                    db.Complete();
                    Companies.Remove(DataGridSelectedCompany);
                }
                DataGridSelectedCompany = null;
                await CompaniesMassage.ShowMessageAsync("تمت العملية", "تم حذف الشركة بنجاح");
            }
        }

        private bool CanFillUI(object obj)
        {
            if (DataGridSelectedCompany == null)
            {
                return false;
            }
            return true;
        }

        private void DoFillUI(object obj)
        {
            CompanyId = DataGridSelectedCompany.Id;
            Name = DataGridSelectedCompany.Name;
            Balance = DataGridSelectedCompany.Balance;
            Site = DataGridSelectedCompany.Site;
            Image = DataGridSelectedCompany.Image;
            Email = DataGridSelectedCompany.Email;
            Phone = DataGridSelectedCompany.Phone;
            Notes = DataGridSelectedCompany.Notes;
            IsAddCompanyFlyoutOpen = true;
        }

        private bool CanSelectImage(object obj)
        {
            return true;
        }

        private void DoSelectImage(object obj)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            var codecs = ImageCodecInfo.GetImageEncoders();
            dlg.Filter = string.Format("All image files ({1})|{1}|All files|*",
            string.Join("|", codecs.Select(codec =>
            string.Format("({1})|{1}", codec.CodecName, codec.FilenameExtension)).ToArray()),
            string.Join(";", codecs.Select(codec => codec.FilenameExtension).ToArray()));
            var result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                Image = File.ReadAllBytes(filename);
            }
        }

        private bool CanOpenAddCompanyFlyout(object obj)
        {
            return true;
        }

        private void DoOpenAddCompanyFlyout(object obj)
        {
            if (IsAddCompanyFlyoutOpen)
            {
                IsAddCompanyFlyoutOpen = false;
            }
            else
            {
                IsAddCompanyFlyoutOpen = true;
            }
        }
    }
}