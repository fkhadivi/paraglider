using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickAnimation_benja : MonoBehaviour {

    public Transform[] Transforms;
    public float animationTime = 1;
    public float aniTimeVariation = 0.2f;
	public float rotationSpeedup = 1;
	public float timeToNext = 0;
	public int target = 0;
	public int origin = 0;
    private Vector3 velo = new Vector3();
    private Vector3 rotvelo = new Vector3();
    private float factor = 3;
    // Use this for initialization
    void Start () {
        if (Transforms.Length > 1)
        {
            for (int i = 0; i < Transforms.Length; i++)
                Transforms[i].gameObject.active = false;

            target = Random.Range(0, Transforms.Length);
			origin = target;
            transform.position = Transforms[target].position;
            transform.eulerAngles = Transforms[target].eulerAngles;
        }
        else
        {
            this.enabled = false;
        }
    }

	// Update is called once per frame
	void Update () {
        if (Transforms.Length > 1)
        {
            if (BenjasMath.countdownToZero(ref timeToNext))
            {
                factor = (float)animationTime * (1 + Mathf.Lerp(-aniTimeVariation, aniTimeVariation, Random.value));
				timeToNext = factor;
				origin = target;
                target++;

            }
            if (target >= Transforms.Length) target = 0;
			float t;
			t= BenjasMath.easeInOut(1f- timeToNext/factor);
			transform.position = Vector3.Lerp(Transforms[origin].position, Transforms[target].position, t);
			t = BenjasMath.easeInOut(1f-rotationSpeedup*timeToNext/ factor);
			transform.eulerAngles = BenjasMath.angularLerp(Transforms[origin].eulerAngles, Transforms[target].eulerAngles, t);
        }
        else this.enabled = false;    
    }



}
