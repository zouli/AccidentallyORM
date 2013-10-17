using System.Reflection;
using System.Text;
using AccidentallyORM.Entity.Attribute;
using AccidentallyORM.Extensions;

namespace AccidentallyORM.Entity
{
    public class EntityBase
    {
        public override string ToString()
        {
            var fields = new StringBuilder();

            foreach (var propertyInfo in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = propertyInfo.GetValue(this, null);
                fields.Append(propertyInfo.Name + "=" + value + ";");
            }
            return fields.ToString();
        }

        public object GetValue(string propertyName)
        {
            var propertyInfo = GetType().GetProperty(propertyName);
            var dataFieldAttribute =
                (DataFieldAttribute)System.Attribute.GetCustomAttribute(propertyInfo, typeof(DataFieldAttribute));
            var value = propertyInfo.GetValue(this, null);

            if (propertyInfo.PropertyType.FullName.Equals("System.Boolean"))
            {
                if (null != dataFieldAttribute && !dataFieldAttribute.ConvertFormat.Equals(string.Empty))
                {
                    value = bool.Parse(value.ToString()).ToStringByFormat(dataFieldAttribute.ConvertFormat);
                }
            }
            return value;
        }
    }
}