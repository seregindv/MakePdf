using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MakePdf.Properties;

namespace MakePdf.ViewModels
{
    public class GenericTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var fwe = container as FrameworkElement;
            if (item == null || fwe == null)
                return null;
            return fwe.TryFindResource(item.GetType().Name) as DataTemplate;
        }
    }
}
