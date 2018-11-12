using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeySign
{
    public partial class Form_AckMake : Form
    {
        public Form_AckMake()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            MajorLog.Debug("点击确认并制证");

            if (button1.Text == "确认并制证")
            {

                #region 制证

                try
                {
                    MajorLog.Debug("开始制证");
                    string downCmd = "CN=USER,O=" + CertInfo.OnlyID + ",C=CN";
                    //  string downCmdRoot = "CN=ROOT,O=TEST,C=CN";
                    byte[] s = new byte[1024];
                    int ret = 0;

                    ret = Function.Genuserkey();//产生用户密钥对
                    if (ret == -1)
                    {
                        return;
                    }
                    MajorLog.Info("产生用户密钥对");

                    ret = Function.Genuserp10(ref s[0], downCmd);//产生用户P10
                    if (ret == -1)
                    {
                        return;
                    }
                    MajorLog.Info("产生用户P10");

                    ret = Function.Genusercer(ref s[0], "FEDCBA9876543210", "20170101000000", "20270101000000", downCmd, 1);//产生用户证书
                    MajorLog.Info("产生用户证书");

                    string strGet2 = System.Text.Encoding.Default.GetString(s, 0, s.Length);
                    //    int len = strGet2.Length;
                    //    string downStr = strGet2.Substring(0, len);

                    ret = Function.Importcert(strGet2);
                    if (ret > 0)
                    {
                        MajorLog.Debug("写入证书--成功");
                    }
                    else
                    {
                        MajorLog.Debug("写入证书--失败");
                    }

                    timer1.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MajorLog.Debug(ex.ToString());
                }


                #endregion




                CertInfo.state = "0";
                string CmdStr = "insert into tableall(姓名,性别,年龄,手机号,身份证号,邮箱账号,证书类型,安装类型,发证日期,证书有效期,项目名称,APPID,APP密码,所属单位名称,所属单位电话,所属单位地址,备注,OnlyID,状态,产权所属单位) " +
                  "values(@name,@gender, @age, @phone, @id,@mail,@issue_type,@install_type,@issue_day,@valid_period,@project_name,@appid,@appkey,@company_name,@company_phone,@company_address,@remarks,@OnlyID,@state,@belong)";

                if (Function.UseDataBase != 0)
                {
                    using (MySqlConnection con = new MySqlConnection(SQLClass.connsql))
                    using (MySqlCommand cmd = new MySqlCommand(CmdStr, con))
                    {
                        try
                        {
                            cmd.Parameters.AddWithValue("@name", CertInfo.name);
                            cmd.Parameters.AddWithValue("@gender", CertInfo.gender);
                            cmd.Parameters.AddWithValue("@age", CertInfo.age);
                            cmd.Parameters.AddWithValue("@phone", CertInfo.phone);
                            cmd.Parameters.AddWithValue("@id", CertInfo.id);

                            cmd.Parameters.AddWithValue("@mail", CertInfo.email);
                            cmd.Parameters.AddWithValue("@issue_type", CertInfo.issue_type);
                            cmd.Parameters.AddWithValue("@install_type", CertInfo.install_type);
                            cmd.Parameters.AddWithValue("@issue_day", CertInfo.issue_day);
                            cmd.Parameters.AddWithValue("@valid_period", CertInfo.cert_validity_period);
                            cmd.Parameters.AddWithValue("@project_name", CertInfo.project_name);
                            cmd.Parameters.AddWithValue("@appid", CertInfo.appid);
                            cmd.Parameters.AddWithValue("@appkey", CertInfo.appkey);
                            cmd.Parameters.AddWithValue("@company_name", CertInfo.company_name);
                            cmd.Parameters.AddWithValue("@company_phone", CertInfo.company_phone);
                            cmd.Parameters.AddWithValue("@company_address", CertInfo.company_address);
                            cmd.Parameters.AddWithValue("@remarks", CertInfo.remarks);
                            cmd.Parameters.AddWithValue("@OnlyID", CertInfo.OnlyID);
                            cmd.Parameters.AddWithValue("@State", CertInfo.state);
                            cmd.Parameters.AddWithValue("@Belong", CertInfo.company_belong);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();

                            button1.Text = "制证中...";
                            button1.BackColor = Color.Green;

                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                else
                {
                    MajorLog.Debug("未使用数据库");
                }

            }
            else
            {
                MessageBox.Show("制证已完成，请关闭此窗口！");
            }
        }

        private void Form_AckMake_Load(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            button1.Visible = true;
            button2.Visible = false;
            label_onlyid.Text = null;
            button1.Text = "确认并制证";
            button1.BackColor = Color.RoyalBlue;

            label_name.Text = "姓名:" + CertInfo.name;
            label_gender.Text = "性别:" + CertInfo.gender;
            label_age.Text = "年龄:" + CertInfo.age;

            label_phone.Text = "手机号:" + CertInfo.phone;

            label_id.Text = "身份证号:" + CertInfo.id;
            label_mail.Text = "邮箱号:" + CertInfo.email;
            label_issue_type.Text = "证书类型:" + CertInfo.issue_type;
            label_install_type.Text = "设备类型:" + CertInfo.install_type;
            label_issue_day.Text = "安装日期:" + CertInfo.issue_day;
            CertInfo.cert_validity_period = CertInfo.cert_validity_period_start + "至" + CertInfo.cert_validity_period_end;
            label_valid_period.Text = "证书有效期:" + CertInfo.cert_validity_period;

            label_project_name.Text = "项目名称:" + CertInfo.project_name;
            label_appid.Text = "APP ID:" + CertInfo.appid;
            label_appkey.Text = "APP密码:" + CertInfo.appkey;
            label_company_name.Text = "所属单位名称:" + CertInfo.company_name;
            label_company_phone.Text = "所属单位电话:" + CertInfo.company_phone;
            label_company_address.Text = "所属单位地址:" + CertInfo.company_address;
            label_Remarks.Text = "备注:" + CertInfo.remarks;
            label_company_belong.Text = "产权所属单位:" + CertInfo.company_belong;


            progressBar1.Maximum = 100;//设置最大长度值
            progressBar1.Value = 0;//设置当前值
            progressBar1.Step = 5;//设置没次增长多少




        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value += 20;
            if (progressBar1.Value >= 100)
            {
                timer1.Enabled = false;

                button1.Text = "制证完成";
                button1.BackColor = Color.Green;
                // progressBar1.Visible = false;
                button1.Visible = false;
                button2.Visible = true;
                label_onlyid.Text = "恭喜您制证成功！\r\n" +
                    "证书编号：" + CertInfo.OnlyID + "\r\n" +
                    "使用人员：" + CertInfo.name + "\r\n" +
                    "证书有效期：" + CertInfo.cert_validity_period;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            int ret = 0;
            try
            {
                ret = Program.openport("Gprinter GP-3120TU");                                           //Open specified printer driver
                if (ret < 1)
                {
               //     MessageBox.Show("请检查打印机是否就绪！");
                    return;
                }
                string print1 = "姓名:" + CertInfo.name;
                string print2 = "发证日期:" + CertInfo.issue_day.Substring(2,8);
                string print3 = CertInfo.OnlyID;

                ret = Program.setup("30", "17", "6", "10", "0", "1", "0");                           //Setup the media size and sensor type info
                ret = Program.clearbuffer();                                                           //Clear image buffer                                                                                                         //    ret = TSCLIB_DLL.barcode("0", "0", "128", "10", "1", "0", "2", "2", "Barcode Test"); //Drawing barcode
                ret = Program.printerfont("0", "24", "TSS24.BF2", "0", "1", "1", print1);        //Drawing printer font
                ret = Program.printerfont("0", "56", "TSS24.BF2", "0", "1", "1", print2);        //Drawing printer font
                ret = Program.printerfont("0", "88", "TSS24.BF2", "0", "1", "1", print3);        //Drawing printer font

                //ret = Program.setup("40", "32", "6", "10", "0", "1", "0");                           //Setup the media size and sensor type info
                //ret = Program.clearbuffer();                                                           //Clear image buffer                                                                                                         //    ret = TSCLIB_DLL.barcode("0", "0", "128", "10", "1", "0", "2", "2", "Barcode Test"); //Drawing barcode
                //ret = Program.printerfont("0", "40", "TSS24.BF2", "0", "1", "1", print1);        //Drawing printer font
                //ret = Program.printerfont("0", "80", "TSS24.BF2", "0", "1", "1", print2);        //Drawing printer font
                //ret = Program.printerfont("0", "120", "TSS24.BF2", "0", "1", "1", print3);        //Drawing printer font

                ret = Program.printlabel("1", "1");                                                    //Print labels
                ret = Program.closeport();
            }
            catch (Exception ex)
            {
                MajorLog.Info("打码机未连接或出现异常情况，无法正常打印标签！" + ex.ToString());
            }
        }
    }
}
