namespace YouthUnion
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Data;

    internal class DataGridHelper
    {
        private static readonly string[] _combBoxColumns =
        {
            "Ответственный",
            "Мероприятие",
            "Волонтёр"
        };

        public static ArrayList ConvertQueryToList(IQueryable query)
        {
            var results = new ArrayList();
            results.AddRange(query.Cast<object>().ToList());
            return results;
        }

        public static DataGridColumn GetNewComboBoxColumn(string header,
            BindingBase bindingBase,
            object itemsSource)
        {
            var comboBoxColumn = new DataGridComboBoxColumn
            {
                Header = header,
                SelectedValueBinding = bindingBase
            };

            var itemsSourceBinding = new Binding
            {
                Source = itemsSource
            };
            BindingOperations.SetBinding(comboBoxColumn, DataGridComboBoxColumn.ItemsSourceProperty,
                itemsSourceBinding);

            return comboBoxColumn;
        }

        public static string? GetPropertyDisplayName(object descriptor, out Type type)
        {
            type = null;
            if (descriptor is PropertyDescriptor pd)
            {
                type = pd.PropertyType;

                // Check for DisplayName attribute and set the column header accordingly
                var displayName = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;
                return IsNullOrDefault(displayName) ? string.Empty : displayName?.DisplayName;
            }

            var pi = descriptor as PropertyInfo;

            if (pi == null)
                return string.Empty;

            // Check for DisplayName attribute and set the column header accordingly
            var attributes = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            foreach (var displayName in attributes.Select(t => t as DisplayNameAttribute)
                         .TakeWhile(displayName => !IsNullOrDefault(displayName)))
            {
                return displayName?.DisplayName;
            }

            return string.Empty;

            bool IsNullOrDefault(DisplayNameAttribute? displayName)
            {
                return displayName == null || displayName == DisplayNameAttribute.Default;
            }
        }

        public static void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs eventArgs)
        {
            var binding = ((DataGridBoundColumn)eventArgs.Column).Binding;
            var displayName = GetPropertyDisplayName(eventArgs.PropertyDescriptor, out var type);
            if (string.IsNullOrEmpty(displayName))
            {
                eventArgs.Cancel = true;
                return;
            }

            if (_combBoxColumns.Contains(displayName))
            {
                var queryable = ContextProvider.Set(type);
                eventArgs.Column = GetNewComboBoxColumn(displayName, binding, ConvertQueryToList(queryable));
            }

            eventArgs.Column.Header = displayName;
        }
    }
}