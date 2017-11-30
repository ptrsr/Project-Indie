using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private Shader _shader;

    [SerializeField]
    private float
        _maxAngle, // max angle for each side in degrees
        _rotateSpeed,
        _diameter,
        _effectMulti,
        _timeMulti = 1,
        _darken = 1;

    [SerializeField]
    private int
        _bounces = 1;

    [SerializeField]
    private Color _color;

    private Vector2 _currentAngle;

    private Material _mat;

    private GameObject[] _parts;

    private void Start()
    {
        _parts = new GameObject[_bounces + 1];

        for (int i = 0; i < _parts.Length; i++)
        {
            //Material mat = new Material(_shader);

            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(cylinder.GetComponent<Collider>());
            cylinder.transform.parent = transform;
            _parts[i] = cylinder;
            //cylinder.GetComponent<MeshRenderer>().material = mat;
        }
    }

    private void Update()
    {
        Vector3 direction = transform.forward;
        Vector3 position = transform.position;

        for (int i = 0; i < _bounces + 1; i++)
        {
            _parts[i].SetActive(true);

            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            Physics.Raycast(ray, out hit);

            Transform trans = _parts[i].transform;

            trans.position = (position + hit.point) / 2;
            trans.localScale = new Vector3(_diameter, Vector3.Distance(position, hit.point), _diameter);
            //trans.rotation = Quaternion.LookRotation(new Vector3(direction.z, 0, -direction.x).normalized, Vector3.forward);
            trans.forward = direction;
            print(trans.forward);

            Collider col = hit.collider;
            if (col != null && col.GetComponentInParent<PlayerController>() != null)
            {
                for (int j = i + 1; j < _bounces + 1; j++)
                    _parts[j].SetActive(false);

                break;
            }

            position = hit.point;
            direction = Vector3.Reflect(direction, hit.normal);
        }

    }

    private Material InitShader(Shader shader)
    {
        Material mat = new Material(shader);
        _mat = mat;
        SetUniforms(mat);
        return mat;
    }

    private void SetUniforms(Material mat)
    {
        mat.SetColor("_color", _color);
        mat.SetFloat("_effectMulti", _effectMulti);
        mat.SetFloat("_darken", _darken);
    }
}
