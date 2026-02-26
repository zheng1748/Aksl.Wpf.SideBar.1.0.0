using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aksl.Toolkit.UI
{
    public static class ListViewItemExtensions
    {
        #region Properties

        public static int GetIndexUnderCursor(this System.Windows.Controls.ListView listView)
        {
            int index = -1;
            for (int i = 0; i < listView.Items.Count; ++i)
            {
                ListViewItem item = listView.GetListViewItemByIndex(i);
                if (IsMouseOver(item))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
      
        public static ListViewItem GetListViewItemByIndex(this System.Windows.Controls.ListView listView, int index)
        {
            if (listView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                return null;
            }

            return listView.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        }

        public static bool IsMouseOver(Visual target)
        {
            // We need to use MouseUtilities to figure out the cursor
            // coordinates because, during a drag-drop operation, the WPF
            // mechanisms for getting the coordinates behave strangely.

            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            //Debug.WriteLine(string.Format("Left:{0}--Top:{1}--Width：{2}--Height：{3}", bounds.Left.ToString(), bounds.Top.ToString(), bounds.Width.ToString(), bounds.Height.ToString()));
            Point mousePos = MouseUtilities.GetMousePosition(target);
            //Debug.WriteLine(string.Format("X:{0}--Y:{1}", mousePos.X.ToString(), mousePos.Y.ToString()));
            return bounds.Contains(mousePos);
        }
        #endregion
    }
}
