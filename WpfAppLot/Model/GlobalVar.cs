using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppLot.Database;
using System.Security.Cryptography;

namespace WpfAppLot.Model
{
    class GlobalVar
    {
        public static DB_Service _DataService;
        public static List<DrawNumber> DrawResult = new List<DrawNumber>();
        public static List<NumberDraw> Univers = new List<NumberDraw>();
        public static string ConnectionStringSQL = "Data Source=MAYTINH-KE1TVDA;Initial Catalog=LottoDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
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
        public static void AddLetter()
        {
            for (int i = 0; i < 50; i++)
            {
                Univers.Add(new NumberDraw((byte)(i + 1)));
            }
        }
        public static System.Data.DataTable LastXgame (byte _Xgame,List<DrawNumber> _Result)
        {
            System.Data.DataTable Insert2grid = new System.Data.DataTable();
            foreach (var Numlet in Univers)
            {
                Insert2grid.Columns.Add(Numlet.NumberLetter.ToString());
                Numlet.LastX = _Xgame;
                Numlet.UpdateLastXGame(_Result);
            }
            for (int i = 0; i<_Result[0].ListMainNum.Length;i++)
            {
                Insert2grid.Columns.Add("Number" + (i+1).ToString());
            }
            for (int i =0; i<= Univers[0].LastXgame.Count(); i++)
            {
                System.Data.DataRow row = Insert2grid.NewRow();
                foreach (var NumLet in Univers)
                {
                    if (i< Univers[0].LastXgame.Count) {
                        row[NumLet.NumberLetter - 1] = NumLet.LastXgame[i].ToString();
                        for (int t = 1; t <= _Result[0].ListMainNum.Length;t++)
                        {
                            try
                            {
                                if ((i + _Xgame) < _Result.Count) row["Number" + t.ToString()] = Univers[_Result[i + _Xgame].ListMainNum[t - 1] - 1].LastXtrend[i];
                            } catch (Exception e)
                            {
                            }
                        } }
                    else {
                        row[NumLet.NumberLetter - 1] = NumLet.LastXtrend[i - 1];
                    }
                }
                Insert2grid.Rows.Add(row);
            }
            return Insert2grid;
        }
        
        //check if the draw number is in the high range or not
        private static bool CheckRange( DrawNumber _NeedCheck)
        {
            return ((_NeedCheck.MainSum>=96 && _NeedCheck.MainSum<=159) && (_NeedCheck.SideSum >= 5 && _NeedCheck.SideSum <= 15));
        }

        private static bool CheckRange(int[] _Main, int[] _Side)
        {
            return ((_Main.Sum() >=96 && _Main.Sum() <= 159) && (_Side.Sum() >= 5 && _Side.Sum() <= 15));
        }

        private static int NextInt (int _Min, int _Max)
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

        // Ramdom main numbers of a pick
        private static int[] RandomNums(List<int> _numbers, int _NumberOfPick)
        {
            int[] PickedNums = new int[_NumberOfPick];
            int _index;
            for (int i = 0; i < PickedNums.Length; i++)
            {
                _index = NextInt(0,_numbers.Count);
                PickedNums[i] = _numbers[_index];
                _numbers.RemoveAt(_index);
            }

            return PickedNums;
        }

        private static DrawNumber PickaSet(List<int> _Mainnumbers, List<int> _Sidenumbers, DateTime _DrawDate)
        {
            DrawNumber _output;
            int[] _MainRandom;
            int[] _SideRandom;
            _MainRandom = RandomNums(_Mainnumbers, 5);
            _SideRandom = RandomNums(_Sidenumbers, 2);
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

        //randomly pick 10 In range
        private static List<WpfAppLot.Model.DrawNumber> TenPicks(DateTime _DrawDate, int _InRange)
        {
            List<DrawNumber> _output = new List<DrawNumber>();
            List<int> Main = new List<int>();
            List<int> Side = new List<int>();
            int count;
            DrawNumber _Add;
            do
            {
                _output.Clear();
                count = 0; 
                for (int i = 0; i < 50; i++) { Main.Add(i + 1); }
                for (int i = 0; i < 10; i++) { Side.Add(i + 1); }
                do
                {
                    if (Side.Sum() == 0) { for (int i = 0; i < 10; i++) { Side.Add(i + 1); } }
                    _Add = PickaSet(Main, Side, _DrawDate);
                    _output.Add(_Add);
                    count = (CheckRange(_Add)) ? count + 1 : count;
                } while (Main.Count != 0);
            } while (count < _InRange );
            
            return _output;
        }
        

        //
        public static System.Data.DataTable RandomPick(List<DrawNumber> _Result)
        {
            System.Data.DataTable Insert2grid = new System.Data.DataTable();

            Insert2grid.Columns.Add("Date");
            Insert2grid.Columns.Add("Wins");
            for (int i = 0; i<10; i++)
            {
                Insert2grid.Columns.Add("Pick " + (i + 1).ToString());
            }
            foreach (var DateOfDraw in _Result)
            {
                List<DrawNumber> Picks = TenPicks(DateOfDraw.DrawDate,5);
                System.Data.DataRow row = Insert2grid.NewRow();
                row["Date"] = DateOfDraw.DrawDate.ToShortDateString();
                int count = 0;
                for (int i = 0; i < Picks.Count; i++)
                {
                    row["Pick " + (i + 1).ToString()] = Picks[i].ToResultString();
                    
                    row["Wins"] = (DateOfDraw.CheckResult(Picks[i])) ? row["Wins"] + 
                        (DateOfDraw.ResultDetail(Picks[i])[0].ToString("00") + " - "+  
                        DateOfDraw.ResultDetail(Picks[i])[1].ToString("00") + ", ") : row["Wins"];
                }
                Insert2grid.Rows.Add(row);
            }

            return Insert2grid;
        }
    }
}
