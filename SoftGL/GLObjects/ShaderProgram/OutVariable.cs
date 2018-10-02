﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftGL
{

    class OutVariable
    {
        public uint location = uint.MaxValue;
        public readonly PropertyInfo propertyInfo;

        public OutVariable(PropertyInfo field)
        {
            this.propertyInfo = field;
        }

        public override string ToString()
        {
            return string.Format("P:{0}, loc:{1}", this.propertyInfo, this.location);
        }
    }
}
