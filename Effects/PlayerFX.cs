using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFX : EntityFX
{
    [Header("Screen shake FX")]
    // CinemachineImpulseSource ������ͨ���ᱻ���ӵ���ҿ��ƵĶ�����κ���Ҫ������Ч���ĵط�
    private CinemachineImpulseSource screenShake;
    [SerializeField] private float shakeMultiplier;
    public Vector3 shakeSwordImpact;
    public Vector3 shakeHighDamage;


    [Header("After image fx")]
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float afterImageCooldown;
    [SerializeField] private float colorLooseRate;
    private float afterImageCooldownTimer;

    [Space]
    [SerializeField] private ParticleSystem dustFx;

    protected override void Start()
    {
        base.Start();
        screenShake = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        afterImageCooldownTimer -= Time.deltaTime;
    }

    public void ScreenShake(Vector3 _shakePower)
    {
        // m_DefaultVelocity �� CinemachineImpulseSource ����е�һ�����ԣ����������𶯵ĳ�ʼ�ٶ�
        // GenerateImpulse ����������������Ч��
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x * player.facingDir, _shakePower.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }


    public void CreateAfterImage()
    {
        if (afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;
            GameObject newAfterImage = Instantiate(afterImagePrefab, new Vector3(transform.position.x, transform.position.y + .4f), transform.rotation);
            newAfterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate, sr.sprite);
        }

    }

    public void PlayDustFX()
    {
        if (dustFx != null)
        {
            dustFx.Play();
        }
    }
}
