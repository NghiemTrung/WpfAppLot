using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Collections.Generic;
using WpfAppLot.Model;
using System.Linq;

namespace WpfAppLot.Database
{
    static class CreateDatabase
    {
        public static void InitDatabase()
        {
            #region initial excel connection
            string FilePath = "../../Database/lotto.xlsx";
            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=Excel 12.0";
            OleDbConnection ExcelConn = new OleDbConnection(connString);
            DataSet Dset = new DataSet();
            #endregion

            #region get data from excel file
            try
            {
                ExcelConn.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", ExcelConn);
                OleDbDataAdapter Ad = new OleDbDataAdapter(cmd);
                Ad.Fill(Dset);
            }
            catch { }
            finally
            {
                ExcelConn.Close();
            }
            foreach (DataRow row in Dset.Tables[0].Rows)
            {
                DrawNumber addNumber = new DrawNumber(
                    Convert.ToInt16(row[0].ToString()),
                    Convert.ToDateTime(row[1].ToString()),
                    Convert.ToByte(row[2].ToString()),
                    Convert.ToByte(row[3].ToString()),
                    Convert.ToByte(row[4].ToString()),
                    Convert.ToByte(row[5].ToString()),
                    Convert.ToByte(row[6].ToString()),
                    Convert.ToByte(row[7].ToString()),
                    Convert.ToByte(row[8].ToString())
                    );
                GlobalVar.DrawResult.Add(addNumber);
            }
            #endregion

            #region Create and add result data to SQL server
            List<DateTime> DrawDate = new List<DateTime>();
            DateTime StarDate = new DateTime(2012, 03, 23);
            while (DateTime.Compare(StarDate, DateTime.Now) <= 0)
            {
                DrawDate.Add(StarDate);
                StarDate = StarDate.AddDays(7);
            }
            SqlConnection MyConnection = new SqlConnection(GlobalVar.ConnectionStringSQL);
            SqlCommand MyCommand = new SqlCommand();
            MyCommand.Connection = MyConnection;
            MyConnection.Open();
            MyCommand.CommandText = "DROP TABLE DrawResult";
            MyCommand.ExecuteNonQuery();
            MyCommand.CommandText = "create table DrawResult ( ID smallint NOT NULL Primary key identity(1,1), DrawDate Date, Num1 tinyint Default 0, Num2 tinyint Default 0, Num3 tinyint Default 0, Num4 tinyint Default 0, Num5 tinyint Default 0, EURnum1 tinyint Default 0, EURnum2 tinyint Default 0 )";
            MyCommand.ExecuteNonQuery();
            MyCommand.Parameters.Add(new SqlParameter("@DrDate", System.Data.SqlDbType.Date));
            foreach (DateTime DrDate in DrawDate)
            {
                MyCommand.CommandText = "INSERT INTO DrawResult (DrawDate) VALUES (@DrDate)";
                MyCommand.Parameters[0].Value = DrDate;
                MyCommand.ExecuteNonQuery();
            }
            #endregion

            #region Create and add statistic to SQL server
            foreach (DrawNumber Result in GlobalVar.DrawResult)
            {
                DrawDateContext.UpdateDrawNumber(Result, GlobalVar._DataService);
            }

            MyCommand.CommandText = "DROP TABLE SumStatistic";
            MyCommand.ExecuteNonQuery();


            MyCommand.CommandText = "create table SumStatistic ( ID smallint NOT NULL Primary key identity(1,1), DrawSum int Default 0, NumberGroup tinyint Default 0, AppearCount smallint Default 0 ); ";
            MyCommand.ExecuteNonQuery();
            
            MyCommand.Parameters.Add(new SqlParameter("@DrawSum", System.Data.SqlDbType.Int));
            MyCommand.Parameters.Add(new SqlParameter("@NumberGroup", System.Data.SqlDbType.TinyInt));
            MyCommand.Parameters.Add(new SqlParameter("@AppearCount", System.Data.SqlDbType.SmallInt));
            for(int i =0; i<2;i++)
            {
                int[,] SumVal = CalculateSum(GlobalVar.DrawResult)[i];
                for(int t = 0;t<SumVal.GetLength(0);t++)
                {
                    MyCommand.CommandText = "INSERT INTO SumStatistic (DrawSum,NumberGroup,AppearCount) VALUES (@DrawSum,@NumberGroup,@AppearCount)";
                    MyCommand.Parameters["@DrawSum"].Value = SumVal[t, 0];
                    MyCommand.Parameters["@NumberGroup"].Value = i;
                    MyCommand.Parameters["@AppearCount"].Value = SumVal[t, 1];
                    MyCommand.ExecuteNonQuery();
                }
            }
            #endregion
        }

        public static List<int[,]> CalculateSum(List<DrawNumber> ListOfResult)
        {

            int[,] MainSum = new int[GlobalVar.NumberOfMain * 5 - 15, 2];
            int[,] SideSum = new int[GlobalVar.NumberOfSide * 2 - 3, 2];
            for (int i = 0; i < MainSum.Length; i++) {
                MainSum[i, 0] = i + 15;
                MainSum[i, 1] = 0;
            }
            for (int i = 0; i < SideSum.Length; i++)
            {
                SideSum[i, 0] = i + 3;
                SideSum[i, 1] = 1;
            }
            foreach (DrawNumber Result in ListOfResult)
            {
                if (Result.MainSum >= 15) {
                MainSum[Result.MainSum - 15,1] += 1; }
                if (Result.SideSum >= 3)
                {
                    SideSum[Result.SideSum - 3, 1] += 1;
                }
            }
            List<int[,]> ReturnResult = new List<int[,]>();
            ReturnResult.Add(MainSum);
            ReturnResult.Add(SideSum);
            return ReturnResult;
        }
    }
}
