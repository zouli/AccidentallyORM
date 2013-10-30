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

        public SqlUpdateFactory<T> Update()
        {
            Sql.Append("UPDATE ");
            Sql.Append(SqlTableName);
            return this;
        }

        public SqlUpdateFactory<T> Set(Expression<Func<T, object>> predicate, bool useDefault = false)
        {
            return Set(predicate, null, useDefault);
        }

        public SqlUpdateFactory<T> Set(Expression<Func<T, object>> predicate, object value, bool useDefault = false)
        {
            return Set(EntityHelper.GetPropertyName(predicate), value, useDefault);
        }

        public SqlUpdateFactory<T> Set(string fieldName, bool useDefault = false)
        {
            return Set(fieldName, null, useDefault);
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
                    var paraName = SqlParameter.GetParameterName(field.FieldName);
                    setString += " = " + paraName;
                    Parameters.Add(paraName, field.ColumnType, value);
                }
            }

            _sqlSet.Append(_sqlSet.Length > 0 ? "," : " SET ");
            _sqlSet.Append(setString);

            return this;
        }


        public SqlUpdateFactory<T> Where(SqlField<T> sqlField)
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

        public override string ToSql()
        {
            Sql.Append(_sqlSet);
            if (_sqlWhere.Length > 0) Sql.Append(_sqlWhere);

            return Sql.ToString();
        }
    }
}