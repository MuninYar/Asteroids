using Data;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;


public sealed class LaserBulletsPool : MonoBehaviour
{
    private LaserWeaponData _laserWeaponData;
    private LaserHitGenerator _laserHitGenerator;
    private ObjectPool<GameObject> _laserBulletsPool;
    public ObjectPool<GameObject> Pool => _laserBulletsPool;

    [Inject]
    public void Construct (LaserWeaponData data, LaserHitGenerator hitGenerator)
    {
        _laserWeaponData = data;
        _laserHitGenerator = hitGenerator;
    }

    private void Awake()
    {
        _laserBulletsPool = new ObjectPool<GameObject>(CreateBullet, GetBullet, ReleaseBullet,
            Destroy, true,
            _laserWeaponData.PoolCapacity, _laserWeaponData.MaxPoolCapacity);
    }

    private GameObject CreateBullet()
    {
        GameObject bullet = Instantiate(_laserWeaponData.BulletPrefab);
        LaserBulletView bulletView = bullet.GetComponent<LaserBulletView>();
        bulletView.Damage = _laserWeaponData.Damage;
        bulletView.ReleaseRequest += delegate(GameObject incomingBullet) {_laserBulletsPool.Release(incomingBullet);};
        bulletView.OnLaserBulletCollision += _laserHitGenerator.ShowLaserHit;
        return bullet;
    }

    private void GetBullet(GameObject obj)
    {
       obj.SetActive(true);
    }

    private void ReleaseBullet(GameObject obj)
    {
        obj.SetActive(false);
    }
}
