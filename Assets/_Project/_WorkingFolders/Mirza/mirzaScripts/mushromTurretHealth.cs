using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mushromTurretHealth : MonoBehaviour, ICombat
{
    [SerializeField] private int currentHealth;

    [Header("Damage Flash Settings")]
    [SerializeField] private List<SkinnedMeshRenderer> renderers;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;

    private Coroutine flashRoutine;
    public Transform GetTransform()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
       
        DamagePopUpGenerator.instance.CreatePopUp(transform.position + Vector3.up * 2, damage.ToString());

       if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator FlashEffect()
    {
        SetMaterials(flashMaterial);
        Debug.Log("flashlandý");
        yield return new WaitForSeconds(flashDuration);
        ResetMaterials();
    }

    private void SetMaterials(Material mat)
    {
        foreach (var renderer in renderers)
        {
            if (renderer != null)
                renderer.material = mat;
        }
    }

    private void ResetMaterials()
    {
        SetMaterials(normalMaterial);
    }
}
