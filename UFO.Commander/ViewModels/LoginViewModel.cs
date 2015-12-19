﻿namespace UFO.Commander.ViewModels
{
    using System.Windows;
    using System.Windows.Input;
    using UFO.Commander.Views;
    using UFO.Server;
    using UFO.Server.Implementation;

    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            LoginCommand = new LambdaCommand(Login);
        }

        #region Properties

        public ICommand LoginCommand { get; }
        public string Password { get; set; }
        public string Username { get; set; }

        #endregion

        public void Login()
        {
            if (Server.UserServer.CheckLoginData(Username, Password))
            {
                new MainView().Show();
                RaiseRequestClose();
            }
            else
            {
                MessageBox.Show("Invalid username or password");
            }
        }
    }
}