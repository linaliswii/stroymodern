using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stroymogern
{
    public partial class MainForm : Form
    {
        private string userRole;
        private Form1 previousForm;
        private string _role;
        private Form1 _form1;
        private DataTable originalDataTable;
        private string connectionString = "Host=localhost;Username=postgres;Password=1234;Database=stroymodern";

        public MainForm(string role, Form1 form1)
        {
            InitializeComponent();
            _role = role;
            _form1 = form1;
        }

        public void UpdateUsernameLabel(string username)
        {
            Label lblUsername = Controls.OfType<Label>().FirstOrDefault();
            if (lblUsername != null)
            {
                lblUsername.Text = "Логин: " + username;
            }
        }

        private List<string> GetUniqueTypesFromDatabase()
        {
            List<string> types = new List<string>();
            string selectTypesQuery = "SELECT DISTINCT тип FROM tovar";


            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand(selectTypesQuery, connection))
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        types.Add(reader["тип"].ToString());
                    }
                }
            }
            return types;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            var types = GetUniqueTypesFromDatabase();
            comboBox2.Items.AddRange(types.ToArray());
            string connectionString = "Host=localhost;Username=postgres;Password=1234;Database=stroymodern";
            dataGridView1.CellClick += dataGridView1_CellClick;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT id, фото, наименование, цена, артикул, количество, тип FROM tovar";

                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(selectQuery, connection))
                    {
                        originalDataTable = new DataTable(); // Сохраняем исходные данные
                        dataAdapter.Fill(originalDataTable);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        foreach (DataRow row in dataTable.Rows)
                        {
                            byte[] imageData = (byte[])row["фото"];
                            Image originalImage = ByteArrayToImage(imageData);
                            int desiredWidth = 100; // Замените на ваше желаемое значение ширины
                            int desiredHeight = 100; // Замените на ваше желаемое значение высоты
                            Image resizedImage = ResizeImage(originalImage, desiredWidth, desiredHeight);

                            row["фото"] = ImageToByteArray(resizedImage);
                        }
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            dataGridView1.RowTemplate.Height = 100;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = dataGridView1.RowTemplate.Height;
            }
        }

        private Image ResizeImage(Image image, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(image, 0, 0, width, height);
            }
            return result;
        }

        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();

            if (originalDataTable != null)
            {
                DataView dv = originalDataTable.DefaultView;
                dv.RowFilter = $"наименование LIKE '%{searchText}%'"; // Настройте фильтр под ваши требования
                dataGridView1.DataSource = dv.ToTable();
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string selectedSortOption = comboBox1.SelectedItem.ToString();

                switch (selectedSortOption)
                {
                    case "Наименование (по возрастанию)":
                        dataGridView1.Sort(dataGridView1.Columns["наименование"], ListSortDirection.Ascending);
                        break;

                    case "Категория (по возрастанию)":
                        dataGridView1.Sort(dataGridView1.Columns["артикул"], ListSortDirection.Ascending);
                        break;

                    case "Цена (по возрастанию)":
                        dataGridView1.Sort(dataGridView1.Columns["цена"], ListSortDirection.Ascending);
                        break;

                    case "Наименование (по убыванию)":
                        dataGridView1.Sort(dataGridView1.Columns["наименование"], ListSortDirection.Descending);
                        break;

                    case "Категория (по убыванию)":
                        dataGridView1.Sort(dataGridView1.Columns["артикул"], ListSortDirection.Descending);
                        break;

                    case "Цена (по убыванию)":
                        dataGridView1.Sort(dataGridView1.Columns["цена"], ListSortDirection.Descending);
                        break;
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (comboBox2.SelectedItem != null)
            {
                string selectedType = comboBox2.SelectedItem.ToString();
                (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"тип = '{selectedType}'";
            }
        }
    }
}

