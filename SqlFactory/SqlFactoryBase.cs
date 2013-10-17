using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using AccidentallyORM.DBHelper;
using AccidentallyORM.Entity;

namespace AccidentallyORM.SqlFactory
{
    public class SqlFactoryBase<T> where T : EntityBase
    {
        protected internal List<DbParameter> Parameters = new List<DbParameter>();
        protected StringBuilder Sql = new StringBuilder();
        protected string SqlTableName;
        protected SqlFieldFactory<T> SqlFieldFactory;

        public SqlFactoryBase()
        {
            SqlTableName = EntityHelper.GetTableName<T>();
            SqlHelper = new DbService();
        }

        public SqlFieldFactory<T> Field(string fieldName)
        {
            return SqlFieldFactory<T>.Field(fieldName);
        }

        public DbService SqlHelper { get; set; }

        public virtual string ToSql()
        {
            return Sql.ToString();
        }
    }
}