using System.Collections;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    protected Player player;
    protected SpriteRenderer sr;

    [Header("Pop Up Text")]
    [SerializeField] private GameObject popUpTextPrefab;

    [Header("Flash FX")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;
    private Material originalMat;


    [Header("Aliment colors")]
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    [Header("Ailment particles")]
    [SerializeField] private ParticleSystem igniteFx;
    [SerializeField] private ParticleSystem chillFx;
    [SerializeField] private ParticleSystem shockFx;

    [Header("Hit fx")]
    [SerializeField] private GameObject hitFx;
    [SerializeField] private GameObject criticalHitFx;

    private GameObject MyHealthBar;

    protected virtual void Start()
    {
        //这里为sr获取组件，而非直接赋值
        //注:如果此时Player作为parent组件有SpriteRenderer会获取到Player的SpriteRenderer，就不会让下面的Animation获取到了
        sr = GetComponentInChildren<SpriteRenderer>();
        player = PlayerManager.instance.player;
        originalMat = sr.material;

        MyHealthBar = GetComponentInChildren<UI_HealthBar>().gameObject;
    }

    public void CreatePopUpText(string _text)
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(0, 3);

        Vector3 positionOffset = new Vector3(randomX, randomY);

        GameObject newText = Instantiate(popUpTextPrefab, transform.position + positionOffset, Quaternion.identity);

        newText.GetComponent<TextMeshPro>().text = _text;
    }

    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
        {
            MyHealthBar.SetActive(false);
            sr.color = Color.clear;
        }
        else
        {
            MyHealthBar.SetActive(true);
            sr.color = Color.white;
        }
    }

    private IEnumerator FlashFX()  //又是一个协程
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originalMat;

    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
        {
            sr.color = Color.white;
        }
        else
        {
            sr.color = Color.red;  //设置这个函数让他有一个闪红白光的效果
        }
    }

    private void CancelColorChange()   //虽然是private 但是有了Invoke方法，所以仍然能调用这个方法
    {
        CancelInvoke();  //用于取消Invoke方法
        sr.color = Color.white;

        igniteFx.Stop();
        chillFx.Stop();
        shockFx.Stop();
    }
    public void IgniteFxFor(float _seconds)
    {
        igniteFx.Play();

        InvokeRepeating("IgniteColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }
    public void ChillFxFor(float _seconds)
    {
        chillFx.Play();

        InvokeRepeating("ChillColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ShockFxFor(float _seconds)
    {
        shockFx.Play();

        InvokeRepeating("ShockColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }



    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
        {
            sr.color = igniteColor[0];
        }
        else
        {
            sr.color = igniteColor[1];
        }
    }
    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
        {
            sr.color = chillColor[0];
        }
        else
        {
            sr.color = chillColor[1];
        }
    }

    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
        {
            sr.color = shockColor[0];
        }
        else
        {
            sr.color = shockColor[1];
        }
    }

    public void CreateHitFx(Transform _target, bool _critical)
    {

        float zRotation = Random.Range(-90, 90);
        float xPosition = Random.Range(-.5f, .5f);
        float yPosition = Random.Range(-.5f, .5f);

        Vector3 hitFxRotation = new Vector3(0, 0, zRotation);

        GameObject hitPrefab = hitFx;

        if (_critical)
        {
            hitPrefab = criticalHitFx;

            float yRotation = 0;
            zRotation = Random.Range(-45, 45);

            if (GetComponent<Entity>().facingDir == -1)
            {
                yRotation = 180;
            }

            hitFxRotation = new Vector3(0, yRotation, zRotation);

        }

        // 这里 加 _target 使 这个 newHitFx 变成 _target 的 子级
        GameObject newHitFx = Instantiate(hitPrefab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity);  //_target);

        newHitFx.transform.Rotate(hitFxRotation);

        Destroy(newHitFx, .5f);
    }
}
