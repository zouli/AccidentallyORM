using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using AccidentallyORM.DBHelper;
using AccidentallyORM.Entity;
using AccidentallyORM.Entity.Attribute;
using AccidentallyORM.SqlFieldFactory;

namespace AccidentallyORM.SqlFactory
{
    public class SqlFactoryBase<T> where T : EntityBase, new()
    {
        protected internal SqlParameter Parameters = new SqlParameter();
        protected StringBuilder Sql = new StringBuilder();

        protected static string _sqlTableName = EntityHelper.GetTableName<T>();
        public string TableName
        {
            get { return _sqlTableName; }
        }

        public static Dictionary<string, DataFieldAttribute> _sqlField = EntityHelper.GetFieldAttributes<T>(_sqlTableName);
        public Dictionary<string, DataFieldAttribute> SqlFields
        {
            get { return _sqlField; }
        }

        public SqlFactoryBase()
        {
            SqlHelper = new DbService();
        }

        public SqlField<T> Field(string fieldName)
        {
            var field = SqlFields[fieldName];
            return field;
        }

        public SqlField<T> Field(Expression<Func<T, object>> predicate)
        {
            return Field(EntityHelper.GetPropertyName(predicate));
        }

        public DbService SqlHelper { get; set; }

        public virtual string ToSql()
        {
            return Sql.ToString();
        }
    }
}