using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MitraSehatHospital
{
    public partial class Form2 : Form
    {
        private Form1 form1;
        private MySqlConnection con;
        public Form2()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                Port = 3307,
                UserID = "root",
                Password = "root",
                Database = "hospital"
            };

            con = new MySqlConnection(builder.ConnectionString);

            con.Open();

            using (var dataCommand = new MySqlCommand("SELECT * FROM pasien_periksa", con))
            using (var dataReader = dataCommand.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);
                dataGridView1.DataSource = dataTable;
            }

            con.Close();
        }

        private void search_data_pasien(String keyword)
        {
            con.Open();
            MySqlCommand dataCommand = new MySqlCommand("SELECT * FROM pasien_periksa WHERE `Nama Lengkap` LIKE @keyword", this.con);
            dataCommand.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
            MySqlDataReader dataReader = dataCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataGridView1.DataSource = dataTable;
            con.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            search_data_pasien(textBox6.Text);
        }
    }
}
