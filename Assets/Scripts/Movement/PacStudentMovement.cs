using System;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public sealed class PacStudentMovement : MonoBehaviour
{
    [Min(0.01f)] public float speed = 2f;
    public AudioClip movementClip;
    public Vector3[] corners = { 
        new Vector3(1, -1, 0), new Vector3(6, -1, 0), new Vector3(6, -5, 0), new Vector3(1, -5, 0) 
    };
    public int CurrentSegment { 
        get; 
        private set; 
    }
    public double DistanceTravelled { 
        get; 
        private set; 
    }
    Animator animator;
    AudioSource source;
    double perimeter;
    int lastSegment = -1;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Preview", false);
        source = GetComponent<AudioSource>();
        source.clip = movementClip; source.loop = true; source.playOnAwake = false; source.spatialBlend = 0;
        for (int i = 0; i < corners.Length; i++) {
            perimeter = perimeter + Vector3.Distance(corners[i], corners[(i + 1) % corners.Length]);
        }
        if (corners.Length < 2 || perimeter <= 0) { 
            enabled = false; 
            return; 
        }
        Advance(0);
    }
    void Start() { 
        if (source.clip != null && enabled) {
           source.Play(); 
        }
    }
    void OnDisable() { 
        if (source != null) source.Stop(); 
    }
    void Update() => Advance(Time.deltaTime);

    public void Advance(float deltaTime)
    {
        if (perimeter <= 0 || deltaTime < 0) {
           return;
        }

        DistanceTravelled = DistanceTravelled + (double)deltaTime * speed;
        double distance = DistanceTravelled % perimeter;
        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 start = corners[i], end = corners[(i + 1) % corners.Length];
            float length = Vector3.Distance(start, end);
            if (length <= 0) {
               continue;
            }
            if (distance >= length && i < corners.Length - 1) { 
                distance -= length; continue; 
            }
            CurrentSegment = i;
            transform.position = Vector3.LerpUnclamped(start, end, (float)(distance / length));
            if (lastSegment != i && animator != null)
            {
                Vector3 direction = end - start;
                string state = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ?
                    direction.x > 0 ? "WalkingRight" : "WalkingLeft" : direction.y > 0 ? "WalkingUp" : "WalkingDown";
                animator.Play(state, 0, 0f); lastSegment = i;
            }
            return;
        }
    }
}
