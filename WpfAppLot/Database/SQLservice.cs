using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace WpfAppLot.Database
{
    class SQLservice : DB_Service
    {
        #region Declare Propertises
        protected SqlConnection _connector;
        protected SqlDataAdapter _DataAdapter;
        protected SqlCommand _command;
        private SqlConnectionStringBuilder ConnStrBuilder;
        #endregion
        //getting value from outside
        #region Get-Set
        public string ServerAddress
        {
            set { ConnStrBuilder.DataSource = value; UpdateConnectionString(); }
        }
        public string DatabaseName
        {
            set { ConnStrBuilder.InitialCatalog = value; UpdateConnectionString(); }
        }
        public string Username
        {
            set { ConnStrBuilder.UserID = value; UpdateConnectionString(); }
        }
        public string Password
        {
            set { ConnStrBuilder.Password = value; UpdateConnectionString(); }
        }
        #endregion
        //Constructor
        #region Constructor
        public SQLservice()
        {
            _connector = new SqlConnection();
            _DataAdapter = new SqlDataAdapter();
            _command = new SqlCommand();
            _command.Connection = _connector;
            ConnStrBuilder = new SqlConnectionStringBuilder();
            ConnStrBuilder.ConnectTimeout = 15;
            ConnStrBuilder.IntegratedSecurity = false;
            ConnStrBuilder.TrustServerCertificate = true;
        }
        public SQLservice(string ConnectionString) : this()
        {
            _connector.ConnectionString = ConnectionString;
            _command.Connection = _connector;
        }

        public SQLservice(string _Server, string _Database) : this()
        {
            ConnStrBuilder.DataSource = _Server;
            ConnStrBuilder.InitialCatalog = _Database;
            UpdateConnectionString();
        }
        public SQLservice(string _Server, string _Database, string _username, string _password) : this(_Server,_Database)
        {
            ConnStrBuilder.UserID = _username;
            ConnStrBuilder.Password = _password;
            UpdateConnectionString();
        }
        #endregion
        //method
        #region Method
        public void UpdateConnectionString()
        {
            _connector.ConnectionString = ConnStrBuilder.ConnectionString;
        }
        public override void OpenConnection()   { _connector.Open(); }
        public override void CloseConnection()  { _connector.Close(); }
        public override DataTable outTable()
        {
            _command.CommandText = _query;
            _DataAdapter.SelectCommand = _command;
            _DataAdapter.Fill(_DataTable);
            return _DataTable;
        }
        public IEnumerable<IDataRecord> SelectAll(string TableName)
        {
            _query = "SELECT * FROM " + TableName;
            _command.CommandText = _query;
            using (SqlDataReader Reader = _command.ExecuteReader())
            {
                while (Reader.Read())
                {
                    yield return Reader;
                }
            }
        }
        public override void nonQueryCmd()
        {
            _command.ExecuteNonQuery();
            _command.Parameters.Clear();
        }
        #region Command Construction
        private void AddParameter (string[] ParaName,string[] ParaValue)
        {
            for (int i = 0; i < ParaName.Length; i++)
            {
                _command.Parameters.Add(new SqlParameter(ParaName[i], ParaValue[i]));
            }
        }
        public override void ConstructSelect(string[] Column,string[] Table, string[] Where, string[] ParaName, string[] ParaValues)
        {
            base.ConstructSelect(Column, Table, Where, ParaName);
            _command.CommandText = _query;
            AddParameter(ParaName, ParaValues);
        }
        public override void ConstructInsert(string Table, string[] Column, string[] ParaName, string[] ParaValue)
        {
            base.ConstructInsert(Table, Column, ParaName);
            _command.CommandText = _query;
            AddParameter(ParaName, ParaValue);
        }
        public override void UpdateCommandWithKey(string Table, string[] Column, string[] ParaName, string[] ParaValue)
        {
            base.ConstructUpdateWithKey(Table, Column, ParaName);
            _command.CommandText = _query;
            AddParameter(ParaName, ParaValue);
        }
        public override void UpdateCommandWithoutkey(string Table, string[] Column, string[] ValueParaName, string[] ValueParaValue, string[] Row, string[] WhereParaName, string[] WhereParaValue)
        {
            base.ConstructUpdateWithoutkey(Table, Column, ValueParaName, Row, WhereParaName);
            _command.CommandText = _query;
            AddParameter(ValueParaName, ValueParaValue);
            AddParameter(WhereParaName, WhereParaValue);
        }
        #endregion
        #endregion
    }
}
