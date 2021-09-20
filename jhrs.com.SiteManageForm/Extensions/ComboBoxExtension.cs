using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jhrs.com.SiteMange.Extensions
{
    public static class UIComboBoxExtension
    {
        /// <summary>
        /// 绑定枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comboBox">下拉框</param>
        /// <param name="attribute">过滤特性</param>
        public static void BindEnum<T>(this ComboBox comboBox, Type attribute = null) where T : Enum
        {
            var fields = typeof(T).GetFields();
            foreach (var field in fields)
            {
                if (!field.FieldType.IsEnum) continue;
                if (attribute == null)
                {
                    comboBox.Items.Add(field.Name);
                }
                else
                {
                    if (field.GetCustomAttribute(attribute) == null) return;
                    comboBox.Items.Add(field.Name);
                }
            }
        }
    }
}
