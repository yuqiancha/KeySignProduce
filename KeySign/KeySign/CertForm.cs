using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeySign
{
    public partial class CertForm : Form
    {

        string downCmd = "CN=USER,O=" + CertInfo.OnlyID + ",C=CN";
        string downCmdRoot = "CN=ROOT,O=TEST,C=CN";
        int ret = 0;
        byte[] s = new byte[1024];
        public CertForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ret = Function.Genrootkey(ref s[0]);//产生根证书密钥对
            if (ret > 0) MajorLog.Debug("产生根证书密钥对--成功");
            else MajorLog.Debug("产生根证书密钥对--失败");
            string strGet2 = System.Text.Encoding.Default.GetString(s, 0, s.Length);
            textBox1.Text = strGet2;
        }

        private void CertForm_Load(object sender, EventArgs e)
        {
            if(CertInfo.OnlyID!=null)
            textBox2.Text = CertInfo.OnlyID;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ret = Function.Genrootp10(ref s[0], downCmdRoot);//产生根证书P10   
            if (ret > 0) MajorLog.Debug("产生根证书P10--成功");
            else MajorLog.Debug("产生根证书P10--失败");
            string strGet2 = System.Text.Encoding.Default.GetString(s, 0, s.Length);
            textBox1.Text = strGet2;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ret = Function.Genrootcer(ref s[0], "FEDCBA9876543210", "20170101000000", "20270101000000", "CN=USER,O=TEST,C=CN", 1);
            //    ret = Function.Genrootcer(ref s[0], CertInfo.OnlyID, startday, endday, downCmdRoot, 1);//产生根证书
            if (ret > 0) MajorLog.Debug("产生根证书--成功");
            else MajorLog.Debug("产生根证书--失败");
            string strGet2 = System.Text.Encoding.Default.GetString(s, 0, s.Length);
            textBox1.Text = strGet2;
        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ret = Function.Genuserkey();//产生用户密钥对
            if (ret > 0) MajorLog.Debug("产生用户密钥对--成功");
            else MajorLog.Debug("产生用户密钥对--失败");
            textBox1.Text = "产生用户密钥对--成功";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ret = Function.Genuserp10(ref s[0], downCmd);//产生用户P10
            string strGet2 = System.Text.Encoding.Default.GetString(s, 0, s.Length);
            textBox1.Text = strGet2;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            downCmd = "CN=USER,O=" + textBox2.Text + ",C=CN";
            ret = Function.Genusercer(ref s[0], "FEDCBA9876543210", "20170101000000", "20270101000000", downCmd, 1);//产生用户证书
            string strGet2 = System.Text.Encoding.Default.GetString(s, 0, s.Length);
            textBox1.Text = strGet2;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string strGet2 = textBox1.Text;
            ret = Function.Importcert(strGet2);
            if (ret > 0) MajorLog.Debug("写入用户证书--成功");
            else MajorLog.Debug("写入用户证书--失败");
        }
    }
}
