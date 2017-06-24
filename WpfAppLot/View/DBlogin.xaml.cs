﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WpfAppLot.Database;

namespace WpfAppLot.View
{
    /// <summary>
    /// Interaction logic for DBlogin.xaml
    /// </summary>
    public partial class DBlogin : Window
    {
        public DBlogin()
        {
            InitializeComponent();
        }

        private void connectBTNclicked(object sender, RoutedEventArgs e)
        {
            SQLservice conn = new Database.SQLservice(txtBxServer.Text, txtBxDatabase.Text, txtBxUsername.Text, txtBxPassword.Password);
            try
            {
                conn.OpenConnection();
                MessageBox.Show("Connection success !!" + conn.ConnectionString, "Success!");
                Properties.Settings.Default.Server = txtBxServer.Text;
                Properties.Settings.Default.DatabaseName = txtBxDatabase.Text;
                Properties.Settings.Default.Username = txtBxUsername.Text;
                Properties.Settings.Default.Password = txtBxPassword.Password;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
