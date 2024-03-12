using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace ReceiptManager.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class MyReceipt
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string StoreName { get; set; }
        public double Amounts { get; set; }
        public ImageSource Image { get; set; }
        public bool Pay { get; set;}
        public int GroupId { get; set; }
        public string ReceiptColor { get; set; }
    }
}
