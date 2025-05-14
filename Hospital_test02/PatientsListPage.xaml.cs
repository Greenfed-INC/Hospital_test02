using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Hospital_test02
{
    public partial class PatientsListPage : Page
    {
        private string connectionString;

        public PatientsListPage()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["HospitalConnection"].ConnectionString;
            LoadPatients();
        }

        private void LoadPatients()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT p.id, p.FIO_patient, p.date_birth, p.Number_Phone
                                    FROM patient p";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    PatientsGrid.ItemsSource = table.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new PatientEditWindow();
            editWindow.ShowDialog();
            LoadPatients();
        }

        private void EditPatient_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string patientId = button.Tag.ToString();

            var editWindow = new PatientEditWindow(patientId);
            editWindow.ShowDialog();
            LoadPatients();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadPatients();
        }
    }
}