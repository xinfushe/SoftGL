﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SoftGL
{
    partial class SoftGLRenderContext
    {
        private unsafe PassBuffer[] VertexShaderStage(DrawTarget mode, int count, DrawElementsType type, IntPtr indices, VertexArrayObject vao, ShaderProgram program, GLBuffer indexBuffer)
        {
            PassBuffer[] passBuffers = null;
            VertexShader vs = program.VertexShader; if (vs == null) { return passBuffers; }

            // init pass-buffers to record output from vertex shader.
            FieldInfo[] outFieldInfos = (from item in vs.outVariableDict select item.Value.fieldInfo).ToArray();
            uint vertexCount = GetVertexCount(vao, indexBuffer, type);
            int vertexSize = GetVertexSize(outFieldInfos);
            passBuffers = new PassBuffer[outFieldInfos.Length];
            for (int i = 0; i < passBuffers.Length; i++)
            {
                var outField = outFieldInfos[i];
                PassType passType = outField.FieldType.GetPassType();
                var passBuffer = new PassBuffer(passType, (int)vertexCount);
                passBuffer.Mapbuffer();
                passBuffers[i] = passBuffer;
            }

            // execute vertex shader for each vertex.
            byte[] indexData = indexBuffer.Data;
            int byteLength = indexData.Length;
            GCHandle pin = GCHandle.Alloc(indexData, GCHandleType.Pinned);
            IntPtr pointer = pin.AddrOfPinnedObject();
            var gl_VertexIDList = new List<uint>();
            for (int indexID = 0; indexID < count; indexID++)
            {
                uint gl_VertexID = GetVertexID(pointer, type, indexID);
                if (gl_VertexIDList.Contains(gl_VertexID)) { continue; }
                else { gl_VertexIDList.Add(gl_VertexID); }

                var instance = vs.CreateCodeInstance() as VertexCodeBase; // an executable vertex shader.
                instance.gl_VertexID = (int)gl_VertexID; // setup gl_VertexID.
                // setup "in SomeType varName;" vertex attributes.
                Dictionary<uint, VertexAttribDesc> locVertexAttribDict = vao.LocVertexAttribDict;
                foreach (InVariable inVar in vs.inVariableDict.Values) // Dictionary<string, InVariable>.Values
                {
                    VertexAttribDesc desc = null;
                    if (locVertexAttribDict.TryGetValue(inVar.location, out desc))
                    {
                        byte[] dataStore = desc.vbo.Data;
                        int byteIndex = desc.GetDataIndex(indexID);
                        VertexAttribType vertexAttribType = (VertexAttribType)desc.dataType;
                        object value = dataStore.ToStruct(vertexAttribType.GetManagedType(), byteIndex);
                        inVar.fieldInfo.SetValue(instance, value);
                    }
                }

                instance.main(); // execute vertex shader code.

                // copy data to pass-buffer.
                for (int i = 0; i < passBuffers.Length; i++)
                {
                    var outField = outFieldInfos[i];
                    var obj = outField.GetValue(instance);
                    byte[] bytes = obj.ToBytes();
                    PassBuffer passBuffer = passBuffers[i];
                    var array = (byte*)passBuffer.AddrOfPinnedObject();
                    for (int t = 0; t < bytes.Length; t++)
                    {
                        array[gl_VertexID * vertexSize + t] = bytes[t];
                    }
                }
            }
            pin.Free();

            for (int i = 0; i < passBuffers.Length; i++)
            {
                passBuffers[i].Unmapbuffer();
            }

            return passBuffers;
        }

        /// <summary>
        /// how many bytes a vertex has?
        /// </summary>
        /// <param name="vao"></param>
        /// <returns></returns>
        private int GetVertexSize(FieldInfo[] outVariables)
        {
            int result = 0;
            if (outVariables == null || outVariables.Length == 0) { return result; }

            foreach (var item in outVariables)
            {
                int size = item.FieldType.GetPassType().ByteSize();
                result += size;
            }

            return result;
        }

        private unsafe uint GetVertexID(IntPtr pointer, DrawElementsType type, int indexID)
        {
            uint gl_VertexID = uint.MaxValue;
            switch (type)
            {
                case DrawElementsType.UnsignedByte:
                    {
                        byte* array = (byte*)pointer.ToPointer();
                        gl_VertexID = array[indexID];
                    }
                    break;
                case DrawElementsType.UnsignedShort:
                    {
                        ushort* array = (ushort*)pointer.ToPointer();
                        gl_VertexID = array[indexID];
                    }
                    break;
                case DrawElementsType.UnsignedInt:
                    {
                        uint* array = (uint*)pointer.ToPointer();
                        gl_VertexID = array[indexID];
                    }
                    break;
                default:
                    throw new NotDealWithNewEnumItemException(typeof(DrawElementsType));
            }

            return gl_VertexID;
        }

        /// <summary>
        /// How many vertexIDs are there in the specified <paramref name="byteArray"/>.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private uint GetVertexIDCount(byte[] byteArray, DrawElementsType type)
        {
            uint result = 0;
            uint byteLength = (uint)byteArray.Length;
            switch (type)
            {
                case DrawElementsType.UnsignedByte: result = byteLength; break;
                case DrawElementsType.UnsignedShort: result = byteLength / 2; break;
                case DrawElementsType.UnsignedInt: result = byteLength / 4; break;
                default:
                    throw new NotDealWithNewEnumItemException(typeof(DrawElementsType));
            }

            return result;
        }

        /// <summary>
        /// Gets the maximum vertexID in the specified <paramref name="byteArray"/>.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private unsafe uint GetMaxVertexID(byte[] byteArray, DrawElementsType type)
        {
            int byteLength = byteArray.Length;
            GCHandle pin = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            IntPtr pointer = pin.AddrOfPinnedObject();
            uint gl_VertexID = 0;
            switch (type)
            {
                case DrawElementsType.UnsignedByte:
                    {
                        byte* array = (byte*)pointer.ToPointer();
                        for (int i = 0; i < byteLength; i++)
                        {
                            if (gl_VertexID < array[i]) { gl_VertexID = array[i]; }
                        }
                    }
                    break;
                case DrawElementsType.UnsignedShort:
                    {
                        ushort* array = (ushort*)pointer.ToPointer();
                        int length = byteLength / 2;
                        for (int i = 0; i < length; i++)
                        {
                            if (gl_VertexID < array[i]) { gl_VertexID = array[i]; }
                        }
                    }
                    break;
                case DrawElementsType.UnsignedInt:
                    {
                        uint* array = (uint*)pointer.ToPointer();
                        int length = byteLength / 4;
                        for (int i = 0; i < length; i++)
                        {
                            if (gl_VertexID < array[i]) { gl_VertexID = array[i]; }
                        }
                    }
                    break;
                default:
                    throw new NotDealWithNewEnumItemException(typeof(DrawElementsType));
            }
            pin.Free();

            return gl_VertexID;
        }

        private uint GetVertexCount(VertexArrayObject vao, GLBuffer indexBuffer, DrawElementsType type)
        {
            uint vertexCount = 0;
            VertexAttribDesc[] descs = vao.LocVertexAttribDict.Values.ToArray();
            if (descs.Length > 0)
            {
                int c = descs[0].GetVertexCount();
                if (c >= 0) { vertexCount = (uint)c; }
            }
            else
            {
                uint maxvertexID = GetMaxVertexID(indexBuffer.Data, type);
                uint vertexIDCount = GetVertexIDCount(indexBuffer.Data, type);
                vertexCount = Math.Min(maxvertexID, vertexIDCount);
            }

            return vertexCount;
        }
    }
}