﻿using SoftGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftOpengl32
{
    partial class StaticCalls
    {
        public static void glSamplerParameteri(uint sampler, uint pname, int param)
        {
            SoftGLRenderContext context = StaticCalls.GetCurrentContextObj();
            if (context != null)
            {
                context.SamplerParameteri(sampler, pname, param);
            }
        }
    }
}
