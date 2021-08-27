﻿using MCC.Core.Manager;
using MCC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MultiCommentCollector
{
    /// <summary>
    /// UserDataWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class UserDataWindow : MahApps.Metro.Controls.MetroWindow
    {
        public UserDataWindow()
        {
            InitializeComponent();
        }

        public void CreateViewModel(CommentDataEx user)
        {
            this.Title = $"{user.LiveName} - " + (user.UserName is null || user.UserName.Equals("") ? user.UserID : user.UserName);
            this.DataContext = new UserDataWindowViewModel(user);
        }
    }
}
