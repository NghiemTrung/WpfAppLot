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

        #region global method
        public static void AddLetter()
        {
            Univers = new List<NumberDraw>();
            for (int i = 0; i < UniverOfMain; i++)
            {
                Univers.Add(new NumberDraw((byte)(i + 1)));
            }
        }

        //count the occurrence during last numbers of game
        public static System.Data.DataTable LastXgame(byte _Xgame, List<DrawNumber> _Result)
        {
            System.Data.DataTable Insert2grid = new System.Data.DataTable();
            #region add column to the output table
            foreach (var Numlet in Univers)
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
            for (int i = 0; i <= Univers[0].LastXgame.Count(); i++)
            {
                System.Data.DataRow row = Insert2grid.NewRow();
                foreach (var NumLet in Univers)
                {
                    if (i < Univers[0].LastXgame.Count)
                    {
                        row[NumLet.NumberLetter - 1] = NumLet.LastXgame[i].ToString();
                        for (int t = 1; t <= _Result[0].ListMainNum.Length; t++)
                        {
                            try
                            {
                                if ((i + _Xgame) < _Result.Count) row["Number" + t.ToString()] = Univers[_Result[i + _Xgame].ListMainNum[t - 1] - 1].LastXtrend[i];
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

        //check if the draw number is in the high range or not
        private static bool CheckRange(DrawNumber _NeedCheck)
        {
            return ((_NeedCheck.MainSum >= 96 && _NeedCheck.MainSum <= 159) && (_NeedCheck.SideSum >= 5 && _NeedCheck.SideSum <= 15));
        }

        private static bool CheckRange(int[] _Main, int[] _Side)
        {
            return ((_Main.Sum() >= 96 && _Main.Sum() <= 159) && (_Side.Sum() >= 5 && _Side.Sum() <= 15));
        }

        //This method is for better random number
        private static int NextInt(int _Min, int _Max)
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

        // Ramdon numbet base on sample and quantity
        private static int[] RandomNums(List<int> _numbers, int _NumberOfPick)
        {
            int[] PickedNums = new int[_NumberOfPick];
            int _index;
            for (int i = 0; i < PickedNums.Length; i++)
            {
                _index = NextInt(0, _numbers.Count);
                PickedNums[i] = _numbers[_index];
                _numbers.RemoveAt(_index);
            }

            return PickedNums;
        }

        // Pick a Set and return a Pick
        private static DrawNumber PickaSet(List<int> _Mainnumbers, List<int> _Sidenumbers, DateTime _DrawDate)
        {
            DrawNumber _output;
            int[] _MainRandom;
            int[] _SideRandom;
            _MainRandom = RandomNums(_Mainnumbers, NumberOfMain);
            _SideRandom = RandomNums(_Sidenumbers, NumberOfSide);
            _output = new DrawNumber(_DrawDate,
                (byte)_MainRandom[0],
                (byte)_MainRandom[1],
                (byte)_MainRandom[2],
                (byte)_MainRandom[3],
                (byte)_MainRandom[4],
                (byte)_SideRandom[0],
                (byte)_SideRandom[1]
                );
            _output.SortNum();
            return _output;
        }

        //randomly pick 10 with number of picks in range
        private static List<WpfAppLot.Model.DrawNumber> TenPicks(DateTime _DrawDate, int _InRange)
        {
            int[] Main = new int[UniverOfMain];
            int[] Side = new int[UniverOfSide];
            for (int i = 0; i < UniverOfMain; i++) Main[i] = i + 1;
            for (int i = 0; i < UniverOfSide; i++) Side[i] = i + 1;
            return PickNumBaseOnSamples(Main, Side, 20, _InRange, _DrawDate);
        }

        //Random pick Numbers base on input sample and range criteria
        private static List<WpfAppLot.Model.DrawNumber> PickNumBaseOnSamples(int[] _SampleMain, int[] _SampleSide, int _NumberOfPicks, int _Inrange, DateTime _DrawDate)
        {
            List<DrawNumber> _output = new List<DrawNumber>();
            DrawNumber Pick;
            int count_Inrange = 0;
            List<int> _numbers;
            List<int> _side;
            do
            {
                _output.Clear();
                _numbers = _SampleMain.ToList();
                _side = new List<int>(_SampleSide);
                count_Inrange = 0;
                do
                {
                    if (_side.Count < NumberOfSide) { _side = new List<int>(_SampleSide); }
                    if (_numbers.Count < NumberOfMain) { _numbers = _SampleMain.ToList(); }
                    Pick = PickaSet(_numbers, _side, _DrawDate);
                    count_Inrange = (CheckRange(Pick)) ? count_Inrange + 1 : count_Inrange;
                    _output.Add(Pick);
                } while (_output.Count < _NumberOfPicks);

            } while (count_Inrange < _Inrange);

            return _output;
        }

        //Return the random picks for datagrid
        public static System.Data.DataTable RandomPick(List<DrawNumber> _Result)
        {
            System.Data.DataTable Insert2grid = new System.Data.DataTable();

            Insert2grid.Columns.Add("Date");
            Insert2grid.Columns.Add("Wins");
            for (int i = 0; i < 20; i++)
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


