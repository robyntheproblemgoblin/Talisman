using System.Drawing;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.Splines;

namespace AISystem.Pathing
{
    public struct Path
    {
        public float3 m_destination { get; private set; }
        public float3 m_heading { get; private set; }
        public float m_length { get; private set; }

        public Spline m_pathSpline;
        public bool m_isEmpty => m_pathSpline == null;

        bool m_isNewPath;

        public Path(Spline spline, float3 dest, float3 heading)
        {
            m_destination = dest;
            m_heading = heading;
            m_length = spline.GetLength();
            m_pathSpline = spline;
            m_isNewPath = false;
        }

        public static Path Empty => new Path()
        {
            m_destination = float3.zero,
            m_heading = float3.zero,
            m_length = 0f,
            m_pathSpline = null,
        };

        public void DebugIssues(float3 point, float distance)
        {
            SplineUtility.GetNearestPoint(m_pathSpline, point, out float3 nearest, out float nearestT);

            float agentDist = nearestT * m_length;
            float targetDist = agentDist + distance;

            if(nearestT != 0f)
            {
                UnityEngine.Debug.Log("Error " + nearestT + " and the state of Path is "  + m_isNewPath);
            else
{
UnityEngine.Debug.Log(m_isNewPath);
}
        }


        public void GetRelativePoint(float3 point, float distance, out float3 position, out float3 tangent)
        {
            SplineUtility.GetNearestPoint(m_pathSpline, point, out float3 nearest, out float nearestT);
            float agentDist = nearestT * m_length;
            float targetDist = agentDist + distance;

            if (targetDist >= m_length)
            {
                position = m_destination;
                tangent = m_heading;
                m_isNewPath = false;
UnityEngine.Debug.Log("Target distance is greater than spline length?");
            }
            else
            {
                float t = targetDist / m_length;
                m_pathSpline.Evaluate(t, out position, out tangent, out float3 up);
                m_isNewPath = true;
UnityEngine.Debug.Log("Path Revaluated");
            }
        }
    }
}
