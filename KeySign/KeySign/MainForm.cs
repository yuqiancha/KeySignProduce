using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace KeySign
{
    
    public partial class MainForm : Form
    {

        [DllImport("MFCLibrary1.dll", EntryPoint = "test",CallingConvention = CallingConvention.Cdecl)]
        static extern int test(int a,int b);

        [DllImport("MFCLibrary1.dll", EntryPoint = "Genrootkey", CallingConvention = CallingConvention.Cdecl)]
        static extern int Genrootkey(ref byte str);

        [DllImport("MFCLibrary1.dll", EntryPoint = "Genrootp10", CallingConvention = CallingConvention.Cdecl)]
        static extern int Genrootp10(ref byte str,string sub_name);

        [DllImport("MFCLibrary1.dll", EntryPoint = "Genrootcer", CallingConvention = CallingConvention.Cdecl)]
        static extern int Genrootcer(ref byte str,string serial,string not_befor,string not_after, string sub_name,int usep10);

        [DllImport("MFCLibrary1.dll", EntryPoint = "Genuserkey", CallingConvention = CallingConvention.Cdecl)]
        static extern int Genuserkey(int usegen);


        Form_AckMake myAckMakeForm = new Form_AckMake();
        SQLTestUnit mySQLTestUnit = new SQLTestUnit();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
         //   int t = test(3,4);
            try
            {
                byte[] s = new byte[1024];
                int ret = Genrootkey(ref s[0]);
                string strGet = System.Text.Encoding.Default.GetString(s, 0, s.Length);

                ret = Genrootp10(ref s[0], "CN=USER,O=TEST,C=CN");
                strGet = System.Text.Encoding.Default.GetString(s, 0, s.Length);

                ret = Genrootcer(ref s[0], "FEDCBA9876543210", "20170101000000", "20270101000000", "CN=USER,O=TEST,C=CN",1);
                strGet = System.Text.Encoding.Default.GetString(s, 0, s.Length);

                ret = Genuserkey(1);
                strGet = System.Text.Encoding.Default.GetString(s, 0, s.Length);

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            dateTimePicker_valid_start.Text = (System.DateTime.Now).ToString("yyyy-MM-dd");
            dateTimePicker_valid_end.Text = (System.DateTime.Now.AddYears(1)).ToString("yyyy-MM-dd");


            textBox_name.Text = ConfigurationManager.AppSettings["name"];
            textBox_age.Text = ConfigurationManager.AppSettings["age"];
            textBox_phone.Text = ConfigurationManager.AppSettings["phone"];
            textBox_id.Text = ConfigurationManager.AppSettings["id"];
            textBox_mail.Text = ConfigurationManager.AppSettings["mail"];
            textBox_project_name.Text = ConfigurationManager.AppSettings["project_name"];
            textBox_appid.Text = ConfigurationManager.AppSettings["appid"];
            textBox_appkey.Text = ConfigurationManager.AppSettings["appkey"];

            textBox_company_name.Text = ConfigurationManager.AppSettings["company_name"];
            textBox_company_phone.Text = ConfigurationManager.AppSettings["company_phone"];
            textBox_company_address.Text = ConfigurationManager.AppSettings["company_address"];
        }


        public int VerifyInfo()
        {
            
            CertInfo.name = textBox_name.Text;


            if (rdo_male.Checked)
                CertInfo.gender = "男";
            else
                CertInfo.gender = "女";

            CertInfo.age = textBox_age.Text;
            CertInfo.phone = textBox_phone.Text; ;
            CertInfo.id = textBox_id.Text;
            CertInfo.email = textBox_mail.Text;
            if (rdo_new.Checked)//发证类型
                CertInfo.issue_type = "新领";
            else
                CertInfo.issue_type = "补证";

            CertInfo.install_type = null;//安装类型
            if (checkBox1.Checked)
                CertInfo.install_type += checkBox1.Text;

            foreach (CheckBox item in panel3.Controls)
            {
                if (item.Checked)
                {
                    CertInfo.install_type += item.Text + ",";
                }
            }
            if (CertInfo.install_type!=null)
                CertInfo.install_type = CertInfo.install_type.Substring(0, CertInfo.install_type.Length - 1);
            else
            {
                MessageBox.Show("至少选择一种安装类型");
                return -1;
            }

            CertInfo.issue_day = dateTimePicker_issue.Text;//发证日期

            CertInfo.cert_validity_period_start = dateTimePicker_valid_start.Text;//证书有效期开始
            CertInfo.cert_validity_period_end = dateTimePicker_valid_end.Text;//证书有效期结束

            CertInfo.project_name = textBox_project_name.Text;
            CertInfo.appid = textBox_appid.Text;
            CertInfo.appkey = textBox_appkey.Text;
            CertInfo.company_name = textBox_company_name.Text;
            CertInfo.company_phone = textBox_company_phone.Text;
            CertInfo.company_address = textBox_company_address.Text;
            CertInfo.remarks = textBox_Remarks.Text;//备注


   

            string OnlyIDwithoutCRC = CertInfo.id.Substring(CertInfo.id.Length - 6, 6) + CertInfo.issue_day.Replace("/","");

            byte[] temp = Function.StrToHexByte(OnlyIDwithoutCRC + "C0DEC0DEC0DEC0DEC0DEC0DEC0DEC0DE");

            ushort CRC = 0xffff;
            ushort genpoly = 0x1021;
            for (int i = 0; i < temp.Length; i = i + 1)
            {
                CRC = Function.CRChware(temp[i], genpoly, CRC);
            }
            CertInfo.OnlyID = OnlyIDwithoutCRC +CRC.ToString("x4");

            //全部通过进入下一个页面，否则提示出错需要重新设置
            if (myAckMakeForm != null)
            {
                myAckMakeForm.Activate();
            }
            else
            {
                myAckMakeForm = new Form_AckMake();
            }
            myAckMakeForm.ShowDialog();

            return 1;
        }

        private void btn_Verify_Click(object sender, EventArgs e)
        {

            try
            {
                VerifyInfo();               

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button_sqlform_Click(object sender, EventArgs e)
        {
            if(mySQLTestUnit!=null)
            {
                mySQLTestUnit.Activate();
            }
            else
            {
                mySQLTestUnit = new SQLTestUnit();
            }
            mySQLTestUnit.ShowDialog();
        }
    }
}
