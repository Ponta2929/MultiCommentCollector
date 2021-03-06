﻿using MCC.Core;
using MCC.Core.Manager;
using System.Windows;

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
            CommentListView.ItemsSource = CommentManager.GetInstance();
            ConnectionListView.ItemsSource = ConnectionManager.GetInstance();
        }
    }
}
