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
                string CmdStr = "insert into tableall(姓名,性别,年龄,手机号,身份证号,邮箱账号,证书类型,安装类型,发证日期,证书有效期,项目名称,APPID,APP密码,所属单位名称,所属单位电话,所属单位地址,备注,OnlyID) " +
                  "values(@name,@gender, @age, @phone, @id,@mail,@issue_type,@install_type,@issue_day,@valid_period,@project_name,@appid,@appkey,@company_name,@company_phone,@company_address,@remarks,@OnlyID)";

                if (Function.UseDataBase!=0)
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

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();

                            timer1.Enabled = true;

                            button1.Text = "制证中...";
                            button1.BackColor = Color.Green;

                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("数据库中已存在此身份证号，请核查！" + ex.Message);
                        }
                    }
                }
                else
                {
                    MajorLog.Debug("未使用数据库");
                }

                #region 制证

                try
                {
                    MajorLog.Debug("开始制证");

                    string startday = CertInfo.cert_validity_period_start.Replace("/", "") + "000000";
                    string endday = CertInfo.cert_validity_period_end.Replace("/", "") + "000000";
                    string downCmd = "CN=USER,O="+CertInfo.OnlyID+",C=CN";//替换原来的"CN=USER,O=TEST,C=CN"

                    string downCmdRoot = "CN=ROOT,O=TEST,C=CN";//替换原来的"CN=USER,O=TEST,C=CN"
               //     downCmd = "CN=USER,O=TEST,C=CN";//替换原来的"CN=USER,O=TEST,C=CN"

                    byte[] s = new byte[1024];
                    int ret = 0;
                    ret = Function.Genrootkey(ref s[0]);//产生根证书密钥对
                    if(ret>0) MajorLog.Debug("产生根证书密钥对--成功");
                    else MajorLog.Debug("产生根证书密钥对--失败");

                    ret = Function.Genrootp10(ref s[0], downCmdRoot);//产生根证书P10   
                    if (ret > 0) MajorLog.Debug("产生根证书P10--成功");
                    else MajorLog.Debug("产生根证书P10--失败");

                    //     ret = Function.Genrootcer(ref s[0], "FEDCBA9876543210", "20170101000000", "20270101000000", "CN=USER,O=TEST,C=CN", 1);
                    ret = Function.Genrootcer(ref s[0], CertInfo.OnlyID, startday, endday, downCmdRoot, 1);//产生根证书
                    if (ret > 0) MajorLog.Debug("产生根证书P10--成功");
                    else MajorLog.Debug("产生根证书--失败");

                    ret = Function.Genuserkey();//产生用户密钥对
                    if (ret > 0) MajorLog.Debug("产生用户密钥对--成功");
                    else MajorLog.Debug("产生用户密钥对--失败");

                    ret = Function.Genuserp10(ref s[0], downCmd);//产生用户P10

                    ret = Function.Genusercer(ref s[0], CertInfo.OnlyID, startday, endday, downCmd, 1);//产生用户证书
                    //     ret = Function.Genusercer(ref s[0], "FEDCBA9876543210", "20170101000000", "20270101000000", "CN=USER,O=TEST,C=CN", 1);

         
                    string strGet2 = System.Text.Encoding.Default.GetString(s, 0, s.Length);
                    int len = strGet2.Length;
                    string downStr = strGet2.Substring(0, len);

                    ret = Function.Importcert(strGet2);
                    if (ret > 0) MajorLog.Debug("写入用户证书--成功");
                    else MajorLog.Debug("写入用户证书--失败");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MajorLog.Debug(ex.ToString());
                }


                #endregion



            }
            else
            {
                MessageBox.Show("制证已完成，请关闭此窗口！");
            }
        }

        private void Form_AckMake_Load(object sender, EventArgs e)
        {
            label_onlyid.Text = null;
            button1.Text = "确认并制证";
            button1.BackColor = Color.RoyalBlue;

            label_name.Text = "姓名:" + CertInfo.name;
            label_gender.Text = "性别:" + CertInfo.gender;
            label_age.Text = "年龄:" + CertInfo.age;

            label_phone.Text = "手机号:" + CertInfo.phone;

            label_id.Text = "身份证号:" + CertInfo.id;
            label_mail.Text = "邮箱号:" + CertInfo.age;
            label_issue_type.Text = "证书类型:" + CertInfo.issue_type;
            label_install_type.Text = "安装类型:" + CertInfo.install_type;
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

            progressBar1.Maximum = 100;//设置最大长度值
            progressBar1.Value = 0;//设置当前值
            progressBar1.Step = 5;//设置没次增长多少
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value += 5;
            if (progressBar1.Value >= 100)
            {
                timer1.Enabled = false;
                //int ret = 0;
                //try
                //{
                //    ret = Program.openport("Gprinter GP-3120TU");                                           //Open specified printer driver
                //    ret = Program.setup("30", "17", "6", "10", "0", "1", "0");                           //Setup the media size and sensor type info
                //    ret = Program.clearbuffer();                                                           //Clear image buffer                                                                                                         //    ret = TSCLIB_DLL.barcode("0", "0", "128", "10", "1", "0", "2", "2", "Barcode Test"); //Drawing barcode
                //    ret = Program.printerfont("0", "24", "TSS24.BF2", "0", "1", "1", CertInfo.name);        //Drawing printer font
                //    ret = Program.printerfont("0", "56", "TSS24.BF2", "0", "1", "1", CertInfo.issue_day);        //Drawing printer font
                //    ret = Program.printerfont("0", "88", "TSS24.BF2", "0", "1", "1", CertInfo.OnlyID);        //Drawing printer font
                //    ret = Program.printlabel("1", "1");                                                    //Print labels
                //    ret = Program.closeport();
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show("打码机未连接或出现异常情况，无法正常打印标签！" + ex.Message);
                //}

                button1.Text = "制证完成";
                button1.BackColor = Color.Green;

                label_onlyid.Text = "您的数字身份证已制作完成，编号为:"+CertInfo.OnlyID+ "(身份证后六位+制证日期八位+随机验证码四位";
            }
        }
    }
}
