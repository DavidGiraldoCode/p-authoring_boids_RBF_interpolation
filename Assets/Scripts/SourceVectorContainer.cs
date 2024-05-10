using System.Collections.Generic;
using UnityEngine;

public class SourceVectorContainer : MonoBehaviour
{
    // Start is called before the first frame update
    private SourcePointAndVector[] _sourcePointsAndVectors;
    private Vector3[] _sourcePositions;
    public Vector3[] SourcePositions => _sourcePositions != null ? _sourcePositions : null;
    private Vector3[] _sourceVectors;
    public Vector3[] SourceVectors => _sourceVectors != null ? _sourceVectors : null;
    void Awake()
    {
        _sourcePointsAndVectors = GetComponentsInChildren<SourcePointAndVector>();
        _sourcePositions = new Vector3[_sourcePointsAndVectors.Length];
        _sourceVectors = new Vector3[_sourcePointsAndVectors.Length];

        Debug.Log("Number of P and V: " + _sourcePointsAndVectors.Length);

        for (int i = 0; i < _sourcePointsAndVectors.Length; i++)
        {
            //Debug.Log("_sourcePointsAndVectors[" + i + "]");
            //Debug.Log(_sourcePointsAndVectors[i].SourcePoint);
            //Debug.Log(_sourcePointsAndVectors[i].SourceVector);
            _sourcePositions[i] = _sourcePointsAndVectors[i].SourcePoint;
            _sourceVectors[i] = _sourcePointsAndVectors[i].SourceVector;
            Debug.Log("_sourceVectors["+i+"]" +  _sourceVectors[i]);
        }
        
    }

    // Update is called once per frame
    void Update()
    {


    }
}
