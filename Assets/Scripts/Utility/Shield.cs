using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField]
    private Shader _shader;
    private Material _mat;

    [SerializeField]
    private Texture _texture;

    [SerializeField]
    private Color _color;

    [SerializeField]
    private float
        _tiling = 1,
        _fallOff = 1,
        _warp = 1,
        _timeMulti = 1,
        _noiseMulti = 1,
        _noisePow = 1;

    void Start ()
    {
        _mat = new Material(_shader);

        SetUniforms(_mat);

        GetComponent<MeshRenderer>().material = _mat;
	}

    private void Update()
    {
        _mat.SetFloat("_time", Time.timeSinceLevelLoad * _timeMulti);
    }

    private void OnValidate()
    {
        if (!Application.isPlaying || _mat == null)
            return;

        SetUniforms(_mat);
    }

    private void SetUniforms(Material mat)
    {
        _mat.SetTexture("_MainTex", _texture);
        _mat.SetFloat("_tiling", _tiling);
        _mat.SetFloat("_fallOff", _fallOff);
        _mat.SetFloat("_warp", _warp);
        _mat.SetColor("_baseColor", _color);
        _mat.SetFloat("_noiseMulti", _noiseMulti);
        _mat.SetFloat("_noisePow", _noisePow);
    }
}
