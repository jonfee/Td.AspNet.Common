using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Td.AspNet.Utils
{
    public class DicMapper
    {
        /// <summary>
        ///
        /// 将对象属性转换为key-value对
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToMap(object o)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            Type t = o.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            object value;

            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();
                if (mi != null && mi.IsPublic)
                {
                    value = mi.Invoke(o, new string[] { });

                    if (value != null)
                    {
                        map.Add(p.Name, value.ToString());
                    }
                    else
                    {
                        map.Add(p.Name, string.Empty);
                    }
                }
            }
            return map;
        }

    }
}
