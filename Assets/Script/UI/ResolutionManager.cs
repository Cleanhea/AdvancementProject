	using UnityEngine;
	using System.Collections.Generic;
    using TMPro;

public class ResolutionManager : MonoBehaviour
	{
	    public TMP_Dropdown resolutionDropdown;
	 
	    private List<Resolution> resolutions = new List<Resolution>();
	    private int optimalResolutionIndex = 2;
	 
	    private void Start()
	    {
	        resolutions.Add(new Resolution { width = 1024, height = 640 });
	        resolutions.Add(new Resolution { width = 1152, height = 720 });
	        resolutions.Add(new Resolution { width = 1280, height = 800 });
	        resolutions.Add(new Resolution { width = 1440, height = 900 });
	        resolutions.Add(new Resolution { width = 1680, height = 1050 });

	 
	        resolutionDropdown.ClearOptions();
	 
	        List<string> options = new List<string>();
	 
	        for (int i = 0; i < resolutions.Count; i++)
	        {
	            string option = resolutions[i].width + " x " + resolutions[i].height;
	            if (resolutions[i].width == Screen.currentResolution.width &&
	                resolutions[i].height == Screen.currentResolution.height)
	            {
	                optimalResolutionIndex = i;
	                option += " *";
	            }
	            options.Add(option);
	        }
	 
	        resolutionDropdown.AddOptions(options);
	        resolutionDropdown.value = optimalResolutionIndex;
	        resolutionDropdown.RefreshShownValue();
	        SetResolution(optimalResolutionIndex);
	    }
	 
	    public void SetResolution(int resolutionIndex)
	    {
	        Resolution resolution = resolutions[resolutionIndex];
	        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	    }
	}