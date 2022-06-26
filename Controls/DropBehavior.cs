using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using System.Windows.Media.Animation;

namespace MakePdf.Controls
{
    public class DropBehavior : Behavior<Control>
    {
        private bool _prevAllowDrop;

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(DropBehavior), new PropertyMetadata(default(ICommand)));

        public DropBehavior()
        {
            Formats = new List<string>();
        }

        public List<string> Formats { set; get; }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        protected override void OnAttached()
        {
            _prevAllowDrop = AssociatedObject.AllowDrop;
            AssociatedObject.AllowDrop = true;
            AssociatedObject.DragOver += AssociatedObject_DragOver;
            AssociatedObject.Drop += AssociatedObject_Drop;
        }

        void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            //Debug.WriteLine(e.Data.GetFormats().Aggregate(new StringBuilder(), (sb, s) => sb.AppendLine(s), sb => sb.ToString()));
            e.Effects = GetKosherFormats(e).Any() ? DragDropEffects.Link : DragDropEffects.None;
            e.Handled = true;
        }

        private IEnumerable<string> GetKosherFormats(DragEventArgs e)
        {
            EnsureFormats();
            var draggedFormats = e.Data.GetFormats();
            return draggedFormats.Join(Formats, s => s, s => s, (s1, s2) => s1);
        }

        void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            var formats = GetKosherFormats(e).ToArray();
            var result = Formats.Count == 1
                ? e.Data.GetData(formats[0])
                : formats
                    .Select(format => new object[] { format, e.Data.GetData(format) })
                    .ToArray();
            if (Command != null)
                Command.Execute(result);
        }

        void EnsureFormats()
        {
            if (Formats == null || Formats.Count == 0)
                Formats = new List<string> { DataFormats.FileDrop };
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AllowDrop = _prevAllowDrop;
            AssociatedObject.DragOver -= AssociatedObject_DragOver;
            AssociatedObject.Drop -= AssociatedObject_Drop;
        }
    }
}
