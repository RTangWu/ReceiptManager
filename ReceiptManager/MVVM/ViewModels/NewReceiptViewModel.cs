using ReceiptManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiptManager.MVVM.ViewModels
{
    public class NewReceiptViewModel
    {
        public string StoreName {  get; set; }
        public double Amounts { get; set; }
        public int Id { get; set; }
        public ImageSource Image { get; set; }
        public string Address { get; set; }

        public ObservableCollection<MyReceipt> Receipts { get; set; }
        public ObservableCollection<Group> Groups { get; set; }

    }
}
