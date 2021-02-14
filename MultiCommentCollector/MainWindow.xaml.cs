using MCC.Core;
using MCC.Plugin;
using MCC.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MultiCommentCollector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // バインド
            this.CommentListView.ItemsSource = CommentManager.GetInstance().Items;
            this.ConnectionListView.ItemsSource = ConnectionManager.GetInstance().Items;          
        }
    }
}
