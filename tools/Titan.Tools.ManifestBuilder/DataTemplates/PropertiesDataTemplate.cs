using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Titan.Tools.ManifestBuilder.DataTemplates.Attributes;
using Titan.Tools.ManifestBuilder.DataTemplates.Descriptors;
using Titan.Tools.ManifestBuilder.ViewModels.Manifests;

namespace Titan.Tools.ManifestBuilder.DataTemplates;

public class PropertiesDataTemplate : IDataTemplate
{
    public IControl Build(object? param)
    {
        if (param == null)
        {
            return new TextBlock
            {
                Text = "Param was null.. This is not what we expected.",
                Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0))
            };
        }

        var type = param.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var descriptors = CreatePropertyDescriptors(type, properties)
            .OrderByDescending(d => d.Order);

        const int PropertiesSpacing = 20;
        var separatorBrush = SolidColorBrush.Parse("#333333");
        var stackPanel = new StackPanel { Spacing = PropertiesSpacing };
        foreach (var descriptor in descriptors)
        {
            stackPanel.Children.Add(descriptor.Build());
            stackPanel.Children.Add(new Separator { Height = 1, Background = separatorBrush });
        }
        return stackPanel;
    }

    private static IEnumerable<EditorPropertyDescriptor> CreatePropertyDescriptors(object obj, PropertyInfo[] properties)
    {
        //NOTE(Jens): Need to add validation on the properties.
        foreach (var propertyInfo in properties)
        {
            var attr = Attribute.GetCustomAttribute(propertyInfo, typeof(EditorNodeAttribute));

            // Ignore any attribute that doesn't have a EditorNodeAttribute
            if (attr == null)
            {
                continue;
            }
            var orderAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(OrderAttribute)) as OrderAttribute;
            var nameAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(DisplayNameAttribute)) as DisplayNameAttribute;
            var accessor = () => propertyInfo.GetMethod?.Invoke(obj, null);
            var propertyName = propertyInfo.Name;
            var name = nameAttribute?.DisplayName ?? propertyName;
            var order = orderAttribute?.Order ?? 0;

            switch (attr)
            {
                case EditorStringAttribute strAttr:
                    yield return new StringEditorDescriptor
                    {
                        Name = propertyName,
                        PropertyName = propertyName,
                        Accessor = accessor, //NOTE(Jens): Add check for missing getter?
                        Order = order,
                        Watermark = strAttr.Watermark
                    };
                    break;
                case EditorReadOnlyAttribute:
                    yield return new ReadOnlyEditorDescriptor
                    {
                        Name = name,
                        PropertyName = propertyName,
                        Accessor = accessor,
                        Order = order
                    };
                    break;
                case EditorNumberAttribute numAttr:
                    yield return new NumberEditorDescriptor
                    {
                        Name = name,
                        PropertyName = propertyName,
                        Accessor = accessor,
                        Order = order,
                        Min = numAttr.Min,
                        Max = numAttr.Max
                    };
                    break;
                case EditorEnumAttribute:

                    if (!propertyInfo.PropertyType.IsEnum)
                    {
                        throw new InvalidOperationException($"The {nameof(EditorEnumAttribute)} can only be used on enum properties.");
                    }
                    yield return new EnumEditorDescriptor
                    {
                        Name = name,
                        PropertyName = propertyName,
                        Accessor = accessor,
                        Order = order,
                        Values = Enum.GetValues(propertyInfo.PropertyType),
                        TypeName = propertyInfo.PropertyType.Name
                    };
                    break;
            }

        }
    }

    public bool Match(object? data)
        => data?.GetType().IsAssignableTo(typeof(IManifestTreeNode)) ?? false;
}
