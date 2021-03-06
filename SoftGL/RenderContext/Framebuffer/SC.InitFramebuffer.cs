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
        private void InitDefaultFramebuffer(int width, int height)
        {
            // Create renderbuffer object for color attachment.
            {
                var ids = new uint[1];
                glGenRenderbuffers(ids.Length, ids);
                glBindRenderbuffer(GL.GL_RENDERBUFFER, ids[0]);
                glRenderbufferStorage(GL.GL_RENDERBUFFER, GL.GL_RGBA, width, height);
                glBindRenderbuffer(GL.GL_RENDERBUFFER, 0);
                glFramebufferRenderbuffer((uint)BindFramebufferTarget.Framebuffer, GL.GL_COLOR_ATTACHMENT0, GL.GL_RENDERBUFFER, ids[0]);
            }
            // Create renderbuffer object for depth attachment.
            {
                var ids = new uint[1];
                glGenRenderbuffers(ids.Length, ids);
                glBindRenderbuffer(GL.GL_RENDERBUFFER, ids[0]);
                glRenderbufferStorage(GL.GL_RENDERBUFFER, GL.GL_DEPTH_COMPONENT, width, height);
                glBindRenderbuffer(GL.GL_RENDERBUFFER, 0);
                glFramebufferRenderbuffer((uint)BindFramebufferTarget.Framebuffer, GL.GL_DEPTH_ATTACHMENT, GL.GL_RENDERBUFFER, ids[0]);
            }
            glDrawBuffers(1, new uint[] { GL.GL_FRONT_LEFT }); // GL_COLOR_ATTACHMENT0 use the same buffer in SoftGL.
            glCheckFramebufferStatus((uint)BindFramebufferTarget.Framebuffer);
            //glBindFramebuffer((uint)BindFramebufferTarget.Framebuffer, 0); // not needed.

            //throw new NotImplementedException();
        }
    }
}
