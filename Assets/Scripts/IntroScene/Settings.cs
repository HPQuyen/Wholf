using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    #region Attributes

    #region Players Pref Key Contact

    private const string RESOLUTION_PREF_KEY = "resolution";

    #endregion

    #region Resolution

    [SerializeField]
    private TextMeshProUGUI resolutionText = null;

    private Resolution[] resolutions;

    private int currentResolutionIndex = 0;

    [SerializeField]
    private Toggle toggler = null;

    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        HashSet<Resolution> filteredResolutions = new HashSet<Resolution>();

        resolutions = Screen.resolutions;

        for (int i = 0; i < resolutions.Length; i++)
        {
            filteredResolutions.Add(resolutions[i]);
        }
        
        filteredResolutions.RemoveWhere(IsUnsuitableRefreshFrame);
        filteredResolutions.RemoveWhere(IsUnsuitableResolution);
        Debug.Log("Resolution.Count: " + resolutions.Length);

        resolutions = new Resolution[filteredResolutions.Count];
        Debug.Log("Resolution: " + filteredResolutions.Count);
        filteredResolutions.CopyTo(resolutions);

        currentResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_PREF_KEY, 0);

        SetResolutionText(resolutions[currentResolutionIndex]);
    }

    #region Support Methods/Predicates
    private bool IsUnsuitableRefreshFrame(Resolution resolution)
    {
        if (resolution.refreshRate < 40)
        {
            return true;
        }
        else
        {
            foreach (var item in resolutions)
            {
                if(resolution.width == item.width && resolution.height == item.height)
                {
                    if (resolution.refreshRate < item.refreshRate)
                        return true;
                }
            }
        }
        return false;  
    }

    private bool IsUnsuitableResolution(Resolution resolution)
    {
        int[] unsuitableWidth = { 1280 };
        int[] unsuitableHeight = { 600 };

        if (resolution.width < 800 || resolution.height < 600)
        {
            return true;
        }
        else
        {
            for (int i = 0; i < unsuitableWidth.Length; i++)
            {
                if (resolution.width == unsuitableWidth[i] 
                    && resolution.height == unsuitableHeight[i])
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region Resolution Cycling

    private void SetResolutionText(Resolution resolution)
    {
        resolutionText.text = resolution.width + "x" + resolution.height;
    }

    public void SetNextResolution()
    {
        currentResolutionIndex = GetNextWrappedIndex(resolutions, currentResolutionIndex);
        SetResolutionText(resolutions[currentResolutionIndex]);
    }

    public void SetPreviousResolution()
    {
        currentResolutionIndex = GetPreviousWrappedIndex(resolutions, currentResolutionIndex);
        SetResolutionText(resolutions[currentResolutionIndex]);
    }

    #endregion

    #region Apply Resolution

    private void SetAndApplyingResolution(int newResolutionIndex)
    {
        currentResolutionIndex = newResolutionIndex;
        ApplyCurrentResolution();
    }

    private void ApplyCurrentResolution()
    {
        ApplyResolution(resolutions[currentResolutionIndex]);
    }

    private void ApplyResolution(Resolution resolution)
    {
        SetResolutionText(resolution);

        if (toggler.isOn)
        {
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen);
            Debug.Log(Screen.fullScreenMode);
        }
        else
        {
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode = FullScreenMode.Windowed);
            Debug.Log(Screen.fullScreenMode);
        }

        PlayerPrefs.SetInt(RESOLUTION_PREF_KEY, currentResolutionIndex);
    }

    #endregion

    #region Misc Helpers

    #region Index Wrap Helpers
    private int GetNextWrappedIndex<T>(IList<T> collection, int currentIndex)
    {
        if (collection.Count < 1) return 0;
        if (currentIndex >= collection.Count - 1) return collection.Count - 1;
        else
            return (currentIndex + 1) % collection.Count;
    }

    private int GetPreviousWrappedIndex<T>(IList<T> collection, int currentIndex)
    {
        if (collection.Count < 1) return 0;
        if ((currentIndex - 1) < 0) return 0;
        else
            return (currentIndex - 1) % collection.Count;
    }

    #endregion

    #endregion

    public void ApplyChanges()
    {
        SetAndApplyingResolution(currentResolutionIndex);
    }
}
