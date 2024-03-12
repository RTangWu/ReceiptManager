using ReceiptManager.MVVM.View;

namespace ReceiptManager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var navigationPage = new NavigationPage(new AllReceiptsView());

       //     navigationPage.BarBackgroundColor = Color.FromArgb("#FFC14F");
     //       navigationPage.BarTextColor = Color.FromArgb("Black");



            MainPage = navigationPage;

        }
    }
}
