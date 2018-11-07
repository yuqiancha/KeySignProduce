﻿using System;
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
using log4net;
using log4net.Config;

namespace KeySign
{

    public partial class MainForm : Form
    {

        Form_AckMake myAckMakeForm = new Form_AckMake();
        SQLTestUnit mySQLTestUnit = new SQLTestUnit();
        CertForm myCertForm = new CertForm();
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MajorLog.Debug("这是一个Debug日志");
            label_age.Visible = false;
            Function.UseDataBase = int.Parse(ConfigurationManager.AppSettings["UseDataBase"]);

            if (int.Parse(ConfigurationManager.AppSettings["UseCertForm"]) == 1) button2.Visible = true;
            else button2.Visible = false;

            //MySQLPath
            SQLClass.connsql = @"" + ConfigurationManager.AppSettings["MySQLPath"];

            //SQLClass.connsql = @"server=" + ConfigurationManager.AppSettings["SQLIP"] +
            //    ";Database=dmkeybase;uid=" + ConfigurationManager.AppSettings["SQLNAME"] +
            //    ";pwd=" + ConfigurationManager.AppSettings["SQLPSWD"] + 
            //    ";SslMode="+ ConfigurationManager.AppSettings["SSLMODE"];

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
            textBox_belong_company.Text = ConfigurationManager.AppSettings["company_belong"];
        }


        public int VerifyInfo()
        {

            CertInfo.name = textBox_name.Text;


            if (rdo_male.Checked)
                CertInfo.gender = "男";
            else
                CertInfo.gender = "女";

            CertInfo.age = textBox_age.Text;
            int Fage = 0;
            int.TryParse(textBox_age.Text, out Fage);
            if (Fage < 1 || Fage > 80)
            {
                label_age.Visible = true;
                return -1;
            }
            else
            {
                label_age.Visible = false;
            }

            Regex regex = new Regex(@"1[3456789]\d{9}$");
            if (regex.IsMatch(textBox_phone.Text))
            {
                label_phone.Visible = false;
            }
            else
            {
                label_phone.Visible = true;
                return -1;
            }

            CertInfo.phone = textBox_phone.Text; ;


            if ((!Regex.IsMatch(textBox_id.Text, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", RegexOptions.IgnoreCase)))
            {
                label_id.Visible = true;
                return -1;
            }
            else
            {
                label_id.Visible = false;

            }


            CertInfo.id = textBox_id.Text;

            if (rdo_new.Checked)//发证类型
                CertInfo.issue_type = "新领";
            else
                CertInfo.issue_type = "补证";

            string CmdStr = "SELECT * FROM tableall WHERE 身份证号 = " + CertInfo.id + ";";

            if (Function.UseDataBase != 0)
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
                                string CmdStr2 = "UPDATE tableall SET 状态='作废' WHERE 身份证号 = @id ";
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
                        MessageBox.Show(ex.ToString());
                        MajorLog.Error(ex.ToString());
                    }

                }
            }

            CertInfo.email = textBox_mail.Text;

            CertInfo.install_type = null;//安装类型

            if (checkBox7.Checked)
            {
                CertInfo.install_type = textBox_instype.Text;
                if(CertInfo.install_type==null|| CertInfo.install_type == "")
                {
                    MessageBox.Show("至少选择一种安装类型");
                    return -1;
                }
            }
            else
            {
                foreach (CheckBox item in panel3.Controls)
                {
                    if (item.Checked)
                    {
                        CertInfo.install_type += item.Text + ",";
                    }
                }
                if (CertInfo.install_type != null)
                {
                    CertInfo.install_type = CertInfo.install_type.Substring(0, CertInfo.install_type.Length - 1);
                }
                else
                {
                    MessageBox.Show("至少选择一种安装类型");
                    return -1;
                }
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
            CertInfo.company_belong = textBox_belong_company.Text;

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

        private void textBox_age_TextChanged(object sender, EventArgs e)
        {
            int Fage = 0;
            int.TryParse(textBox_age.Text, out Fage);
            if (Fage < 1 || Fage > 80)
            {
                label_age.Visible = true;
            }
            else
            {
                label_age.Visible = false;
            }
        }

        private void textBox_phone_TextChanged(object sender, EventArgs e)
        {
            Regex regex = new Regex(@"1[3456789]\d{9}$");
            if (regex.IsMatch(textBox_phone.Text))
            {
                label_phone.Visible = false;
            }
            else
            {
                label_phone.Visible = true;
            }
        }

        private void textBox_id_TextChanged(object sender, EventArgs e)
        {

            if ((!Regex.IsMatch(textBox_id.Text, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", RegexOptions.IgnoreCase)))
            {
                label_id.Visible = true;
            }
            else
            {
                label_id.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (myCertForm != null)
            {
                myCertForm.Activate();
            }
            else
            {
                myCertForm = new CertForm();
            }
            myCertForm.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            string year = dt.Year.ToString();
            Trace.WriteLine(year);
            if (year != "2018") this.Close();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                foreach (CheckBox item in panel3.Controls)
                {
                    item.Checked = false;
                    item.Enabled = false;
                }
            }
            else
            {
                foreach (CheckBox item in panel3.Controls)
                {
                    item.Enabled = true;
                }
            }
        }
    }
}
