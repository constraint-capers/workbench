﻿using System;
using System.Collections.Generic;

namespace DynaApp.Models
{
    /// <summary>
    /// The solution model.
    /// </summary>
    [Serializable]
    public class SolutionModel
    {
        public List<ValueModel> Values { get; set; }

        public SolutionModel()
        {
            this.Values = new List<ValueModel>();
        }

        public void AddValue(ValueModel theValue)
        {
            this.Values.Add(theValue);
        }
    }
}