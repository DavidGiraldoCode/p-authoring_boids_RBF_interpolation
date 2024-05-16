using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using p_bois_steering_behaviors.Scripts;

public class GridRenderer : MonoBehaviour
{
    private UniformGrid m_grid;
    [SerializeField] private int numberOfColumns = 2;
    [SerializeField] private int numberOfRows = 2;
    [SerializeField] private int numberOfLayers = 2;
    [SerializeField] private int[] minPoint = { -10, 10, -10 };
    [SerializeField] private int[] maxPoint = { 10, 10, 10 };
    //TODO Pending for abstraction =======================================
    private Vector3 sourceP1 = new Vector3(40, 0, -40);
    private Vector3 sourceP2 = new Vector3(10, 0, -10);
    private Vector3 sourceP3 = new Vector3(-10, 0, 10);
    private Vector3 sourceP4 = new Vector3(-40, 0, 40);

    private Vector3 sourceV1 = new Vector3(5, 0, 20);
    private Vector3 sourceV2 = new Vector3(-20, 0, -5);
    private Vector3 sourceV3 = new Vector3(5, 0, 20);
    private Vector3 sourceV4 = new Vector3(-20, 0, -5);
    private List<Vector3> sourcePoints = new List<Vector3>();
    private List<Vector3> sourceVectors = new List<Vector3>();
    //? Temporal LIST of interpolated vectors
    private List<Vector3> lerpVectors = new List<Vector3>();

    private double[,] matrixPHIforX;
    private double[,] matrixPHIforY;
    private double[] m_XLamdas;
    private double[] m_YLamdas;

    //TODO ------------------------------------- Vector Field Manual Control
    [Header("Manual Controller -----------")]
    [SerializeField] private GameObject sourceVectorsGO;
    private SourceVectorContainer sourceVectorContainer;

    void Awake()
    {
        sourceVectorContainer = sourceVectorsGO.GetComponent<SourceVectorContainer>();
        for (int i = 0; i < sourceVectorContainer.SourcePositions.Length; i++)
        {
            sourcePoints.Add(sourceVectorContainer.SourcePositions[i]);
            sourceVectors.Add(sourceVectorContainer.SourceVectors[i]);
        }


        Debug.Log("GridRenderer up and running");
        // sourcePoints.Add(sourceP1);
        // sourcePoints.Add(sourceP2);
        // sourcePoints.Add(sourceP3);
        // sourcePoints.Add(sourceP4);

        // sourceVectors.Add(sourceV1);
        // sourceVectors.Add(sourceV2);
        // sourceVectors.Add(sourceV3);
        // sourceVectors.Add(sourceV4);

        m_XLamdas = new double[sourcePoints.Count];
        m_YLamdas = new double[sourcePoints.Count];

        ComputeInterpolationMatricesXY(sourcePoints, sourceVectors);
        GaussianElimination(matrixPHIforX);
        GaussianElimination(matrixPHIforY);
        ComputeLamdasVector(matrixPHIforX, m_XLamdas);
        ComputeLamdasVector(matrixPHIforY, m_YLamdas);

        //* Sample points
        m_grid = new UniformGrid(numberOfColumns, numberOfRows, numberOfLayers, minPoint, maxPoint);
        foreach (Vector3 point in m_grid)
        {
            lerpVectors.Add(InterpolateVector(point));
        }
    }
    void PrintArrayItems<T>(T[] arr)
    {
        foreach (T item in arr)
        {
            Debug.Log(item);
        }
        /*
        string XrowString = "";
        int rows = matrixPHIforX.GetLength(0);
        int columns = matrixPHIforX.GetLength(1);
        //Debug.Log("rows: " + rows + " columns: " + columns);

        //Debug.Log("matrixPHIforX");
        for (int j = 0; j < rows; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                XrowString += " " + matrixPHIforX[j, i].ToString() + " ";
            }
            //Debug.Log(XrowString);
            XrowString = "";
        }
        
        */
    }

    void Update()
    {
        RenderPointUniformGrid();
        //DONE use a interation
        // int index = 0;
        // foreach (Vector3 point in sourcePoints)
        // {
        //     RenderSourceVectorAtPoint(point, sourceVectors[index]);
        //     index++;
        // }
        // index = 0;
    }

    public Vector3 InterpolateVector(Vector3 samplePoint)
    {
        float interpolantX = 0;
        float interpolantY = 0;
        //Debug.Log("sourcePoints.Count" + sourcePoints.Count);
        //Debug.Log("m_XLamdas[i]" + m_XLamdas[0]);
        for (int i = 0; i < sourcePoints.Count; i++)
        {
            interpolantX += (float)m_XLamdas[i] * (float)Phi(samplePoint, sourcePoints[i]);
            interpolantY += (float)m_YLamdas[i] * (float)Phi(samplePoint, sourcePoints[i]);
        }
        Vector3 interpolatedVector = new Vector3(interpolantX, 0, interpolantY);
        //Debug.Log("samplePoint: " + samplePoint + " interpolantXY ( " + interpolantX + ", " + interpolantY + " )");
        //Debug.Log(" InterpolateVector() ->" + interpolatedVector);
        return interpolatedVector;
    }
    private void RenderPointUniformGrid()
    {
        int index = 0;
        foreach (Vector3 point in m_grid)
        {
            Vector3 dir = lerpVectors[index];
            Vector3 direction = point + (dir * 0.2f);
            Debug.DrawLine(point, direction, Color.grey);
            index++;
        }
    }

    private void RenderSourceVectorAtPoint(Vector3 point, Vector3 vector)
    {
        Vector3 direction = point + vector;
        Debug.DrawLine(point, direction, Color.red);
    }
    //! the computation of the matrix to be laying donw on the floor is changed to (x,z)
    private void ComputeInterpolationMatricesXY(List<Vector3> points, List<Vector3> vectors) //Relationship between source points
    {
        int rows = points.Count;
        int columns = points.Count + 1;
        double[,] matrixX = new double[rows, columns];
        double[,] matrixY = new double[rows, columns];

        for (int j = 0; j < rows; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                if (i < rows)
                {
                    matrixX[j, i] = Phi(points[j], points[i]);
                    matrixY[j, i] = Phi(points[j], points[i]);
                }
                else
                {
                    matrixX[j, i] = vectors[j].x;
                    matrixY[j, i] = vectors[j].z;
                }
            }
        }

        matrixPHIforX = matrixX;
        matrixPHIforY = matrixY;
    }

    private double Phi(Vector3 vector_j, Vector3 vector_i) //RBF
    {
        Vector3 distance = vector_j - vector_i;
        float r = distance.magnitude;
        //TODO: Define the shape of the Gaussian kernel as a variable
        //Gaussian (GS)
        double GSkernel = Math.Exp(-0.001 * Math.Pow(r, 2));
        //Spline (S)
        double Skernel = r;

        return Skernel;
    }

    //TODO Make it into another class

    void GaussianElimination(double[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            // Find pivot row
            int pivotRow = i;
            for (int j = i + 1; j < rows; j++)
            {
                if (Math.Abs(matrix[j, i]) > Math.Abs(matrix[pivotRow, i]))
                {
                    pivotRow = j;
                }
            }

            // Swap rows
            if (pivotRow != i)
            {
                for (int j = i; j < cols; j++)
                {
                    double temp = matrix[i, j];
                    matrix[i, j] = matrix[pivotRow, j];
                    matrix[pivotRow, j] = temp;
                }
            }

            // Eliminate coefficients below pivot
            for (int j = i + 1; j < rows; j++)
            {
                double factor = matrix[j, i] / matrix[i, i];
                for (int k = i; k < cols; k++)
                {
                    matrix[j, k] -= factor * matrix[i, k];
                }
            }
        }
    }

    void ComputeLamdasVector(double[,] matrix, double[] lamdas)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int i = rows - 1; i >= 0; i--)
        {
            double sum = matrix[i, cols - 1];
            for (int j = i + 1; j < cols - 1; j++)
            {
                sum -= matrix[i, j] * lamdas[j];
            }
            lamdas[i] = sum / matrix[i, i];
        }
    }
}
