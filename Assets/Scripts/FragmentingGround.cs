using UnityEngine;
using System.Collections;

public class FragmentingGround : MonoBehaviour
{
    [Header("Fragment Settings")]
    public float fragmentDelay = 10f;
    public GameObject[] fragmentPrefabs; // Individual pieces
    public float explosionForce = 5f;
    public float explosionRadius = 5f;
    
    [Header("Warning Effects")]
    public float warningTime = 3f; // Start shaking 3 seconds before
    public float shakeIntensity = 0.1f;
    
    private bool hasFragmented = false;
    private float timer = 0f;
    private Vector3 originalPosition;
    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    
    [Header("Visual Feedback")]
    public Material warningMaterial; // Red/cracking material
    
    void Start()
    {
        originalPosition = transform.position;
        meshRenderer = GetComponent<MeshRenderer>();
        
        if (meshRenderer != null)
            originalMaterial = meshRenderer.material;
    }
    
    void Update()
    {
        // Don't update timer if game is paused
        if (PauseMenuController.isPaused)
            return;
        
        timer += Time.deltaTime;
        
        // Warning phase
        if (timer >= fragmentDelay - warningTime && timer < fragmentDelay)
        {
            // Shake ground
            float shake = Mathf.Sin(Time.time * 20f) * shakeIntensity;
            transform.position = originalPosition + new Vector3(shake, 0, shake);
            
            // Change color to red
            if (meshRenderer != null && warningMaterial != null)
                meshRenderer.material = warningMaterial;
        }
        
        // Fragment!
        if (timer >= fragmentDelay && !hasFragmented)
        {
            Fragment();
        }
    }
    
    void Fragment()
    {
        hasFragmented = true;
        Debug.Log("Ground fragmenting!");
        
        // Spawn fragments
        if (fragmentPrefabs != null && fragmentPrefabs.Length > 0)
        {
            foreach (GameObject fragmentPrefab in fragmentPrefabs)
            {
                GameObject fragment = Instantiate(fragmentPrefab, transform.position, transform.rotation);
                
                // Add physics
                Rigidbody rb = fragment.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Add explosion force
                    Vector3 explosionPos = transform.position - Vector3.up;
                    rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius);
                    
                    // Add some randomness
                    rb.AddTorque(Random.insideUnitSphere * 2f);
                }
            }
        }
        
        // Destroy original platform
        Destroy(gameObject);
    }
    
    // Call this to reset timer (useful for testing)
    public void ResetTimer()
    {
        timer = 0f;
        hasFragmented = false;
        transform.position = originalPosition;
        
        if (meshRenderer != null && originalMaterial != null)
            meshRenderer.material = originalMaterial;
    }
}