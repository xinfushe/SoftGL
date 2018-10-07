﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftGL
{
    public abstract partial class VertexCodeBase : CodeBase
    {
        public int gl_VertexID;

        [Out]
        public vec4 gl_Position;

        public abstract void main();
    }
}
