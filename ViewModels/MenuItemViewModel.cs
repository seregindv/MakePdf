﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MakePdf.Stuff;

namespace MakePdf.ViewModels
{
    public class MenuItemViewModel : BaseViewModel
    {
        public MenuItemViewModel(DelegateCommand command)
        {
            Command = command;
        }

        public string Title { set; get; }
        public bool IsDefault { set; get; }
        public AddressType? AddressType { set; get; }
        public DelegateCommand Command { set; get; }
    }
}
