﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftGL
{
    partial class ShaderProgram
    {
        /// <summary>
        /// name generated by glCreateProgram().
        /// </summary>
        public uint Id { get; private set; }

        /// <summary>
        /// Creates a program object.
        /// </summary>
        /// <param name="id"></param>
        public ShaderProgram(uint id)
        {
            this.Id = id;
        }

        private VertexShader vertexShader;
        private GeometryShader geometryShader;
        private FragmentShader fragmentShader;

        private IList<Shader> attachedShaders = new List<Shader>();
        /// <summary>
        /// 
        /// </summary>
        public IList<Shader> AttachedShaders { get { return this.attachedShaders; } }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("ShaderProgram: Id:{0}", this.Id);
        }

        public int GetAttribLocation(string name)
        {
            int result = -1;
            if (this.logInfo.Length > 0) { return -1; }
            VertexShader vs = this.vertexShader;
            if (vs != null)
            {
                result = vs.GetAttribLocation(name);
            }

            return result;
        }
    }
}
