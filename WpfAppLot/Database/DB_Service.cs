using System.Data;
using System.Data.Common;

namespace WpfAppLot.Database
{
    public class DataOutput
    {

    }
    class DB_Service
    {
        #region Propertises and get-set
        protected string _ConnectionString;
        protected string _query;
        protected DataTable _DataTable;
        //get and set value
        public string Query
        {
            set { _query = value; }
        }
        public virtual string ConnectionString
        {
            set { _ConnectionString = value; }
            get { return _ConnectionString; }
        }
        public virtual System.Data.DataTable OutTable
        {
            get { return outTable(); }
        }
        #endregion
        //basic method
        #region Virtual method
        public virtual System.Data.DataTable outTable() { return new DataTable(); }
        public virtual void nonQueryCmd()       { }
        public virtual void OpenConnection()    { }
        public virtual void CloseConnection()   { }
        public virtual void ConstructSelect         (string[] Column, string[] Table, string[] Where, string[] ParaName, string[] ParaValues) { }
        public virtual void ConstructInsert         (string Table, string[] Column, string[] ParaName, string[] ParaValue) { }
        public virtual void UpdateCommandWithKey    (string Table, string[] Column, string[] ParaName,string[] ParaValue) { }
        public virtual void UpdateCommandWithoutkey (string Table, string[] Column, string[] ValueParaName, string[] ValueParaValue, string[] Row, string[] WhereParaName,string[] WhereParaValue) { }
        #endregion
        #region Command Construction
        protected void      ConstructSelect         (string[] Column, string[] Table, string[] Where, string[] ParaName)
        {
            _query = "SELECT";
            foreach (string ColumnName in Column)   { _query += " " + ColumnName + "],"; }
            _query = _query.Remove(_query.Length - 1) + " FROM";
            foreach (string TableName in Table)     { _query += " [" + TableName + "],"; }
            _query = _query.Remove(_query.Length - 1) + " WHERE [";
            for (int i = 0; i < Where.Length; i++)  { _query = _query + Where[i] + "] = @" + ParaName[i] + " AND ["; }
            _query = _query.Remove(_query.Length - 6);
        }
        protected void      ConstructInsert         (string Table, string[] Column, string[] ParaName)
        {
            _query = "INSERT INTO " + Table + "(";
            foreach (string ColumnName in Column)   { _query = _query +"["+ ColumnName + "], "; }
            _query = _query.Remove(_query.Length - 2) + ") VALUES (";
            foreach (string Para in ParaName)       { _query = _query + "@" + Para + ", "; }
            _query = _query.Remove(_query.Length - 2) + ");";
        }
        protected void      ConstructUpdateWithKey  (string Table, string[] Column, string[] ParaName)
        {
            this.ConstructInsert(Table, Column, ParaName);
            _query = _query + " ON DUPLICATE KEY UPDATE ";
            foreach (string ColumnName in Column)   { _query = _query + ColumnName + "= VALUES(" + ColumnName + "),"; }
            _query = _query.Remove(_query.Length - 1);
        }
        protected void      ConstructUpdateWithoutkey(string Table, string[] Column, string[] ValueParaName, string[] Row, string[] WhereParaName)
        {
            _query = "Update " + Table + " SET ";
            for (int i = 0; i < Column.Length; i++) { _query += Column[i] + "=@" + ValueParaName[i] + ", "; }
            _query = _query.Remove(_query.Length - 2) + " WHERE ";
            for (int i = 0; i < Row.Length; i++)    { _query += Row[i] + "=@" + WhereParaName[i] + " AND "; }
            _query = _query.Remove(_query.Length - 5);
        }
        #endregion
    }
}
