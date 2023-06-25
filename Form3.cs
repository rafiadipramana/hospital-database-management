using MySqlConnector;
using System;
using System.Data;
using System.Linq.Expressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MitraSehatHospital
{
    public partial class Form3 : Form
    {
        private Form1 form1;
        private MySqlConnection con;

        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
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

            using (var command = new MySqlCommand("SELECT * FROM doctor_type;", con))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    comboBox3.Items.Add(String.Concat(reader["id"].ToString(), ". ", reader["type"]));
                }
            }

            using (var dataCommand = new MySqlCommand("SELECT * FROM daftar_dokter", con))
            using (var dataReader = dataCommand.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);
                dataGridView1.DataSource = dataTable;
            }

            con.Close();
        }

        private void search_data_dokter(String keyword)
        {
            con.Open();
            MySqlCommand dataCommand = new MySqlCommand("SELECT * FROM daftar_dokter WHERE `Nama Lengkap` LIKE @keyword", this.con);
            dataCommand.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
            MySqlDataReader dataReader = dataCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataGridView1.DataSource = dataTable;
            con.Close();
        }

        private void load_data_dokter()
        {
            con.Open();
            MySqlCommand dataCommand = new MySqlCommand("SELECT * FROM daftar_dokter", this.con);
            MySqlDataReader dataReader = dataCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataGridView1.DataSource = dataTable;
            con.Close();
        }

        private void clear_form()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox4.Text = "";
            textBox6.Text = "";
            dateTimePicker1.ResetText();
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            comboBox3.SelectedItem = null;
        }

        private void load_single_dokter(int doctor_id)
        {
            con.Open();
            MySqlCommand dataCommand = new MySqlCommand("SELECT * FROM doctors WHERE id = @doctor_id", con);
            dataCommand.Parameters.AddWithValue("@doctor_id", doctor_id);
            MySqlDataReader dataReader = dataCommand.ExecuteReader();

            if (dataReader.HasRows == true)
            {
                dataReader.Read();
                textBox1.Text = dataReader.GetString(dataReader.GetOrdinal("fullname"));
                textBox2.Text = dataReader.GetString(dataReader.GetOrdinal("nik"));
                var fulldate = dataReader.GetDateOnly("birth_date").ToString("yyyy-MM-dd").Split("-");

                dateTimePicker1.Value = new DateTime(Int32.Parse(fulldate[0]), Int32.Parse(fulldate[1]), Int32.Parse(fulldate[2]));
                textBox4.Text = dataReader.GetString(dataReader.GetOrdinal("birth_place"));
                if (dataReader.GetString(dataReader.GetOrdinal("sex")) == "M")
                {
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                }
                else
                {
                    radioButton1.Checked = false;
                    radioButton2.Checked = true;
                }

                var comboIndex = comboBox3.FindString(String.Concat(dataReader.GetInt16(dataReader.GetOrdinal("type_id")).ToString(), "."));
            }
            else
            {
                MessageBox.Show("Data Tidak Ditemukan", "Perhatian");
            }
            con.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            var rowIndex = dataGridView1.CurrentCell.RowIndex;
            var doctor_id = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();
            textBox6.Text = doctor_id;
            load_single_dokter(Int16.Parse(doctor_id));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            search_data_dokter(textBox5.Text);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var nama_lengkap = textBox1.Text;
            var nik = textBox2.Text;
            var tgl_lahir = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            var tmp_lahir = textBox4.Text;

            var gender = "";
            bool isChecked = radioButton1.Checked;
            if (isChecked)
            {
                gender = "M";
            }
            else
            {
                gender = "F";
            }

            string spesialisasi = comboBox3.Text;
            string[] spesialisasi_components = spesialisasi.Split(' ');

            con.Open();
            MySqlCommand dataCommand;
            if (textBox6.Text == "")
            {
                dataCommand = new MySqlCommand("INSERT INTO doctors (fullname, nik, birth_date, birth_place, sex, type_id) VALUE (@fullname, @nik, @birth_date, @birth_place, @sex, @type_id", con);
            }
            else
            {
                dataCommand = new MySqlCommand("UPDATE doctors SET fullname = @fullname, nik = @nik, birth_date = @birth_date, birth_place = @birth_place, sex = @sex, type_id = @type_id WHERE id = @doctor_id", con);
                dataCommand.Parameters.AddWithValue("@doctor_id", textBox6.Text);
            }
            dataCommand.Parameters.AddWithValue("@fullname", nama_lengkap);
            dataCommand.Parameters.AddWithValue("@nik", nik);
            dataCommand.Parameters.AddWithValue("@birth_date", tgl_lahir);
            dataCommand.Parameters.AddWithValue("@birth_place", tmp_lahir);
            dataCommand.Parameters.AddWithValue("@sex", gender);
            dataCommand.Parameters.AddWithValue("@type_id", spesialisasi_components[0]);
            int affected_rows = dataCommand.ExecuteNonQuery();
            if (affected_rows > 0)
            {
                MessageBox.Show("Berhasil menyimpan data", "Informasi");
            }
            con.Close();
            clear_form();
            load_data_dokter();
        }
    }
}
