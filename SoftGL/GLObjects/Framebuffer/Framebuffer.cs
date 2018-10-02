﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftGL
{
    partial class Framebuffer : IDisposable
    {
        public uint Id { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public BindFramebufferTarget Target { get; set; }

        public Framebuffer(uint id) { this.Id = id; }

        /// <summary>
        /// glGet(GL_MAX_COLOR_ATTACHMENTS, ..);
        /// </summary>
        internal const int maxColorAttachments = 8;
        private IAttachable[] colorbufferAttachments = new IAttachable[maxColorAttachments]; // OpenGL supports at least 8 color attachement points.

        #region for default framebuffer object only.

        public IAttachable FrontLeft { get { return this.colorbufferAttachments[0]; } set { this.colorbufferAttachments[0] = value; } }

        public IAttachable FrontRight { get { return this.colorbufferAttachments[1]; } set { this.colorbufferAttachments[1] = value; } }

        public IAttachable BackLeft { get { return this.colorbufferAttachments[2]; } set { this.colorbufferAttachments[2] = value; } }

        public IAttachable BackRight { get { return this.colorbufferAttachments[3]; } set { this.colorbufferAttachments[3] = value; } }

        #endregion for default framebuffer object only.

        public IAttachable[] ColorbufferAttachments { get { return this.colorbufferAttachments; } }

        public IAttachable DepthbufferAttachment { get; set; }

        public IAttachable StencilbufferAttachment { get; set; }

        private List<uint> drawBuffers = new List<uint>();
        public IList<uint> DrawBuffers { get { return this.drawBuffers; } }
    }
}
