// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.


using UnityEngine;
using UnityStandardAssets.ImageEffects;

//This code is a sample implementation of the work described in
//SeeingVR: A Set of Tools to Make Virtual Reality More Accessible to People with Low Vision
//Yuhang Zhao, Ed Cutrell, Christian Holz, Meredith Ringel Morris, Eyal Ofek, Andy Wilson
//CHI 2019 | May 2019
//
//https://www.microsoft.com/en-us/research/publication/seeingvr-a-set-of-tools-to-make-virtual-reality-more-accessible-to-people-with-low-vision-2/



public class AddAllOverlays : MonoBehaviour
{

    public Camera mainCamera;

    public GameObject leftHand;
    public GameObject rightHand;

    public GameObject magnififcationPrefab;
    public GameObject bifocalPrefab;
    public GameObject remappingPrefab;

    [Space]
    public bool magnification;

    [Range(1, 10)]
    public int magnificationLevel = 2;

    [Space]
    public bool bifocal;
    [Range(1, 10)] public int bifocalLevel = 2;
	[Range(-0.3f, 0.5f)]public float bifocalShift = 0.0f;

    [Space]
    public bool brightness;
    [Range(0, 2)] public float brightnessLevel = 1.0f;

    [Space]
    public bool contrast;
    [Range(0, 1)] public float contrastIntensity = 0.5f;

    [Space]
    public bool edgeEnhancement;
    [Range(0, 5)] public float edgeThickness = 1;
    public Color edgeColor = Color.green;

    [Space]
    public bool remapping;
    public Color remappingEdgeColor = Color.green;
	[Range(-0.3f, 0.3f)]public float XShift = 0f;
	[Range(-0.3f, 0.3f)] public float YShift = 0f;
	[Range(-0.3f, 0.3f)] public float ZShift = 0f;
	[Range(-0.2f, 0.2f)] public float SizeShift = 0f;

    [Space]
    public bool textAugmentation;
	public bool bold;
    public int fontSizeIncrease = 0;
    public bool textDynamicScanning;
	public bool addTextCollider = false;

    [Space]
    public bool textToSpeech;

    [Space]
    public bool objectDescription;

    [Space]
    public bool depthMeasurement;
    public Color depthColor = Color.green;

    public float laserRotation = 0;
    public float laserShift = 0;

    [Space]
    public bool highlight;
    public Color highlightColor = Color.green;

    [Space]
    public bool guideline;
    public Color guidelineColor = Color.red;
	[Range(0.1f, 1f)] public float forwardFactor = 0.5f;
	[Range(0, 0.5f)] public float radius = 0.25f;

    public bool salientObjDynamicScanning;

    [Space]
    public bool recoloring;
    public bool objectDynamicScanning;
    public float distanceThreshold;


    private GameObject magnificationObj;
    private GameObject bifocalObj;
    private GameObject remappingObj;
    private BrightnessAdjustment brightnessAdjustment;
    private ContrastEnhance contrastEnhance;
    private EdgeDetection edgeDetection;
    private GameObject augmentations;
    private TextAugmentationByShader textAugment;
    private VoiceComponent leftVoice;
    private VoiceComponent rightVoice;
    private DepthLaser leftDepth;
    private DepthLaser rightDepth;
    private HighlightSalience highlightSalience;
    private RecolorManagement recolorManagement;


    void Start ()
	{
	    if (magnififcationPrefab != null)
	    {
	        magnificationObj = (GameObject) Instantiate(magnififcationPrefab);
	        magnificationObj.transform.parent = mainCamera.transform;
	        magnificationObj.GetComponent<AdjustMagnificationLevel>().magnificationLevel = 5;

	        magnificationObj.transform.localPosition = new Vector3(0, 0, 0.5f);
	        magnificationObj.transform.localRotation = Quaternion.Euler(-90, 0, 0);
	        magnificationObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
	        magnificationObj.layer = 31;
	        magnificationObj.transform.GetChild(0).gameObject.layer = 31;

	        Camera cam = magnificationObj.GetComponentInChildren<Camera>();
	        if (cam != null)
	        {
	            cam.cullingMask &= ~((1 << 31));
		        cam.clearFlags = mainCamera.clearFlags;
		        cam.backgroundColor = mainCamera.backgroundColor;
	        }

	        magnificationObj.SetActive(false);
	    }

	    if (bifocalPrefab != null)
	    {
	        bifocalObj = Instantiate(bifocalPrefab);
	        bifocalObj.GetComponent<AdjustBifocal>().magnificationLevel = 5;
	        bifocalObj.transform.parent = mainCamera.transform;
	        bifocalObj.transform.localPosition = new Vector3(0, -0.2f + bifocalShift, 0.5f);
	        bifocalObj.transform.localRotation = Quaternion.Euler(-90, 0, 0);
	        bifocalObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
	        bifocalObj.layer = 31;
	        bifocalObj.transform.GetChild(0).gameObject.layer = 31;
	        Camera cam = bifocalObj.GetComponentInChildren<Camera>();
	        if (cam != null)
	        {
	            cam.cullingMask &= ~((1 << 31));
		        cam.transform.localRotation = Quaternion.Euler(90 - Mathf.Atan2(-0.2f + bifocalShift, 0.5f)*180/Mathf.PI, 0, 0);
		        cam.transform.localPosition = new Vector3(0, 5, 2 - 10 * bifocalShift);
		        cam.clearFlags = mainCamera.clearFlags;
		        cam.backgroundColor = mainCamera.backgroundColor;
	        }
	        bifocalObj.SetActive(false);
        }

	    if (remappingPrefab != null)
	    {
	        remappingObj = Instantiate(remappingPrefab);
	        remappingObj.transform.parent = mainCamera.transform;
		    remappingObj.transform.localScale = new Vector3(0.2f + SizeShift, 0.2f + SizeShift, 0.01f);
	        remappingObj.transform.localPosition = new Vector3(XShift, YShift, 0.5f + ZShift);
	        remappingObj.transform.localRotation = Quaternion.Euler(0, 180, 0);
	        remappingObj.GetComponent<Renderer>().material.color = remappingEdgeColor;
	        remappingObj.layer = 30;
	        Camera cam = remappingObj.GetComponentInChildren<Camera>();
	        cam.cullingMask &= ~((1 << 31) | (1 << 30));
		    cam.transform.localPosition = new Vector3(XShift / 0.2f, -YShift/0.2f, (0.5f + ZShift)/0.01f);
		    cam.clearFlags = mainCamera.clearFlags;
		    cam.backgroundColor = mainCamera.backgroundColor;
	        remappingObj.GetComponent<BoxCollider>().enabled = false;
	        remappingObj.SetActive(false);
        }

	    brightnessAdjustment = mainCamera.gameObject.AddComponent<BrightnessAdjustment>();
	    brightnessAdjustment.intensity = brightnessLevel;
        brightnessAdjustment.brightnessShader = Shader.Find("Custom/BrightnessShader");
	    brightnessAdjustment.enabled = false;

	    contrastEnhance = mainCamera.gameObject.AddComponent<ContrastEnhance>();
        contrastEnhance.contrastCompositeShader = Shader.Find("Hidden/ContrastComposite");
        contrastEnhance.separableBlurShader = Shader.Find("Hidden/SeparableBlur");
	    contrastEnhance.threshold = 0;
	    contrastEnhance.blurSpread = 1;
	    contrastEnhance.intensity = contrastIntensity;
	    contrastEnhance.enabled = false;

	    edgeDetection = mainCamera.gameObject.AddComponent<EdgeDetection>();
	    edgeDetection.edgesOnly = 0;
	    edgeDetection.edgesColor = edgeColor;
	    edgeDetection.sampleDist = edgeThickness;
        edgeDetection.edgeDetectShader = Shader.Find("Hidden/EdgeDetect");
	    edgeDetection.enabled = false;

        augmentations = new GameObject("_augmentations");
	    textAugment = augmentations.AddComponent<TextAugmentationByShader>();
	    textAugment.addColliderToText = addTextCollider;
	    textAugment.fontSizeIncrease = fontSizeIncrease;
		textAugment.bold = false;
	    textAugment.isAugmented = false;
	    textAugment.dynamicScanning = false;

	    augmentations.AddComponent<WindowsVoice>();
	    if (leftHand != null)
	    {
	        leftVoice = leftHand.AddComponent<VoiceComponent>();
	        leftVoice.TextToSpeech = false;
	        leftVoice.objectDescription = false;
	        VoiceLaser leftLaser = leftHand.GetComponent<VoiceLaser>();
	        leftLaser.rotateAngle = laserRotation;
	        leftLaser.shiftDistance = laserShift;

	        leftDepth = leftHand.AddComponent<DepthLaser>();
	        leftDepth.isDepthMeasured = false;
	        leftDepth.color = depthColor;
	        leftDepth.rotateAngle = laserRotation;
	        leftDepth.shiftDistance = laserShift;
	    }

	    if (rightHand != null)
	    {
	        rightVoice = rightHand.AddComponent<VoiceComponent>();
	        rightVoice.TextToSpeech = false;
	        rightVoice.objectDescription = false;
	        VoiceLaser rightLaser = rightHand.GetComponent<VoiceLaser>();
	        rightLaser.rotateAngle = laserRotation;
	        rightLaser.shiftDistance = laserShift;

            rightDepth = rightHand.AddComponent<DepthLaser>();
	        rightDepth.isDepthMeasured = false;
	        rightDepth.color = depthColor;
	        rightDepth.rotateAngle = laserRotation;
	        rightDepth.shiftDistance = laserShift;
        }

	    highlightSalience = augmentations.AddComponent<HighlightSalience>();
	    highlightSalience.highlight = false;
	    highlightSalience.highlightColor = highlightColor;
        highlightSalience.Guideline = false;
	    highlightSalience.guidelineColor = guidelineColor;
		highlightSalience.forwardFactor = forwardFactor;
		highlightSalience.radius = radius;
	    highlightSalience.dynamicScanning = salientObjDynamicScanning;

	    recolorManagement = augmentations.AddComponent<RecolorManagement>();
	    recolorManagement.enableRecoloring = false;
	    recolorManagement.threshould = distanceThreshold;
	    recolorManagement.dynamicScanning = objectDynamicScanning;

	}

    void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))        {          bifocal = false; magnification = !magnification;        }
        if (Input.GetKeyDown(KeyCode.Alpha2))        {          magnification = false;  bifocal = !bifocal;        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) { edgeEnhancement = false; highlight = !highlight; }
        if (Input.GetKeyDown(KeyCode.Alpha3))        {          highlight = false;  edgeEnhancement = !edgeEnhancement;    }

        if (Input.GetKeyDown(KeyCode.Alpha5))        {           brightness = !brightness;        }

        if (Input.GetKeyDown(KeyCode.Alpha6))        {           textToSpeech = !textToSpeech;     }
        if (Input.GetKeyDown(KeyCode.Alpha7))        {           objectDescription = !objectDescription;        }

        if (Input.GetKeyDown(KeyCode.Alpha8))        {            remapping = !remapping;        }
        if (Input.GetKeyDown(KeyCode.Alpha9))        {            guideline = !guideline;        }

        magnificationObj.SetActive(magnification);
	    magnificationObj.GetComponent<AdjustMagnificationLevel>().magnificationLevel = magnificationLevel;

        bifocalObj.SetActive(bifocal);
	    bifocalObj.GetComponent<AdjustBifocal>().magnificationLevel = bifocalLevel;
	    bifocalObj.transform.localPosition = new Vector3(0, -0.2f + bifocalShift, 0.5f);
	    Camera cam = bifocalObj.GetComponentInChildren<Camera>();
	    if (cam != null)
	    {
		    cam.transform.localRotation = Quaternion.Euler(90 - Mathf.Atan2(-0.2f + bifocalShift, 0.5f)*180/Mathf.PI, 0, 0);
		    cam.transform.localPosition = new Vector3(0, 5, 2 - 10 * bifocalShift);

	    }


        remappingObj.SetActive(remapping);
	    remappingObj.GetComponent<Renderer>().material.color = remappingEdgeColor;
	    remappingObj.transform.localScale = new Vector3(0.2f + SizeShift, 0.2f + SizeShift, 0.01f);
	    remappingObj.transform.localPosition = new Vector3(XShift, YShift, 0.5f+ZShift);

	    Camera camBi = remappingObj.GetComponentInChildren<Camera>();
	    if (camBi != null)
	    {
		    camBi.transform.localPosition = new Vector3(XShift / 0.2f, -YShift / 0.2f, (0.5f+ZShift)/0.01f);
	    }


	    brightnessAdjustment.enabled = brightness;
        brightnessAdjustment.intensity = brightnessLevel;

        contrastEnhance.enabled = contrast;
        contrastEnhance.intensity = contrastIntensity;

        edgeDetection.enabled = edgeEnhancement;
        edgeDetection.edgesColor = edgeColor;
        edgeDetection.sampleDist = edgeThickness;

        Camera magCam = magnificationObj.GetComponentInChildren<Camera>();
        EdgeDetection magEdge = magCam.GetComponent<EdgeDetection>();
        magEdge.enabled = edgeEnhancement;
        magEdge.edgesColor = edgeColor;
        magEdge.sampleDist = edgeThickness;

        Camera biCam = bifocalObj.GetComponentInChildren<Camera>();
        EdgeDetection biEdge = biCam.GetComponent<EdgeDetection>();
        biEdge.enabled = edgeEnhancement;
        biEdge.edgesColor = edgeColor;
        biEdge.sampleDist = edgeThickness;

        textAugment.isAugmented = textAugmentation;
        textAugment.dynamicScanning = textDynamicScanning;
        textAugment.fontSizeIncrease = fontSizeIncrease;
	    textAugment.addColliderToText = addTextCollider;
	    textAugment.bold = bold;

        if (leftVoice != null)
        {
            leftVoice.TextToSpeech = textToSpeech;
            leftVoice.objectDescription = objectDescription;
            VoiceLaser leftLaser = leftHand.GetComponent<VoiceLaser>();
            leftLaser.rotateAngle = laserRotation;
            leftLaser.shiftDistance = laserShift;
        }

        if (rightVoice != null)
        {
            rightVoice.TextToSpeech = textToSpeech;
            rightVoice.objectDescription = objectDescription;
            VoiceLaser rightLaser = rightHand.GetComponent<VoiceLaser>();
            rightLaser.rotateAngle = laserRotation;
            rightLaser.shiftDistance = laserShift;
        }

        if (leftDepth != null)
        {
            leftDepth.isDepthMeasured = depthMeasurement;
            leftDepth.color = depthColor;
            leftDepth.rotateAngle = laserRotation;
            leftDepth.shiftDistance = laserShift;
        }

        if (rightDepth != null)
        {
            rightDepth.isDepthMeasured = depthMeasurement;
            rightDepth.color = depthColor;
            rightDepth.rotateAngle = laserRotation;
            rightDepth.shiftDistance = laserShift;
        }

        highlightSalience.highlight = highlight;
        highlightSalience.highlightColor = highlightColor;
        highlightSalience.Guideline = guideline;
        highlightSalience.guidelineColor = guidelineColor;
	    highlightSalience.forwardFactor = forwardFactor;
	    highlightSalience.radius = radius;
        highlightSalience.dynamicScanning = salientObjDynamicScanning;

        recolorManagement.enableRecoloring = recoloring;
        recolorManagement.threshould = distanceThreshold;
        recolorManagement.dynamicScanning = objectDynamicScanning;
    }
}
