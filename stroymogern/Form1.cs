using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;
using stroymogern;
using static stroymogern.AuthenticationManager;

namespace stroymogern
{
    public partial class Form1 : Form
    {
        private const int maxLoginAttempts = 3;
        private const int lockoutDurationSeconds = 30;
        private int loginAttempts = 0;
        private string connectionString = "Host=localhost;Port=5432;Database=stroymodern;Username=postgres;Password=1234;";
        private capcha capcha;
        private int lockoutSecondsRemaining = 0;
        private User currentUser;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Image.FromFile("C:\\Users\\liswi\\OneDrive\\Рабочий стол\\Выходной ДЭ\\logo.jpg");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            AuthenticationManager authManager = new AuthenticationManager(connectionString);
            currentUser = authManager.AuthenticateUser(username, password);

            if (currentUser != null)
            {
                // Открываем основную форму с учетом роли пользователя
                ShowMainFormForRole(currentUser.Role, currentUser);
            }
            else
            {
                loginAttempts++;

                if (loginAttempts >= maxLoginAttempts)
                {
                    LockoutUser();
                }

                MessageBox.Show("Неверные учетные данные. Попробуйте снова.");
            }
        }

        private void ShowMainFormForRole(string role, User user)
        {
            MainForm mainForm = new MainForm(role, this);
            mainForm.UpdateUsernameLabel(user.Username);
            mainForm.Show();
            Hide();
        }


        private void LockoutUser()
        {
            lockoutSecondsRemaining = lockoutDurationSeconds;
            MessageBox.Show($"Вы ввели неверные данные {maxLoginAttempts} раза. Учетная запись заблокирована на {lockoutDurationSeconds} секунд.");

            // Запуск таймера для отсчета времени блокировки
            Timer lockoutTimer = new Timer();
            lockoutTimer.Interval = 1000; // таймер срабатывает каждую секунду
            lockoutTimer.Tick += (sender, args) =>
            {
                lockoutSecondsRemaining--;

                if (lockoutSecondsRemaining <= 0)
                {
                    loginAttempts = 0; // Сбрасываем счетчик попыток после снятия блокировки
                    lockoutTimer.Stop();
                }
            };
            lockoutTimer.Start();

            // Показываем капчу
            using (capcha captchaForm = new capcha())
            {
                if (captchaForm.ShowDialog() == DialogResult.OK)
                {
                    // В этом месте можно добавить дополнительную логику для проверки капчи
                    captchaForm.ValidateCaptcha(captchaForm.EnteredCaptcha);
                }
            }
        }
    }
}
