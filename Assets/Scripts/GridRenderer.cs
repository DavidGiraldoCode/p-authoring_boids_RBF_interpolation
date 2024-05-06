using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using p_bois_steering_behaviors.Scripts;
public class GridRenderer : MonoBehaviour
{
    private UniformGrid m_grid;
    [SerializeField] private int numberOfColumns = 32;
    [SerializeField] private int numberOfRows = 32;
    [SerializeField] private int[] minPoint = { -40, -40 };
    [SerializeField] private int[] maxPoint = { 40, 40 };
    //? Pending for abstraction =======================================
    private Vector3 sourceP1 = new Vector3(20, 20, 0);
    private Vector3 sourceP2 = new Vector3(-20, -20, 0);
    private Vector3 sourceP3 = new Vector3(0, 0, 0);

    private Vector3 sourceV1 = new Vector3(5, 10, 0);
    private Vector3 sourceV2 = new Vector3(-2, 6, 0);
    private Vector3 sourceV3 = new Vector3(8, 0, 0);
    private List<Vector3> sourcePoints = new List<Vector3>();
    private List<Vector3> sourceVectors = new List<Vector3>();

    private double[,] matrixPHIforX;
    private double[,] matrixPHIforY;
    private double[] m_XLamdas;
    private double[] m_YLamdas;
    void Start()
    {
        m_grid = new UniformGrid(numberOfColumns, numberOfRows, minPoint, maxPoint);

        sourcePoints.Add(sourceP1);
        sourcePoints.Add(sourceP2);
        sourcePoints.Add(sourceP3);

        sourceVectors.Add(sourceV1);
        sourceVectors.Add(sourceV2);
        sourceVectors.Add(sourceV3);
        m_XLamdas = new double[sourcePoints.Count];
        m_YLamdas = new double[sourcePoints.Count];

        ComputeInterpolationMatrix(sourcePoints, sourceVectors);

        string XrowString = "";
        string YrowString = "";
        // Get the number of rows (first dimension)
        int rows = matrixPHIforX.GetLength(0);

        // Get the number of columns (second dimension)
        int columns = matrixPHIforX.GetLength(1);
        Debug.Log("matrixPHIforX");
        for (int j = 0; j < rows; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                XrowString += " " + matrixPHIforX[j, i].ToString() + " ";
            }
            Debug.Log(XrowString);
            XrowString = "";
        }
        Debug.Log("matrixPHIforY");
        for (int j = 0; j < rows; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                YrowString += " " + matrixPHIforY[j, i].ToString() + " ";
            }
            Debug.Log(YrowString);
            YrowString = "";
        }

        // Perform Gaussian elimination
        GaussianElimination(matrixPHIforX);
        GaussianElimination(matrixPHIforY);
        ComputeLamdasVector(matrixPHIforX, m_XLamdas);
        ComputeLamdasVector(matrixPHIforY, m_YLamdas);



    }

    void Update()
    {
        RenderPointUniformGrid();
        //TODO use a interation
        RenderSourceVectorAtPoint(sourceP1, sourceV1);
        RenderSourceVectorAtPoint(sourceP2, sourceV2);
        RenderSourceVectorAtPoint(sourceP3, sourceV3);


    }
    private void RenderPointUniformGrid()
    {
        foreach (Vector3 point in m_grid)
        {
            Vector3 direction = point + new Vector3(1, 1, 0);
            Debug.DrawLine(point, direction, Color.yellow);
        }
    }

    private void RenderSourceVectorAtPoint(Vector3 point, Vector3 vector)
    {
        Vector3 direction = point + vector;
        Debug.DrawLine(point, direction, Color.blue);
    }

    /*
    double[,] matrix = { //WithBoundY
            {0.0, 720.0, 720.0, 1018.2, 792.0, 226.3, 509.1, 0},
            {720.0, 0.0, 1018.2, 720.0, 582.4, 582.4, 509.1, 0},
            {720.0, 1018.2, 0.0, 720.0, 582.4, 582.4, 509.1, 0},
            {1018.2, 720.0, 720.0, 0.0, 226.3, 792.0, 509.1, 0},
            {792.0, 582.4, 582.4, 226.3, 0.0, 565.7, 282.8, 50},
            {226.3, 582.4, 582.4, 792.0, 565.7, 0.0, 282.8, -20},
            {509.1, 509.1, 509.1, 509.1, 282.8, 282.8, 0.0, 80}
        };
    
    */
    private void ComputeInterpolationMatrix(List<Vector3> points, List<Vector3> vectors) //Relationship between source points
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
                    matrixY[j, i] = vectors[j].y;
                }
            }
        }

        matrixPHIforX = matrixX;
        matrixPHIforY = matrixY;
    }

    private float Phi(Vector3 vector_i, Vector3 vector_j)
    {

        Vector3 distance = vector_j - vector_i;
        //Debug.Log("vector_i: " + vector_i + " - vector_j: " + vector_j + " = " + distance);
        float kernel = distance.magnitude;
        return kernel;
    }

    //TODO Make it into another class

    static void GaussianElimination(double[,] matrix)
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

    static void PrintMatrix(double[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                //Console.Write(matrix[i, j] + "\t");
            }
            //Console.WriteLine();
        }
    }

    static void ComputeLamdasVector(double[,] matrix, double[] lamdas)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        double[] result = new double[rows];

        for (int i = rows - 1; i >= 0; i--)
        {
            double sum = matrix[i, cols - 1];
            for (int j = i + 1; j < cols - 1; j++)
            {
                sum -= matrix[i, j] * result[j];
            }
            result[i] = sum / matrix[i, i];
        }
        lamdas = result;
        //Console.WriteLine("Resulting vector:");
        for (int i = 0; i < rows; i++)
        {
            //Console.WriteLine($"x[{i}] = {result[i]}");
        }
    }
}
