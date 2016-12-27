using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCMS.Common.Data
{
    public class ColumnAttribute : Attribute
    {
        private readonly string _name;
        public string Name
        {
            get
            {
                return this._name;
            }
        }
        public ColumnAttribute(string name)
        {
            this._name = name;
        }
    }
}
