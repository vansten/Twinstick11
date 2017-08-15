using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables

    [Header("Camera settings")]
    [SerializeField]
    protected float _angle;
    [SerializeField]
    protected float _offset;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    protected float _cameraSpeed;

    [Header("Mesh control")]
    [SerializeField]
    protected LayerMask _obscuringLayers;
    [SerializeField]
    protected float _hiddenAlpha;
    [SerializeField]
    protected float _alphaChangeSpeed;

    [SerializeField]
    protected Transform _target;
    [SerializeField]
    protected Transform _crosshair;

    protected List<MeshRenderer> _obscuringMeshes = new List<MeshRenderer>();

    #endregion

    #region Unity Methods

    protected void OnValidate()
    {
        transform.forward = (Quaternion.Euler(-_angle, 0.0f, 0.0f) * Vector3.down).normalized;
        if (_target != null)
        {
            transform.position = _target.position + _offset * -transform.forward;
        }
    }

    protected void Start()
    {
        OnValidate();
    }

    protected void Update()
    {
        ProcessObscurance();
    }

    protected void LateUpdate()
    {
        Vector3 targetPosition = _target.position + _offset * -transform.forward;
        transform.position = Vector3.Lerp(transform.position, targetPosition, _cameraSpeed);
    }

    #endregion

    #region Methods

    protected void ProcessObscurance()
    {
        List<MeshRenderer> meshes = GetAllObstaclesObscuringTarget(_target);
        meshes.AddRange(GetAllObstaclesObscuringTarget(_crosshair));
        List<MeshRenderer> toRemove = new List<MeshRenderer>();
        foreach(MeshRenderer mr in _obscuringMeshes)
        {
            if(!meshes.Contains(mr))
            {
                Material mat = mr.material;
                Color c = mat.color;
                c.a += _alphaChangeSpeed * Time.deltaTime;
                mat.color = c;
                mr.material = mat;

                if(c.a >= 1.0f)
                {
                    toRemove.Add(mr);
                }
            }
        }


        foreach (MeshRenderer mr in meshes)
        {
            Material mat = mr.material;
            Color c = mat.color;
            c.a = Mathf.Clamp(c.a - Time.deltaTime * _alphaChangeSpeed, _hiddenAlpha, 1.0f);
            mat.color = c;
            mr.material = mat;
            if(!_obscuringMeshes.Contains(mr))
            {
                _obscuringMeshes.Add(mr);
            }
        }

        foreach(MeshRenderer mr in toRemove)
        {
            _obscuringMeshes.Remove(mr);
        }
    }

    protected List<MeshRenderer> GetAllObstaclesObscuringTarget(Transform target)
    {
        List<MeshRenderer> toReturn = new List<MeshRenderer>();

        Vector3 direction = (target.position - transform.position);
        Ray ray = new Ray(transform.position, direction.normalized);
        RaycastHit[] hits = Physics.RaycastAll(ray, direction.magnitude, _obscuringLayers.value, QueryTriggerInteraction.Ignore);
        if(hits.Length > 0)
        {
            toReturn = hits.Select<RaycastHit, MeshRenderer>(GetRendererFromHit).Where((mr) => { return mr != null; }).ToList();
        }

        return toReturn;
    }

    protected MeshRenderer GetRendererFromHit(RaycastHit hit)
    {
        if(hit.collider == null)
        {
            return null;
        }

        return hit.collider.gameObject.GetComponent<MeshRenderer>();
    }

    #endregion
}
