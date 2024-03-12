using ReceiptManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Input;
using ReceiptManager.MVVM.View;

namespace ReceiptManager.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AllReceiptsViewModel
    {
        public ObservableCollection<Group> Groups { get; set; }
        public ObservableCollection<MyReceipt> Receipts { get; set; }

        public AllReceiptsViewModel()
        {
            FillData();
            Receipts.CollectionChanged += Receipts_CollectionChanged;
        }

        private void Receipts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateData();
        }



        private void FillData()
        {
            Groups = new ObservableCollection<Group>
            {
                new Group
                {
                    Id = 1,
                    GroupName = "Shopping",
                    Color = "#285C42"
                },
                new Group
                {
                    Id = 2,
                    GroupName = "Restaurant",
                    Color = "#285C42"
                },
                new Group
                {
                    Id = 3,
                    GroupName = "Internet",
                    Color = "#285C42"
                },
                new Group
                {
                    Id = 4,
                    GroupName = "Other",
                    Color = "#285C42"
                }

            };

            Receipts = new ObservableCollection<MyReceipt>
            {
                new MyReceipt
                {
                    StoreName ="Tesco",
                    Amounts = 12.50,
                    Pay = false,
                    GroupId = 1,
                    Id = 1,
                    Address ="16 Winstaley Rd",
                    Image = ImageSource.FromFile("receipt1")

                },
                new MyReceipt
                {
                    StoreName ="BookShop",
                    Amounts = 13.0,
                    Pay = false,
                    GroupId = 4,
                    Id = 2,
                    Address ="16 Jane Street",
                    Image = ImageSource.FromFile("receipt2")

                },
                new MyReceipt
                {
                    StoreName ="KFC",
                    Amounts = 15.50,
                    Pay = false,
                    GroupId = 2,
                    Id=3,
                    Address="12a Oxford Rd",
                    Image = ImageSource.FromFile("receipt3")
                },
                new MyReceipt
                {
                    StoreName ="Amazon",
                    Amounts = 7.50,
                    Pay = false,
                    GroupId = 3,
                    Id=4,
                    Address="51 Metcombe Way",
                    Image = ImageSource.FromFile("receipt4")
                },
            };

            UpdateData();

            
        }

        public void UpdateData()
        {
            foreach (var c in Groups) 
            { 
                var receipts = from t in Receipts
                               where t.GroupId == c.Id
                               select t;

                var pay = from t in receipts
                               where t.Pay == true
                               select t;

                var notPay = from t in receipts
                               where t.Pay == false
                               select t;

                c.PendingReceipts = notPay.Count();
                c.Percentage = (float)pay.Count() / (float)receipts.Count();
            }
            foreach (var t in Receipts)
            {
                var receiptColor = (from c in Groups
                                    where c.Id == t.GroupId
                                    select c.Color).FirstOrDefault();
                t.ReceiptColor = receiptColor;
            }
        }

        

    }
}
