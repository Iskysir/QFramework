using UnityEngine;
using System.Collections;

public class ChangeLight : MonoBehaviour {

    public Material _lightMaterial;
    private float _curPower;
    private float _targetPower;

    private bool _startLight = false;
	// Use this for initialization
    void OnEnable() 
    {
        StartLight();
    }

	// Update is called once per frame
    void Update()
    {
        if (_startLight)
        {
            _curPower = Mathf.Lerp(_curPower, _targetPower, Time.deltaTime*5 );
            _lightMaterial.SetFloat("_AllPower", _curPower);
            if (Mathf.Abs(_curPower - _targetPower) < 0.01f && _targetPower==0)
            {
                this.enabled = false;
            }
        }

	}
    void StartLight() 
    {
        _curPower = 0;
        _targetPower = 10;
        _startLight = true;
        Invoke("EndLight", 0.2f);
    }

    void EndLight() 
    {
        _curPower = 10;
        _targetPower = 0;
    }
}
