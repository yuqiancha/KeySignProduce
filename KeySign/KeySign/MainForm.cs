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
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace KeySign
{

    public partial class MainForm : Form
    {

        Form_AckMake myAckMakeForm = new Form_AckMake();
        SQLTestUnit mySQLTestUnit = new SQLTestUnit();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Function.UseDataBase = int.Parse(ConfigurationManager.AppSettings["UseDataBase"]);


            SQLClass.connsql = @"server="+ConfigurationManager.AppSettings["SQLIP"] +
                ";Database=dmkeybase;uid="+ ConfigurationManager.AppSettings["SQLNAME"] + 
                ";pwd="+ConfigurationManager.AppSettings["SQLPSWD"];

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

            if (rdo_new.Checked)//发证类型
                CertInfo.issue_type = "新领";
            else
                CertInfo.issue_type = "补证";

            string CmdStr = "SELECT * FROM tableall WHERE `身份证号` = " + CertInfo.id + ";";

            if (Function.UseDataBase!=0)
            {
                using (MySqlConnection con = new MySqlConnection(SQLClass.connsql))
                using (MySqlCommand cmd = new MySqlCommand(CmdStr, con))
                {
                    try
                    {
                        con.Open();
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                        {
                            String dr = obj.ToString();
                            Trace.WriteLine(dr);
                            if (CertInfo.issue_type == "新领")//新领冲突
                            {
                                MessageBox.Show("数据库中已存在此身份证号，请返回进行补证操作！");
                                con.Close();
                                return -1;
                            }
                            else//补证
                            {
                                string CmdStr2 = "UPDATE tableall SET `备注`='作废' WHERE `身份证号` = @id ";
                                MySqlCommand cmd2 = new MySqlCommand(CmdStr2, con);
                                cmd2.Parameters.AddWithValue("@id", CertInfo.id);
                                cmd2.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                        else
                        {
                            con.Close();
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("数据库中已存在此身份证号，请核查！" + ex.Message);
                    }

                }
            }

            CertInfo.email = textBox_mail.Text;


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
            if (CertInfo.install_type != null)
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


            Random rd = new Random();

            string OnlyIDwithoutCRC = CertInfo.id.Substring(CertInfo.id.Length - 6, 6) + CertInfo.issue_day.Replace("/", "");

            CertInfo.OnlyID = OnlyIDwithoutCRC + rd.Next(0, 32767).ToString("x4");

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button_sqlform_Click(object sender, EventArgs e)
        {
            if (mySQLTestUnit != null)
            {
                mySQLTestUnit.Activate();
            }
            else
            {
                mySQLTestUnit = new SQLTestUnit();
            }
            mySQLTestUnit.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
    }
}
