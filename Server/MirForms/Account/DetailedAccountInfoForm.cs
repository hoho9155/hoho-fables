using Server.MirDatabase;
using Server.MirEnvir;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.MirForms.Account
{
    public partial class DetailedAccountInfoForm : Form
    {
        public DetailedAccountInfoForm()
        {
            InitializeComponent();

            List<AccountInfo> accounts = SMain.Envir.AccountList;
            textBox1.Text = accounts[0].Characters.ToString();
        }
    }
}
