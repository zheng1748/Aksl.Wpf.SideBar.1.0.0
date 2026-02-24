using System;
using System.Windows.Markup;

namespace Aksl.Toolkit.Controls
{
    [MarkupExtensionReturnType(typeof(PackIcon))]
    public class PackIconExtension : MarkupExtension
    {
        #region Constructors
        public PackIconExtension()
        { }

        public PackIconExtension(PackIconKind kind)
        {
            Kind = kind;
        }

        public PackIconExtension(PackIconKind kind, double size)
        {
            Kind = kind;
            Size = size;
        }
        #endregion

        #region Properties
        [ConstructorArgument("kind")]
        public PackIconKind Kind { get; set; }

        [ConstructorArgument("size")]
        public double? Size { get; set; }
        #endregion

        #region ProvideValue Method
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            PackIcon result = new() { Kind = Kind };

            if (Size.HasValue)
            {
                result.Height = Size.Value;
                result.Width = Size.Value;
            }

            return result;
        }
        #endregion
    }
}
