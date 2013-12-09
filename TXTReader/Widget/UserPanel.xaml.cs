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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TXTReader.Widget {
    /// <summary>
    /// UserPanel.xaml 的交互逻辑
    /// </summary>
    public partial class UserPanel : UserControl {

        public enum UserStatus { Login, Register, Online };

        private UserStatus status;
        public UserStatus Status {
            get { return status; }
            set {
                status = value;
                switch (status) {
                    case UserStatus.Login: 
                        g_login.Visibility = Visibility.Visible;
                        g_register.Visibility = Visibility.Collapsed;
                        g_online.Visibility = Visibility.Collapsed;
                        break;
                    case UserStatus.Register:
                        g_login.Visibility = Visibility.Collapsed;
                        g_register.Visibility = Visibility.Visible;
                        g_online.Visibility = Visibility.Collapsed;
                        break;
                    case UserStatus.Online:
                        g_login.Visibility = Visibility.Collapsed;
                        g_register.Visibility = Visibility.Collapsed;
                        g_online.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        public UserPanel() {
            InitializeComponent();
            Status = UserStatus.Login;
        }

        private void bn_login_login_Click(object sender, RoutedEventArgs e) {
            Status = UserStatus.Online;
        }

        private void bn_login_reg_Click(object sender, RoutedEventArgs e) {
            Status = UserStatus.Register;
        }

        private void bn_reg_login_Click(object sender, RoutedEventArgs e) {
            Status = UserStatus.Login;
        }

        private void bn_reg_reg_Click(object sender, RoutedEventArgs e) {
            Status = UserStatus.Online;
        }

        private void bn_logout_Click(object sender, RoutedEventArgs e) {
            Status = UserStatus.Login;
        }
    }
}
