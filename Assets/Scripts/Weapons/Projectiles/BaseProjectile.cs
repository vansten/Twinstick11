using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    #region Variables

    [SerializeField]
    protected float _lifetime;

    protected Vector3 _direction;
    protected float _speed;
    protected float _damage;

    protected float _timer;

    #endregion

    #region Unity Methods

    protected void Update()
    {
        ProcessTranslation();

        _timer += Time.deltaTime;
        if(_timer >= _lifetime)
        {
            gameObject.SetActive(false);
        }
    }

    protected void OnTriggerEnter(Collider collider)
    {
        HandleCollision(collider);
    }

    #endregion

    #region Methods

    public void Shoot(Vector3 direction, float speed, float damage)
    {
        _direction = direction;
        _speed = speed;
        _damage = damage;

        transform.forward = _direction;
        transform.parent = null;

        _timer = 0.0f;
    }

    protected void ProcessTranslation()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }

    protected abstract void HandleCollision(Collider collider);

    #endregion
}
