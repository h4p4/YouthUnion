namespace YouthUnion
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    internal class LogViewModel : ViewModel
    {
        private const string MESSAGE = "Неверный логин или пароль\nПопробовать снова?";
        private string _login;

        public LogViewModel()
        {
            LogCommand = new RelayCommand(Execute);
        }

        public ICommand LogCommand { get; set; }

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        private bool CheckLogin()
        {
            var user = ContextProvider.YouthUnionContext.Users.FirstOrDefault(x => x.Username == Login);
            if (user != null)
                return true;
            TryAgain();
            return false;

        }

        private bool CheckPassword(string password)
        {
            var foundUser = ContextProvider.YouthUnionContext.Users
                .FirstOrDefault(x => x.Username == Login &&
                                     x.UserPassword == password);
            if (foundUser != null)
                return true;
            TryAgain();
            return false;
        }


        private void Execute(object obj)
        {
            if (obj is not PasswordBox passwordBox)
                return;
            var password = passwordBox.Password;
            if (!CheckLogin())
                return;
            if (!CheckPassword(password))
                return;
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Exit();
        }

        private void TryAgain()
        {
            var result = MessageBox.Show(MESSAGE, "Ошибка", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
                Exit();
        }

        private void Exit()
        {
            Application.Current.Windows.OfType<LogWindow>().First().Close();
        }
    }
}