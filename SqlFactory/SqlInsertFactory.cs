using System.Collections.Generic;
using System.Linq;
using AccidentallyORM.DBHelper;
using AccidentallyORM.Entity;
using AccidentallyORM.Entity.Attribute;

namespace AccidentallyORM.SqlFactory
{
    public class SqlInsertFactory<T> : SqlFactoryBase<T> where T : EntityBase, new()
    {
        private Dictionary<string, DataFieldAttribute> _sqlUsingFields = new Dictionary<string, DataFieldAttribute>();

        public SqlInsertFactory<T> Insert()
        {
            Sql.Append("INSERT INTO ");
            Sql.Append(SqlTableName);
            return this;
        }

        public SqlInsertFactory<T> Fields(params string[] usingFields)
        {
            if (usingFields.Length > 0)
            {
                foreach (var field in usingFields)
                {
                    _sqlUsingFields.Add(field, SqlFieldFactory<T>.SqlFields[field]);
                }
            }
            else
            {
                _sqlUsingFields = SqlFieldFactory<T>.SqlFields;
            }

            Sql.Append(" (");
            Sql.Append(string.Join(",", EntityHelper.GetFieldNames(_sqlUsingFields).ToArray()));
            Sql.Append(")");
            return this;
        }

        public SqlInsertFactory<T> Values(EntityBase entity, params string[] defaultFields)
        {
            var defaultFieldsList = new List<string>(defaultFields);

            Sql.Append(" VALUES (");
            foreach (var field in _sqlUsingFields)
            {
                if (defaultFieldsList.Contains(field.Key))
                {
                    Sql.Append(field.Value.Default + ",");
                }
                else
                {
                    string paraName = "@" + field.Value.FieldName;
                    Sql.Append(paraName + ",");
                    Parameters.Add(
                        DbService.CreateParameter(paraName, field.Value.ColumnType, entity.GetValue(field.Key)));
                }
            }
            Sql.Remove(Sql.Length - 1, 1);
            Sql.Append(")");
            return this;
        }

        public SqlInsertFactory<T> ValueToReplace(string field, string value)
        {
            var usingField = _sqlUsingFields[field];
            var fieldName = "@" + usingField.FieldName;

            Sql = Sql.Replace(fieldName, value);

            foreach (var parameter in Parameters.Where(parameter => parameter.ParameterName.Equals(fieldName)))
            {
                Parameters.Remove(parameter);
                break;
            }

            return this;
        }

        public SqlInsertFactory<T> ValueToReplace(string field, SqlFactoryBase<T> value)
        {
            var usingField = _sqlUsingFields[field];
            var fieldName = "@" + usingField.FieldName;

            Sql = Sql.Replace(fieldName, "(" + value.ToSql() + ")");

            foreach (var parameter in
                from parameter in value.Parameters
                let addFlag = Parameters.All(dbParameter => !dbParameter.ParameterName.Equals(parameter.ParameterName))
                where addFlag
                select parameter)
            {
                Parameters.Add(parameter);
            }
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
    }
}