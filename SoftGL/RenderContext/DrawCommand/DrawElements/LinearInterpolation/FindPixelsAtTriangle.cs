﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftGL
{
    partial class SoftGLRenderContext
    {
        private const float inner = 0.0f;
        private const float line01 = 1.0f;
        private const float line02 = 2.0f;
        private const float line12 = 3.0f;
        private static void FindPixelsAtTriangle(vec3 p0, vec3 p1, vec3 p2, List<vec3> result)
        {
            List<vec3> points = new List<vec3>();
            FindPixelsAtLine(p0, p1, points);
            FindPixelsAtLine(p0, p2, points);
            FindPixelsAtLine(p1, p2, points);
            var groups = from item in points
                         group item by ((int)item.x) into g
                         select g;
            foreach (var g in groups) // find pixels in each vertical line.
            {
                var pointsAtLine = new List<int>();
                vec3 min = new vec3(), max = new vec3(); bool first = true;
                foreach (var pixel in g)
                {
                    if (first) { min = pixel; max = pixel; first = false; }
                    else
                    {
                        if (pixel.y < min.y) { min = pixel; }
                        if (max.y < pixel.y) { max = pixel; }
                    }
                    //pointsAtLine.Add((int)(pixel.y));
                }
                //min.y += 0.5f;
                //max.y += 0.5f;
                for (int y = (int)Math.Ceiling(min.y); y <= (int)(max.y); y++)
                {
                    //if (pointsAtLine.Contains(y)) { continue; }

                    float alpha = (y - min.y) / (max.y - min.y);
                    float x = (max.x - min.x) * alpha + min.x;
                    float z = (max.z - min.z) * alpha + min.z;
                    result.Add(new vec3(x, y, z));
                }
            }

            //result.AddRange(points);
        }
    }
}
