using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using MakePdf.Configuration;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace MakePdf.ViewModels
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            IUnityContainer container = new UnityContainer();
            container.RegisterInstance(container);
            //container.RegisterType<IConfig, Config>(new ContainerControlledLifetimeManager());
            container.RegisterInstance(Config.Instance);
            container.RegisterType<MainWindowViewModel, MainWindowViewModel>(new ContainerControlledLifetimeManager());
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            DataContext = container.Resolve<MainWindowViewModel>();

            //Content.AddHandler(DragEnterEvent, new DragEventHandler(Content_DragEnter), true);
        }

        private void Content_DragEnter(object sender, DragEventArgs e)
        {
            Debug.WriteLine("drag enter");
        }

        private void Content_PreviewDragEnter(object sender, DragEventArgs e)
        {
            Debug.WriteLine("preview drag enter");
            e.Effects = DragDropEffects.All;
        }

        private void Content_DragOver(object sender, DragEventArgs e)
        {
            Debug.WriteLine("drag over");
        }

        private void Content_PreviewDragOver(object sender, DragEventArgs e)
        {
            Debug.WriteLine("preview drag over");
            e.Effects = DragDropEffects.All;
        }

        private void Content_Drop(object sender, DragEventArgs e)
        {
            Debug.WriteLine("drop");
        }
    }
}
