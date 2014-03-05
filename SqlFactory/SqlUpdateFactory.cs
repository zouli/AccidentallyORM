using System;
using System.Linq.Expressions;
using System.Text;
using AccidentallyORM.Entity;
using AccidentallyORM.SqlFieldFactory;

namespace AccidentallyORM.SqlFactory
{
    public class SqlUpdateFactory<T> : SqlFactoryBase<T> where T : EntityBase, new()
    {
        private readonly StringBuilder _sqlSet = new StringBuilder();
        private readonly StringBuilder _sqlWhere = new StringBuilder();
        private T _entity;

        private SqlUpdateFactory<T> Update()
        {
            Sql.Append("UPDATE ");
            Sql.Append(TableName);
            return this;
        }

        public SqlUpdateFactory<T> Update(T entity)
        {
            _entity = entity;
            return Update();
        }

        public SqlUpdateFactory<T> Set(Expression<Func<T, object>> predicate, bool useDefault = false)
        {
            var propertyName = EntityHelper.GetPropertyName(predicate);
            return Set(propertyName, _entity.GetValue(propertyName), useDefault);
        }

        public SqlUpdateFactory<T> Set(Expression<Func<T, object>> predicate, object value, bool useDefault = false)
        {
            return Set(EntityHelper.GetPropertyName(predicate), value, useDefault);
        }

        public SqlUpdateFactory<T> Set(string fieldName, bool useDefault = false)
        {
            return Set(fieldName, _entity.GetValue(fieldName), useDefault);
        }

        public SqlUpdateFactory<T> Set(string fieldName, object value, bool useDefault = false)
        {
            var field = SqlFields[fieldName];
            var setString = field.FieldName;

            if (useDefault)
            {
                setString += " = " + field.Default;
            }
            else
            {
                if (null == value)
                {
                    setString += " = NULL";
                }
                else
                {
                    var paraName = SqlParameter.GetParameterName(field.FieldName.Replace(".", "_"));
                    setString += " = " + paraName;
                    Parameters.Add(paraName, field.ColumnType, value);
                }
            }

            _sqlSet.Append(_sqlSet.Length > 0 ? "," : " SET ");
            _sqlSet.Append(setString);

            return this;
        }

        public SqlUpdateFactory<T> Where<TSub>(SqlField<TSub> sqlField) where TSub : EntityBase, new()
        {
            _sqlWhere.Append(" WHERE ");
            _sqlWhere.Append(sqlField.ToString());

            Parameters.AddRange(sqlField.Parameters);

            return this;
        }

        public int Go()
        {
            if (Parameters.Count > 0)
            {
                return SqlHelper.ExecuteNonQuery(ToSql(), Parameters.ToArray());
            }
            return SqlHelper.ExecuteNonQuery(ToSql());
        }

        public int Raw(string sql)
        {
            return Raw(sql, new SqlParameter());
        }

        public int Raw(string sql, SqlParameter parameters)
        {
            Parameters = parameters;
            if (Parameters.Count > 0)
            {
                return SqlHelper.ExecuteNonQuery(sql, Parameters.ToArray());
            }
            return SqlHelper.ExecuteNonQuery(sql);
        }

        public override string ToSql()
        {
            Sql.Append(_sqlSet);
            if (_sqlWhere.Length > 0) Sql.Append(_sqlWhere);

            return Sql.ToString();
        }
    }
}