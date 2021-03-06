﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoftGL
{
    partial class ShaderProgram
    {
        private string logInfo = string.Empty;

        /// <summary>
        /// Link error exists if LogInfo is not String.Empty.
        /// </summary>
        public string LogInfo { get { return logInfo; } }

        internal readonly Dictionary<string, UniformValue> nameUniformDict = new Dictionary<string, UniformValue>();
        internal readonly Dictionary<int, UniformValue> locationUniformDict = new Dictionary<int, UniformValue>();
        ///// <summary>
        ///// Storage
        ///// </summary>
        //private byte[] uniformBytes;

        /// <summary>
        /// from Shader to exe.
        /// </summary>
        public void Link()
        {
            if (!FindTypedShaders()) { return; }
            if (!FindUniforms(this.nameUniformDict, this.locationUniformDict)) { return; }
            if (!MakeSureVariablesMatch()) { return; }
            // TODO: do something else.
        }

        /// <summary>
        /// ex. Make sure vs.out and fs.in match.
        /// </summary>
        /// <returns></returns>
        private bool MakeSureVariablesMatch()
        {
            if (this.VertexShader != null && this.FragmentShader != null)
            {
                Dictionary<string, OutVariable> outDict = this.VertexShader.outVariableDict;
                Dictionary<string, InVariable> inDict = this.FragmentShader.inVariableDict;
                if (outDict.Count != inDict.Count) { this.logInfo = string.Format("Variables number ({0} and {1}) not match!", outDict.Count, inDict.Count); return false; }
                foreach (var outItem in outDict)
                {
                    InVariable inVar = null;
                    if (inDict.TryGetValue(outItem.Key, out inVar))
                    {
                        if (inVar.fieldInfo.FieldType != outItem.Value.fieldInfo.FieldType)
                        {
                            this.logInfo = string.Format("Variable [{0}] not the same type!", outItem.Key);
                            return false;
                        }
                    }
                    else
                    {
                        this.logInfo = string.Format("No variable matches [{0}] in {1}", outItem.Key, this.VertexShader.ShaderType);
                        return false;
                    }
                }
                var list = new List<InVariable>();
                foreach (var outItem in outDict)
                {
                    var inVar = inDict[outItem.Key];
                    list.Add(inVar);
                }
                inDict.Clear();
                foreach (var item in list)
                {
                    inDict.Add(item.fieldInfo.Name, item);
                }
            }

            return true;
        }

        private bool FindUniforms(Dictionary<string, UniformValue> nameUniformDict, Dictionary<int, UniformValue> locationUniformDict)
        {
            nameUniformDict.Clear(); locationUniformDict.Clear();
            int nextLoc = 0;
            foreach (var shader in this.attachedShaders)
            {
                foreach (var item in shader.UniformVariableDict)
                {
                    string varName = item.Key;
                    UniformVariable v = item.Value;
                    if (nameUniformDict.ContainsKey(varName))
                    {
                        if (v.fieldInfo.FieldType != nameUniformDict[varName].variable.fieldInfo.FieldType)
                        {
                            this.logInfo = string.Format("Different uniform variable types of the same name[{0}!]", varName);
                            return false;
                        }
                    }
                    else
                    {
                        v.location = nextLoc;
                        int byteSize = this.GetByteSize(v.fieldInfo.FieldType);
                        nextLoc += byteSize;
                        var value = new UniformValue(v, null);
                        nameUniformDict.Add(varName, value);
                        locationUniformDict.Add(v.location, value);
                    }
                }
            }

            return true;
        }

        private int GetByteSize(Type type)
        {
            int size = 0;
            if (type == typeof(float)) { size = sizeof(float); }
            if (type == typeof(vec2)) { size = sizeof(float) * 2; }
            if (type == typeof(vec3)) { size = sizeof(float) * 3; }
            if (type == typeof(vec4)) { size = sizeof(float) * 4; }
            else if (type == typeof(int)) { size = sizeof(int); }
            else if (type == typeof(ivec2)) { size = sizeof(int) * 2; }
            else if (type == typeof(ivec3)) { size = sizeof(int) * 3; }
            else if (type == typeof(ivec4)) { size = sizeof(int) * 4; }
            else if (type == typeof(uint)) { size = sizeof(uint); }
            else if (type == typeof(uvec2)) { size = sizeof(uint) * 2; }
            else if (type == typeof(uvec3)) { size = sizeof(uint) * 3; }
            else if (type == typeof(uvec4)) { size = sizeof(uint) * 4; }
            else if (type == typeof(double)) { size = sizeof(double); }
            else if (type == typeof(dvec2)) { size = sizeof(double) * 2; }
            else if (type == typeof(dvec3)) { size = sizeof(double) * 3; }
            else if (type == typeof(dvec4)) { size = sizeof(double) * 4; }
            else if (type == typeof(mat2)) { size = sizeof(float) * 4; }
            else if (type == typeof(mat3)) { size = sizeof(float) * 9; }
            else if (type == typeof(mat4)) { size = sizeof(float) * 16; }
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                size = GetByteSize(elementType);
            }
            else
            {
                throw new ArgumentException(string.Format("Type[{0}] is not supported in uniform variable!", type));
            }

            return size;
        }

        /// <summary>
        /// find the vertex shader and other shaders.
        /// </summary>
        /// <returns></returns>
        private bool FindTypedShaders()
        {
            bool result = true;

            VertexShader vertexShader = null;
            TessControlShader tessControlShader = null;
            TessEvaluationShader tessEvaluationShader = null;
            GeometryShader geometryShader = null;
            FragmentShader fragmentShader = null;
            ComputeShader computeShader = null;
            foreach (var item in this.attachedShaders)
            {
                if (item.InfoLog != string.Empty) { this.logInfo = "Shader Compiling Error!"; result = false; break; }

                switch (item.ShaderType)
                {
                    case ShaderType.VertexShader:
                        if (vertexShader != null) { this.logInfo = "Multiple VertexShader!"; result = false; break; }
                        else { vertexShader = item as VertexShader; }
                        break;
                    case ShaderType.TessControlShader:
                        if (tessControlShader != null) { this.logInfo = "Multiple TessControlShader!"; result = false; break; }
                        else { tessControlShader = item as TessControlShader; }
                        break;
                    case ShaderType.TessEvaluationShader:
                        if (tessEvaluationShader != null) { this.logInfo = "Multiple TessEvaluationShader!"; result = false; break; }
                        else { tessEvaluationShader = item as TessEvaluationShader; }
                        break;
                    case ShaderType.GeometryShader:
                        if (geometryShader != null) { this.logInfo = "Multiple GeometryShader!"; result = false; break; }
                        else { geometryShader = item as GeometryShader; }
                        break;
                    case ShaderType.FragmentShader:
                        if (fragmentShader != null) { this.logInfo = "Multiple FragmentShader!"; result = false; break; }
                        else { fragmentShader = item as FragmentShader; }
                        break;
                    case ShaderType.ComputeShader:
                        if (computeShader != null) { this.logInfo = "Multiple ComputeShader!"; result = false; break; }
                        else { computeShader = item as ComputeShader; }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (vertexShader == null) { this.logInfo = "No VertexShader found!"; result = false; return result; }

            {
                // TODO: support other shaders.
                this.VertexShader = vertexShader;
                //this.tessControlShader = tessControlShader;
                //this.tessEvaluationShader = tessEvaluationShader;
                //this.geometryShader = geometryShader;
                this.FragmentShader = fragmentShader;
                //this.computeShader = computeShader;
            }

            return result;
        }
    }

    class UniformValue
    {
        public readonly UniformVariable variable;
        public Object value;

        public UniformValue(UniformVariable variable, Object value = null)
        {
            this.variable = variable;
            this.value = value;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}: [{2}]", this.variable.fieldInfo.FieldType, this.variable.fieldInfo.Name, this.value);
        }
    }
}

