using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ReceiptManager.MVVM.Models;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReceiptManager.MVVM.ViewModels
{
    public class EditReceiptViewModel
    {
        public string StoreName { get; set; }
        public double Amounts { get; set; }
        public int Id { get; set; }
        public ImageSource Image { get; set; }
        public string Address { get; set; }
        public ObservableCollection<MyReceipt> Receipts { get; set; }
        public ObservableCollection<Group> Groups { get; set; }


        public event EventHandler ChangesSaved;

        // Method to raise the event when changes are saved
        private void OnChangesSaved()
        {
            ChangesSaved?.Invoke(this, EventArgs.Empty);
        }

        // Method to handle saving changes
        public void SaveChanges()
        {
            OnChangesSaved();
        }




    }

}
