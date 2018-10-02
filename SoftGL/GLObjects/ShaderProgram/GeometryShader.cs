﻿using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftGL
{
    class GeometryShader : Shader
    {
        public GeometryShader(uint id) : base(ShaderType.GeometryShader, id) { }

        protected override string AfterCompile(Assembly assembly)
        {
            throw new NotImplementedException();
        }
    }
}
