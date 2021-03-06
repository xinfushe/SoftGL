﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace SoftGL
{
    partial class SoftGLRenderContext
    {

        /// <summary>
        /// RenderContextHandle -> Render Context Object.
        /// </summary>
        internal static readonly Dictionary<IntPtr, SoftGLRenderContext> handleContextDict = new Dictionary<IntPtr, SoftGLRenderContext>();
        /// <summary>
        /// Thread -> Binding Render Context Object.
        /// </summary>
        internal static readonly Dictionary<Thread, SoftGLRenderContext> threadContextDict = new Dictionary<Thread, SoftGLRenderContext>();

        /// <summary>
        /// creates render context.
        /// </summary>
        /// <param name="deviceContext"></param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="paramNames">parameters' names.</param>
        /// <param name="paramValues">parameters' values.</param>
        public SoftGLRenderContext(IntPtr deviceContext, int width, int height, string[] paramNames, uint[] paramValues)
        {
            {
                if (paramNames != null)
                {
                    if (paramValues == null || paramNames.Length != paramValues.Length)
                    { throw new ArgumentException("Names no matching with values!"); }
                }
                else if (paramValues != null)
                { throw new ArgumentException("Names no matching with values!"); }
                else // both are null.
                {
                    paramNames = new string[0];
                    paramValues = new uint[0];
                }

                this.DeviceContextHandle = deviceContext;
                this.Width = width;
                this.Height = height;
                this.ParamNames = paramNames;
                this.ParamValues = paramValues;
            }
            {
                GCHandle handle = GCHandle.Alloc(this, GCHandleType.WeakTrackResurrection);
                this.RenderContextHandle = GCHandle.ToIntPtr(handle);
                handle.Free();
                handleContextDict.Add(this.RenderContextHandle, this);
                //allRenderContexts.Add(this);
            }

            ContextManager.MakeCurrent(this.DeviceContextHandle, this.RenderContextHandle);

            // Create the default framebuffer object and make it the current one.
            {
                var ids = new uint[1];
                glGenFramebuffers(ids.Length, ids);
                if (ids[0] != 0) { throw new Exception("This framebuffer must be 0!"); }
                glBindFramebuffer((uint)BindFramebufferTarget.Framebuffer, ids[0]); // bind the default framebuffer object.
                this.defaultFramebuffer = this.nameFramebufferDict[0];
                InitDefaultFramebuffer(width, height);
            }

            InitBufferDict();
        }

        /// <summary>
        /// Makes the render context current.
        /// </summary>
        public void MakeCurrent()
        {
            if (this.RenderContextHandle != IntPtr.Zero)
            {
                //Win32.wglMakeCurrent(this.DeviceContextHandle, this.RenderContextHandle);
                ContextManager.MakeCurrent(this.DeviceContextHandle, this.RenderContextHandle);
            }
        }

    }
}