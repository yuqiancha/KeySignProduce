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
                dataGridView1.Columns.Clear();

                if (textBox1.Text == null || textBox1.Text == "")
                {
                    string myQuery = "select 证书编号,姓名,性别,年龄,手机号,身份证号,邮箱账号,证书类型,设备类型,发证日期,证书有效期,项目名称,APPID,APP密码,所属单位名称,所属单位电话,所属单位地址,设备所属单位,备注 from tableall ORDER BY `发证日期` DESC";
                    dataGridView1.DataSource = SQLClass.ExcuteQueryUsingDataAdapter(myQuery);
                }
                else
                {
                    string myQuery = string.Format(@"SELECT 证书编号,姓名,性别,年龄,手机号,身份证号,邮箱账号,证书类型,设备类型,发证日期,证书有效期,项目名称,APPID,APP密码,所属单位名称,所属单位电话,所属单位地址,设备所属单位,备注  FROM tableall WHERE (`姓名` LIKE '%{0}%' OR `APPID` LIKE '%{0}%') ORDER BY `发证日期` DESC", textBox1.Text);
                    dataGridView1.DataSource = SQLClass.ExcuteQueryUsingDataAdapter(myQuery);
                }
                DataGridViewButtonColumn btncolumn = new DataGridViewButtonColumn();
                btncolumn.HeaderText = "打印";
                btncolumn.Name = "print";
                btncolumn.DefaultCellStyle.NullValue = "打印";
                dataGridView1.Columns.Add(btncolumn);

                dataGridView1.Columns[0].Width = 120;
                dataGridView1.Columns[1].Width = 60;
                dataGridView1.Columns[2].Width = 60;
                dataGridView1.Columns[3].Width = 60;

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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (dataGridView1.Columns[e.ColumnIndex].HeaderText == "打印")
                {

                    int ret = 0;
                    try
                    {
                        ret = Program.openport("Gprinter GP-3120TU");                                           //Open specified printer driver
                        if (ret < 1)
                        {
                            return;
                        }
                        MajorLog.Info(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                        string print1 = "姓名:" + dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();//

                        MajorLog.Info(dataGridView1.Rows[e.RowIndex].Cells[9].Value.ToString());

                        MajorLog.Info(dataGridView1.Rows[e.RowIndex].Cells[9].Value.ToString().Substring(2, 8));

                        string print2 = "发证日期:" + dataGridView1.Rows[e.RowIndex].Cells[9].Value.ToString().Substring(2, 8);

                        MajorLog.Info(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());

                        string print3 = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();

                        DialogResult dr = MessageBox.Show(print1 +"\n"+print2+"\n"+print3, "是否打印", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (dr == DialogResult.OK)
                        {

                            ret = Program.setup("30", "17", "6", "10", "0", "1", "0");                           //Setup the media size and sensor type info
                            ret = Program.clearbuffer();                                                           //Clear image buffer                                                                                                         //    ret = TSCLIB_DLL.barcode("0", "0", "128", "10", "1", "0", "2", "2", "Barcode Test"); //Drawing barcode
                            ret = Program.printerfont("0", "24", "TSS24.BF2", "0", "1", "1", print1);        //Drawing printer font
                            ret = Program.printerfont("0", "56", "TSS24.BF2", "0", "1", "1", print2);        //Drawing printer font
                            ret = Program.printerfont("0", "88", "TSS24.BF2", "0", "1", "1", print3);        //Drawing printer font
                            ret = Program.printlabel("1", "1");                                                    //Print labels
                            ret = Program.closeport();
                        }
                        else
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("打码机未连接或出现异常情况，无法正常打印标签!");
                        MajorLog.Info("打码机未连接或出现异常情况，无法正常打印标签！" + ex.ToString());
                    }

                }
            }


        }
    }
}
