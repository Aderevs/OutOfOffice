using Microsoft.AspNetCore.Mvc.Rendering;
using OutOfOffice.Attributes;
using System.ComponentModel;
using System.Reflection;

namespace OutOfOffice
{
    public class EnumHelper
    {
        public static IEnumerable<SelectListItem> GetSelectListItemsFromEnum<TEnum>() where TEnum : Enum
        {
            var enumType = typeof(TEnum);

            return Enum.GetValues(enumType)
                       .Cast<TEnum>()
                       .Select(value => new SelectListItem
                       {
                           Value = value.ToString(),
                           Text = GetEnumTranslation(value)
                       });
        }
        private static string GetEnumTranslation(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            return field?.GetCustomAttribute<DisplayJsonAttribute>()?.DisplayedJson;
        }
    }
}
