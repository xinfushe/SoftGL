﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SoftGL
{
    static class TypeHelper
    {
        public static uint ByteSize(this Type type)
        {
            uint result = 0;
            if (!type.IsValueType) { throw new ArgumentException(string.Format("type[{0}] should be a value type!", type)); }

            if (type == typeof(float)) { result = sizeof(float); }
            else if (type == typeof(int)) { result = sizeof(int); }
            else if (type == typeof(uint)) { result = sizeof(uint); }
            else if (type == typeof(vec2)) { result = sizeof(float) * 2; }
            else if (type == typeof(vec3)) { result = sizeof(float) * 3; }
            else if (type == typeof(vec4)) { result = sizeof(float) * 4; }
            else if (type == typeof(ivec2)) { result = sizeof(int) * 2; }
            else if (type == typeof(ivec3)) { result = sizeof(int) * 3; }
            else if (type == typeof(ivec4)) { result = sizeof(int) * 4; }
            else if (type == typeof(uvec2)) { result = sizeof(uint) * 2; }
            else if (type == typeof(uvec3)) { result = sizeof(uint) * 3; }
            else if (type == typeof(uvec4)) { result = sizeof(uint) * 4; }
            else if (type == typeof(dvec2)) { result = sizeof(double) * 2; }
            else if (type == typeof(dvec3)) { result = sizeof(double) * 3; }
            else if (type == typeof(dvec4)) { result = sizeof(double) * 4; }
            else if (type == typeof(mat2)) { result = sizeof(float) * 4; }
            else if (type == typeof(mat3)) { result = sizeof(float) * 9; }
            else if (type == typeof(mat4)) { result = sizeof(float) * 16; }
            else { result = (uint)Marshal.SizeOf(type); }

            return result;
        }
    }
}
