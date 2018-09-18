﻿using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftGL
{
    class VertexShader : Shader
    {
        private Dictionary<string, InVariable> inVariableDict = new Dictionary<string, InVariable>();

        public InVariable[] GetAllInVariables()
        {
            return this.inVariableDict.Values.ToArray();
        }

        public VertexShader(uint id) : base(ShaderType.VertexShader, id) { }

        public int GetAttribLocation(string name)
        {
            int result = -1;
            if (this.InfoLog.Length > 0) { return result; }
            Dictionary<string, InVariable> dict = this.inVariableDict;
            if (dict == null) { return result; }
            InVariable v = null;
            if (dict.TryGetValue(name, out v))
            {
                result = (int)v.location;
            }

            return result;
        }

        protected override string AfterCompile()
        {
            {
                string result = FindInVariables(this.codeType, this.inVariableDict);
                if (result != string.Empty) { return result; }
            }

            return string.Empty;
        }

        private string FindInVariables(Type vsType, Dictionary<string, InVariable> dict)
        {
            dict.Clear();
            uint nextLoc = 0;
            foreach (var item in vsType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object[] inAttribute = item.GetCustomAttributes(typeof(InAttribute), false);
                if (inAttribute != null && inAttribute.Length > 0) // this is a 'in ...;' field.
                {
                    var v = new InVariable(item);
                    object[] locationAttribute = item.GetCustomAttributes(typeof(LocationAttribute), false);
                    if (locationAttribute != null && locationAttribute.Length > 0) // (location = ..) in ...;
                    {
                        uint loc = (locationAttribute[0] as LocationAttribute).location;
                        if (loc < nextLoc) { return "location error in VertexShader!"; }
                        v.location = loc;
                        nextLoc = loc + 1;
                    }
                    else
                    {
                        v.location = nextLoc++;
                    }
                    dict.Add(item.Name, v);
                }
            }

            return string.Empty;
        }


    }
}
