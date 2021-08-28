﻿using MCC.Core.Manager;
using MCC.Utility;
using MCC.Utility.IO;
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
    public partial class UserSettingWindow : MahApps.Metro.Controls.MetroWindow
    {
        public UserSettingWindow()
        {
            InitializeComponent();
        }

        public void CreateViewModel(UserData user)
        {
            this.DataContext = new UserSettingWindowViewModel (user);
        }
    }
}