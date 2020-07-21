/*
Advanced Polygon Collider (c) 2015 Digital Ruby, LLC
http://www.digitalruby.com

Source code may not be redistributed. Use in apps and games is fine.
*/

// This source code has been modified with great care from the farseer physics engine

/* ORIGINAL LICENSE COMPLIANCE REQUIRES THE FOLLOWING BE POSTED:
*
* Farseer Physics Engine:
* Copyright (c) 2012 Ian Qvist
* 
* Original source Box2D:
* Copyright (c) 2006-2011 Erin Catto http://www.box2d.org 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace DigitalRuby.AdvancedPolygonCollider
{
    public sealed class TextureConverter
    {
        private const int closePixelsLength = 8;
        private static int[,] closePixels = new int[closePixelsLength, 2] { { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 } };
        private const float hullTolerance = 0.9f;

        private byte[] solids;
        private int solidsLength;
        private int width;
        private int height;

        public List<Vertices> DetectVertices(UnityEngine.Color[] colors, int width, int alphaTolerance)
        {
            this.width = width;
            this.height = colors.Length / width;
            int n, s;

            solids = new byte[colors.Length];

            // calculate solids once, this makes tolerance lookups much faster
            for (int i = 0; i < solids.Length; i++)
            {
                // Diff will be negative for visitable value, resulting in a sign multiply and value of 0.
                // Sign multiply will be -1 for positive visitable values, resulting in non-zero value.
                n = alphaTolerance - (int)(colors[i].a * 255.0f);
                s = ((int)((n & 0x80000000) >> 31)) - 1; // sign multiplier is -1 for positive numbers and 0 for negative numbers
                n = n * s * s; // multiply n by sign squared to get 0 for negative numbers (solid) and > zero for positive numbers (pass through)
                solids[i] = (byte)n; // assign value back
            }

            solidsLength = colors.Length;

            List<Vertices> detectedVerticesList = DetectVertices();
            List<Vertices> result = new List<Vertices>();

            for (int i = 0; i < detectedVerticesList.Count; i++)
            {
                result.Add(detectedVerticesList[i]);
            }

            return result;
        }

        public List<Vertices> DetectVertices()
        {
            List<Vertices> detectedPolygons = new List<Vertices>();
            Vector2? holeEntrance = null;
            Vector2? polygonEntrance = null;
            List<Vector2> blackList = new List<Vector2>();

            bool searchOn;
            do
            {
                Vertices polygon;
                if (detectedPolygons.Count == 0)
                {
                    // First pass / single polygon
                    polygon = new Vertices(CreateSimplePolygon(Vector2.zero, Vector2.zero));
                    if (polygon.Count > 2)
                    {
                        polygonEntrance = GetTopMostVertex(polygon);
                    }
                }
                else if (polygonEntrance.HasValue)
                {
                    // Multi pass / multiple polygons
                    polygon = new Vertices(CreateSimplePolygon(polygonEntrance.Value, new Vector2(polygonEntrance.Value.x - 1f, polygonEntrance.Value.y)));
                }
                else
                {
                    break;
                }
                searchOn = false;

                if (polygon.Count > 2)
                {
                    do
                    {
                        holeEntrance = SearchHoleEntrance(polygon, holeEntrance);

                        if (holeEntrance.HasValue)
                        {
                            if (!blackList.Contains(holeEntrance.Value))
                            {
                                blackList.Add(holeEntrance.Value);
                                Vertices holePolygon = CreateSimplePolygon(holeEntrance.Value, new Vector2(holeEntrance.Value.x + 1, holeEntrance.Value.y));
                                if (holePolygon != null && holePolygon.Count > 2)
                                {
                                    // Add first hole polygon vertex to close the hole polygon.
                                    holePolygon.Add(holePolygon[0]);
                                    int vertex1Index, vertex2Index;
                                    if (SplitPolygonEdge(polygon, holeEntrance.Value, out vertex1Index, out vertex2Index))
                                    {
                                        polygon.InsertRange(vertex2Index, holePolygon);
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (true);

                    detectedPolygons.Add(polygon);
                }

                if (polygonEntrance.HasValue && SearchNextHullEntrance(detectedPolygons, polygonEntrance.Value, out polygonEntrance))
                {
                    searchOn = true;
                }
            }
            while (searchOn);

            if (detectedPolygons == null || (detectedPolygons != null && detectedPolygons.Count == 0))
            {
                throw new Exception("Couldn't detect any vertices.");
            }

            return detectedPolygons;
        }

        private void ApplyTriangulationCompatibleWinding(ref List<Vertices> detectedPolygons)
        {
            for (int i = 0; i < detectedPolygons.Count; i++)
            {
                detectedPolygons[i].Reverse();
            }
        }

        public bool IsSolid(ref Vector2 v)
        {
            return IsSolid((int)v.x + ((int)v.y * width));
        }

        public bool IsSolid(int x, int y)
        {
            return IsSolid(x + (y * width));
        }

        public bool IsSolid(int index)
        {
            return (index >= 0 && index < solids.Length && solids[index] == 0);
        }

        public bool InBounds(ref Vector2 coord)
        {
            return (coord.x >= 0f && coord.x < width && coord.y >= 0f && coord.y < height);
        }

        /// <summary>
        /// Function to search for an entrance point of a hole in a polygon. It searches the polygon from top to bottom between the polygon edges.
        /// </summary>
        /// <param name="polygon">The polygon to search in.</param>
        /// <param name="lastHoleEntrance">The last entrance point.</param>
        /// <returns>The next holes entrance point. Null if ther are no holes.</returns>
        private Vector2? SearchHoleEntrance(Vertices polygon, Vector2? lastHoleEntrance)
        {
            if (polygon == null)
            {
                throw new ArgumentNullException("'polygon' can't be null.");
            }
            else if (polygon.Count < 3)
            {
                throw new ArgumentException("'polygon.MainPolygon.Count' can't be less then 3.");
            }

            List<float> xCoords;
            Vector2? entrance;

            int startY;
            int endY;

            int lastSolid = 0;
            bool foundSolid;
            bool foundTransparent;

            // Set start y coordinate.
            if (lastHoleEntrance.HasValue)
            {
                // We need the y coordinate only.
                startY = (int)lastHoleEntrance.Value.y;
            }
            else
            {
                // Start from the top of the polygon if last entrance == null.
                startY = (int)GetTopMostCoord(polygon);
            }

            // Set the end y coordinate.
            endY = (int)GetBottomMostCoord(polygon);

            if (startY >= 0 && startY < height && endY > 0 && endY < height)
            {
                // go from top to bottom of the polygon
                for (int y = startY; y <= endY; y++)
                {
                    // get x-coord of every polygon edge which crosses y
                    xCoords = SearchCrossingEdges(polygon, y);

                    // We need an even number of crossing edges. 
                    // It's always a pair of start and end edge: nothing | polygon | hole | polygon | nothing ...
                    // If it's not then don't bother, it's probably a peak ...
                    // ...which should be filtered out by SearchCrossingEdges() anyway.
                    if (xCoords.Count > 1 && xCoords.Count % 2 == 0)
                    {
                        // Ok, this is short, but probably a little bit confusing.
                        // This part searches from left to right between the edges inside the polygon.
                        // The problem: We are using the polygon data to search in the texture data.
                        // That's simply not accurate, but necessary because of performance.
                        for (int i = 0; i < xCoords.Count; i += 2)
                        {
                            foundSolid = false;
                            foundTransparent = false;

                            // We search between the edges inside the polygon.
                            for (int x = (int)xCoords[i]; x <= (int)xCoords[i + 1]; x++)
                            {
                                // First pass: IsSolid might return false.
                                // In that case the polygon edge doesn't lie on the texture's solid pixel, because of the hull tolearance.
                                // If the edge lies before the first solid pixel then we need to skip our transparent pixel finds.

                                // The algorithm starts to search for a relevant transparent pixel (which indicates a possible hole) 
                                // after it has found a solid pixel.

                                // After we've found a solid and a transparent pixel (a hole's left edge) 
                                // we search for a solid pixel again (a hole's right edge).
                                // When found the distance of that coodrinate has to be greater then the hull tolerance.

                                if (IsSolid(x, y))
                                {
                                    if (!foundTransparent)
                                    {
                                        foundSolid = true;
                                        lastSolid = x;
                                    }

                                    if (foundSolid && foundTransparent)
                                    {
                                        entrance = new Vector2(lastSolid, y);

                                        if (DistanceToHullAcceptable(polygon, entrance.Value, true))
                                            return entrance;

                                        entrance = null;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (foundSolid)
                                        foundTransparent = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (xCoords.Count % 2 == 0)
                            System.Diagnostics.Debug.WriteLine("SearchCrossingEdges() % 2 != 0");
                    }
                }
            }

            return null;
        }

        private bool DistanceToHullAcceptableHoles(Vertices polygon, Vector2 point, bool higherDetail)
        {
            if (polygon == null)
                throw new ArgumentNullException("polygon", "'polygon' can't be null.");

            if (polygon.Count < 3)
                throw new ArgumentException("'polygon.MainPolygon.Count' can't be less then 3.");

            // Check the distance to main polygon.
            if (DistanceToHullAcceptable(polygon, point, higherDetail))
            {
                // All distances are larger then _hullTolerance.
                return true;
            }

            // Default to false.
            return false;
        }

        private bool DistanceToHullAcceptable(Vertices polygon, Vector2 point, bool higherDetail)
        {
            if (polygon == null)
                throw new ArgumentNullException("polygon", "'polygon' can't be null.");

            if (polygon.Count < 3)
                throw new ArgumentException("'polygon.Count' can't be less then 3.");


            Vector2 edgeVertex2 = polygon[polygon.Count - 1];
            Vector2 edgeVertex1;

            if (higherDetail)
            {
                for (int i = 0; i < polygon.Count; i++)
                {
                    edgeVertex1 = polygon[i];

                    if (LineTools.DistanceBetweenPointAndLineSegment(ref point, ref edgeVertex1, ref edgeVertex2) <= hullTolerance || Vector2.Distance(point, edgeVertex1) <= hullTolerance)
                        return false;

                    edgeVertex2 = polygon[i];
                }

                return true;
            }
            else
            {
                for (int i = 0; i < polygon.Count; i++)
                {
                    edgeVertex1 = polygon[i];

                    if (LineTools.DistanceBetweenPointAndLineSegment(ref point, ref edgeVertex1, ref edgeVertex2) <= hullTolerance)
                        return false;

                    edgeVertex2 = polygon[i];
                }

                return true;
            }
        }

        private bool InPolygon(Vertices polygon, Vector2 point)
        {
            bool inPolygon = !DistanceToHullAcceptableHoles(polygon, point, true);

            if (!inPolygon)
            {
                List<float> xCoords = SearchCrossingEdgesHoles(polygon, (int)point.y);

                if (xCoords.Count > 0 && xCoords.Count % 2 == 0)
                {
                    for (int i = 0; i < xCoords.Count; i += 2)
                    {
                        if (xCoords[i] <= point.x && xCoords[i + 1] >= point.x)
                            return true;
                    }
                }

                return false;
            }

            return true;
        }

        private Vector2? GetTopMostVertex(Vertices vertices)
        {
            float topMostValue = float.MaxValue;
            Vector2? topMost = null;

            for (int i = 0; i < vertices.Count; i++)
            {
                if (topMostValue > vertices[i].y)
                {
                    topMostValue = vertices[i].y;
                    topMost = vertices[i];
                }
            }

            return topMost;
        }

        private float GetTopMostCoord(Vertices vertices)
        {
            float returnValue = float.MaxValue;

            for (int i = 0; i < vertices.Count; i++)
            {
                if (returnValue > vertices[i].y)
                {
                    returnValue = vertices[i].y;
                }
            }

            return returnValue;
        }

        private float GetBottomMostCoord(Vertices vertices)
        {
            float returnValue = float.MinValue;

            for (int i = 0; i < vertices.Count; i++)
            {
                if (returnValue < vertices[i].y)
                {
                    returnValue = vertices[i].y;
                }
            }

            return returnValue;
        }

        private List<float> SearchCrossingEdgesHoles(Vertices polygon, int y)
        {
            if (polygon == null)
                throw new ArgumentNullException("polygon", "'polygon' can't be null.");

            if (polygon.Count < 3)
                throw new ArgumentException("'polygon.MainPolygon.Count' can't be less then 3.");

            List<float> result = SearchCrossingEdges(polygon, y);

            result.Sort();
            return result;
        }

        /// <summary>
        /// Searches the polygon for the x coordinates of the edges that cross the specified y coordinate.
        /// </summary>
        /// <param name="polygon">Polygon to search in.</param>
        /// <param name="y">Y coordinate to check for edges.</param>
        /// <returns>Descending sorted list of x coordinates of edges that cross the specified y coordinate.</returns>
        private List<float> SearchCrossingEdges(Vertices polygon, int y)
        {
            // sick-o-note:
            // Used to search the x coordinates of edges in the polygon for a specific y coordinate.
            // (Usualy comming from the texture data, that's why it's an int and not a float.)

            List<float> edges = new List<float>();

            // current edge
            Vector2 slope;
            Vector2 vertex1;    // i
            Vector2 vertex2;    // i - 1

            // next edge
            Vector2 nextSlope;
            Vector2 nextVertex; // i + 1

            bool addFind;

            if (polygon.Count > 2)
            {
                // There is a gap between the last and the first vertex in the vertex list.
                // We will bridge that by setting the last vertex (vertex2) to the last 
                // vertex in the list.
                vertex2 = polygon[polygon.Count - 1];

                // We are moving along the polygon edges.
                for (int i = 0; i < polygon.Count; i++)
                {
                    vertex1 = polygon[i];

                    // Approx. check if the edge crosses our y coord.
                    if ((vertex1.y >= y && vertex2.y <= y) ||
                        (vertex1.y <= y && vertex2.y >= y))
                    {
                        // Ignore edges that are parallel to y.
                        if (vertex1.y != vertex2.y)
                        {
                            addFind = true;
                            slope = vertex2 - vertex1;

                            // Special threatment for edges that end at the y coord.
                            if (vertex1.y == y)
                            {
                                // Create preview of the next edge.
                                nextVertex = polygon[(i + 1) % polygon.Count];
                                nextSlope = vertex1 - nextVertex;

                                // Ignore peaks. 
                                // If thwo edges are aligned like this: /\ and the y coordinate lies on the top,
                                // then we get the same x coord twice and we don't need that.
                                if (slope.y > 0)
                                    addFind = (nextSlope.y <= 0);
                                else
                                    addFind = (nextSlope.y >= 0);
                            }

                            if (addFind)
                                edges.Add((y - vertex1.y) / slope.y * slope.x + vertex1.x); // Calculate and add the x coord.
                        }
                    }

                    // vertex1 becomes vertex2 :).
                    vertex2 = vertex1;
                }
            }

            edges.Sort();
            return edges;
        }

        private bool SplitPolygonEdge(Vertices polygon, Vector2 coordInsideThePolygon, out int vertex1Index, out int vertex2Index)
        {
            Vector2 slope;
            int nearestEdgeVertex1Index = 0;
            int nearestEdgeVertex2Index = 0;
            bool edgeFound = false;

            float shortestDistance = float.MaxValue;

            bool edgeCoordFound = false;
            Vector2 foundEdgeCoord = Vector2.zero;

            List<float> xCoords = SearchCrossingEdges(polygon, (int)coordInsideThePolygon.y);

            vertex1Index = 0;
            vertex2Index = 0;

            foundEdgeCoord.y = coordInsideThePolygon.y;

            if (xCoords != null && xCoords.Count > 1 && xCoords.Count % 2 == 0)
            {
                float distance;
                for (int i = 0; i < xCoords.Count; i++)
                {
                    if (xCoords[i] < coordInsideThePolygon.x)
                    {
                        distance = coordInsideThePolygon.x - xCoords[i];

                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            foundEdgeCoord.x = xCoords[i];

                            edgeCoordFound = true;
                        }
                    }
                }

                if (edgeCoordFound)
                {
                    shortestDistance = float.MaxValue;

                    int edgeVertex2Index = polygon.Count - 1;

                    int edgeVertex1Index;
                    for (edgeVertex1Index = 0; edgeVertex1Index < polygon.Count; edgeVertex1Index++)
                    {
                        Vector2 tempVector1 = polygon[edgeVertex1Index];
                        Vector2 tempVector2 = polygon[edgeVertex2Index];
                        distance = LineTools.DistanceBetweenPointAndLineSegment(ref foundEdgeCoord,
                            ref tempVector1, ref tempVector2);
                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;

                            nearestEdgeVertex1Index = edgeVertex1Index;
                            nearestEdgeVertex2Index = edgeVertex2Index;

                            edgeFound = true;
                        }

                        edgeVertex2Index = edgeVertex1Index;
                    }

                    if (edgeFound)
                    {
                        slope = polygon[nearestEdgeVertex2Index] - polygon[nearestEdgeVertex1Index];
                        slope.Normalize();

                        Vector2 tempVector = polygon[nearestEdgeVertex1Index];
                        distance = Vector2.Distance(tempVector, foundEdgeCoord);

                        vertex1Index = nearestEdgeVertex1Index;
                        vertex2Index = nearestEdgeVertex1Index + 1;

                        polygon.Insert(nearestEdgeVertex1Index, distance * slope + polygon[vertex1Index]);
                        polygon.Insert(nearestEdgeVertex1Index, distance * slope + polygon[vertex2Index]);

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entrance"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        private Vertices CreateSimplePolygon(Vector2 entrance, Vector2 last)
        {
            bool entranceFound = false;
            bool endOfHull = false;

            Vertices polygon = new Vertices(32);
            Vertices hullArea = new Vertices(32);
            Vertices endOfHullArea = new Vertices(32);

            Vector2 current = Vector2.zero;

            #region Entrance check

            // Get the entrance point.
            if (entrance == Vector2.zero || !InBounds(ref entrance))
            {
                entranceFound = SearchHullEntrance(out entrance);
                if (entranceFound)
                {
                    current = new Vector2(entrance.x - 1f, entrance.y);
                }
            }
            else
            {
                if (IsSolid(ref entrance))
                {
                    if (IsNearPixel(ref entrance, ref last))
                    {
                        current = last;
                        entranceFound = true;
                    }
                    else
                    {
                        Vector2 temp;
                        if (SearchNearPixels(false, ref entrance, out temp))
                        {
                            current = temp;
                            entranceFound = true;
                        }
                        else
                        {
                            entranceFound = false;
                        }
                    }
                }
            }

            #endregion

            if (entranceFound)
            {
                polygon.Add(entrance);
                hullArea.Add(entrance);

                Vector2 next = entrance;

                do
                {
                    // Search in the pre vision list for an outstanding point.
                    Vector2 outstanding;
                    if (SearchForOutstandingVertex(hullArea, out outstanding))
                    {
                        if (endOfHull)
                        {
                            // We have found the next pixel, but is it on the last bit of the hull?
                            if (endOfHullArea.Contains(outstanding))
                            {
                                // Indeed.
                                polygon.Add(outstanding);
                            }

                            // That's enough, quit.
                            break;
                        }

                        // Add it and remove all vertices that don't matter anymore
                        // (all the vertices before the outstanding).
                        polygon.Add(outstanding);
                        hullArea.RemoveRange(0, hullArea.IndexOf(outstanding));
                    }

                    // Last point gets current and current gets next. Our little spider is moving forward on the hull ;).
                    last = current;
                    current = next;

                    // Get the next point on hull.
                    // TODO: Fix infinite loop here sometimes...
                    if (GetNextHullPoint(ref last, ref current, out next))
                    {
                        // Add the vertex to a hull pre vision list.
                        hullArea.Add(next);
                    }
                    else
                    {
                        // Quit
                        break;
                    }

                    if (next == entrance && !endOfHull)
                    {
                        // It's the last bit of the hull, search on and exit at next found vertex.
                        endOfHull = true;
                        endOfHullArea.AddRange(hullArea);

                        // We don't want the last vertex to be the same as the first one, because it causes the triangulation code to crash.
                        if (endOfHullArea.Contains(entrance))
                            endOfHullArea.Remove(entrance);
                    }

                } while (true);
            }

            return polygon;
        }

        private bool SearchNearPixels(bool searchingForSolidPixel, ref Vector2 current, out Vector2 foundPixel)
        {
            for (int i = 0; i < closePixelsLength; i++)
            {
                int x = (int)current.x + closePixels[i, 0];
                int y = (int)current.y + closePixels[i, 1];

                if (!searchingForSolidPixel ^ IsSolid(x, y))
                {
                    foundPixel = new Vector2(x, y);
                    return true;
                }
            }

            // Nothing found.
            foundPixel = Vector2.zero;
            return false;
        }

        private bool IsNearPixel(ref Vector2 current, ref Vector2 near)
        {
            for (int i = 0; i < closePixelsLength; i++)
            {
                int x = (int)current.x + closePixels[i, 0];
                int y = (int)current.y + closePixels[i, 1];

                if (x >= 0 && x <= width && y >= 0 && y <= height)
                {
                    if (x == (int)near.x && y == (int)near.y)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool SearchHullEntrance(out Vector2 entrance)
        {
            // Search for first solid pixel.
            for (int y = 0; y <= height; y++)
            {
                for (int x = 0; x <= width; x++)
                {
                    if (IsSolid(x, y))
                    {
                        entrance = new Vector2(x, y);
                        return true;
                    }
                }
            }

            // If there are no solid pixels.
            entrance = Vector2.zero;
            return false;
        }

        /// <summary>
        /// Searches for the next shape.
        /// </summary>
        /// <param name="detectedPolygons">Already detected polygons.</param>
        /// <param name="start">Search start coordinate.</param>
        /// <param name="entrance">Returns the found entrance coordinate. Null if no other shapes found.</param>
        /// <returns>True if a new shape was found.</returns>
        private bool SearchNextHullEntrance(List<Vertices> detectedPolygons, Vector2 start, out Vector2? entrance)
        {
            int x;

            bool foundTransparent = false;
            bool inPolygon = false;

            for (int i = (int)start.x + (int)start.y * width; i <= solidsLength; i++)
            {
                if (IsSolid(i))
                {
                    if (foundTransparent)
                    {
                        x = i % width;
                        entrance = new Vector2(x, (i - x) / (float)width);

                        inPolygon = false;
                        for (int polygonIdx = 0; polygonIdx < detectedPolygons.Count; polygonIdx++)
                        {
                            if (InPolygon(detectedPolygons[polygonIdx], entrance.Value))
                            {
                                inPolygon = true;
                                break;
                            }
                        }

                        if (inPolygon)
                            foundTransparent = false;
                        else
                            return true;
                    }
                }
                else
                    foundTransparent = true;
            }

            entrance = null;
            return false;
        }

        private bool GetNextHullPoint(ref Vector2 last, ref Vector2 current, out Vector2 next)
        {
            int x;
            int y;

            int indexOfFirstPixelToCheck = GetIndexOfFirstPixelToCheck(ref last, ref current);
            int indexOfPixelToCheck;

            for (int i = 0; i < closePixelsLength; i++)
            {
                indexOfPixelToCheck = (indexOfFirstPixelToCheck + i) % closePixelsLength;

                x = (int)current.x + closePixels[indexOfPixelToCheck, 0];
                y = (int)current.y + closePixels[indexOfPixelToCheck, 1];

                if (x >= 0 && x < width && y >= 0 && y <= height)
                {
                    if (IsSolid(x, y))
                    {
                        next = new Vector2(x, y);
                        return true;
                    }
                }
            }

            next = Vector2.zero;
            return false;
        }

        private bool SearchForOutstandingVertex(Vertices hullArea, out Vector2 outstanding)
        {
            Vector2 outstandingResult = Vector2.zero;
            bool found = false;

            if (hullArea.Count > 2)
            {
                int hullAreaLastPoint = hullArea.Count - 1;

                Vector2 tempVector1;
                Vector2 tempVector2 = hullArea[0];
                Vector2 tempVector3 = hullArea[hullAreaLastPoint];

                // Search between the first and last hull point.
                for (int i = 1; i < hullAreaLastPoint; i++)
                {
                    tempVector1 = hullArea[i];

                    // Check if the distance is over the one that's tolerable.
                    if (LineTools.DistanceBetweenPointAndLineSegment(ref tempVector1, ref tempVector2, ref tempVector3) >= hullTolerance)
                    {
                        outstandingResult = hullArea[i];
                        found = true;
                        break;
                    }
                }
            }

            outstanding = outstandingResult;
            return found;
        }

        private int GetIndexOfFirstPixelToCheck(ref Vector2 last, ref Vector2 current)
        {
            // .: pixel
            // l: last position
            // c: current position
            // f: first pixel for next search

            // f . .
            // l c .
            // . . .

            //Calculate in which direction the last move went and decide over the next pixel to check.
            switch ((int)(current.x - last.x))
            {
                case 1:
                    switch ((int)(current.y - last.y))
                    {
                        case 1:
                            return 1;

                        case 0:
                            return 0;

                        case -1:
                            return 7;
                    }
                    break;

                case 0:
                    switch ((int)(current.y - last.y))
                    {
                        case 1:
                            return 2;

                        case -1:
                            return 6;
                    }
                    break;

                case -1:
                    switch ((int)(current.y - last.y))
                    {
                        case 1:
                            return 3;

                        case 0:
                            return 4;

                        case -1:
                            return 5;
                    }
                    break;
            }

            return 0;
        }
    }

    public class Vertices : List<Vector2>
    {
        public Vertices() { }

        public Vertices(int capacity) : base(capacity) { }

        public Vertices(IEnumerable<Vector2> vertices)
        {
            AddRange(vertices);
        }

        /// <summary>
        /// Gets the next index. Used for iterating all the edges with wrap-around.
        /// </summary>
        /// <param name="index">The current index</param>
        public int NextIndex(int index)
        {
            return (index + 1 > Count - 1) ? 0 : index + 1;
        }

        /// <summary>
        /// Gets the next vertex. Used for iterating all the edges with wrap-around.
        /// </summary>
        /// <param name="index">The current index</param>
        public Vector2 NextVertex(int index)
        {
            return this[NextIndex(index)];
        }

        /// <summary>
        /// Gets the previous index. Used for iterating all the edges with wrap-around.
        /// </summary>
        /// <param name="index">The current index</param>
        public int PreviousIndex(int index)
        {
            return index - 1 < 0 ? Count - 1 : index - 1;
        }

        /// <summary>
        /// Gets the previous vertex. Used for iterating all the edges with wrap-around.
        /// </summary>
        /// <param name="index">The current index</param>
        public Vector2 PreviousVertex(int index)
        {
            return this[PreviousIndex(index)];
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < Count; i++)
            {
                builder.Append(this[i].ToString());
                if (i < Count - 1)
                {
                    builder.Append(" ");
                }
            }
            return builder.ToString();
        }
    }

    /// <summary>
    /// Collection of helper methods for misc collisions.
    /// Does float tolerance and line collisions with lines and AABBs.
    /// </summary>
    public static class LineTools
    {
        public static float DistanceBetweenPointAndLineSegment(ref Vector2 point, ref Vector2 start, ref Vector2 end)
        {
            if (start == end)
                return Vector2.Distance(point, start);

            Vector2 v = end - start;
            Vector2 w = point - start;

            float c1 = Vector2.Dot(w, v);
            if (c1 <= 0) return Vector2.Distance(point, start);

            float c2 = Vector2.Dot(v, v);
            if (c2 <= c1) return Vector2.Distance(point, end);

            float b = c1 / c2;
            Vector2 pointOnLine = (start + (v * b));
            return Vector2.Distance(point, pointOnLine);
        }

        // From Eric Jordan's convex decomposition library
        /// <summary>
        ///Check if the lines a0->a1 and b0->b1 cross.
        ///If they do, intersectionPoint will be filled
        ///with the point of crossing.
        ///
        ///Grazing lines should not return true.
        /// 
        /// </summary>
        public static bool LineIntersect2(ref Vector2 a0, ref Vector2 a1, ref Vector2 b0, ref Vector2 b1, out Vector2 intersectionPoint)
        {
            intersectionPoint = Vector2.zero;

            if (a0 == b0 || a0 == b1 || a1 == b0 || a1 == b1)
                return false;

            float x1 = a0.x;
            float y1 = a0.y;
            float x2 = a1.x;
            float y2 = a1.y;
            float x3 = b0.x;
            float y3 = b0.y;
            float x4 = b1.x;
            float y4 = b1.y;

            //AABB early exit
            if (Math.Max(x1, x2) < Math.Min(x3, x4) || Math.Max(x3, x4) < Math.Min(x1, x2))
                return false;

            if (Math.Max(y1, y2) < Math.Min(y3, y4) || Math.Max(y3, y4) < Math.Min(y1, y2))
                return false;

            float ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3));
            float ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3));
            float denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            if (Math.Abs(denom) < Mathf.Epsilon)
            {
                //Lines are too close to parallel to call
                return false;
            }
            ua /= denom;
            ub /= denom;

            if ((0 < ua) && (ua < 1) && (0 < ub) && (ub < 1))
            {
                intersectionPoint.x = (x1 + ua * (x2 - x1));
                intersectionPoint.y = (y1 + ua * (y2 - y1));
                return true;
            }

            return false;
        }

        //From Mark Bayazit's convex decomposition algorithm
        public static Vector2 LineIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            Vector2 i = Vector2.zero;
            float a1 = p2.y - p1.y;
            float b1 = p1.x - p2.x;
            float c1 = a1 * p1.x + b1 * p1.y;
            float a2 = q2.y - q1.y;
            float b2 = q1.x - q2.x;
            float c2 = a2 * q1.x + b2 * q1.y;
            float det = a1 * b2 - a2 * b1;

            if (Mathf.Abs(det) > Mathf.Epsilon)
            {
                // lines are not parallel
                i.x = (b2 * c1 - b1 * c2) / det;
                i.y = (a1 * c2 - a2 * c1) / det;
            }
            return i;
        }

        /// <summary>
        /// This method detects if two line segments (or lines) intersect,
        /// and, if so, the point of intersection. Use the <paramref name="firstIsSegment"/> and
        /// <paramref name="secondIsSegment"/> parameters to set whether the intersection point
        /// must be on the first and second line segments. Setting these
        /// both to true means you are doing a line-segment to line-segment
        /// intersection. Setting one of them to true means you are doing a
        /// line to line-segment intersection test, and so on.
        /// Note: If two line segments are coincident, then 
        /// no intersection is detected (there are actually
        /// infinite intersection points).
        /// Author: Jeremy Bell
        /// </summary>
        /// <param name="point1">The first point of the first line segment.</param>
        /// <param name="point2">The second point of the first line segment.</param>
        /// <param name="point3">The first point of the second line segment.</param>
        /// <param name="point4">The second point of the second line segment.</param>
        /// <param name="point">This is set to the intersection
        /// point if an intersection is detected.</param>
        /// <param name="firstIsSegment">Set this to true to require that the 
        /// intersection point be on the first line segment.</param>
        /// <param name="secondIsSegment">Set this to true to require that the
        /// intersection point be on the second line segment.</param>
        /// <returns>True if an intersection is detected, false otherwise.</returns>
        public static bool LineIntersect(ref Vector2 point1, ref Vector2 point2, ref Vector2 point3, ref Vector2 point4, bool firstIsSegment, bool secondIsSegment, out Vector2 point)
        {
            point = new Vector2();

            // these are reused later.
            // each lettered sub-calculation is used twice, except
            // for b and d, which are used 3 times
            float a = point4.y - point3.y;
            float b = point2.x - point1.x;
            float c = point4.x - point3.x;
            float d = point2.y - point1.y;

            // denominator to solution of linear system
            float denom = (a * b) - (c * d);

            // if denominator is 0, then lines are parallel
            if (Mathf.Abs(denom) > Mathf.Epsilon)
            {
                float e = point1.y - point3.y;
                float f = point1.x - point3.x;
                float oneOverDenom = 1.0f / denom;

                // numerator of first equation
                float ua = (c * e) - (a * f);
                ua *= oneOverDenom;

                // check if intersection point of the two lines is on line segment 1
                if (!firstIsSegment || ua >= 0.0f && ua <= 1.0f)
                {
                    // numerator of second equation
                    float ub = (b * e) - (d * f);
                    ub *= oneOverDenom;

                    // check if intersection point of the two lines is on line segment 2
                    // means the line segments intersect, since we know it is on
                    // segment 1 as well.
                    if (!secondIsSegment || ub >= 0.0f && ub <= 1.0f)
                    {
                        // check if they are coincident (no collision in this case)
                        if (ua != 0f || ub != 0f)
                        {
                            //There is an intersection
                            point.x = point1.x + ua * b;
                            point.y = point1.y + ua * d;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// This method detects if two line segments (or lines) intersect,
        /// and, if so, the point of intersection. Use the <paramref name="firstIsSegment"/> and
        /// <paramref name="secondIsSegment"/> parameters to set whether the intersection point
        /// must be on the first and second line segments. Setting these
        /// both to true means you are doing a line-segment to line-segment
        /// intersection. Setting one of them to true means you are doing a
        /// line to line-segment intersection test, and so on.
        /// Note: If two line segments are coincident, then 
        /// no intersection is detected (there are actually
        /// infinite intersection points).
        /// Author: Jeremy Bell
        /// </summary>
        /// <param name="point1">The first point of the first line segment.</param>
        /// <param name="point2">The second point of the first line segment.</param>
        /// <param name="point3">The first point of the second line segment.</param>
        /// <param name="point4">The second point of the second line segment.</param>
        /// <param name="intersectionPoint">This is set to the intersection
        /// point if an intersection is detected.</param>
        /// <param name="firstIsSegment">Set this to true to require that the 
        /// intersection point be on the first line segment.</param>
        /// <param name="secondIsSegment">Set this to true to require that the
        /// intersection point be on the second line segment.</param>
        /// <returns>True if an intersection is detected, false otherwise.</returns>
        public static bool LineIntersect(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, bool firstIsSegment, bool secondIsSegment, out Vector2 intersectionPoint)
        {
            return LineIntersect(ref point1, ref point2, ref point3, ref point4, firstIsSegment, secondIsSegment, out intersectionPoint);
        }

        /// <summary>
        /// This method detects if two line segments intersect,
        /// and, if so, the point of intersection. 
        /// Note: If two line segments are coincident, then 
        /// no intersection is detected (there are actually
        /// infinite intersection points).
        /// </summary>
        /// <param name="point1">The first point of the first line segment.</param>
        /// <param name="point2">The second point of the first line segment.</param>
        /// <param name="point3">The first point of the second line segment.</param>
        /// <param name="point4">The second point of the second line segment.</param>
        /// <param name="intersectionPoint">This is set to the intersection
        /// point if an intersection is detected.</param>
        /// <returns>True if an intersection is detected, false otherwise.</returns>
        public static bool LineIntersect(ref Vector2 point1, ref Vector2 point2, ref Vector2 point3, ref Vector2 point4, out Vector2 intersectionPoint)
        {
            return LineIntersect(ref point1, ref point2, ref point3, ref point4, true, true, out intersectionPoint);
        }

        /// <summary>
        /// This method detects if two line segments intersect,
        /// and, if so, the point of intersection. 
        /// Note: If two line segments are coincident, then 
        /// no intersection is detected (there are actually
        /// infinite intersection points).
        /// </summary>
        /// <param name="point1">The first point of the first line segment.</param>
        /// <param name="point2">The second point of the first line segment.</param>
        /// <param name="point3">The first point of the second line segment.</param>
        /// <param name="point4">The second point of the second line segment.</param>
        /// <param name="intersectionPoint">This is set to the intersection
        /// point if an intersection is detected.</param>
        /// <returns>True if an intersection is detected, false otherwise.</returns>
        public static bool LineIntersect(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, out Vector2 intersectionPoint)
        {
            return LineIntersect(ref point1, ref point2, ref point3, ref point4, true, true, out intersectionPoint);
        }

        /// <summary>
        /// Get all intersections between a line segment and a list of vertices
        /// representing a polygon. The vertices reuse adjacent points, so for example
        /// edges one and two are between the first and second vertices and between the
        /// second and third vertices. The last edge is between vertex vertices.Count - 1
        /// and verts0. (ie, vertices from a Geometry or AABB)
        /// </summary>
        /// <param name="point1">The first point of the line segment to test</param>
        /// <param name="point2">The second point of the line segment to test.</param>
        /// <param name="vertices">The vertices, as described above</param>
        public static Vertices LineSegmentVerticesIntersect(ref Vector2 point1, ref Vector2 point2, Vertices vertices)
        {
            Vertices intersectionPoints = new Vertices();

            for (int i = 0; i < vertices.Count; i++)
            {
                Vector2 point;
                if (LineIntersect(vertices[i], vertices[vertices.NextIndex(i)], point1, point2, true, true, out point))
                {
                    intersectionPoints.Add(point);
                }
            }

            return intersectionPoints;
        }
    }

    /// <summary>
    /// Convex decomposition algorithm created by Mark Bayazit (http://mnbayazit.com/)
    /// For more information about this algorithm, see http://mnbayazit.com/406/bayazit
    /// </summary>
    public static class BayazitDecomposer
    {
        private static Vector2 At(int i, List<Vector2> vertices)
        {
            int s = vertices.Count;

            return s == 1 ? vertices[0] : vertices[i < 0 ? s - (- i % s) : i % s];
        }

        private static List<Vector2> Copy(int i, int j, List<Vector2> vertices)
        {
            List<Vector2> p = new List<Vector2>();
            while (j < i) j += vertices.Count;
            //p.reserve(j - i + 1);
            for (; i <= j; ++i)
            {
                p.Add(At(i, vertices));
            }
            return p;
        }

        /// <summary>
        /// Decompose the polygon into several smaller non-concave polygon.
        /// If the polygon is already convex, it will return the original polygon, unless it is over Settings.MaxPolygonVertices.
        /// Precondition: Counter Clockwise polygon
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static List<List<Vector2>> ConvexPartition(List<Vector2> vertices)
        {
            //We force it to CCW as it is a precondition in this algorithm.
            ForceCounterClockWise(vertices);

            List<List<Vector2>> list = new List<List<Vector2>>();
            float d, lowerDist, upperDist;
            Vector2 p;
            Vector2 lowerInt = new Vector2();
            Vector2 upperInt = new Vector2(); // intersection points
            int lowerIndex = 0, upperIndex = 0;
            List<Vector2> lowerPoly, upperPoly;

            for (int i = 0; i < vertices.Count; ++i)
            {
                if (Reflex(i, vertices))
                {
                    lowerDist = upperDist = float.MaxValue; // std::numeric_limits<qreal>::max();
                    for (int j = 0; j < vertices.Count; ++j)
                    {
                        // if line intersects with an edge
                        if (Left(At(i - 1, vertices), At(i, vertices), At(j, vertices)) &&
                            RightOn(At(i - 1, vertices), At(i, vertices), At(j - 1, vertices)))
                        {
                            // find the point of intersection
                            p = LineIntersect(At(i - 1, vertices), At(i, vertices), At(j, vertices),
                                At(j - 1, vertices));
                            if (Right(At(i + 1, vertices), At(i, vertices), p))
                            {
                                // make sure it's inside the poly
                                d = SquareDist(At(i, vertices), p);
                                if (d < lowerDist)
                                {
                                    // keep only the closest intersection
                                    lowerDist = d;
                                    lowerInt = p;
                                    lowerIndex = j;
                                }
                            }
                        }

                        if (Left(At(i + 1, vertices), At(i, vertices), At(j + 1, vertices)) &&
                            RightOn(At(i + 1, vertices), At(i, vertices), At(j, vertices)))
                        {
                            p = LineIntersect(At(i + 1, vertices), At(i, vertices), At(j, vertices),
                                At(j + 1, vertices));
                            if (Left(At(i - 1, vertices), At(i, vertices), p))
                            {
                                d = SquareDist(At(i, vertices), p);
                                if (d < upperDist)
                                {
                                    upperDist = d;
                                    upperIndex = j;
                                    upperInt = p;
                                }
                            }
                        }
                    }

                    // if there are no vertices to connect to, choose a point in the middle
                    if (lowerIndex == (upperIndex + 1) % vertices.Count)
                    {
                        Vector2 sp = ((lowerInt + upperInt) / 2);

                        lowerPoly = Copy(i, upperIndex, vertices);
                        lowerPoly.Add(sp);
                        upperPoly = Copy(lowerIndex, i, vertices);
                        upperPoly.Add(sp);
                    }
                    else
                    {
                        double highestScore = 0, bestIndex = lowerIndex;
                        while (upperIndex < lowerIndex) upperIndex += vertices.Count;
                        for (int j = lowerIndex; j <= upperIndex; ++j)
                        {
                            if (CanSee(i, j, vertices))
                            {
                                double score = 1 / (SquareDist(At(i, vertices), At(j, vertices)) + 1);
                                if (Reflex(j, vertices))
                                {
                                    if (RightOn(At(j - 1, vertices), At(j, vertices), At(i, vertices)) &&
                                        LeftOn(At(j + 1, vertices), At(j, vertices), At(i, vertices)))
                                    {
                                        score += 3;
                                    }
                                    else
                                    {
                                        score += 2;
                                    }
                                }
                                else
                                {
                                    score += 1;
                                }
                                if (score > highestScore)
                                {
                                    bestIndex = j;
                                    highestScore = score;
                                }
                            }
                        }
                        lowerPoly = Copy(i, (int)bestIndex, vertices);
                        upperPoly = Copy((int)bestIndex, i, vertices);
                    }
                    list.AddRange(ConvexPartition(lowerPoly));
                    list.AddRange(ConvexPartition(upperPoly));
                    return list;
                }
            }

            // polygon is already convex
            list.Add(vertices);

            //The polygons are not guaranteed to be without collinear points. We remove
            //them to be sure.
            for (int i = 0; i < list.Count; i++)
            {
                //list[i] = SimplifyTools.CollinearSimplify(list[i], 0);
            }

            //Remove empty vertice collections
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Count == 0)
                    list.RemoveAt(i);
            }

            return list;
        }

        private static bool CanSee(int i, int j, List<Vector2> vertices)
        {
            if (Reflex(i, vertices))
            {
                if (LeftOn(At(i, vertices), At(i - 1, vertices), At(j, vertices)) &&
                    RightOn(At(i, vertices), At(i + 1, vertices), At(j, vertices)))
                    return false;
            }
            else
            {
                if (RightOn(At(i, vertices), At(i + 1, vertices), At(j, vertices)) ||
                    LeftOn(At(i, vertices), At(i - 1, vertices), At(j, vertices)))
                    return false;
            }
            if (Reflex(j, vertices))
            {
                if (LeftOn(At(j, vertices), At(j - 1, vertices), At(i, vertices)) &&
                    RightOn(At(j, vertices), At(j + 1, vertices), At(i, vertices)))
                    return false;
            }
            else
            {
                if (RightOn(At(j, vertices), At(j + 1, vertices), At(i, vertices)) ||
                    LeftOn(At(j, vertices), At(j - 1, vertices), At(i, vertices)))
                    return false;
            }
            for (int k = 0; k < vertices.Count; ++k)
            {
                if ((k + 1) % vertices.Count == i || k == i || (k + 1) % vertices.Count == j || k == j)
                {
                    continue; // ignore incident edges
                }
                Vector2 intersectionPoint;
                if (LineIntersect2(At(i, vertices), At(j, vertices), At(k, vertices), At(k + 1, vertices), out intersectionPoint))
                {
                    return false;
                }
            }
            return true;
        }

        // precondition: ccw
        private static bool Reflex(int i, List<Vector2> vertices)
        {
            return Right(i, vertices);
        }

        private static bool Right(int i, List<Vector2> vertices)
        {
            return Right(At(i - 1, vertices), At(i, vertices), At(i + 1, vertices));
        }

        private static bool Left(Vector2 a, Vector2 b, Vector2 c)
        {
            return Area(ref a, ref b, ref c) > 0;
        }

        private static bool LeftOn(Vector2 a, Vector2 b, Vector2 c)
        {
            return Area(ref a, ref b, ref c) >= 0;
        }

        private static bool Right(Vector2 a, Vector2 b, Vector2 c)
        {
            return Area(ref a, ref b, ref c) < 0;
        }

        private static bool RightOn(Vector2 a, Vector2 b, Vector2 c)
        {
            return Area(ref a, ref b, ref c) <= 0;
        }

        private static float SquareDist(Vector2 a, Vector2 b)
        {
            float dx = b.x - a.x;
            float dy = b.y - a.y;
            return dx * dx + dy * dy;
        }

        //forces counter clock wise order.
        private static void ForceCounterClockWise(List<Vector2> vertices)
        {
            if (!IsCounterClockWise(vertices))
            {
                vertices.Reverse();
            }
        }

        private static bool IsCounterClockWise(List<Vector2> vertices)
        {
            //We just return true for lines
            if (vertices.Count < 3)
                return true;

            return (GetSignedArea(vertices) > 0.0f);
        }

        //gets the signed area.
        private static float GetSignedArea(List<Vector2> vertices)
        {
            int i;
            float area = 0;

            for (i = 0; i < vertices.Count; i++)
            {
                int j = (i + 1) % vertices.Count;
                area += vertices[i].x * vertices[j].y;
                area -= vertices[i].y * vertices[j].x;
            }
            area /= 2.0f;
            return area;
        }

        //From Mark Bayazit's convex decomposition algorithm
        private static Vector2 LineIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            Vector2 i = Vector2.zero;
            float a1 = p2.y - p1.y;
            float b1 = p1.x - p2.x;
            float c1 = a1 * p1.x + b1 * p1.y;
            float a2 = q2.y - q1.y;
            float b2 = q1.x - q2.x;
            float c2 = a2 * q1.x + b2 * q1.y;
            float det = a1 * b2 - a2 * b1;

            if (!FloatEquals(det, 0))
            {
                // lines are not parallel
                i.x = (b2 * c1 - b1 * c2) / det;
                i.y = (a1 * c2 - a2 * c1) / det;
            }
            return i;
        }

        //from Eric Jordan's convex decomposition library, it checks if the lines a0->a1 and b0->b1 cross.
        //if they do, intersectionPoint will be filled with the point of crossing. Grazing lines should not return true.
        private static bool LineIntersect2(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1, out Vector2 intersectionPoint)
        {
            intersectionPoint = Vector2.zero;

            if (a0 == b0 || a0 == b1 || a1 == b0 || a1 == b1)
                return false;

            float x1 = a0.x;
            float y1 = a0.y;
            float x2 = a1.x;
            float y2 = a1.y;
            float x3 = b0.x;
            float y3 = b0.y;
            float x4 = b1.x;
            float y4 = b1.y;

            //AABB early exit
            if (Math.Max(x1, x2) < Math.Min(x3, x4) || Math.Max(x3, x4) < Math.Min(x1, x2))
                return false;

            if (Math.Max(y1, y2) < Math.Min(y3, y4) || Math.Max(y3, y4) < Math.Min(y1, y2))
                return false;

            float ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3));
            float ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3));
            float denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            if (Math.Abs(denom) < Mathf.Epsilon)
            {
                //Lines are too close to parallel to call
                return false;
            }
            ua /= denom;
            ub /= denom;

            if ((0 < ua) && (ua < 1) && (0 < ub) && (ub < 1))
            {
                intersectionPoint.x = (x1 + ua * (x2 - x1));
                intersectionPoint.y = (y1 + ua * (y2 - y1));
                return true;
            }

            return false;
        }

        private static bool FloatEquals(float value1, float value2)
        {
            return Math.Abs(value1 - value2) <= Mathf.Epsilon;
        }

        //returns a positive number if c is to the left of the line going from a to b. Positive number if point is left, negative if point is right, and 0 if points are collinear.</returns>
        private static float Area(ref Vector2 a, ref Vector2 b, ref Vector2 c)
        {
            return a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y);
        }
    }

    /// <summary>
    /// Provides a set of tools to simplify polygons in various ways.
    /// </summary>
    public static class SimplifyTools
    {
        /// <summary>
        /// Ramer-Douglas-Peucker polygon simplification algorithm. This is the general recursive version that does not use the
        /// speed-up technique by using the Melkman convex hull.
        /// 
        /// If you pass in 0, it will remove all collinear points.
        /// </summary>
        /// <returns>The simplified polygon</returns>
        public static Vertices DouglasPeuckerSimplify(Vertices vertices, float distanceTolerance)
        {
            if (vertices.Count <= 3)
                return vertices;

            bool[] usePoint = new bool[vertices.Count];

            for (int i = 0; i < vertices.Count; i++)
                usePoint[i] = true;

            SimplifySection(vertices, 0, vertices.Count - 1, usePoint, distanceTolerance);

            Vertices simplified = new Vertices(vertices.Count);

            for (int i = 0; i < vertices.Count; i++)
            {
                if (usePoint[i])
                    simplified.Add(vertices[i]);
            }

            return simplified;
        }

        private static void SimplifySection(Vertices vertices, int i, int j, bool[] usePoint, float distanceTolerance)
        {
            if ((i + 1) == j)
                return;

            Vector2 a = vertices[i];
            Vector2 b = vertices[j];

            double maxDistance = -1.0;
            int maxIndex = i;
            for (int k = i + 1; k < j; k++)
            {
                Vector2 point = vertices[k];

                double distance = LineTools.DistanceBetweenPointAndLineSegment(ref point, ref a, ref b);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxIndex = k;
                }
            }

            if (maxDistance <= distanceTolerance)
            {
                for (int k = i + 1; k < j; k++)
                {
                    usePoint[k] = false;
                }
            }
            else
            {
                SimplifySection(vertices, i, maxIndex, usePoint, distanceTolerance);
                SimplifySection(vertices, maxIndex, j, usePoint, distanceTolerance);
            }
        }

        private static float Cross(ref Vector2 a, ref Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        /// <summary>
        /// Merges all parallel edges in the list of vertices
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="tolerance">The tolerance.</param>
        public static Vertices MergeParallelEdges(Vertices vertices, float tolerance)
        {
            //From Eric Jordan's convex decomposition library

            if (vertices.Count <= 3)
                return vertices; //Can't do anything useful here to a triangle

            bool[] mergeMe = new bool[vertices.Count];
            int newNVertices = vertices.Count;

            //Gather points to process
            for (int i = 0; i < vertices.Count; ++i)
            {
                int lower = (i == 0) ? (vertices.Count - 1) : (i - 1);
                int middle = i;
                int upper = (i == vertices.Count - 1) ? (0) : (i + 1);

                float dx0 = vertices[middle].x - vertices[lower].x;
                float dy0 = vertices[middle].y - vertices[lower].y;
                float dx1 = vertices[upper].y - vertices[middle].x;
                float dy1 = vertices[upper].y - vertices[middle].y;
                float norm0 = Mathf.Sqrt(dx0 * dx0 + dy0 * dy0);
                float norm1 = Mathf.Sqrt(dx1 * dx1 + dy1 * dy1);

                if (!(norm0 > 0.0f && norm1 > 0.0f) && newNVertices > 3)
                {
                    //Merge identical points
                    mergeMe[i] = true;
                    --newNVertices;
                }

                dx0 /= norm0;
                dy0 /= norm0;
                dx1 /= norm1;
                dy1 /= norm1;
                float cross = dx0 * dy1 - dx1 * dy0;
                float dot = dx0 * dx1 + dy0 * dy1;

                if (Math.Abs(cross) < tolerance && dot > 0 && newNVertices > 3)
                {
                    mergeMe[i] = true;
                    --newNVertices;
                }
                else
                    mergeMe[i] = false;
            }

            if (newNVertices == vertices.Count || newNVertices == 0)
                return vertices;

            int currIndex = 0;

            //Copy the vertices to a new list and clear the old
            Vertices newVertices = new Vertices(newNVertices);

            for (int i = 0; i < vertices.Count; ++i)
            {
                if (mergeMe[i] || newNVertices == 0 || currIndex == newNVertices)
                    continue;

                System.Diagnostics.Debug.Assert(currIndex < newNVertices);

                newVertices.Add(vertices[i]);
                ++currIndex;
            }

            return newVertices;
        }

        /// <summary>
        /// Merges the identical points in the polygon.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        public static Vertices MergeIdenticalPoints(Vertices vertices)
        {
            HashSet<Vector2> unique = new HashSet<Vector2>();

            foreach (Vector2 vertex in vertices)
            {
                unique.Add(vertex);
            }

            return new Vertices(unique);
        }

        /// <summary>
        /// Reduces the polygon by distance.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="distance">The distance between points. Points closer than this will be removed.</param>
        public static Vertices ReduceByDistance(Vertices vertices, float distance)
        {
            if (vertices.Count <= 3)
                return vertices;

            float distance2 = distance * distance;

            Vertices simplified = new Vertices(vertices.Count);

            for (int i = 0; i < vertices.Count; i++)
            {
                Vector2 current = vertices[i];
                Vector2 next = vertices.NextVertex(i);

                //If they are closer than the distance, continue
                if ((next - current).sqrMagnitude <= distance2)
                    continue;

                simplified.Add(current);
            }

            return simplified;
        }

        /// <summary>
        /// Reduces the polygon by removing the Nth vertex in the vertices list.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="nth">The Nth point to remove. Example: 5.</param>
        /// <returns></returns>
        public static Vertices ReduceByNth(Vertices vertices, int nth)
        {
            if (vertices.Count <= 3)
                return vertices;

            if (nth == 0)
                return vertices;

            Vertices simplified = new Vertices(vertices.Count);

            for (int i = 0; i < vertices.Count; i++)
            {
                if (i % nth == 0)
                    continue;

                simplified.Add(vertices[i]);
            }

            return simplified;
        }

        /// <summary>
        /// Simplify the polygon by removing all points that in pairs of 3 have an area less than the tolerance.
        /// 
        /// Pass in 0 as tolerance, and it will only remove collinear points.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="areaTolerance"></param>
        /// <returns></returns>
        public static Vertices ReduceByArea(Vertices vertices, float areaTolerance)
        {
            //From physics2d.net

            if (vertices.Count <= 3)
                return vertices;

            if (areaTolerance < 0)
                throw new ArgumentOutOfRangeException("areaTolerance", "must be equal to or greater than zero.");

            Vertices simplified = new Vertices(vertices.Count);
            Vector2 v3;
            Vector2 v1 = vertices[vertices.Count - 2];
            Vector2 v2 = vertices[vertices.Count - 1];
            areaTolerance *= 2;

            for (int i = 0; i < vertices.Count; ++i, v2 = v3)
            {
                v3 = i == vertices.Count - 1 ? simplified[0] : vertices[i];

                float old1 = Cross(ref v1, ref v2);
                float old2 = Cross(ref v2, ref v3);
                float new1 = Cross(ref v1, ref v3);

                if (Math.Abs(new1 - (old1 + old2)) > areaTolerance)
                {
                    simplified.Add(v2);
                    v1 = v2;
                }
            }

            return simplified;
        }
    }
}