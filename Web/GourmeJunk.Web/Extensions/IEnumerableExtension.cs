using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GourmeJunk.Web.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, string selectedValue)
        {
            foreach (var item in items)
            {
                yield return new SelectListItem
                {
                    Text = GetPropertyValue(item, "Name"),
                    Value = GetPropertyValue(item, "Id"),
                    Selected = GetPropertyValue(item, "Id").Equals(selectedValue)
                };
            }
        }

        private static string GetPropertyValue<T>(T item, string propertyName)
        {
            return item.GetType().GetProperty(propertyName).GetValue(item).ToString();
        }
    }
}
