﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TodoCsharp.common.Responses
{
    public class Response
    {
        public bool isSuccess { get; set; }
        public string Mesages { get; set; }
        public object Result { get; set; }
    }
}
