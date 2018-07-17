using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickAnimation_benja : MonoBehaviour {

    public Transform[] Transforms;
    public float AnimationFrequency = 1;
    public float frequencyVariation = 0.2f;
    private float timeToNext = 0;
    private int target = 0;
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
            transform.position = repos(transform.position, Transforms[target].position);
            transform.eulerAngles = rerot(transform.eulerAngles, Transforms[target].eulerAngles);
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
            if (timeToNext > 0)
                timeToNext -= Time.deltaTime;
            else
            {
                timeToNext = (float)AnimationFrequency * (1 + Mathf.Lerp(-frequencyVariation, frequencyVariation, Random.value));
                target++;
                factor = Mathf.Lerp(1, 3, Time.deltaTime - Mathf.Floor(Time.deltaTime));
            }
            if (target >= Transforms.Length) target = 0;
            transform.position = repos(transform.position, Transforms[target].position);
            transform.eulerAngles = rerot(transform.eulerAngles, Transforms[target].eulerAngles);
        }
        else this.enabled = false;    

    }


    public Vector3 repos(Vector3 from, Vector3 to)
    {
        return new Vector3(
        Mathf.SmoothDamp(from.x, to.x, ref velo.x, AnimationFrequency/ factor),
        Mathf.SmoothDamp(from.y, to.y, ref velo.y, AnimationFrequency/ factor),
        Mathf.SmoothDamp(from.z, to.z, ref velo.z, AnimationFrequency/ factor)
        );
    }

    public Vector3 rerot(Vector3 from, Vector3 to)
    {
        return new Vector3(
        Mathf.SmoothDampAngle(from.x, to.x, ref rotvelo.x, AnimationFrequency / 3),
        Mathf.SmoothDampAngle(from.y, to.y, ref rotvelo.y, AnimationFrequency / 3),
        Mathf.SmoothDampAngle(from.z, to.z, ref rotvelo.z, AnimationFrequency / 3)
        );
    }
}
