using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aksl.Toolkit.UI
{
    public class VisualTreeFinder
    {
        #region Find Visual Child Method
        public T FindVisualChild<T>(DependencyObject currenyObject) where T : DependencyObject
        {
            T child = default;

            try
            {
                if (HasChild(currenyObject))
                {
                    RecursiveVisualChild(currenyObject);
                }

                void RecursiveVisualChild(DependencyObject parent)
                {
                    if (parent is not null && parent is T t)
                    {
                        child = t;

                        return;
                    }

                    if (HasChild(parent))
                    {
                        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                        {
                            DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                            RecursiveVisualChild(child);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            bool HasChild(DependencyObject dep) => dep is not null && VisualTreeHelper.GetChildrenCount(dep) > 0;

            return child;
        }

        public List<T> FindVisualChilds<T>(DependencyObject currenyObject) where T : DependencyObject
        {
            List<T> allChilds = new();

            try
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(currenyObject); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(currenyObject, i);

                    if (HasChild(child))
                    {
                        RecursiveVisualChild(child);
                    }
                }

                void RecursiveVisualChild(DependencyObject parent)
                {
                    if (!allChilds.Contains(parent) && parent is not null && parent is T t)
                    {
                        allChilds.Add(t);
                    }

                    if (HasChild(parent))
                    {
                        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                        {
                            DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                            RecursiveVisualChild(child);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            bool HasChild(DependencyObject dep) => dep is not null && VisualTreeHelper.GetChildrenCount(dep) > 0;

            return allChilds;
        }
        #endregion

        #region Find Logical Child Method
        public T FindLogicalChild<T>(DependencyObject currenyObject) where T : DependencyObject
        {
            T child = default;

            try
            {
                RecursiveVisualChild(currenyObject);

                void RecursiveVisualChild(DependencyObject parent)
                {
                    if (parent is not null && parent is T t)
                    {
                        child = t;

                        return;
                    }

                    foreach (var child in LogicalTreeHelper.GetChildren(parent))
                    {
                        if (child is DependencyObject dep)
                        {
                            RecursiveVisualChild(dep);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return child;
        }

        public List<T> FindLogicalChilds<T>(DependencyObject currenyObject) where T : DependencyObject
        {
            List<T> allChilds = new();

            try
            {
                foreach (DependencyObject child in LogicalTreeHelper.GetChildren(currenyObject))
                {
                    if (child is DependencyObject dep)
                    {
                        RecursiveVisualChild(child);
                    }
                }

                void RecursiveVisualChild(DependencyObject parent)
                {
                    if (!allChilds.Contains(parent) && parent is not null && parent is T tt)
                    {
                        allChilds.Add(tt);
                    }

                    foreach (var child in LogicalTreeHelper.GetChildren(parent))
                    {
                        if (child is DependencyObject dep)
                        {
                            RecursiveVisualChild(dep);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return allChilds;
        }
        #endregion

        #region Find Visual Parent Method
        public T FindVisualParent<T>(DependencyObject currenyObject) where T : FrameworkElement
        {
            T ancestorer = default;

            if (HasParent(currenyObject))
            {
                RecursiveVisualParent(currenyObject);
            }

            void RecursiveVisualParent(DependencyObject child)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(child);
                if (parent is not null && parent is T t)
                {
                    ancestorer = t;

                    return;
                }

                if (HasParent(parent))
                {
                    RecursiveVisualParent(parent);
                }
            }

            bool HasParent(DependencyObject dep) => dep is not null && VisualTreeHelper.GetParent(dep) is not null;

            return ancestorer;
        }

        public List<T> FindVisualParents<T>(DependencyObject currenyObject) where T : DependencyObject
        {
            List<T> allParents = new();

            try
            {
                if (HasParent(currenyObject))
                {
                    RecursiveVisualParent(currenyObject);
                }

                void RecursiveVisualParent(DependencyObject child)
                {
                    DependencyObject parent = VisualTreeHelper.GetParent(child);
                    if (!allParents.Contains(parent) && parent is not null && parent is T t)
                    {
                        allParents.Add(t);
                    }

                    if (HasParent(parent))
                    {
                        RecursiveVisualParent(parent);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            bool HasParent(DependencyObject dep) => dep is not null && VisualTreeHelper.GetParent(dep) is not null;

            return allParents;
        }
        #endregion

        #region Find Logical Parent Method
        public T FindLogicalParent<T>(DependencyObject currenyObject) where T : DependencyObject
        {
            T ancestorer = default;

            if (HasParent(currenyObject))
            {
                RecursiveVisualParent(currenyObject);
            }

            void RecursiveVisualParent(DependencyObject child)
            {
                DependencyObject parent = LogicalTreeHelper.GetParent(child);
                if (parent is not null && parent is T t)
                {
                    ancestorer = t;

                    return;
                }

                if (HasParent(parent))
                {
                    RecursiveVisualParent(parent);
                }
            }

            bool HasParent(DependencyObject dep) => dep is not null && LogicalTreeHelper.GetParent(dep) is not null;

            return ancestorer;
        }

        public List<T> FindLogicalParents<T>(DependencyObject currenyObject) where T : DependencyObject
        {
            List<T> allParents = new();

            try
            {
                if (HasParent(currenyObject))
                {
                    RecursiveVisualParent(currenyObject);
                }

                void RecursiveVisualParent(DependencyObject child)
                {
                    DependencyObject parent = LogicalTreeHelper.GetParent(child);
                    if (!allParents.Contains(parent) && parent is not null && parent is T t)
                    {
                        allParents.Add(t);
                    }

                    if (HasParent(parent))
                    {
                        RecursiveVisualParent(parent);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            bool HasParent(DependencyObject dep) => dep is not null && LogicalTreeHelper.GetParent(dep) is not null;

            return allParents;
        }
        #endregion
    }
}
