﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SoftGL;

namespace SoftGL
{
    /// <summary>
    /// Initialization component calls Operating System. Operating System calls Hardware Driver. Hardware Driver is actually SoftGL.
    /// This is the 'opengl32.dll' in SoftGL environment.
    /// </summary>
    public partial class ContextManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceContext"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="paramNames">parameters' names.</param>
        /// <param name="paramValues">parameters' values.</param>
        /// <returns></returns>
        public static IntPtr CreateContext(IntPtr deviceContext, int width, int height, string[] paramNames, uint[] paramValues)
        {
            var context = new SoftGLRenderContext(deviceContext, width, height, paramNames, paramValues);

            return context.RenderContextHandle;
        }

        /// <summary>
        /// Make specified <paramref name="renderContext"/> the current one of current thread.
        /// </summary>
        /// <param name="deviceContext"></param>
        /// <param name="renderContext"></param>
        public static void MakeCurrent(IntPtr deviceContext, IntPtr renderContext)
        {
            var threadContextDict = SoftGLRenderContext.threadContextDict;
            if (renderContext == IntPtr.Zero) // cancel current render context to current thread.
            {
                SoftGLRenderContext context = null;

                Thread thread = Thread.CurrentThread;
                if (threadContextDict.TryGetValue(thread, out context))
                {
                    threadContextDict.Remove(thread);
                }
                else
                {
                    // TODO: what should I do?
                }
            }
            else // change current render context to current thread.
            {
                SoftGLRenderContext context = GetContextObj(renderContext);
                if (context != null)
                {
                    SoftGLRenderContext oldContext = GetCurrentContextObj();
                    if (oldContext != context)
                    {
                        Thread thread = Thread.CurrentThread;
                        if (oldContext != null) { threadContextDict.Remove(thread); }
                        threadContextDict.Add(thread, context);
                        context.DeviceContextHandle = deviceContext;
                    }
                }
                else
                {
                    // TODO: update last error.
                }
            }
        }

        /// <summary>
        /// Gets current render context's handle.
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetCurrentContext()
        {
            IntPtr result = IntPtr.Zero;

            var threadContextDict = SoftGLRenderContext.threadContextDict;
            SoftGLRenderContext context = null;
            Thread thread = Thread.CurrentThread;
            if (threadContextDict.TryGetValue(thread, out context))
            {
                result = context.RenderContextHandle;
            }

            return result;
        }

        /// <summary>
        /// Gets current render context.
        /// </summary>
        /// <returns></returns>
        public static SoftGLRenderContext GetCurrentContextObj()
        {
            var threadContextDict = SoftGLRenderContext.threadContextDict;
            SoftGLRenderContext context = null;
            Thread thread = Thread.CurrentThread;
            if (!threadContextDict.TryGetValue(thread, out context))
            {
                context = null;
            }

            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static SoftGLRenderContext GetContextObj(IntPtr handle)
        {
            SoftGLRenderContext context = null;
            if (!SoftGLRenderContext.handleContextDict.TryGetValue(handle, out context))
            {
                context = null;
            }

            return context;
        }

        /// <summary>
        /// Gets current device context.
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetCurrentDC()
        {
            SoftGLRenderContext context = GetCurrentContextObj();
            return context != null ? context.DeviceContextHandle : IntPtr.Zero;
        }

        /// <summary>
        /// Delete specified render context.
        /// </summary>
        /// <param name="renderContext"></param>
        public static void DeleteContext(IntPtr renderContext)
        {
            if (renderContext == IntPtr.Zero) { return; }

            IntPtr current = GetCurrentContext();
            if (current == renderContext) // make this render context not current any more.
            {
                IntPtr deviceContext = GetCurrentDC();
                MakeCurrent(deviceContext, IntPtr.Zero);
            }

            SoftGLRenderContext context = null;
            if (SoftGLRenderContext.handleContextDict.TryGetValue(renderContext, out context))
            {
                context.Dispose();
            }
        }
    }
}
