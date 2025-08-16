	using UnityEngine;
	using System.Collections.Generic;
    using TMPro;

public class ResolutionManager : MonoBehaviour
{
	public TMP_Dropdown resolutionDropdown;

	List<Resolution> resolutions = new List<Resolution>();
	int currentResolutionIndex = 2;
	const string RESOLUTION_KEY = "Resolution";

	void Start()
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
			options.Add(option);
		}
		resolutionDropdown.AddOptions(options);

		currentResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_KEY, 2);
		resolutionDropdown.value = currentResolutionIndex;
		resolutionDropdown.RefreshShownValue();

		SetResolution(currentResolutionIndex);

		resolutionDropdown.onValueChanged.AddListener(SetResolution);
	}

	public void SetResolution(int resolutionIndex)
	{
		Resolution resolution = resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, false);

		currentResolutionIndex = resolutionIndex;

		PlayerPrefs.SetInt(RESOLUTION_KEY, currentResolutionIndex);
		PlayerPrefs.Save();
	}
}