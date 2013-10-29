using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using AccidentallyORM.DBHelper;
using AccidentallyORM.Entity;
using AccidentallyORM.Entity.Attribute;

namespace AccidentallyORM.SqlFactory
{
    public class SqlFactoryBase<T> where T : EntityBase, new()
    {
        protected internal SqlParameter Parameters = new SqlParameter();
        protected StringBuilder Sql = new StringBuilder();

        protected static string SqlTableName = EntityHelper.GetTableName<T>();
        protected static Dictionary<string, DataFieldAttribute> SqlFields = EntityHelper.GetFieldAttributes<T>();
        protected static Dictionary<Expression<Func<T, object>>, string> Properties = new Dictionary<Expression<Func<T, object>>, string>();

        public SqlFactoryBase()
        {
            SqlHelper = new DbService();
        }

        public SqlFieldFactory<T> Field(string fieldName)
        {
            var field = SqlFields[fieldName];
            return field;
        }

        public SqlFieldFactory<T> Field(Expression<Func<T, object>> predicate)
        {
            if (!Properties.ContainsKey(predicate))
            {
                Properties.Add(predicate, EntityHelper.GetPropertyName(predicate));
            }
            return Field(Properties[predicate]);
        }

        public DbService SqlHelper { get; set; }

        public virtual string ToSql()
        {
            return Sql.ToString();
        }
    }
}