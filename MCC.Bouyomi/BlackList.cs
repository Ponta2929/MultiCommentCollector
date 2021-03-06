﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC.Bouyomi
{
    public class BlackList : ObservableCollection<BlackListItem>
    {
        #region Singleton

        private static BlackList instance;
        public static BlackList GetInstance() => instance ?? (instance = new());
        public static void SetInstance(BlackList inst) => instance = inst;

        #endregion
    }
}
