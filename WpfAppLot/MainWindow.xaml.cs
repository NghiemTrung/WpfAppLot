using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.OleDb;
//using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
using WpfAppLot.Database;
using WpfAppLot.Model;
using WpfAppLot.View;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

namespace WpfAppLot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GlobalVar.AddLetter();
            GlobalVar._DataService = new Database.SQLservice("Data Source=MAYTINH-KE1TVDA;Initial Catalog=LottoDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            GlobalVar._DataService.OpenConnection();

            Values1 = new ChartValues<double> { 3, 4, 6, 3, 2, 6 };
            Values2 = new ChartValues<double> { 5, 3, 5, 7, 3, 9 };
            DataContext = this;
        }
        public ChartValues<double> Values1 { get; set; }
        public ChartValues<double> Values2 { get; set; }
        private void Menu_DatabaseConnection_Clicked(object sender, RoutedEventArgs e)
        {
            DBlogin DatabaseLogin = new DBlogin();
            DatabaseLogin.Show();
        }

        private void CbBoxDrawDate_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void UpdateNumber(object sender, RoutedEventArgs e)
        {
            GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].Num1 = Convert.ToByte(Number1.Text);
            GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].Num2 = Convert.ToByte(Number2.Text);
            GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].Num3 = Convert.ToByte(Number3.Text);
            GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].Num4 = Convert.ToByte(Number4.Text);
            GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].Num5 = Convert.ToByte(Number5.Text);
            GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].EUNum1 = Convert.ToByte(Number01.Text);
            GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].EUNum2 = Convert.ToByte(Number02.Text);
            DrawDateContext.UpdateDrawNumber(GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex], GlobalVar._DataService);
        }

        private void CbBoxDrawDate_selectChange(object sender, SelectionChangedEventArgs e)
        {
            Number1.Text = GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].Num1.ToString();
            Number2.Text = GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].Num2.ToString();
            Number3.Text = GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].Num3.ToString();
            Number4.Text = GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].Num4.ToString();
            Number5.Text = GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].Num5.ToString();
            Number01.Text = GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].EUNum1.ToString();
            Number02.Text = GlobalVar.DrawResult[CbBoxDrawDate.SelectedIndex].EUNum2.ToString();
        }

        private void FormLoad(object sender, RoutedEventArgs e)
        {
            GlobalVar.DrawResult.Clear();
            foreach (var DrawResult in DrawDateContext.GetALLDrawNumbers(GlobalVar._DataService))
            {
                GlobalVar.DrawResult.Add(DrawResult);
            }
            CbBoxDrawDate.ItemsSource = GlobalVar.DrawResult;
            CbBoxDrawDate.DisplayMemberPath = "DrawDate";
            CbBoxDrawDate.SelectedValuePath = "ID";
            CbBoxDrawDate.SelectedIndex = GlobalVar.DrawResult.Count() - 1;
            UpdateLetters();
            //DataGridSkipped();
            StatisComboBox.ItemsSource = GlobalVar.ComboBoxCollection;
            StatisComboBox.SelectedIndex = 0;
            DataGridSkipped();
            #region init DB
            //CreateDatabase.InitDatabase();
            
            #endregion
        }
        private void DataGridSkipped()
        {
            DataTable insert2grid = new DataTable();
            foreach(var Numlet in GlobalVar.Univers) {
                insert2grid.Columns.Add(Numlet.NumberLetter.ToString());
            }
            for(int i = 0; i < GlobalVar.Univers[0].Skipped.Count(); i++)
            {
                DataRow Add2Table = insert2grid.NewRow();
                foreach(var NumLet in GlobalVar.Univers)
                {
                    Add2Table[NumLet.NumberLetter.ToString()] = NumLet.Skipped[i].ToString();
                }
                insert2grid.Rows.Add(Add2Table);
            }
            MainGrid.ItemsSource = insert2grid.DefaultView;
        }
        private void SelectAllTextBox(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                Keyboard.Focus(textBox);
                textBox.SelectAll();
            }
        }

        private void Edit_StartDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateDatabase.InitDatabase();
                MessageBox.Show("Success");
            }
            catch
            {
                MessageBox.Show("Fail");
            }
        }

        private void UpdateLetters()
        {
            foreach(DrawNumber RS in GlobalVar.DrawResult)
            {
                if (RS.ID != 1) {
                    foreach (NumberDraw num in GlobalVar.Univers)
                    {
                        num.UpdateSkip(RS);
                    }
                } else
                {
                    foreach(byte b in RS.ListMainNum)
                    {
                        GlobalVar.Univers[b - 1] = new NumberDraw(b);
                        GlobalVar.Univers[b - 1].AddSkip(0);
                    }
                }
            }
        }
        private void GameSkipped()
        {

        }
        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            FormLoad(new object(), e);
        }

        private void StatisCbBoxSelectChange(object sender, SelectionChangedEventArgs e)
        {
            switch (StatisComboBox.SelectedIndex)
            {
                case 0: DataGridSkipped(); break;
                case 1: MainGrid.ItemsSource = GlobalVar.LastXgame(10,GlobalVar.DrawResult).DefaultView; break;
                case 2: MainGrid.ItemsSource = GlobalVar.RandomPick(GlobalVar.DrawResult).DefaultView;break;
            }
        }
    }
}
