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
using TXTReader.Net;

namespace TXTReader.ToolPanel {
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
                        //if (Book.Books != null) Book.Books.UploadAll();
                        break;
                }
            }
        }

        public UserPanel() {
            InitializeComponent();
            Status = UserStatus.Login;
            
            //Loaded += (d, e) => {
            //    tb_login_id.Text = "123456";
            //    tb_login_pw.Password = "123456";
            //    bn_login_login_Click(bn_login_login, null);
            //};
        }

        private async void bn_login_login_Click(object sender, RoutedEventArgs e) {
            try {
                String id=tb_login_id.Text;
                String pw=tb_login_pw.Password;
                ResponseEntity res = await G.Net.Login(id, pw);
                if (res.status!=MyHttp.successCode) {
                    MessageBox.Show(res.msg);
                    return;
                }
                Status = UserStatus.Online;
            } catch {
                MessageBox.Show("请检查网络是否连接。");
            }            
        }

        private void bn_login_reg_Click(object sender, RoutedEventArgs e) {
            Status = UserStatus.Register;
        }

        private void bn_reg_login_Click(object sender, RoutedEventArgs e) {
            Status = UserStatus.Login;
        }

        private async void bn_reg_reg_Click(object sender, RoutedEventArgs e) {
            try {
                if (tb_reg_pw.Password != tb_reg_conf.Password) {
                    MessageBox.Show("两次输入的密码不一样。");
                    return;
                }
                String id = tb_reg_id.Text;
                String pw = tb_reg_pw.Password;
                ResponseEntity res = await G.Net.SignUp(id, pw);
                if (res.status != MyHttp.successCode) {
                    MessageBox.Show(res.msg);
                    return;
                }
                Status = UserStatus.Login;
                tb_login_id.Text = tb_reg_id.Text;
                tb_login_pw.Password = tb_reg_pw.Password;
                bn_login_login_Click(bn_login_login, e);
            } catch {
                MessageBox.Show("请检查网络是否连接。");
            }
        }

        private async void bn_logout_Click(object sender, RoutedEventArgs e) {
            
            try {
                ResponseEntity res = await G.Net.Logout();
                if (res.status != MyHttp.successCode) {
                    MessageBox.Show(res.msg);
                }
            } catch {
                MessageBox.Show("请检查网络是否连接。");
            }
            Status = UserStatus.Login;
        }
    }
}
