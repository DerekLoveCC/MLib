﻿namespace TreeViewDemo
{
    using Settings.UserProfile;
    using ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MWindowLib.SimpleMetroWindow
                                     , IViewSize  // Implements saving and loading/repositioning of Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            var viewModel = this.DataContext as AppViewModel;
            viewModel.Demo.InitRootCommand.Execute(null);
        }
    }
}
