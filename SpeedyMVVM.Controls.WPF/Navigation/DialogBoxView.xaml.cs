using SpeedyMVVM.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpeedyMVVM.Controls.Navigation
{
    /// <summary>
    /// Logica di interazione per DialogBoxView.xaml
    /// </summary>
    public partial class DialogBoxView : Window
    {
        public DialogBoxView(DialogBoxViewModel context, Window owner)
        {
            this.Owner = owner;
            InitializeComponent();
            context.DialogResultChanged += Context_DialogResultChanged;
            this.DataContext = context;
            this.txb_input.Focus();
        }

        private void Context_DialogResultChanged(object sender, bool? e)
        {
            DialogResult = e;
            this.Close();
        }
    }
}
