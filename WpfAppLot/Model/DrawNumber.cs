using System;
using System.Linq;
using WpfAppLot.Database;
using System.Collections.Generic;
using System.Data;
namespace WpfAppLot.Model
{
    class DrawNumber
    {
        #region Properties
        private short _ID;
        private DateTime _DrawDate;
        private byte _Num1;
        private byte _Num2;
        private byte _Num3;
        private byte _Num4;
        private byte _Num5;
        private byte _EUNum1;
        private byte _EUNum2;
        #endregion
        #region Properties get set
        public DateTime DrawDate {get { return _DrawDate; }}
        public short ID { get { return _ID; } }
        public byte Num1 { get { return _Num1; } set { _Num1 = value; } }
        public byte Num2 { get { return _Num2; } set { _Num2 = value; } }
        public byte Num3 { get { return _Num3; } set { _Num3 = value; } }
        public byte Num4 { get { return _Num4; } set { _Num4 = value; } }
        public byte Num5 { get { return _Num5; } set { _Num5 = value; } }
        public byte EUNum1 { get { return _EUNum1; } set { _EUNum1 = value; } }
        public byte EUNum2 { get { return _EUNum2; } set { _EUNum2 = value; } }
        public int MainSum { get { return _Num1 + _Num2 + _Num3 + _Num4 + _Num5; } }
        public int SideSum { get { return _EUNum1 + _EUNum2; } }
        public byte[] ListMainNum { get { return new byte[] { _Num1, _Num2, _Num3, _Num4, _Num5 }; } }
        public byte[] ListSideNum { get { return new byte[] { _EUNum1, _EUNum2 }; } }
        #endregion
        #region Construct
        public DrawNumber(DateTime _DrawDate)
        {
            this._DrawDate = _DrawDate;
        }
        public DrawNumber(short ID, DateTime _DrawDate) : this(_DrawDate)
        {
            _ID = ID;
        }
        public DrawNumber(short ID, DateTime _DrawDate, byte Num1, byte Num2, byte Num3, byte Num4, byte Num5, byte EuNum1, byte EuNum2) : this(ID, _DrawDate)
        {
            _Num1 = Num1; _Num2 = Num2; _Num3 = Num3; _Num4 = Num4; _Num5 = Num5;
            _EUNum1 = EuNum1; _EUNum2 = EuNum2;
        }
        public DrawNumber(DateTime _DrawDate, byte Num1, byte Num2, byte Num3, byte Num4, byte Num5, byte EuNum1, byte EuNum2) : this(_DrawDate)
        {
            _Num1 = Num1; _Num2 = Num2; _Num3 = Num3; _Num4 = Num4; _Num5 = Num5;
            _EUNum1 = EuNum1; _EUNum2 = EuNum2;
        }
        #endregion
        #region Methods
        public void SortNum()
        {
            byte[] _ListMainNum = this.ListMainNum;
            Array.Sort(_ListMainNum);
            this._Num1 = _ListMainNum[0];
            this._Num2 = _ListMainNum[1];
            this._Num3 = _ListMainNum[2];
            this._Num4 = _ListMainNum[3];
            this._Num5 = _ListMainNum[4];
        }

        public string ToResultString()
        {
            return (_Num1.ToString("00") +" "+ 
                _Num2.ToString("00") + " " + 
                _Num3.ToString("00") + " " + 
                _Num4.ToString("00") + " " + 
                _Num5.ToString("00") + " - " + 
                _EUNum1.ToString("00") + " " + 
                _EUNum2.ToString("00"));
        }

        // Check if is the draw number is winning or not
        public bool CheckResult(DrawNumber _Checker)
        {
            int count = 0;
            foreach (var WinNum in this.ListMainNum)
            {
                foreach (var CounterNum in _Checker.ListMainNum) { count = (WinNum == CounterNum) ? count + 1 : count; }
            }
            foreach (var WinNum in this.ListSideNum)
            {
                foreach (var CounterNum in _Checker.ListSideNum) { count = (WinNum == CounterNum) ? count + 1 : count; }
            }
            return (count >= 3);
        }

        public int[] ResultDetail(DrawNumber _Checker)
        {
            int[] _output = new int[] { 0, 0 };

            foreach (var WinNum in this.ListMainNum)
            {
                foreach (var CounterNum in _Checker.ListMainNum) { _output[0] = (WinNum == CounterNum) ? _output[0] + 1 : _output[0]; }
            }
            foreach (var WinNum in this.ListSideNum)
            {
                foreach (var CounterNum in _Checker.ListSideNum) { _output[1] = (WinNum == CounterNum) ? _output[1] + 1 : _output[1]; }
            }

            return _output;
        }
        #endregion
    }

    class NumberDraw
    {
        #region Properties
        private byte _NumberLetter;
        private byte _CurrentSkip;
        private byte _LastX;
        private List<byte> _Skipped;
        private List<byte> _LastXGame;
        private List<string> _LastXtrend;
        private List<string> _HistoryxgameTrend;
        #endregion

        #region get set
        public byte NumberLetter { get { return _NumberLetter; } }
        public byte CurrentSkip { get { return _CurrentSkip; } }
        public List<byte> Skipped { get { return _Skipped; } }
        public byte LastX { get { return _LastX; } set { _LastX = value; } }
        public List<byte> LastXgame { get { return _LastXGame; } }
        public string Gametrend { get
            {
                return DefindTrend(_LastXGame.LastIndexOf(0), _LastXGame.Count() - 1);
            } }
        public List<string> LastXtrend { get { return _LastXtrend; } }
        #endregion

        #region Construct
        public NumberDraw()
        {
            _Skipped = new List<byte>();
            _LastXGame = new List<byte>();
            _LastXtrend = new List<string>();
            _HistoryxgameTrend = new List<string>();
            AddSkip(1);
        }
        public NumberDraw(byte _Letter) : this()
        {
            _NumberLetter = _Letter;
        }
        #endregion

        #region method
        public void AddSkip(byte _Skip)
        {
            _Skipped.Add(_Skip);
            _CurrentSkip = _Skip;
        }
        public void UpdateSkip(DrawNumber _Result)
        {
            _Skipped.Add((byte)(_Skipped[_Skipped.Count() - 1] + 1));
            foreach (byte b in _Result.ListMainNum)
            {
                if (b == _NumberLetter) { _Skipped[_Skipped.Count() - 1]=0; }
            }
            _CurrentSkip = _Skipped[_Skipped.Count() - 1];
        }
        public void UpdateLastXGame(List<DrawNumber> _ListResult)//the occur during the last X games
        {
            _LastXGame.Clear();
            _LastXtrend.Clear();
            int OutofXgame=0;
            for (int i = 0; i <= _ListResult.Count-(int)_LastX; i++)
            {
                byte[] LastXNumbers=new byte[] { 0 };
                for(int t = 0; t < (int)_LastX; t++)
                {
                    LastXNumbers = LastXNumbers.Concat(_ListResult[i + t].ListMainNum).ToArray<byte>();
                }
                _LastXGame.Add(0);
                foreach(byte num in LastXNumbers)
                {
                    _LastXGame[i] = (num == _NumberLetter) ? (byte)(_LastXGame[i] + 1) : _LastXGame[i];
                }
                if (_LastXGame[i]==0) OutofXgame = i;
                _LastXtrend.Add(DefindTrend(OutofXgame, i));
            }
        }

        public string DefindTrend(int _OutofXgame, int _CurrentXgammeIndex)
        {
            string _trend="";
            if (_OutofXgame == _CurrentXgammeIndex) { _trend = "outed"; }
            else if (_CurrentXgammeIndex - _OutofXgame == 1)
                {
                    _trend = "start";
                }
            else if (_LastXGame[_CurrentXgammeIndex]<_LastXGame.Skip(_OutofXgame+1).Max()) {
                _trend = "Cooling";
            }else if (_LastXGame[_CurrentXgammeIndex] >= _LastXGame.Max()){
                _trend = "real hot";
            } else if (_LastXGame[_CurrentXgammeIndex] == _LastXGame.Skip(_OutofXgame+1).Max())
            {
                if(_LastXGame[_CurrentXgammeIndex] !=1) { _trend = "hot"; } else { _trend = "start"; }
            }
            return _trend;
        }
        #endregion
    }
    class DrawDateContext
    {
        #region Prop
        private List<DrawNumber> _DrawNumberS;
        private DB_Service _DBconnect;
        #endregion
        #region get set
        public List<DrawNumber> DrawNumberS { get { return _DrawNumberS; } }
        public DB_Service DbConnection { set { _DBconnect = value; } }
        #endregion
        #region Construction
        //public DrawDateContext() { _DrawNumberS = new List<DrawNumber>(); }
        //public DrawDateContext(DB_Service DatabaseContext):this()
        //{
        //    _DBconnect = DatabaseContext;
        //}
        #endregion
        #region General Methods
        public static IEnumerable<DrawNumber> GetALLDrawNumbers(DB_Service DBconnect)
        {
            string _Query = "SELECT * FROM DrawResult";
            DBconnect.Query = _Query;
            foreach (DataRow row in DBconnect.OutTable.Rows)
            {
                DrawNumber DrawResult;
                try
                {
                    DrawResult = new DrawNumber(Convert.ToInt16(row["ID"].ToString()), Convert.ToDateTime(row["DrawDate"].ToString()), Convert.ToByte(row["Num1"].ToString()), Convert.ToByte(row["Num2"].ToString()), Convert.ToByte(row["Num3"].ToString()), Convert.ToByte(row["Num4"].ToString()), Convert.ToByte(row["Num5"].ToString()), Convert.ToByte(row["EURnum1"].ToString()), Convert.ToByte(row["EURnum2"].ToString()));
                }
                catch
                {
                    DrawResult = new DrawNumber(Convert.ToInt16(row["ID"].ToString()), Convert.ToDateTime(row["DrawDate"].ToString()));
                }
                yield return DrawResult;
            }
        }
        public static void UpdateDrawNumber(DrawNumber InputNumbers, DB_Service DBconnect)
        {
            string Table = "DrawResult";
            string[] Column = new string[] { "Num1", "Num2", "Num3", "Num4", "Num5", "EURnum1", "EURnum2" };
            string[] ValueParaName = Column;
            string[] ValueParaValue = new string[]
            {
                InputNumbers.Num1.ToString(),
                InputNumbers.Num2.ToString(),
                InputNumbers.Num3.ToString(),
                InputNumbers.Num4.ToString(),
                InputNumbers.Num5.ToString(),
                InputNumbers.EUNum1.ToString(),
                InputNumbers.EUNum2.ToString()
            };
            string[] WhereRow = new string[] { "DrawDate" };
            string[] WherePara = new string[] { "DrawDate" };
            string[] WhereValue = new string[] { InputNumbers.DrawDate.ToString("yyyy-MM-dd") };
            DBconnect.UpdateCommandWithoutkey(Table, Column, ValueParaName, ValueParaValue, WhereRow, WherePara, WhereValue);
            DBconnect.nonQueryCmd();
        }
        #endregion
        #region Calculate statics
        public static List<byte> CollectionShould()
        {
            return new List<byte>();
        }
        public static bool compareDraw(DrawNumber _First, DrawNumber _Second)
        {
            return _First.ListMainNum.Equals(_Second.ListMainNum);
        }
        #endregion
    }
}
