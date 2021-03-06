﻿using SoftGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftOpengl32
{
    partial class StaticCalls
    {
        /// <summary>
        /// simultaneously specify storage for all levels of a two-dimensional or one-dimensional array texture.
        /// </summary>
        /// <param name="target">Specify the target of the operation. target​ must be one of GL_TEXTURE_2D, GL_PROXY_TEXTURE_2D, GL_TEXTURE_1D_ARRAY, GL_PROXY_TEXTURE_1D_ARRAY, GL_TEXTURE_RECTANGLE, GL_PROXY_TEXTURE_RECTANGLE, GL_TEXTURE_CUBE_MAP, or GL_PROXY_TEXTURE_CUBE_MAP.</param>
        /// <param name="levels">Specify the number of texture levels.</param>
        /// <param name="internalformat">Specifies the internal format to be used to store texture image data. This must be a sized image format.</param>
        /// <param name="width">Specifies the width of the texture, in texels.</param>
        /// <param name="height">Specifies the height of the texture, in texels.</param>
        public static void glTexStorage2D(uint target, int levels, uint internalformat, int width, int height)
        {
            SoftGLRenderContext.glTexStorage2D((TexStorageTarget)target, levels, internalformat, width, height);
        }
    }
}
