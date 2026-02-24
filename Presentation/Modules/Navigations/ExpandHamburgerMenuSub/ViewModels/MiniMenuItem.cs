using Prism.Unity;
using Prism;
using System;
using System.Collections.Generic;
using System.Windows;
using Prism.Mvvm;

namespace Aksl.Modules.ExpandHamburgerMenuSub
{
    public class MiniMenuItem : BindableBase
    {
        #region Constructors
        public MiniMenuItem()
        {
        }
        #endregion

        #region Properties
        public string Name { get; set; }

        public string ViewName { get; set; }

        private Type _viewElementType = default;
        public Type ViewElementType
        {
            get
            {
                if (_viewElementType is null)
                {
                    string viewTypeAssemblyQualifiedName = ViewName;
                    _viewElementType = Type.GetType(viewTypeAssemblyQualifiedName);
                }

                return _viewElementType;
            }
        }

        private DependencyObject _viewElement = default;
        public DependencyObject ViewElement
        {
            get
            {
                if (_viewElement is null)
                {
                    if (ViewElementType is not null)
                    {
                        // viewElemen = Activator.CreateInstance(viewType) as DependencyObject;
                        _viewElement = (PrismApplication.Current as PrismApplicationBase).Container.Resolve(ViewElementType) as DependencyObject;
                    }
                }

                return _viewElement;
            }
            set
            {
                _viewElement = value;
            }
        }
        #endregion
    }
}
