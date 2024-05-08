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
    [SerializeField] private int[] minPoint = { -10, -10 };
    [SerializeField] private int[] maxPoint = { 10, 10 };
    //TODO Pending for abstraction =======================================
    private Vector3 sourceP1 = new Vector3(20, 20, 0);
    private Vector3 sourceP2 = new Vector3(0, 0, 0);
    private Vector3 sourceP3 = new Vector3(-20, -20, 0);

    private Vector3 sourceV1 = new Vector3(1, 1, 0);
    private Vector3 sourceV2 = new Vector3(5, 1, 0);
    private Vector3 sourceV3 = new Vector3(-8, 8, 0);
    private List<Vector3> sourcePoints = new List<Vector3>();
    private List<Vector3> sourceVectors = new List<Vector3>();
    //? Temporal LIST of interpolated vectors
    private List<Vector3> lerpVectors = new List<Vector3>();

    private double[,] matrixPHIforX;
    private double[,] matrixPHIforY;
    private double[] m_XLamdas;
    private double[] m_YLamdas;
    void Start()
    {
        sourcePoints.Add(sourceP1);
        sourcePoints.Add(sourceP2);
        sourcePoints.Add(sourceP3);

        sourceVectors.Add(sourceV1);
        sourceVectors.Add(sourceV2);
        sourceVectors.Add(sourceV3);
        m_XLamdas = new double[sourcePoints.Count];
        m_YLamdas = new double[sourcePoints.Count];

        ComputeInterpolationMatricesXY(sourcePoints, sourceVectors);
        GaussianElimination(matrixPHIforX);
        GaussianElimination(matrixPHIforY);
        ComputeLamdasVector(matrixPHIforX, m_XLamdas);
        ComputeLamdasVector(matrixPHIforY, m_YLamdas);

        //* Sample points
        m_grid = new UniformGrid(numberOfColumns, numberOfRows, minPoint, maxPoint);
        foreach (Vector3 point in m_grid)
        {
            lerpVectors.Add(InterpolateVector(point));
        }
        //Debug.Log("lerpVectors" + lerpVectors.Count);
        //Debug.Log(lerpVectors[0]);


        /*
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
        }*/

        // Perform Gaussian elimination

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


        //GaussianElimination(matrixPHIforY);
        //ComputeLamdasVector(matrixPHIforX, m_XLamdas);
        //ComputeLamdasVector(matrixPHIforY, m_YLamdas);
        //PrintArrayItems(m_XLamdas);
        //! READ ME =========================================================
        /* DevLog 2024 05 06 7:55
        The interpolation matrix seems to work.
        I havent check if the Gaussian yields the proper ruesult.
        Check for the output of the Gassiuan matrix and the lamdas vector.
        The interpolated vector now are {0,0,0}
        */
        
    }
    void PrintArrayItems<T>(T[] arr)
    {
        // Iterate over each item in the array
        foreach (T item in arr)
        {
            // Print the item using Debug.Log
            Debug.Log(item);
        }
    }

    void Update()
    {
        RenderPointUniformGrid();
        //TODO use a interation
        RenderSourceVectorAtPoint(sourceP1, sourceV1);
        RenderSourceVectorAtPoint(sourceP2, sourceV2);
        RenderSourceVectorAtPoint(sourceP3, sourceV3);
    }
    private Vector3 InterpolateVector(Vector3 samplePoint)
    {
        float interpolantX = 0;
        float interpolantY = 0;
        for (int i = 0; i < sourcePoints.Count; i++)
        {
            interpolantX += (float)m_XLamdas[i] * (float) Phi(samplePoint, sourcePoints[i]);
            interpolantY += (float)m_YLamdas[i] * (float) Phi(samplePoint, sourcePoints[i]);
        }
        Vector3 interpolatedVector = new Vector3(interpolantX, interpolantY, 0);
        return interpolatedVector;
    }
    private void RenderPointUniformGrid()
    {
        int index = 0;
        foreach (Vector3 point in m_grid)
        {
            //int index = m_grid.GetList().IndexOf(point);
            Vector3 dir = lerpVectors[index]; // new Vector3(1, 1, 0); 
            Vector3 direction = point + (dir * 0.2f);//new Vector3(1, 1, 0);
            Debug.DrawLine(point, direction, Color.grey);
            index++;
        }
    }

    private void RenderSourceVectorAtPoint(Vector3 point, Vector3 vector)
    {
        Vector3 direction = point + vector;
        Debug.DrawLine(point, direction, Color.red);
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
                    matrixY[j, i] = vectors[j].y;
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
        double GSkernel = Math.Exp(-0.001* Math.Pow(r, 2)); //r;
        //Spline (S)
        double Skernel = r;
        
        //r == 0? 0.00000001 : 1.0 / (1.0 + (0.4 * Math.Pow(r, 2))); //Math.Pow(r, 2) * Math.Log(r);
        return Skernel;
    }

    //TODO Make it into another class

    void GaussianElimination(double[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        int[] rowOrder = new int[rows];

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
                    //!----
                    rowOrder[i] = pivotRow;
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
        Debug.Log("rowOrder: ");
        PrintArrayItems(rowOrder);
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

        //double[] result = new double[rows];

        for (int i = rows - 1; i >= 0; i--)
        {
            double sum = matrix[i, cols - 1];
            for (int j = i + 1; j < cols - 1; j++)
            {
                //sum -= matrix[i, j] * result[j];
                sum -= matrix[i, j] * lamdas[j];
            }
            //result[i] = sum / matrix[i, i];
            lamdas[(rows-1) - i] = sum / matrix[i, i];
        }
        //lamdas = result;
        //PrintArrayItems(result);
        //Console.WriteLine("Resulting vector:");
        for (int i = 0; i < rows; i++)
        {
            //Console.WriteLine($"x[{i}] = {result[i]}");
        }
    }
}
