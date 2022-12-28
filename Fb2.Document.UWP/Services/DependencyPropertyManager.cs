using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml;

namespace Fb2.Document.UWP.Services
{
    public class DependencyPropertyManager
    {
        private Dictionary<string, DependencyProperty> RegisteredProperties = new Dictionary<string, DependencyProperty>();

        public void AddOrUpdateProperty(DependencyObject element, string propertyName, string value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            DependencyProperty dProperty = null;

            if (RegisteredProperties.ContainsKey(propertyName))
            {
                dProperty = RegisteredProperties[propertyName];

                var existingTag = GetProperty(element, propertyName);

                if (!string.IsNullOrWhiteSpace(existingTag))
                    value = $"{existingTag}|{value}";
            }
            else
            {
                dProperty = DependencyProperty.RegisterAttached(propertyName,
                    typeof(string),
                    typeof(TextElement),
                    new PropertyMetadata(null));

                RegisteredProperties.Add(propertyName, dProperty);
            }

            element.SetValue(dProperty, value);
        }

        public string GetProperty(DependencyObject element, string propertyName)
        {
            if (element == null)
                throw new ArgumentNullException($"{nameof(element)} is null");

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException($"{nameof(propertyName)} is null or empty string");

            if (!RegisteredProperties.ContainsKey(propertyName))
                return null;

            var dProperty = RegisteredProperties[propertyName];

            return (string)element.GetValue(dProperty);
        }
    }
}
