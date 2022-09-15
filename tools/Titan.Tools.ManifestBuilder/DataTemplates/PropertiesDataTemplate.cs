using System.ComponentModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Titan.Tools.ManifestBuilder.DataTemplates.Attributes;
using Titan.Tools.ManifestBuilder.DataTemplates.Descriptors;

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
            //stackPanel.Children.Add(new Separator { Height = 1, Background = separatorBrush });
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
            var descriptionAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;
            var nameAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(DisplayNameAttribute)) as DisplayNameAttribute;
            var accessor = () => propertyInfo.GetMethod?.Invoke(obj, null);
            var propertyName = propertyInfo.Name;
            var name = nameAttribute?.DisplayName ?? ToNameWithWhiteSpace(propertyName);
            var order = orderAttribute?.Order ?? 0;
            var description = descriptionAttribute?.Description;

            switch (attr)
            {
                case EditorStringAttribute strAttr:
                    yield return new StringEditorDescriptor
                    {
                        Name = name,
                        PropertyName = propertyName,
                        Accessor = accessor, //NOTE(Jens): Add check for missing getter?
                        Order = order,
                        Watermark = strAttr.Watermark,
                        Description = description
                    };
                    break;
                case EditorReadOnlyAttribute:
                    yield return new ReadOnlyEditorDescriptor
                    {
                        Name = name,
                        PropertyName = propertyName,
                        Accessor = accessor,
                        Order = order,
                        Description = description
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
                        Max = numAttr.Max,
                        Description = description
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
                        TypeName = propertyInfo.PropertyType.Name,
                        Description = description
                    };
                    break;
                case EditorFileAttribute file:
                    yield return new BrowseFileEditorDescriptor
                    {
                        Name = name,
                        PropertyName = propertyName,
                        Accessor = accessor,
                        Order = order,
                        Watermark = file.Watermark,
                        Description = description
                    };
                    break;
            }

        }

        static string ToNameWithWhiteSpace(string name)
        {
            Span<char> buffer = stackalloc char[name.Length * 2];
            var count = 0;
            foreach (var character in name)
            {
                if (char.IsUpper(character) && count > 0)
                {
                    buffer[count++] = ' ';
                }
                buffer[count++] = character;
            }
            return new string(buffer[..count]);
        }
    }

    public bool Match(object? data)
        => data?.GetType().IsAssignableTo(typeof(IPropertyEditor)) ?? false;
}
