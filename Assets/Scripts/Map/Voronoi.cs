using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Voronoi
{
    #region algorithm
    //function BowyerWatson(pointList)
    //  // pointList is a set of coordinates defining the points to be triangulated
    //  triangulation := empty triangle mesh data structure
    //  add super-triangle to triangulation // must be large enough to completely contain all the points in pointList
    //  for each point in pointList do // add all the points one at a time to the triangulation
    //     badTriangles := empty set
    //     for each triangle in triangulation do // first find all the triangles that are no longer valid due to the insertion
    //        if point is inside circumcircle of triangle
    //           add triangle to badTriangles
    //     polygon := empty set
    //     for each triangle in badTriangles do // find the boundary of the polygonal hole
    //        for each edge in triangle do
    //           if edge is not shared by any other triangles in badTriangles
    //              add edge to polygon
    //     for each triangle in badTriangles do // remove them from the data structure
    //        remove triangle from triangulation
    //     for each edge in polygon do // re-triangulate the polygonal hole
    //        newTri := form a triangle from edge to point
    //        add newTri to triangulation
    //  for each triangle in triangulation // done inserting points, now clean up
    //     if triangle contains a vertex from original super-triangle
    //        remove triangle from triangulation
    //  return triangulation
    #endregion

    class TriangleMesh
    {
        List<Triangle> triangles;
        public TriangleMesh(List<Triangle> tris)
        {
            triangles = tris;
        }
    }
    class Triangle
    {
        public Triangle(Vector2 vert0, Vector2 vert1, Vector2 vert2)
        {
            vertices.Add(vert0);
            vertices.Add(vert1);
            vertices.Add(vert2);
        }
        public List<Vector2> vertices = new List<Vector2>();
    }

    List<Vector2> pointList = new List<Vector2>();
    List<Triangle> goodTriangles = new List<Triangle>();
    List<Triangle> badTriangles = new List<Triangle>();

    class Polygon
    {
        List<Edge> edges = new List<Edge>();
        public void AddEdge(Edge edge)
        {
            edges.Add(edge);
        }
    }

    class Edge
    {
        Vector3 point0;
        Vector3 point1;
    }

    public Voronoi(List<Vector2> points)
    {
        pointList = points;

        AddSuperTriangle();
        var circumCenter = CalculateCircumcircle(goodTriangles[0]);
        CircumCircleContains(circumCenter, goodTriangles[0], points[0]);
    }

    //helpers
    void AddSuperTriangle()
    {
        var boundingPoints = GetBoundingPoints();
        var position = new Vector2(boundingPoints[0], boundingPoints[1]);
        var size = new Vector2(boundingPoints[2] - boundingPoints[0], boundingPoints[3] - boundingPoints[1]);

        var point0 = new Vector2(position.x - size.x, position.y - 1);
        var point1 = new Vector2(position.x + (size.x * 2), point0.y); 
        var point2 = new Vector2((point0.x + point1.x) / 2, size.y * 3);

        AddTriangle(point0, point1, point2);
    }
    float[] GetBoundingPoints()
    {
        var boundingPoints = new float[4];

        boundingPoints[0] = pointList.Min(point => point.x);
        boundingPoints[1] = pointList.Min(point => point.y);
        boundingPoints[2] = pointList.Max(point => point.x);
        boundingPoints[3] = pointList.Max(point => point.y);

        return boundingPoints;
    }
    void AddTriangle(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        var triangle = new Triangle(p0, p1, p2);
        goodTriangles.Add(triangle);
    }
    Vector2 CalculateCircumcircle(Triangle tri)
    {
        //-find midpoints of AB and AC
        var midPointAB = new Vector2((tri.vertices[0].x + tri.vertices[1].x) / 2, (tri.vertices[0].y + tri.vertices[1].y) / 2);
        var midPointAC = new Vector2((tri.vertices[0].x + tri.vertices[2].x) / 2, (tri.vertices[0].y + tri.vertices[2].y) / 2);

        //-find perpendicular bisectors for AB and AC by taking the negative inverse of the vectors
        var vectorAB = tri.vertices[1] - tri.vertices[0];
        var vectorAC = tri.vertices[2] - tri.vertices[0];
        var vectorABPerpBisector = new Vector2(-vectorAB.y, vectorAB.x) / Mathf.Sqrt(Mathf.Pow(vectorAB.x, 2.0f) + Mathf.Pow(vectorAB.y, 2)) * midPointAB.magnitude;
        var vectorACPerpBisector = new Vector2(-vectorAC.y, vectorAC.x) / Mathf.Sqrt(Mathf.Pow(vectorAC.x, 2.0f) + Mathf.Pow(vectorAC.y, 2)) * midPointAC.magnitude;

        var abPerpBiPoint0 = new Vector2(midPointAB.x + vectorABPerpBisector.x, midPointAB.y + vectorABPerpBisector.y);
        var abPerpBiPoint1 = new Vector2(midPointAB.x - vectorABPerpBisector.x, midPointAB.y - vectorABPerpBisector.y);
        var acPerpBiPoint0 = new Vector2(midPointAC.x + vectorACPerpBisector.x, midPointAC.y + vectorACPerpBisector.y);
        var acPerpBiPoint1 = new Vector2(midPointAC.x - vectorACPerpBisector.x, midPointAC.y - vectorACPerpBisector.y);

        //-the intersection between those two lines is the centerpoint of the circumcircle
        var intersectionX = 0.0f;
        var intersectionY = 0.0f;
        var abXdiff = abPerpBiPoint1.x - abPerpBiPoint0.x;
        var abYdiff = abPerpBiPoint1.y - abPerpBiPoint0.y;
        var acXdiff = acPerpBiPoint1.x - acPerpBiPoint0.x;
        var acYdiff = acPerpBiPoint1.y - acPerpBiPoint0.y;

        if(abXdiff == 0) // vertical line has no y intercept
        {
            //M
            var acM = acYdiff / acXdiff;

            //B
            var acB = acPerpBiPoint0.y - (acM * acPerpBiPoint0.x);

            intersectionX = abPerpBiPoint0.x;
            intersectionY = acM * abPerpBiPoint0.x + acB;

        }
        else
        {
            //M
            var abM = abYdiff / abXdiff;
            var acM = acYdiff / acXdiff;

            //B
            var abB = abPerpBiPoint0.y - (abM * abPerpBiPoint0.x);
            var acB = acPerpBiPoint0.y - (acM * acPerpBiPoint0.x);

            intersectionX = (acB - abB) / (abM - acM);
            intersectionY = abM * intersectionX + abB;
        }

        return new Vector2(intersectionX, intersectionY);
    }
    bool CircumCircleContains(Vector2 circumCenter, Triangle tri, Vector2 point)
    {
        //-the radius is the distance between the center point and any of the three points A, B and C
        var radius = Mathf.Sqrt(Mathf.Pow(circumCenter.x - tri.vertices[0].x, 2) + Mathf.Pow(circumCenter.y - tri.vertices[0].y, 2));
        //-create a vector from circumcenter to point in question and note the magnitude
        var vectorToPoint = circumCenter - point;
        return vectorToPoint.magnitude <= radius;
    }
}