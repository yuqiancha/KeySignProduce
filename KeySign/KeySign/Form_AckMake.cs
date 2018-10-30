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

        }

        private void Form_AckMake_Load(object sender, EventArgs e)
        {
            label_name.Text = label_name.Text+":"+CertInfo.name;
            label_gender.Text = label_gender.Text + ":" + CertInfo.gender;
            label_age.Text = label_age.Text + ":" + CertInfo.age;

            label_phone.Text = label_phone.Text + ":"+CertInfo.phone;

            label_id.Text = label_id.Text + ":" + CertInfo.id;
            label_mail.Text = label_mail.Text + ":" + CertInfo.age;
            label_issue_type.Text = label_issue_type.Text + ":" + CertInfo.issue_type;
            label_install_type.Text = label_install_type.Text + ":" + CertInfo.install_type;
            label_issue_day.Text = label_issue_day.Text + ":" + CertInfo.issue_day;
            label_valid_period.Text = label_valid_period.Text + ":" + CertInfo.cert_validity_period_start+"至"+CertInfo.cert_validity_period_end;
            label_project_name.Text = label_project_name.Text + ":" + CertInfo.project_name;
            label_Remarks.Text = label_Remarks.Text + ":" + CertInfo.remarks;
            label_appid.Text = label_appid.Text + ":" + CertInfo.appid;
            label_appkey.Text = label_appkey.Text + ":" + CertInfo.appkey;
            label_company_name.Text = label_company_name.Text + ":" + CertInfo.company_name;
            label_company_phone.Text = label_company_phone.Text + ":" + CertInfo.company_phone;
            label_company_address.Text = label_company_address.Text + ":" + CertInfo.company_address;
        }
    }
}
