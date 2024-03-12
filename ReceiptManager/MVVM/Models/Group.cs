using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace ReceiptManager.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Group
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string Color { get; set; }
        public int PendingReceipts { get; set; }
        public float Percentage { get; set; }
        public bool IsSelected { get; set; }
    }


}
