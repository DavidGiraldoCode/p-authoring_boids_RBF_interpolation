using System.Collections.Generic;
using UnityEngine;

public class SourceVectorContainer : MonoBehaviour
{
    // Start is called before the first frame update
    private SourcePointAndVector[] _sourcePointsAndVectors;
    private List<Vector3> _sourcePositions = new List<Vector3>();
    public List<Vector3> SourcePositions => _sourcePositions != null? _sourcePositions : null;
    private List<Vector3> _sourceVectors = new List<Vector3>();
    public List<Vector3> SourceVectors => _sourceVectors != null? _sourceVectors : null;
    void Awake()
    {
        _sourcePointsAndVectors = GetComponentsInChildren<SourcePointAndVector>();
        Debug.Log("Number of P and V: " + _sourcePointsAndVectors.Length);

        for (int i = 0; i < _sourcePointsAndVectors.Length; i++)
        {
            //Debug.Log("_sourcePointsAndVectors[" + i + "]");
            //Debug.Log(_sourcePointsAndVectors[i].SourcePoint);
            //Debug.Log(_sourcePointsAndVectors[i].SourceVector);
            _sourcePositions.Add(_sourcePointsAndVectors[i].SourcePoint);
            _sourceVectors.Add(_sourcePointsAndVectors[i].SourceVector);
        }

    }

    // Update is called once per frame
    void Update()
    {


    }
}
