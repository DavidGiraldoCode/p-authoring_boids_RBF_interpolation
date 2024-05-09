using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace p_bois_steering_behaviors.Scripts
{
    public class UniformGrid : IEnumerable<Vector3>
    {
        private int m_nx, m_ny, m_nCols, m_nRows; 
        private float m_dx, m_dy;
        private List<Vector3> m_samplePoints; //holds 3D coordinates in space
        public UniformGrid(int nx, int ny, int[] minPoint, int[] maxPoint)
        {
            m_nx = nx <= 1 ? 2 : nx;
            m_ny = ny <= 1 ? 2 : ny;

            m_nCols = m_nx - 1; //number of rows cells
            m_nRows = m_ny - 1; //number of columns cells

            m_samplePoints = new List<Vector3> { Capacity = m_nx * m_ny };

            int xMin = minPoint[0];
            int yMin = minPoint[1];
            int xMax = maxPoint[0];
            int yMax = maxPoint[1];

            m_dx = (xMax - xMin) / (float) m_nCols; // Cell's width
            m_dy = m_ny > 1 ? (yMax - yMin) / (float)  m_nRows : 0; // Cell's height

            for (int y = 0; y < m_ny; y++)
            {
                for (int x = 0; x < m_nx; x++)
                {
                    int linearIndex = (y * m_nx) + x;

                    float Vx = (x * m_dx) + xMin;
                    float Vy = 0f;//(y * m_dy) + yMin;
                    float Vz = (y * m_dy) + yMin;// 0f; // for now.

                    m_samplePoints.Add(new Vector3(Vx, Vy, Vz));
                }
            }
        }
        //! Needs refactor to use it with C# List
        public Vector3 GetSamplePosition(int linearIndex)
        {
            if (linearIndex > m_samplePoints.Count)
            {
                return m_samplePoints[m_samplePoints.Count - 1];
            }
            else if (linearIndex < 0)
            {
                return m_samplePoints[0];
            }
            return m_samplePoints[linearIndex];
        }
        //! STUDY --------------------------------------
        // Method to get enumerator for iteration
        public IEnumerator<Vector3> GetEnumerator()
        {
            return m_samplePoints.GetEnumerator();
        }

        // Explicit interface implementation for non-generic IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // Call the generic version of GetEnumerator
        }
        public int GetSize()
        {
            return m_samplePoints.Count;
        }

        public List<Vector3> GetList()
        {
            return m_samplePoints;
        }

    }

}
