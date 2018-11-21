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
using log4net;
using log4net.Config;
//using Microsoft.International.Converters.PinYinConverter;
using NPinyin;


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
            dateTimePicker_valid_end.Text = (System.DateTime.Now.AddYears(3)).ToString("yyyy-MM-dd");


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

        public int VerifyInfo()
        {
            if (!Regex.IsMatch(textBox_name.Text, @"^[\u4E00-\u9FA5\uf900-\ufa2d·s]{2,20}$"))
            {
                label_name.Visible = true;
                return -1;
            }
            else
            {
                label_name.Visible = false;
                CertInfo.name = textBox_name.Text;
            }

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

            string bornyear_str = textBox_id.Text.Substring(6, 4);
            string bornmonth_str = textBox_id.Text.Substring(10, 2);
            string bornday_str = textBox_id.Text.Substring(12, 2);

            int born_year = int.Parse(bornyear_str);
            int born_month = int.Parse(bornmonth_str);
            int born_day = int.Parse(bornday_str);
            if (born_year < 1900 || born_year > 2030)
            {
                MessageBox.Show("身份证信息有误，请重新输入！");
                return -1;
            }
            if (born_month > 12)
            {
                MessageBox.Show("身份证信息有误，请重新输入！");
                return -1;
            }
            if (born_day < 1 || born_day > 31)
            {
                MessageBox.Show("身份证信息有误，请重新输入！");
                return -1;
            }

            CertInfo.id = textBox_id.Text;

            if (rdo_new.Checked)//发证类型
                CertInfo.issue_type = "新领";
            else
                CertInfo.issue_type = "补证";

            string CmdStr = "SELECT * FROM tableall WHERE 身份证号 = @CertID";

            if (Function.UseDataBase != 0)
            {
                using (MySqlConnection con = new MySqlConnection(SQLClass.connsql))
                using (MySqlCommand cmd = new MySqlCommand(CmdStr, con))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@CertID", CertInfo.id);

                        con.Open();
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                        {
                            String dr = obj.ToString();
                            Trace.WriteLine(dr);
                            if (CertInfo.issue_type == "新领")//新领冲突
                            {
                                MessageBox.Show("该身份证号码已进行过制证，请选择证书类型为补证！");
                                con.Close();
                                return -1;
                            }
                            else//补证
                            {
                                string CmdStr2 = "UPDATE tableall SET 状态 = 2 WHERE 身份证号 = @id ";
                                MySqlCommand cmd2 = new MySqlCommand(CmdStr2, con);
                                cmd2.Parameters.AddWithValue("@id", CertInfo.id);
                                cmd2.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                        else
                        {
                            CertInfo.state = "0";
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

            if (textBox_mail.Text == null || textBox_mail.Text == "")
            {
                label_email.Visible = true;
                return -1;
            }
            CertInfo.email = textBox_mail.Text;

            CertInfo.install_type = null;//安装类型
            if (checkBox7.Checked)
            {
                CertInfo.install_type = textBox_instype.Text;
                if (CertInfo.install_type == null || CertInfo.install_type == "")
                {
                    MessageBox.Show("选择其它时，输入信息不能为空");
                    return -1;
                }
                CertInfo.install_type += ",";
            }

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
                MessageBox.Show("至少选择一种设备类型");
                return -1;
            }


            CertInfo.issue_day = dateTimePicker_issue.Text;//发证日期

            CertInfo.cert_validity_period_start = dateTimePicker_valid_start.Text;//证书有效期开始
            CertInfo.cert_validity_period_end = dateTimePicker_valid_end.Text;//证书有效期结束


            if (textBox_project_name.Text == null || textBox_project_name.Text == "")
            {
                label_project.Visible = true;
                return -1;
            }
            CertInfo.project_name = textBox_project_name.Text;

            if (textBox_appid.Text == null || textBox_appid.Text == "")
            {
                label_appid1.Visible = true;
                return -1;
            }
            else
            {
                label_appid1.Visible = false;
            }
            CertInfo.appid = textBox_appid.Text;



            #region 验证APPID

            if (rdo_new.Checked)
            {
                string appidstr = textBox_appid.Text;
                string CmdStr3 = "SELECT * FROM tableall WHERE `APPID`= @CertAPPID";
                if (Function.UseDataBase != 0)
                {
                    using (MySqlConnection con = new MySqlConnection(SQLClass.connsql))
                    using (MySqlCommand cmd = new MySqlCommand(CmdStr3, con))
                    {
                        try
                        {
                            cmd.Parameters.AddWithValue("@CertAPPID", appidstr);

                            con.Open();
                            object obj = cmd.ExecuteScalar();
                            if (obj != null)
                            {
                                label_appid2.Visible = true;
                                return -1;
                            }
                            else
                            {
                                textBox_appid.Text = appidstr;
                                label_appid2.Visible = false;

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
            }
            else
            {
                string CmdStr3 = "SELECT appid FROM tableall WHERE `身份证号`= @CertID";
                if (Function.UseDataBase != 0)
                {
                    using (MySqlConnection con = new MySqlConnection(SQLClass.connsql))
                    using (MySqlCommand cmd = new MySqlCommand(CmdStr3, con))
                    {
                        try
                        {
                            cmd.Parameters.AddWithValue("@CertID", textBox_id.Text);

                            con.Open();
                            object obj = cmd.ExecuteScalar();
                            if (obj != null && obj.ToString()==textBox_appid.Text)
                            {
                                label_appid2.Visible = false;
                                con.Close();
                            }
                            else
                            {
                                label_appid2.Visible = true;
                                con.Close();
                                return -1;                                
                            }
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show(ex.ToString());
                            MajorLog.Error(ex.ToString());
                        }

                    }
                }
            }
            #endregion

            if (Regex.IsMatch(textBox_appkey.Text, @"^[a-zA-Z][a-zA-Z0-9]\w{5,15}$", RegexOptions.IgnoreCase))
            {
                if (Regex.IsMatch(textBox_appkey.Text, @"[\d]", RegexOptions.IgnoreCase))
                {
                    CertInfo.appkey = textBox_appkey.Text;
                }
                else
                {
                    label_appkey.Visible = true;

                    return -1;
                }
            }
            else
            {
                label_appkey.Visible = true;
                return -1;
            }



            if (textBox_company_name.Text == null || textBox_company_name.Text == "")
            {
                label_company_name.Visible = true;
                return -1;
            }
            CertInfo.company_name = textBox_company_name.Text;


            if (textBox_company_phone.Text == null || textBox_company_phone.Text == "")
            {
                label_company_phone.Visible = true;
                return -1;
            }
            CertInfo.company_phone = textBox_company_phone.Text;


            if (textBox_company_address.Text == null || textBox_company_address.Text == "")
            {
                label_company_address.Visible = true;
                return -1;
            }
            CertInfo.company_address = textBox_company_address.Text;


            CertInfo.remarks = textBox_Remarks.Text;//备注

            if (textBox_belong_company.Text == null || textBox_belong_company.Text == "")
            {
                label_belong.Visible = true;
                return -1;
            }
            CertInfo.company_belong = textBox_belong_company.Text;

            Random rd = new Random();

            string OnlyIDwithoutCRC = CertInfo.id.Substring(CertInfo.id.Length - 6, 6) + CertInfo.issue_day.Replace("/", "").Substring(0, 8);

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
            string valid = ConfigurationManager.AppSettings["used"];
            if (valid != "paid")
            {
                DateTime dt = DateTime.Now;
                string year = dt.Year.ToString();
                Trace.WriteLine(year);
                if (year != "2018") this.Close();
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //  label_appid1.Visible = false;
            //  label_appid2.Visible = false;
            textBox_appid.Text = "";

            if (rdo_new.Checked)
            {
                string trs = Pinyin.GetPinyin(textBox_name.Text);
                string[] nmlist = Regex.Split(trs, @"\s+");
                if (nmlist.Length >= 2)
                {
                    nmlist[0] = nmlist[0].Substring(0, 1).ToUpper();
                    textBox_appid.Text += nmlist[0];
                    for (int i = 1; i < nmlist.Length; i++)
                    {
                        string temp = nmlist[i];
                        nmlist[i] = temp.Substring(0, 1).ToUpper() + temp.Substring(1, temp.Length - 1);
                        textBox_appid.Text += nmlist[i];
                    }

                    int count = 1;
                    bool valid = false;
                    string appidstr = textBox_appid.Text;
                    string CmdStr = "SELECT * FROM tableall WHERE `APPID`= @CertAPPID";
                    while (valid != true)
                    {
                        if (Function.UseDataBase != 0)
                        {
                            using (MySqlConnection con = new MySqlConnection(SQLClass.connsql))
                            using (MySqlCommand cmd = new MySqlCommand(CmdStr, con))
                            {
                                try
                                {
                                    cmd.Parameters.AddWithValue("@CertAPPID", appidstr);

                                    con.Open();
                                    object obj = cmd.ExecuteScalar();
                                    if (obj != null)
                                    {

                                        appidstr = textBox_appid.Text + (count++).ToString("x2");
                                        valid = false;

                                    }
                                    else
                                    {
                                        textBox_appid.Text = appidstr;
                                        valid = true;
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
                    }
                }
                else
                {
                    MajorLog.Error("输入姓名不正确!");
                }
            }
            else
            {
                string CmdStr = "SELECT appid FROM tableall WHERE `身份证号`= @CertID";
                if (Function.UseDataBase != 0)
                {
                    using (MySqlConnection con = new MySqlConnection(SQLClass.connsql))
                    using (MySqlCommand cmd = new MySqlCommand(CmdStr, con))
                    {
                        try
                        {
                            cmd.Parameters.AddWithValue("@CertID", textBox_id.Text);

                            con.Open();
                            object obj = cmd.ExecuteScalar();
                            if (obj != null)
                            {
                                textBox_appid.Text = obj.ToString();
                            }
                            else
                            {
                                MessageBox.Show("数据库中不存在此身份证，请新领证书");
                            }
                            con.Close();
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show(ex.ToString());
                            MajorLog.Error(ex.ToString());
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (rdo_new.Checked)
            {
                if ((!Regex.IsMatch(textBox_id.Text, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", RegexOptions.IgnoreCase)))
                {
                    label_id.Visible = true;
                    label_firstID.Visible = true;
                }
                else
                {
                    label_id.Visible = false;
                    label_firstID.Visible = false;

                    textBox_appkey.Text = 'a' + textBox_id.Text.Substring(textBox_id.Text.Length - 6, 6);
                }
            }
            else
            {
                string CmdStr = "SELECT app密码 FROM tableall WHERE `身份证号`= @CertID";
                if (Function.UseDataBase != 0)
                {
                    using (MySqlConnection con = new MySqlConnection(SQLClass.connsql))
                    using (MySqlCommand cmd = new MySqlCommand(CmdStr, con))
                    {
                        try
                        {
                            cmd.Parameters.AddWithValue("@CertID", textBox_id.Text);

                            con.Open();
                            object obj = cmd.ExecuteScalar();
                            if (obj != null)
                            {
                                textBox_appkey.Text = obj.ToString();
                            }
                            else
                            {
                                MessageBox.Show("数据库中不存在此身份证，请新领证书");
                            }
                            con.Close();
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show(ex.ToString());
                            MajorLog.Error(ex.ToString());
                        }
                    }
                }



            }
        }

        private void textBox_mail_TextChanged(object sender, EventArgs e)
        {
            //"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"

            if ((!Regex.IsMatch(textBox_mail.Text, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.IgnoreCase)))
            {
                label_email.Visible = true;
            }
            else
            {
                label_email.Visible = false;
            }
        }

        private void textBox_company_phone_TextChanged(object sender, EventArgs e)
        {
            //^(\d{3.4}-)\d{7,8}$
            if (Regex.IsMatch(textBox_company_phone.Text, @"^(\d{3,4}-)\d{7,8}$") || Regex.IsMatch(textBox_company_phone.Text, @"1[3456789]\d{9}$"))
            {
                label_company_phone.Visible = false;
            }
            else
            {

                label_company_phone.Visible = true;


            }
        }

        private void textBox_belong_company_TextChanged(object sender, EventArgs e)
        {
            if (textBox_belong_company.Text == null || textBox_belong_company.Text == "")
            {
                label_belong.Visible = true;
            }
            else
            {
                label_belong.Visible = false;
            }
        }

        private void textBox_company_address_TextChanged(object sender, EventArgs e)
        {
            if (textBox_company_address.Text == null || textBox_company_address.Text == "")
            {
                label_company_address.Visible = true;
            }
            else
            {
                label_company_address.Visible = false;
            }
        }

        private void textBox_company_name_TextChanged(object sender, EventArgs e)
        {
            if (textBox_company_name.Text == null || textBox_company_name.Text == "")
            {
                label_company_name.Visible = true;
            }
            else
            {
                label_company_name.Visible = false;
            }
        }

        private void textBox_appkey_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(textBox_appkey.Text, @"^[a-zA-Z][a-zA-Z0-9]\w{5,15}$", RegexOptions.IgnoreCase))
            {
                if (Regex.IsMatch(textBox_appkey.Text, @"[\d]", RegexOptions.IgnoreCase))
                    label_appkey.Visible = false;
            }
            else
            {
                label_appkey.Visible = true;
            }
        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(textBox_name.Text, @"^[\u4E00-\u9FA5\uf900-\ufa2d·s]{2,20}$"))
            {
                label_name.Visible = true;
            }
            else
            {
                label_name.Visible = false;
            }
        }

        private void textBox_project_name_TextChanged(object sender, EventArgs e)
        {
            if (textBox_project_name.Text == null || textBox_project_name.Text == "")
            {
                label_project.Visible = true;
            }
            else
            {
                label_project.Visible = false;
            }
        }

        private void textBox_appid_TextChanged(object sender, EventArgs e)
        {
            if (textBox_appid.Text == null || textBox_appid.Text == "")
            {
                label_appid1.Visible = true;
            }
            else
            {
                label_appid1.Visible = false;
            }
        }

        private void rdo_new_Click(object sender, EventArgs e)
        {
            if (rdo_new.Checked)
            {
                textBox_appid.Enabled = true;
            }
            else
            {
                textBox_appid.Enabled = false;
            }
        }
    }
}
