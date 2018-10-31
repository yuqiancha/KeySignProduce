using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            string CmdStr = "insert into tableall(姓名,性别,年龄,手机号,身份证号,邮箱账号,证书类型,安装类型,发证日期,证书有效期,项目名称,APPID,APP密码,所属单位名称,所属单位电话,所属单位地址,备注) " +
              "values(@name,@gender, @age, @phone, @id,@mail,@issue_type,@install_type,@issue_day,@valid_period,@project_name,@appid,@appkey,@company_name,@company_phone,@company_address,@remarks)";


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

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    timer1.Enabled = true;

                }
                catch(MySqlException ex)
                {
                    MessageBox.Show("数据库中已存在此身份证号，请核查！"+ex.Message);
                }
            }
        }

        private void Form_AckMake_Load(object sender, EventArgs e)
        {
            label_name.Text = "姓名:"+CertInfo.name;
            label_gender.Text = "性别:" + CertInfo.gender;
            label_age.Text = "年龄:" + CertInfo.age;

            label_phone.Text = "手机号:"+CertInfo.phone;

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
            if (progressBar1.Value>=100)
            {
                timer1.Enabled = false;
                this.Close();
            }


        }
    }
}
