using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class experimentaAnimationPresets : MonoBehaviour {



    public string info = "for notes, wont influence the script";
    public bool visible = true;
    private bool lastFrameVisible = true;
    public bool alwaysCompleteTransition = false;
    private bool dontTouchAlpha = false;


    private float visibilityTarget = 1;
    private float visibilityTargetFinal = 1;
    public float visibility = 1;
    private bool easeIn = false;
    private bool easeOut = false;
    private float transitionTime;
    public float delay = 0;
    public  bool inTransition = false;
    private float[] defaultAlphaImages;
    private float[] defaultAlphaTexts;
    private RectTransform trafo;
    private Image[] images;
    private Text[] texts;
    public bool forceReset = false;

    public bool introEaseIn = false;
    public bool introEaseOut = false;
    public float introTime = 1;
    public float introDelay = 0;
    public Vector3 introOffset;
    public float introScale =1;
    public float introAlpha =1;

    
    public bool outroEaseIn = false;
    public bool outroEaseOut = false;
    public float outroTime = 1;
    public float outroDelay = 0;
    public bool outroLikeIntro = true;
    public Vector3 outroOffset;
    public float outroScale = 1;
    public float outroAlpha = 1;


    public Keyframe intro;
    public Keyframe idle;
    public Keyframe outro;
    public Keyframe target;
 
    public class Keyframe
    {
        public Vector3 position;
        public Vector3 scale;
        public float alpha;

        /*
        public int ease = 0;
        public float transitionTime = 1;

        public Keyframe(RectTransform nullTrafo, 
                        Vector3 offset = new Vector3(), 
                        float newScale = 1f, 
                        float newAlpha = 1f,
                        int newEase = 0,
                        float newTransitionTime = 0)
        {
            position = nullTrafo.anchoredPosition3D + offset;
            scale = nullTrafo.localScale*newScale;
            alpha = newAlpha;
            ease = newEase;
            if (newTransitionTime < 0)
                transitionTime = -1;
            else if (newTransitionTime > 0)
                transitionTime = 1;
            else
                transitionTime = 0;
        }*/

        public Keyframe(RectTransform nullTrafo,
                Vector3 offset = new Vector3(),
                float newScale = 1f,
                float newAlpha = 1f)
        {
            
            position = nullTrafo.anchoredPosition3D + offset;
            scale = nullTrafo.localScale * newScale;
            alpha = newAlpha;
        }
    }

    // Use this for initialization
    void Start () {
        forceReset = false;
        trafo = gameObject.GetComponent<RectTransform>();
        if (introAlpha == 1 && outroAlpha == 1)
        {
            dontTouchAlpha = true;
        }
        else
        {
            
            images = GetComponentsInChildren<Image>();
            defaultAlphaImages = new float[images.Length];
            for (int i = 0; i < images.Length; i++)
            {
                defaultAlphaImages[i] = images[i].color.a;
            }
            texts = GetComponentsInChildren<Text>();
            defaultAlphaTexts = new float[texts.Length];
            for (int i = 0; i < texts.Length; i++)
            {
                defaultAlphaTexts[i] = texts[i].color.a;
            }
        }
        idle = new Keyframe(trafo, Vector3.zero);
        intro = new Keyframe(trafo, introOffset, introScale, introAlpha);
        if(outroLikeIntro) outro = new Keyframe(trafo, introOffset, introScale, introAlpha);
        else outro = new Keyframe(trafo, outroOffset, outroScale, outroAlpha);
    }

    private void setAlpa(float alpha)
    {
        if (!dontTouchAlpha)
        {
            Mathf.Clamp01(alpha);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i] != null)
                {
                    Color col = images[i].color;
                    col.a = defaultAlphaImages[i] * alpha;
                    images[i].color = col;
                }
            }
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i] != null)
                {
                    Color col = texts[i].color;
                    col.a = defaultAlphaTexts[i] * alpha;
                    texts[i].color = col;
                }
            }
        }
    }

    public bool isReady = true;

    public void updateTransition()
    {
        if (visibility != visibilityTarget)
        {
            if (inTransition || BenjasMath.countdownToZero(ref delay))
            {
                inTransition = true;
                visibility = Mathf.Clamp01(visibility + Time.deltaTime / transitionTime);

                float t = visibility;

                if (easeIn) if (easeOut) t = BenjasMath.easeInOut(t);
                    else                 t = BenjasMath.easeIn(t);
                else if (easeOut)        t = BenjasMath.easeOut(t);
                //t goes automaticly backwards when going out
                trafo.anchoredPosition3D = Vector3.Lerp(target.position, idle.position, t);
                trafo.localScale = Vector3.Lerp(target.scale, idle.scale, t);
                setAlpa(Mathf.Lerp(target.alpha, idle.alpha, t));

                if (visibility == visibilityTarget)
                {
                    visibilityTarget = visibilityTargetFinal;
                    inTransition = false;
                    if(visibility == visibilityTarget)
                    {
                        isReady = true;
                    }
                }
            }
            else if (visibility == visibilityTargetFinal)
            {
                //kill pending transition
                visibilityTarget = visibilityTargetFinal;
                delay = 0;
                inTransition = false;
                isReady = true;
            }

        } 
  
    }


    public void setupTransition(float newVisibilityTarget)
    {
        isReady = false;
        visibilityTarget = newVisibilityTarget;
        if (visibilityTarget == 1)
        {
            transitionTime = introTime;
            easeIn = introEaseIn;
            easeOut = introEaseOut;
            target = intro;
            delay = introDelay;
        }
        else
        {
            target = outro;
            //going out is backward from 1 to 0
            // so outro speed is negative managed by making outro time negative instead
            transitionTime = -outroTime;
            // so easing must be turned aswell;
            easeIn = introEaseOut;
            easeOut = introEaseIn;
            delay = outroDelay;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (forceReset) Start();
        if (visible != lastFrameVisible)
        {
            lastFrameVisible = visible;
            visibilityTargetFinal = visible ? 1 : 0;
            //action reqired
            if (visibilityTargetFinal != visibility)
            {
                if (!alwaysCompleteTransition)
                {
                    setupTransition(visibilityTargetFinal);
                }
            }

        }
        if (visibilityTarget!=visibilityTargetFinal && visibilityTarget == visibility)
        {
            setupTransition(visibilityTargetFinal);
        }
        updateTransition();
    }
}
