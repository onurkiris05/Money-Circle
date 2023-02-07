using DG.Tweening;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material[] materials;

    private int spawnValue;
    private int spawnStrength;
    private bool inTrigger;

    private void OnEnable()
    {
        SpawnerManager.Instance.Spawners.Add(this);
    }

    public void SpawnMoney()
    {
        int rawValue = spawnValue * spawnStrength;
        if (rawValue <= 0) return;

        int incomeValue = (int)(rawValue * EconomyManager.Instance.IncomeRate);

        SpawnerManager.Instance.IncreaseWallet(incomeValue);

        //Floating text spawn and animations
        FloatingText text = FloatingTextPool.Instance.Pool.Get();
        text.transform.position = transform.position;
        text.SetText(incomeValue);
        text.Text.alpha = 1;
        text.Text.DOAlpha(0f, 1f).SetEase(Ease.InExpo);
        text.transform.DOLocalMoveY(transform.position.y + 0.3f, 1f)
            .OnComplete(() => { text.ReturnToPool(); });

        //Spawner animation
        transform.DOComplete();
        transform.DOScale(new Vector3(2f, 2f, 2f), 0.5f).From();
    }

    public void PlaySpeedUpEffect()
    {
        meshRenderer.material.DOComplete();
        meshRenderer.material.DOFade(1, 0.05f)
            .OnComplete(() => { meshRenderer.material.DOFade(0.06f, 0.3f); });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wheel"))
        {
            Wheel wheel = other.GetComponentInParent<Wheel>();

            spawnValue += wheel.Level;
            spawnStrength++;

            OnActivation();
            SetSpawner(spawnStrength);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wheel"))
        {
            Wheel wheel = other.GetComponentInParent<Wheel>();

            spawnValue -= wheel.Level;
            spawnStrength--;

            SetSpawner(spawnStrength);

            if (spawnStrength <= 0) OnDeactivation();
        }
    }

    private void SetSpawner(int index)
    {
        meshRenderer.material = materials[index];
    }

    private void OnActivation()
    {
        transform.DOComplete();
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f, 1);
    }

    private void OnDeactivation()
    {
        transform.DOComplete();
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}