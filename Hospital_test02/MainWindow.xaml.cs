using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Hospital_test02
{
    public partial class MainWindow : Window
    {
        private bool _isSidebarOpen = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SidebarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isSidebarOpen)
            {
                // Закрываем боковую панель
                Storyboard closeAnimation = (Storyboard)FindResource("SidebarCloseAnimation");
                closeAnimation.Begin(Sidebar);

                // Поворачиваем стрелку
                var path = (Path)((Button)sender).Content;
                var transform = (RotateTransform)path.RenderTransform;
                transform.Angle = 180;
            }
            else
            {
                // Открываем боковую панель
                Storyboard openAnimation = (Storyboard)FindResource("SidebarOpenAnimation");
                openAnimation.Begin(Sidebar);

                // Возвращаем стрелку в исходное положение
                var path = (Path)((Button)sender).Content;
                var transform = (RotateTransform)path.RenderTransform;
                transform.Angle = 0;
            }

            _isSidebarOpen = !_isSidebarOpen;
        }

        private void PatientsList_Click(object sender, RoutedEventArgs e)
        {
            TitleText.Text = "Список пациентов";
            MainFrame.Navigate(new PatientsListPage());
        }

        private void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            TitleText.Text = "Добавление пациента";
            var editWindow = new PatientEditWindow();
            editWindow.Owner = this;
            editWindow.ShowDialog();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            TitleText.Text = "О программе";
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            TitleText.Text = "Настройки подключения";
            MainFrame.Navigate(new SettingsPage());
        }
    }
}