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

        protected static string SqlTableName = EntityHelper.GetTableName<T>();
        protected static Dictionary<string, DataFieldAttribute> SqlFields = EntityHelper.GetFieldAttributes<T>();

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