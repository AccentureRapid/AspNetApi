using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCMS.Common.Data
{
    public class TableAttribute : Attribute
    {
        private readonly string _name;
        public string Name
        {
            get
            {
                return this._name;
            }
        }
        public TableAttribute(string name)
        {
            this._name = name;
        }
    }
}
