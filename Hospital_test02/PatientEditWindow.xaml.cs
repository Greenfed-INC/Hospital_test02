using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;

namespace Hospital_test02
{
    public partial class PatientEditWindow : Window
    {
        private string connectionString;
        private string patientId;
        private bool isEditMode;

        public PatientEditWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["HospitalConnection"].ConnectionString;
            isEditMode = false;
            Title = "Добавление нового пациента";
        }

        public PatientEditWindow(string id) : this()
        {
            patientId = id;
            isEditMode = true;
            Title = "Редактирование пациента";
            LoadPatientData();
        }

        private void LoadPatientData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT p.FIO_patient, p.date_birth, p.Number_OMS, p.Number_Snils, p.Number_Phone,
                                   ps.passport_series, ps.passport_number
                                   FROM patient p
                                   JOIN passport ps ON p.pass_id = ps.passport_id
                                   WHERE p.id = @id";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", patientId);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        PassportSeries.Text = reader["passport_series"].ToString();
                        PassportNumber.Text = reader["passport_number"].ToString();
                        FIO.Text = reader["FIO_patient"].ToString();
                        BirthDate.SelectedDate = Convert.ToDateTime(reader["date_birth"]);
                        OMS.Text = reader["Number_OMS"].ToString();
                        Snils.Text = reader["Number_Snils"].ToString();
                        Phone.Text = reader["Number_Phone"].ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        int passportId;

                        if (isEditMode)
                        {
                            SqlCommand getPassportCmd = new SqlCommand("SELECT pass_id FROM patient WHERE id = @id", connection, transaction);
                            getPassportCmd.Parameters.AddWithValue("@id", patientId);
                            passportId = Convert.ToInt32(getPassportCmd.ExecuteScalar());
                            SqlCommand updatePassportCmd = new SqlCommand(
                                "UPDATE passport SET passport_series = @series, passport_number = @number WHERE passport_id = @id",
                                connection, transaction);
                            updatePassportCmd.Parameters.AddWithValue("@series", PassportSeries.Text);
                            updatePassportCmd.Parameters.AddWithValue("@number", PassportNumber.Text);
                            updatePassportCmd.Parameters.AddWithValue("@id", passportId);
                            updatePassportCmd.ExecuteNonQuery();
                            SqlCommand updatePatientCmd = new SqlCommand(
                                @"UPDATE patient SET 
                                    FIO_patient = @fio, 
                                    date_birth = @birth, 
                                    Number_OMS = @oms, 
                                    Number_Snils = @snils, 
                                    Number_Phone = @phone 
                                WHERE id = @id",
                                connection, transaction);
                            updatePatientCmd.Parameters.AddWithValue("@fio", FIO.Text);
                            updatePatientCmd.Parameters.AddWithValue("@birth", BirthDate.SelectedDate);
                            updatePatientCmd.Parameters.AddWithValue("@oms", OMS.Text);
                            updatePatientCmd.Parameters.AddWithValue("@snils", Snils.Text);
                            updatePatientCmd.Parameters.AddWithValue("@phone", Phone.Text);
                            updatePatientCmd.Parameters.AddWithValue("@id", patientId);
                            updatePatientCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand insertPassportCmd = new SqlCommand(
                                "INSERT INTO passport (passport_series, passport_number) OUTPUT INSERTED.passport_id VALUES (@series, @number)",
                                connection, transaction);
                            insertPassportCmd.Parameters.AddWithValue("@series", PassportSeries.Text);
                            insertPassportCmd.Parameters.AddWithValue("@number", PassportNumber.Text);
                            passportId = Convert.ToInt32(insertPassportCmd.ExecuteScalar());

                            SqlCommand insertPatientCmd = new SqlCommand(
                                @"INSERT INTO patient 
                                    (pass_id, FIO_patient, date_birth, Number_OMS, Number_Snils, Number_Phone) 
                                VALUES 
                                    (@pass_id, @fio, @birth, @oms, @snils, @phone)",
                                connection, transaction);
                            insertPatientCmd.Parameters.AddWithValue("@pass_id", passportId);
                            insertPatientCmd.Parameters.AddWithValue("@fio", FIO.Text);
                            insertPatientCmd.Parameters.AddWithValue("@birth", BirthDate.SelectedDate);
                            insertPatientCmd.Parameters.AddWithValue("@oms", OMS.Text);
                            insertPatientCmd.Parameters.AddWithValue("@snils", Snils.Text);
                            insertPatientCmd.Parameters.AddWithValue("@phone", Phone.Text);
                            insertPatientCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(PassportSeries.Text) || !Regex.IsMatch(PassportSeries.Text, @"^\d{4}$"))
            {
                MessageBox.Show("Серия паспорта должна содержать 4 цифры", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(PassportNumber.Text) || !Regex.IsMatch(PassportNumber.Text, @"^\d{6}$"))
            {
                MessageBox.Show("Номер паспорта должен содержать 6 цифр", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(FIO.Text))
            {
                MessageBox.Show("Введите ФИО пациента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (BirthDate.SelectedDate == null || BirthDate.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Укажите корректную дату рождения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(OMS.Text, @"^\d{4}-\d{12}$"))
            {
                MessageBox.Show("Номер ОМС должен быть в формате XXXX-XXXXXXXXXXXX", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(Snils.Text, @"^\d{3}-\d{3}-\d{3} \d{2}$"))
            {
                MessageBox.Show("СНИЛС должен быть в формате XXX-XXX-XXX XX", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(Phone.Text, @"^\+7\d{10}$"))
            {
                MessageBox.Show("Телефон должен быть в формате +7XXXXXXXXXX", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}