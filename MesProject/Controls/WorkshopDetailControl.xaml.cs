using MesProject.ViewModels;
using System.Windows.Controls;

namespace MesProject.Controls
{
    /// <summary>
    /// WorkshopDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class WorkshopDetailControl : UserControl
    {
        public WorkshopDetailControl()
        {
            InitializeComponent();
            DataContext = new WorkshopDetailViewModel();
        }
    }
}
