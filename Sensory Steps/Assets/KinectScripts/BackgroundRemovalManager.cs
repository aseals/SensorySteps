using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class BackgroundRemovalManager : MonoBehaviour 
{
	// camera used to display the foreground texture on the screen, null if on-screen display is not required
	public Camera foregroundCamera;

	// Whether the hi-res (color camera resolution) is preferred for the foreground image, when available; otherwise depth camera resolution is used
	public bool colorCameraResolution = true;
	
	// color to used to paint pixels, where the color camera data is not available
	public Color32 defaultColor = new Color32(64, 64, 64, 255);
	
	// GUI Text to show messages.
	public GUIText debugText;

	// buffer for the raw foreground image
	private byte[] foregroundImage;
	
	// the foreground texture
	private Texture2D foregroundTex;
	
	// rectangle taken by the foreground texture (in pixels)
	private Rect foregroundRect;
	
	// primary sensor data structure
	private KinectInterop.SensorData sensorData = null;
	
	// whether opencv-library is used for smoothing of the foreground image
	private bool isUsingOpenCv = false;
	
	// Bool to keep track whether Kinect and BR library have been initialized
	private bool isBrInited = false;
	
	// The single instance of BackgroundRemovalManager
	private static BackgroundRemovalManager instance;
	
	
	// returns the single BackgroundRemovalManager instance
    public static BackgroundRemovalManager Instance
    {
        get
        {
            return instance;
        }
    }
	
	// returns true if the background removal is successfully initialized, false otherwise
	public bool IsBackgroundRemovalInitialized()
	{
		return isBrInited;
	}
	
	// returns the raw foreground image
	public byte[] GetForegroundImage()
	{
		return foregroundImage;
	}
	
	// returns the foreground image texture
	public Texture2D GetForegroundTex()
	{ 
		return foregroundTex;
	}
	
	//----------------------------------- end of public functions --------------------------------------//
	
	void Start() 
	{
		try 
		{
			// get sensor data
			KinectManager kinectManager = KinectManager.Instance;
			if(kinectManager && kinectManager.IsInitialized())
			{
				sensorData = kinectManager.GetSensorData();
			}
			
			if(sensorData == null || sensorData.sensorInterface == null)
			{
				throw new Exception("Background removal cannot be started, because KinectManager is missing or not initialized.");
			}
			
			// ensure the needed dlls are in place and speech recognition is available for this interface
			bool bNeedRestart = false;
			bool bSuccess = sensorData.sensorInterface.IsBackgroundRemovalAvailable(ref bNeedRestart);

			if(!bSuccess)
			{
				bSuccess = KinectInterop.IsOpenCvAvailable(ref bNeedRestart);
				isUsingOpenCv = bSuccess;
			}

			if(bSuccess)
			{
				if(bNeedRestart)
				{
					KinectInterop.RestartLevel(gameObject, "BR");
					return;
				}
			}
			else
			{
				string sInterfaceName = sensorData.sensorInterface.GetType().Name;
				throw new Exception(sInterfaceName + ": Background removal is not supported!");
			}
			
			// Initialize the background removal
			bSuccess = !isUsingOpenCv ? sensorData.sensorInterface.InitBackgroundRemoval() : true;

			if (!bSuccess)
	        {
				throw new Exception("Background removal could not be initialized.");
	        }

			// create the foreground image and alpha-image
			int imageLength = !isUsingOpenCv ? 
				sensorData.sensorInterface.GetForegroundFrameLength(sensorData, colorCameraResolution) :
					KinectInterop.GetForegroundFrameLength(sensorData, colorCameraResolution);
			foregroundImage = new byte[imageLength];

			// get the needed rectangle
			Rect neededFgRect = !isUsingOpenCv ? 
				sensorData.sensorInterface.GetForegroundFrameRect(sensorData, colorCameraResolution) :
					KinectInterop.GetForegroundFrameRect(sensorData, colorCameraResolution);

			// create the foreground texture
			foregroundTex = new Texture2D((int)neededFgRect.width, (int)neededFgRect.height, TextureFormat.RGBA32, false);

			// calculate the foreground rectangle
			if(foregroundCamera != null)
			{
				Rect cameraRect = foregroundCamera.pixelRect;
				float rectHeight = cameraRect.height;
				float rectWidth = cameraRect.width;
				
				if(rectWidth > rectHeight)
					rectWidth = Mathf.Round(rectHeight * neededFgRect.width / neededFgRect.height);
				else
					rectHeight = Mathf.Round(rectWidth * neededFgRect.height / neededFgRect.width);
				
				foregroundRect = new Rect((cameraRect.width - rectWidth) / 2, cameraRect.height - (cameraRect.height - rectHeight) / 2, rectWidth, -rectHeight);
			}

			instance = this;
			isBrInited = true;
			
			//DontDestroyOnLoad(gameObject);
		} 
		catch(DllNotFoundException ex)
		{
			Debug.LogError(ex.ToString());
			if(debugText != null)
				debugText.GetComponent<GUIText>().text = "Please check the Kinect and BR-Library installations.";
		}
		catch (Exception ex) 
		{
			Debug.LogError(ex.ToString());
			if(debugText != null)
				debugText.GetComponent<GUIText>().text = ex.Message;
		}
	}

	void OnDestroy()
	{
		if(isBrInited && sensorData != null && sensorData.sensorInterface != null)
		{
			// finish background removal
			if(!isUsingOpenCv)
			{
				sensorData.sensorInterface.FinishBackgroundRemoval();
			}
		}
		
		isBrInited = false;
		instance = null;
	}
	
	void Update () 
	{
		if(isBrInited)
		{
			// update the background removal
			bool bSuccess = !isUsingOpenCv ? sensorData.sensorInterface.UpdateBackgroundRemoval() : true;
			
			if(bSuccess)
			{
				KinectManager kinectManager = KinectManager.Instance;
				if(kinectManager && kinectManager.IsInitialized() && kinectManager.IsUserDetected())
				{
					bSuccess = !isUsingOpenCv ? 
						sensorData.sensorInterface.PollForegroundFrame(sensorData, colorCameraResolution, defaultColor, ref foregroundImage) :
							KinectInterop.PollForegroundFrame(sensorData, colorCameraResolution, defaultColor, ref foregroundImage);
					
					if(bSuccess)
					{
						foregroundTex.LoadRawTextureData(foregroundImage);
						foregroundTex.Apply();
					}
				}
			}
		}
	}
	
	void OnGUI()
	{
		if(isBrInited && foregroundCamera && foregroundTex)
		{
			GUI.DrawTexture(foregroundRect, foregroundTex);
		}
	}


}
