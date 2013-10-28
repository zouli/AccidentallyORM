using System.Linq;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;

namespace AccidentallyORM.Extensions
{
    public static class StringExtensions
    {
        public static string ToPinYin(this string source)
        {
            if (source.Length != 0)
            {
                var fullSpellBuild = new StringBuilder();
                foreach (var _char in source)
                {
                    //判断是否是中文
                    var itemFlag = ChineseChar.IsValidChar(_char);
                    if (itemFlag)
                    {
                        var chineseChar = new ChineseChar(_char);
                        foreach (var value in chineseChar.Pinyins.Where(value => !string.IsNullOrEmpty(value)))
                        {
                            fullSpellBuild.Append(value.Remove(value.Length - 1, 1));
                            break;
                        }
                    }
                    else
                    {
                        fullSpellBuild.Append(_char);
                    }
                }
                return fullSpellBuild.ToString();
            }
            return "";
        }

        public static bool ToBooleanByFormat(this string source, string format)
        {
            format += format.ToLower().Contains(":") ? "" : ":";
            var formats = format.Split(new[] { ':' });
            return source.ToLower().Equals(formats[0]);
        }
    }
}