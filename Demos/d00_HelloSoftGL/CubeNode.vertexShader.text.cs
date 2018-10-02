﻿using SoftGL;

namespace d00_HelloSoftGL
{
    class vertexCode : VertexCodeBase
    {
        [In]
        vec3 inPosition;
        [Uniform]
        mat4 mvpMatrix;

        public override void main()
        {
            // transform vertex' position from model space to clip space.
            vec4 result = mvpMatrix * new vec4(inPosition, 1.0f);
            gl_Position = result;
        }
    }
}