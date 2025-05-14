using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.Data.SqlClient;
using System;

namespace Hospital_test02
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["HospitalConnection"].ConnectionString;
                var builder = new SqlConnectionStringBuilder(connectionString);

                ServerTextBox.Text = builder.DataSource;
                DatabaseTextBox.Text = builder.InitialCatalog;

                // Загрузка дополнительных параметров
                TimeoutTextBox.Text = builder.ConnectTimeout.ToString();
                IntegratedSecurityCheckBox.IsChecked = builder.IntegratedSecurity;
            }
            catch
            {
                // Установка значений по умолчанию при ошибке
                ServerTextBox.Text = ".";
                DatabaseTextBox.Text = "Hospital";
                TimeoutTextBox.Text = "30";
                IntegratedSecurityCheckBox.IsChecked = true;
            }
        }

        private void TestConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = ServerTextBox.Text,
                    InitialCatalog = DatabaseTextBox.Text,
                    IntegratedSecurity = IntegratedSecurityCheckBox.IsChecked == true,
                    ConnectTimeout = int.TryParse(TimeoutTextBox.Text, out var timeout) ? timeout : 30
                };

                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    MessageBox.Show("Подключение успешно!", "Проверка подключения",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");

                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = ServerTextBox.Text,
                    InitialCatalog = DatabaseTextBox.Text,
                    IntegratedSecurity = IntegratedSecurityCheckBox.IsChecked == true,
                    ConnectTimeout = int.TryParse(TimeoutTextBox.Text, out var timeout) ? timeout : 30
                };

                connectionStringsSection.ConnectionStrings["HospitalConnection"].ConnectionString = builder.ToString();
                connectionStringsSection.ConnectionStrings["HospitalConnection"].ProviderName = "System.Data.SqlClient";

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");

                MessageBox.Show("Настройки сохранены!\nДля применения изменений перезапустите приложение.",
                              "Сохранение настроек",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}