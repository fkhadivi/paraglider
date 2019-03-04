using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class experimentaAnimationPresets : MonoBehaviour {



    public bool visible = true;
    public bool alwaysCompleteTransition = false;


    [Header("CANT TOUCH THIS")]
    public STATE state = STATE.IDLE;
    private bool dontTouchAlpha = false;
    private bool easeIn = false;
    private bool easeOut = false;
    private float transitionTime;
    public float delay = 0;
    public float visibilityCurrent = 1;
    private float visibilityTarget = 1;

    private float[] defaultAlphaImages;
    private float[] defaultAlphaTexts;
    private RectTransform trafo;
    private Image[] images;
    private Text[] texts;
    public bool forceReset = false;

    [Header("Adjust Intro")]
    public bool introEaseIn = false;
    public bool introEaseOut = false;
    public float introTime = 1;
    public float introDelay = 0;
    public Vector3 introOffset;
    public float introScale =1;
    public float introAlpha =1;

    [Header("Adjust Outro")]
    public bool outroEaseIn = false;
    public bool outroEaseOut = false;
    public float outroTime = 1;
    public float outroDelay = 0;
    public bool outroLikeIntro = true;
    public Vector3 outroOffset;
    public float outroScale = 1;
    public float outroAlpha = 1;


    public Keyframe introVisiblityTarget;
    public Keyframe idle;
    public Keyframe outroVisiblityTarget;
    public Keyframe transitionVisisbiltyTarget;

    public enum STATE
    {
        HIDDEN,
        DELAY_INTRO,
        INTRO,
        IDLE,
        DELAY_OUTRO,
        OUTRO
    }

    public class Keyframe
    {
        public Vector3 position;
        public Vector3 scale;
        public float alpha;


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
        introVisiblityTarget = new Keyframe(trafo, introOffset, introScale, introAlpha);
        if(outroLikeIntro) outroVisiblityTarget = new Keyframe(trafo, introOffset, introScale, introAlpha);
        else outroVisiblityTarget = new Keyframe(trafo, outroOffset, outroScale, outroAlpha);
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

    public void toggleVisibilty(bool shouldBeVisible)
    {
        visible = shouldBeVisible;
    }

    public bool log = false;

    // Update is called once per frame
    void Update ()
    {
        if(log)
        {
            Debug.Log(
                " state = " + state.ToString()
            + " visibilityCurrent = " + visibilityCurrent
            + " visibilityTarget = " + visibilityTarget
            + " visibilityTarget = " + visible
            );
        }
        if (forceReset) Start();

        
        switch(state){

            case STATE.HIDDEN:
                if (visible) setState(STATE.DELAY_INTRO);
                break;

            case STATE.IDLE:
                if (!visible) setState(STATE.DELAY_OUTRO);
                break;

            case STATE.DELAY_OUTRO:
                if (visible) killTransition();
                else if (runDelay()) setState(STATE.OUTRO);
                break;

            case STATE.DELAY_INTRO:
                if (!visible) killTransition();
                else if (runDelay()) setState(STATE.INTRO);
                break;

            case STATE.OUTRO:
                if (visible && !alwaysCompleteTransition) setState(STATE.INTRO);
                else updateTransition();
                break;

            case STATE.INTRO:
                if (!visible && !alwaysCompleteTransition) setState(STATE.OUTRO);
                else updateTransition();
                break;

        }


    }

    private void setState(STATE newState)
    {
        Debug.Log("animation setting state to " + newState);
        state = newState;
        switch (state)
        {
            case STATE.DELAY_INTRO:
                visibilityTarget = 1;
                delay = introDelay;
                break;

            case STATE.DELAY_OUTRO:
                visibilityTarget = 0;
                delay = outroDelay;
                break;

            case STATE.INTRO:
                visibilityTarget = 1;
                easeIn = introEaseIn;
                easeOut = introEaseOut;
                transitionVisisbiltyTarget = introVisiblityTarget;
                transitionTime = introTime;
                break;

            case STATE.OUTRO:
                visibilityTarget = 0;
                easeIn = introEaseOut;
                easeOut = introEaseIn;
                transitionVisisbiltyTarget = outroVisiblityTarget;
                // going out is backward from 1 to 0
                // so outroVisiblityTarget speed is negative 
                // managed by making outroVisiblityTarget time negative instead
                transitionTime = -outroTime;
                break;
        }
    }

    private bool runDelay()
    {
        return BenjasMath.countdownToZero(ref delay);
    }

    private void updateTransition()
    {
        visibilityCurrent = Mathf.Clamp01(visibilityCurrent + Time.deltaTime / transitionTime);

        float t = visibilityCurrent;

        if (easeIn) if (easeOut) t = BenjasMath.easeInOut(t);
            else t = BenjasMath.easeIn(t);
        else if (easeOut) t = BenjasMath.easeOut(t);
        //t goes automaticly backwards when going out
        trafo.anchoredPosition3D = Vector3.Lerp(transitionVisisbiltyTarget.position, idle.position, t);
        trafo.localScale = Vector3.Lerp(transitionVisisbiltyTarget.scale, idle.scale, t);
        setAlpa(Mathf.Lerp(transitionVisisbiltyTarget.alpha, idle.alpha, t));

        if (visibilityCurrent == visibilityTarget)  killTransition();
    }

    private void killTransition()
    {
        setState ((visibilityCurrent == 1) ? STATE.IDLE : STATE.HIDDEN);
        delay = 0;
    }
}


