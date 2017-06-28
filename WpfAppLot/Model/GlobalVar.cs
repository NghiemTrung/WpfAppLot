using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppLot.Database;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace WpfAppLot.Model
{
    class GlobalVar
    {
        #region global variable
        public static DB_Service _DataService = new Database.SQLservice("Data Source=MAYTINH-KE1TVDA;Initial Catalog=LottoDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        public static List<DrawNumber> DrawResult = new List<DrawNumber>();
        public static List<NumberDraw> Univers;
        public static int NumberOfMain = 5;
        public static int NumberOfSide = 2;
        public static int UniverOfMain = 50;
        public static int UniverOfSide = 10;

        /*Please add your connection string to this variable, must be SQLServer connection string
        public static string ConnectionStringSQL = "Data Source=MAYTINH-KE1TVDA;Initial Catalog=LottoDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";*/
        public static string ConnectionStringSQL;
        public static string[] ComboBoxCollection
        {
            get
            {
                return new string[]
                {
                    "Game Skipped",
                    "Last 10 Game",
                    "RandomNumber"
                };
            }
        }
        #endregion

    }
    
    static class GlobalMethod
    {
        #region global method
        public static void AddLetter()
        {
            GlobalVar.Univers = new List<NumberDraw>();
            for (int i = 0; i < GlobalVar.UniverOfMain; i++)
            {
                GlobalVar.Univers.Add(new NumberDraw((byte)(i + 1)));
            }
        }

        //count the occurrence during last numbers of game
        public static System.Data.DataTable LastXgame(byte _Xgame, List<DrawNumber> _Result)
        {
            System.Data.DataTable Insert2grid = new System.Data.DataTable();
            #region add column to the output table
            foreach (var Numlet in GlobalVar.Univers)
            {
                Insert2grid.Columns.Add(Numlet.NumberLetter.ToString());
                Numlet.LastX = _Xgame;
                Numlet.UpdateLastXGame(_Result);
            }
            for (int i = 0; i < _Result[0].ListMainNum.Length; i++)
            {
                Insert2grid.Columns.Add("Number" + (i + 1).ToString());
            }
            #endregion
            #region add row to the outpt table
            for (int i = 0; i <= GlobalVar.Univers[0].LastXgame.Count(); i++)
            {
                System.Data.DataRow row = Insert2grid.NewRow();
                foreach (var NumLet in GlobalVar.Univers)
                {
                    if (i < GlobalVar.Univers[0].LastXgame.Count)
                    {
                        row[NumLet.NumberLetter - 1] = NumLet.LastXgame[i].ToString();
                        for (int t = 1; t <= _Result[0].ListMainNum.Length; t++)
                        {
                            try
                            {
                                if ((i + _Xgame) < _Result.Count) row["Number" + t.ToString()] = GlobalVar.Univers[_Result[i + _Xgame].ListMainNum[t - 1] - 1].LastXtrend[i];
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }
                    else
                    {
                        row[NumLet.NumberLetter - 1] = NumLet.LastXtrend[i - 1];
                    }
                }
                Insert2grid.Rows.Add(row);
            }
            #endregion
            return Insert2grid;
        }

        //This method is for better random number
        public static int NextInt(int _Min, int _Max)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[4];

                rng.GetBytes(buffer);
                int result = BitConverter.ToInt32(buffer, 0);
                int _output = new Random(result).Next(_Min, _Max);
                return _output;
            }
        }
        
        //randomly pick 10 with number of picks in range
        private static List<WpfAppLot.Model.DrawNumber> TenPicks(DateTime _DrawDate, int _InRange)
        {
            int[] Main = new int[GlobalVar.UniverOfMain];
            int[] Side = new int[GlobalVar.UniverOfSide];
            for (int i = 0; i < GlobalVar.UniverOfMain; i++) Main[i] = i + 1;
            for (int i = 0; i < GlobalVar.UniverOfSide; i++) Side[i] = i + 1;
            return DrawDateContext.PickNumBaseOnSamples(Main, Side, 10, _InRange, _DrawDate);
        }

        //Return the random picks for datagrid
        public static System.Data.DataTable RandomPick(List<DrawNumber> _Result)
        {
            System.Data.DataTable Insert2grid = new System.Data.DataTable();

            Insert2grid.Columns.Add("Date");
            Insert2grid.Columns.Add("Wins");
            for (int i = 0; i < 10; i++)
            {
                Insert2grid.Columns.Add("Pick " + (i + 1).ToString());
            }
            foreach (var DateOfDraw in _Result)
            {
                List<DrawNumber> Picks = TenPicks(DateOfDraw.DrawDate, 5);
                System.Data.DataRow row = Insert2grid.NewRow();
                row["Date"] = DateOfDraw.DrawDate.ToShortDateString();
                int count = 0;
                for (int i = 0; i < Picks.Count; i++)
                {
                    row["Pick " + (i + 1).ToString()] = Picks[i].ToResultString();

                    row["Wins"] = (DateOfDraw.CheckResult(Picks[i])) ? row["Wins"] +
                        (DateOfDraw.ResultDetail(Picks[i])[0].ToString("00") + " - " +
                        DateOfDraw.ResultDetail(Picks[i])[1].ToString("00") + ", ") : row["Wins"];
                }
                Insert2grid.Rows.Add(row);
            }

            return Insert2grid;
        }
        #endregion
    }
}


