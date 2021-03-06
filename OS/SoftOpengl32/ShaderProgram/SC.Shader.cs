﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SoftGL;

namespace SoftOpengl32
{
    public partial class StaticCalls
    {
        /// <summary>
        /// Creates a shader object.
        /// </summary>
        /// <param name="shaderType">Specifies the type of shader to be created. Must be one of GL_VERTEX_SHADER, GL_TESS_CONTROL_SHADER, GL_TESS_EVALUATION_SHADER, GL_GEOMETRY_SHADER, GL_FRAGMENT_SHADER, or GL_COMPUTE_SHADER.</param>
        /// <returns></returns>
        public static uint glCreateShader(uint shaderType)
        {
            return SoftGLRenderContext.glCreateShader(shaderType);
        }

        /// <summary>
        /// Replaces the source code in a shader object.
        /// </summary>
        /// <param name="name">Specifies the name of the shader object whose source code is to be replaced.</param>
        /// <param name="count">Specifies the number of elements in the string​ and length​ arrays.</param>
        /// <param name="codes">Specifies an array of pointers to strings containing the source code to be loaded into the shader.</param>
        /// <param name="lengths">Specifies an array of code lengths.</param>
        public static void glShaderSource(uint name, int count, string[] codes, int[] lengths)
        {
            SoftGLRenderContext.glShaderSource(name, count, codes, lengths);
        }

        /// <summary>
        /// Compiles a shader object.
        /// </summary>
        /// <param name="name">Specifies the shader object to be compiled.</param>
        public static void glCompileShader(uint name)
        {
            SoftGLRenderContext.glCompileShader(name);
        }

        /// <summary>
        /// Returns a parameter from a shader object.
        /// </summary>
        /// <param name="name">Specifies the shader object to be queried.</param>
        /// <param name="pname">Specifies the object parameter. Accepted symbolic names are GL_SHADER_TYPE, GL_DELETE_STATUS, GL_COMPILE_STATUS, GL_INFO_LOG_LENGTH, GL_SHADER_SOURCE_LENGTH.</param>
        /// <param name="pValues">Returns the requested object parameter.</param>
        public static void glGetShaderiv(uint name, uint pname, int[] pValues)
        {
            SoftGLRenderContext.glGetShaderiv(name, pname, pValues);
        }
    }
}
