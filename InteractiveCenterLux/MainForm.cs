using System.Data;
using Npgsql;
using ChartForm;

namespace InteractiveCenterLux
{
    public partial class MainForm : Form
    {       

        public MainForm()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (NpgsqlConnection sqlConnection = new NpgsqlConnection("Host=localhost;" +
            "Port=5433;Database=LuxDatabase;Username=postgres;Password=super200;"))
                {
                    sqlConnection.Open();


                    NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter("SELECT * FROM product", sqlConnection);

                    NpgsqlCommandBuilder sqlBuilder = new NpgsqlCommandBuilder(dataAdapter);

                    sqlBuilder.GetInsertCommand();
                    sqlBuilder.GetUpdateCommand();
                    sqlBuilder.GetDeleteCommand();

                    DataSet dataSet = new DataSet();

                    dataAdapter.Fill(dataSet, "product");

                    dataGridView1.DataSource = dataSet.Tables["product"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChartForm.ChartForm chartForm = new ChartForm.ChartForm();

            chartForm.Show();
        }
    }
}