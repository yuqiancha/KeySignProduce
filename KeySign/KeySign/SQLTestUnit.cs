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
    public partial class SQLTestUnit : Form
    {
        public SQLTestUnit()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //SELECT * FROM tableall ORDER BY `发证日期` DESC
                string myQuery = "select * from tableall ORDER BY `发证日期` DESC";
                dataGridView1.DataSource = SQLClass.ExcuteQueryUsingDataAdapter(myQuery);

            }
            catch (Exception ex)
            {
                MessageBox.Show("错误信息：" + ex.Message, "出现错误");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //string CmdStr = "insert into tableall(姓名,年龄,手机号,身份证号) values('平哥', '30', '13817897274', '320981198854012738')";

    //       string CmdStr =  string.Format(@"SELECT * FROM tableall WHERE (`姓名` LIKE '%{0}%')", textBox1.Text);

       //     SQLClass.SqlExcuteCMD(CmdStr);

            string myQuery = string.Format(@"SELECT * FROM tableall WHERE (`姓名` LIKE '%{0}%' OR `APPID` LIKE '%{0}%') ORDER BY `发证日期` DESC", textBox1.Text);
            dataGridView1.DataSource = SQLClass.ExcuteQueryUsingDataAdapter(myQuery);

            //     string myQuery = "select * from tableAll";
            //     dataGridView1.DataSource = SQLClass.ExcuteQueryUsingDataAdapter(myQuery);


        }

        private void button2_Click(object sender, EventArgs e)
        {
            String CmdStr = "delete * from tableAll";
            SQLClass.SqlExcuteCMD(CmdStr);

            string myQuery = "select * from tableAll";
            dataGridView1.DataSource = SQLClass.ExcuteQueryUsingDataAdapter(myQuery);
        }

        private void SQLTestUnit_Load(object sender, EventArgs e)
        {

        }
    }
}
