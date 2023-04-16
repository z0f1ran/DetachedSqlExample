using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace DetachedSqlExample
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataSet dataSet = null;
        private SqlDataAdapter dataAdapter = null; 
        public MainWindow()
        {
            InitializeComponent();
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(ConnectionString);
                SqlCommand cmd = new SqlCommand(requestTextBox.Text, con);
                DataTable table = new DataTable();
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int line = 0;
                do
                {
                    while (reader.Read())
                    {
                        if (line == 0)
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                table.Columns.Add(reader.GetName(i));
                            }
                            line++;
                        }
                        DataRow row = table.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = reader[i];
                        }
                        table.Rows.Add(row);
                    }
                } while (reader.NextResult());


                resultDataGrid.ItemsSource = table.AsDataView();
                con.Close();
                reader.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                if(con != null)
                {
                    con.Close();
                }
            }
        }


        private string ConnectionString {
            get
            {
                return ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            requestTextBox.Text = "select * from vegetables_t";
        }

        private void fillButton_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(ConnectionString);
                dataSet = new DataSet();
                dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = new SqlCommand("select * from Vegetables_t", con);
                SqlCommandBuilder cB = new SqlCommandBuilder(dataAdapter);
                dataAdapter.Fill(dataSet, "Vegetables_t");
                datasetDataGrid.ItemsSource = dataSet.Tables["Vegetables_t"].DefaultView;
            }
            catch
            {

            }
            finally
            {

                con?.Close();
            }
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
           if(dataSet != null)
           {
                dataAdapter.Update(dataSet, "Vegetables_t");
                dataSet.Clear();
                dataAdapter.Fill(dataSet, "Vegetables_t");
           }
        }
    }
}
