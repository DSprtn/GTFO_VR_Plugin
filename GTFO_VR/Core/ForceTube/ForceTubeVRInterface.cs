using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System;
using System.IO;

#if UNITY_ANDROID && !UNITY_EDITOR
    using System.Collections;
#endif
using System.Runtime.InteropServices;


///<summary>
///Useful to target different ForceTubeVR.
///"all" send requests to all, ignoring the channel settings, and "rifle" send requests to both "rifleButt" and "rifleBolt".
///By default, InitAsync() make the first ForceTubeVR detected is placed in the channel "rifleButt", the second is placed in "rifleBolt", and following are placed in channels "pistol1", "pistol2", "other" and "vest".
///</summary>
public enum ForceTubeVRChannel : int { all, rifle, rifleButt, rifleBolt, pistol1, pistol2, other, vest };


public class ForceTubeVRInterface : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR

        private static AndroidJavaObject ForceTubeVRPlugin = null;
        private static ForceTubeVRInterface instance = null;


        private static IEnumerator InitAndroid(bool pistolsFirst)
        {
            yield return new WaitForSeconds(0.1f);
            if (ForceTubeVRPlugin == null)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using (AndroidJavaClass builderClass = new AndroidJavaClass("com.ProTubeVR.ForceTubeVRInterface.ForceTubeVRInterface"))
                {
				    ForceTubeVRPlugin = builderClass.CallStatic<AndroidJavaObject>("getInstance", activity, pistolsFirst);
                }
            }
        }

#endif

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)

        [DllImport("ForceTubeVR_API_x32", EntryPoint = "InitRifle")]
	    private static extern void InitRifle_x32();

	    [DllImport("ForceTubeVR_API_x32", EntryPoint = "InitPistol")]
	    private static extern void InitPistol_x32();

        [DllImport("ForceTubeVR_API_x32", EntryPoint = "SetActive")]
        private static extern void SetActiveResearch_x32(bool active);
    
        [DllImport("ForceTubeVR_API_x32", EntryPoint = "KickChannel")]
	    private static extern void Kick_x32(Byte power, ForceTubeVRChannel channel);
    
	    [DllImport("ForceTubeVR_API_x32", EntryPoint = "RumbleChannel")]
	    private static extern void Rumble_x32(Byte power, float timeInSeconds, ForceTubeVRChannel channel);

	    [DllImport("ForceTubeVR_API_x32", EntryPoint = "ShotChannel")]
	    private static extern void Shot_x32(Byte kickPower, Byte rumblePower, float rumbleDuration, ForceTubeVRChannel channel);
    
        [DllImport("ForceTubeVR_API_x32", EntryPoint = "TempoToKickPower")]
        private static extern Byte TempoToKickPower_x32(float tempo);
    
        [DllImport("ForceTubeVR_API_x32", EntryPoint = "GetBatteryLevel")]
        private static extern Byte GetBatteryLevel_x32();
    
        [DllImport("ForceTubeVR_API_x64", EntryPoint = "InitRifle")]
        private static extern void InitRifle_x64();

	    [DllImport("ForceTubeVR_API_x64", EntryPoint = "InitPistol")]
	    private static extern void InitPistol_x64();

        [DllImport("ForceTubeVR_API_x64", EntryPoint = "SetActive")]
        private static extern void SetActiveResearch_x64(bool active);
    
        [DllImport("ForceTubeVR_API_x64", EntryPoint = "KickChannel")]
	    private static extern void Kick_x64(Byte power, ForceTubeVRChannel channel);
    
        [DllImport("ForceTubeVR_API_x64", EntryPoint = "RumbleChannel")]
	    private static extern void Rumble_x64(Byte power, float timeInSeconds, ForceTubeVRChannel channel);
    
        [DllImport("ForceTubeVR_API_x64", EntryPoint = "ShotChannel")]
	    private static extern void Shot_x64(Byte kickPower, Byte rumblePower, float rumbleDuration, ForceTubeVRChannel channel);
    
        [DllImport("ForceTubeVR_API_x64", EntryPoint = "TempoToKickPower")]
        private static extern Byte TempoToKickPower_x64(float tempo);
    
        [DllImport("ForceTubeVR_API_x64", EntryPoint = "GetBatteryLevel")]
        private static extern Byte GetBatteryLevel_x64();


        [DllImport("ForceTubeVR_API_x64", EntryPoint = "ListConnectedForceTube")]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string ListConnectedForceTube_x64();

        [DllImport("ForceTubeVR_API_x64", EntryPoint = "ListChannels")]
        [return: MarshalAs(UnmanagedType.LPStr)]
    	private static extern string ListChannels_x64();

        [DllImport("ForceTubeVR_API_x64", EntryPoint = "InitChannels")]
    	private static extern bool InitChannels_x64([MarshalAs(UnmanagedType.LPStr)] string sJsonChannelList);

        [DllImport("ForceTubeVR_API_x64", EntryPoint = "AddToChannel")]
    	private static extern bool AddToChannel_x64(int nChannel, [MarshalAs(UnmanagedType.LPStr)] string sName);

        [DllImport("ForceTubeVR_API_x64", EntryPoint = "RemoveFromChannel")]
    	private static extern bool RemoveFromChannel_x64(int nChannel, [MarshalAs(UnmanagedType.LPStr)] string sName);

        [DllImport("ForceTubeVR_API_x64", EntryPoint = "ClearChannel")]
    	private static extern void ClearChannel_x64(int nChannel);

        [DllImport("ForceTubeVR_API_x64", EntryPoint = "ClearAllChannel")]
    	private static extern void ClearAllChannel_x64();


#endif

    ///<summary>
    ///As suggered, this method is asynchronous.
    ///Only need to be called once to let the Dll manage the ForceTubeVR's connection. 
    ///By default, InitAsync() place the first ForceTubeVR detected in the channel "rifleButt" and the second in "rifleBolt". 
    ///If it receives a boolean true as first param, the first forcetubevr is placed in "pistol1" and the second in "pistol2". 
    ///</summary>
    public static void InitAsync(bool pistolsFirst = false)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            if(instance == null)
            {
                instance = (new GameObject("ForceTubeVRInterface")).AddComponent<ForceTubeVRInterface>();
                DontDestroyOnLoad(instance);
            }
            instance.StartCoroutine(InitAndroid(pistolsFirst));     
#endif

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (pistolsFirst) {
				if (IntPtr.Size == 8) { 
					InitPistol_x64 ();
				} else {
					InitPistol_x32 ();
				}
			} else {
				if (IntPtr.Size == 8) { 
					InitRifle_x64 ();
				} else {
					InitRifle_x32 ();
				}
			}
#endif
    }

    ///<summary>
    ///0 = no power, 255 = max power, this function is linear.
    ///</summary>
    public static void Kick(Byte power, ForceTubeVRChannel target = ForceTubeVRChannel.rifle)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (ForceTubeVRPlugin != null)
            {
				ForceTubeVRPlugin.Call("sendKick", power, (int)target);
            }
#endif

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (IntPtr.Size == 8)
            {
				Kick_x64(power, target);
            } else {
				Kick_x32(power, target);
            }
#endif
    }

    ///<summary>
    ///For power : 0 = no power, 255 = max power, if power is 126 or less, only the little motor is activated, this function is linear.
	///For timeInSeconds : 0.0f seconds is a special command that make the ForceTubeVR never stop the rumble.
    ///</summary>
	public static void Rumble(Byte power, float duration, ForceTubeVRChannel target = ForceTubeVRChannel.rifle)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (ForceTubeVRPlugin != null)
            {
				ForceTubeVRPlugin.Call("sendRumble", power, (int) (duration * 1000.0f), (int)target);
            }
#endif

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (IntPtr.Size == 8)
            { 
				Rumble_x64(power, duration, target);
            } else {
				Rumble_x32(power, duration, target);
            }
#endif
    }

    ///<summary>
    ///Combination of kick and rumble methods.
	///Rumble duration still be in seconds and still don't stop if you set this parameter at 0.0f.
    ///</summary>
	public static void Shoot(Byte kickPower, Byte rumblePower, float rumbleDuration, ForceTubeVRChannel target = ForceTubeVRChannel.rifle)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (ForceTubeVRPlugin != null)
            {
				ForceTubeVRPlugin.Call("sendShot", kickPower, rumblePower, (int)(rumbleDuration * 1000.0f), (int)target);
            }
#endif

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (IntPtr.Size == 8)
            { 
				Shot_x64(kickPower, rumblePower, rumbleDuration, target);
            } else {
				Shot_x32(kickPower, rumblePower, rumbleDuration, target);
            }
#endif
    }

    ///<summary>
	///It is true by default.  
	///Set it to false prevent the DLL to make a thread regularly check for connections and (re)connect ForceTubeVR when paired.
    ///</summary>
    public static void SetActiveResearch(bool active)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (ForceTubeVRPlugin != null)
            {
                ForceTubeVRPlugin.Call("setActiveResearch", active);
            }
#endif

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (IntPtr.Size == 8)
            { 
                SetActiveResearch_x64(active);
            } else {
                SetActiveResearch_x32(active);
            }
#endif
    }

    ///<summary>
    ///Take duration in seconds between two shots(for auto-shots) and give you the maximal kick power you can use without any loss. 
	///If you don't use it, you may have some loss of kick if kick power is too big in high shot frequencies.
    ///</summary>
    public static Byte TempoToKickPower(float tempo)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (ForceTubeVRPlugin != null)
            {
                return ForceTubeVRPlugin.Call<Byte>("tempoToKickPower", tempo);
            } else {
                return 255;
            }
#endif

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (IntPtr.Size == 8)
            { 
                return TempoToKickPower_x64(tempo);
            } else {
                return TempoToKickPower_x32(tempo);
            }
#endif
    }

    ///<summary>
    ///Return the battery level in percents. 
	///So it's an unsigned byte value between 0 and 100.
    ///</summary>
    public static Byte GetBatteryLevel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (ForceTubeVRPlugin != null)
            {
                return ForceTubeVRPlugin.Call<Byte>("getBatteryPercent");
            } else {
                return 255;
            }
#endif

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (IntPtr.Size == 8)
            { 
                return GetBatteryLevel_x64();
            } else {
                return GetBatteryLevel_x32();
            }
#endif
    }



    public static string ListConnectedForceTube()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (ForceTubeVRPlugin != null)
            {
                return ForceTubeVRPlugin.Call<string>("ListConnectedForceTube");
            }
            else
            return "";
#endif
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (IntPtr.Size == 8)
            { 
                return ListConnectedForceTube_x64();
            }           
            else
                return "";
#endif
    }
    public static string ListChannels()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (ForceTubeVRPlugin != null)
        {
            return ForceTubeVRPlugin.Call<string>("ListChannels");
        }
        else
        return "";
#endif
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (IntPtr.Size == 8)
            { 
                return ListChannels_x64();
            }     
            else
                return "";
#endif
    }

    public static bool InitChannels(string sJsonChannelList)
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        
        /*
        if (ForceTubeVRPlugin != null)
        {
            Debug.Log("InitChannels"+ sJsonChannelList);
            int nRet=AndroidJNI.AttachCurrentThread();
            Debug.Log("AttachCurrentThread"+nRet);
            jvalue[] method_args = new jvalue[1];
            method_args[0] = new jvalue();
            method_args[0].l = AndroidJNI.NewStringUTF(sJsonChannelList);
            Debug.Log("NewStringUTF "+method_args[0].l);

            IntPtr  cls=AndroidJNI.FindClass("com/ProTubeVR/ForceTubeVRInterface/ForceTubeVRInterface");
            Debug.Log("FindClass "+cls);

            IntPtr MethodId = AndroidJNI.GetMethodID(cls, "InitChannels", "(Ljava/lang/String;)I");
            Debug.Log("GetMethodID "+MethodId);

            bool bRet=AndroidJNI.CallBooleanMethod(ForceTubeVRPlugin.GetRawObject(), MethodId,method_args);
            Debug.Log("Out "+bRet);
            return bRet;

            //return ForceTubeVRPlugin.Call<bool>("InitChannels",sJsonChannelList);
        }
        else
        return false;
        */

        
        string path = Application.persistentDataPath;
        string filePath = path + "/ProTubeVR/Channels.json";
        Debug.Log("filePath : " + filePath);
        File.WriteAllText(filePath, sJsonChannelList);
        LoadChannelJSon();

        return true;


#endif
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (IntPtr.Size == 8)
            { 
                return InitChannels_x64(sJsonChannelList);
            }           
            else
            return false;
#endif
    }


    public static bool LoadChannelJSon()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (ForceTubeVRPlugin != null)
        {
            return ForceTubeVRPlugin.Call<bool>("LoadChannelJSon");
        }
        else
        return false;
#else
        string path = Application.persistentDataPath;
        string filePath = path + "/Channels.json";
        Debug.Log("filePath : " + filePath);
        string dataAsJson = File.ReadAllText(filePath);
        return ForceTubeVRInterface.InitChannels(dataAsJson);
#endif 
    }

    public static bool SaveChannelJSon()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (ForceTubeVRPlugin != null)
        {
            return ForceTubeVRPlugin.Call<bool>("SaveChannelJSon");
        }
        else
        return false;
#else
        string sText = ForceTubeVRInterface.ListChannels();
        string path = Application.persistentDataPath;
        string filePath = path + "/Channels.json";
        Debug.Log("filePath : " + filePath);
        File.WriteAllText(filePath, sText);
        return true;
#endif 
    }



    [Serializable]
    public class FTChannel
    {
        public string name;
        public int batteryLevel;
    }


    [Serializable]
    public class FTCType
    {
        public List<FTChannel> rifleButt;
        public List<FTChannel> rifleBolt;
        public List<FTChannel> pistol1;
        public List<FTChannel> pistol2;
        public List<FTChannel> other;
        public List<FTChannel> vest;
    }
    [Serializable]
    public class FTChannelFile
    {
        public FTCType channels;
    }




    public static bool AddToChannel(int nChannel, string sName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        /*if (ForceTubeVRPlugin != null)
        {
            return ForceTubeVRPlugin.Call<bool>("AddToChannel",nChannel,sName);
        }
        else
           return false;*/

        SaveChannelJSon();

        string path = Application.persistentDataPath;
        string filePath = path + "/ProTubeVR/Channels.json";
        Debug.Log("ReadAllText : " + filePath);
        string dataAsJson = File.ReadAllText(filePath);

        FTChannelFile FTList = JsonUtility.FromJson<FTChannelFile>(dataAsJson);


        FTChannel newChannel = new FTChannel();
        newChannel.name = sName;
        newChannel.batteryLevel = 0;

        ForceTubeVRChannel nFTChannel =(ForceTubeVRChannel)nChannel;
        switch (nFTChannel)
        {
            case ForceTubeVRChannel.rifleButt:
                FTList.channels.rifleButt.Add(newChannel);
                break;
            case ForceTubeVRChannel.rifleBolt:
                FTList.channels.rifleBolt.Add(newChannel);
                break;
            case ForceTubeVRChannel.pistol1:
                FTList.channels.pistol1.Add(newChannel);
                break;
            case ForceTubeVRChannel.pistol2:
                FTList.channels.pistol2.Add(newChannel);
                break;
            case ForceTubeVRChannel.other:
                FTList.channels.other.Add(newChannel);
                break;
            case ForceTubeVRChannel.vest:
                FTList.channels.vest.Add(newChannel);
                break;
        }

        dataAsJson = JsonUtility.ToJson(FTList);

        Debug.Log("WriteAllText : " + filePath);
        File.WriteAllText(filePath, dataAsJson);
        LoadChannelJSon();
        return true;



#endif
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (IntPtr.Size == 8)
            { 
                return AddToChannel_x64(nChannel,sName);
            }           
            else
            return false;
#endif
    }

    public static bool RemoveFromChannel(int nChannel, string sName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        /*if (ForceTubeVRPlugin != null)
        {
            return ForceTubeVRPlugin.Call<bool>("RemoveFromChannel",nChannel,sName);
        }
        else
         return false;*/

        SaveChannelJSon();

        string path = Application.persistentDataPath;
        string filePath = path + "/ProTubeVR/Channels.json";
        Debug.Log("ReadAllText : " + filePath);
        string dataAsJson = File.ReadAllText(filePath);

        FTChannelFile FTList = JsonUtility.FromJson<FTChannelFile>(dataAsJson);



       ForceTubeVRChannel nFTChannel = (ForceTubeVRChannel)nChannel;
        switch (nFTChannel)
        {
            case ForceTubeVRChannel.rifleButt:
                {
                    FTChannel channel = FTList.channels.rifleButt.Find((x) => x.name == sName);
                    FTList.channels.rifleButt.Remove(channel);
                }
                break;
            case ForceTubeVRChannel.rifleBolt:
                {
                    FTChannel channel = FTList.channels.rifleBolt.Find((x) => x.name == sName);
                    FTList.channels.rifleBolt.Remove(channel);
                }
                break;
            case ForceTubeVRChannel.pistol1:
                {
                    FTChannel channel = FTList.channels.pistol1.Find((x) => x.name == sName);
                    FTList.channels.pistol1.Remove(channel);
                }
                break;
            case ForceTubeVRChannel.pistol2:
                {
                    FTChannel channel = FTList.channels.pistol2.Find((x) => x.name == sName);
                    FTList.channels.pistol2.Remove(channel);
                }
                break;
            case ForceTubeVRChannel.other:
                {
                    FTChannel channel = FTList.channels.other.Find((x) => x.name == sName);
                    FTList.channels.other.Remove(channel);
                }
                break;
            case ForceTubeVRChannel.vest:
                {
                    FTChannel channel = FTList.channels.vest.Find((x) => x.name == sName);
                    FTList.channels.vest.Remove(channel);
                }
                break;
        }

        dataAsJson = JsonUtility.ToJson(FTList);

        Debug.Log("WriteAllText : " + filePath);
        File.WriteAllText(filePath, dataAsJson);
        LoadChannelJSon();
        return true;

#endif
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
                    if (IntPtr.Size == 8)
                    { 
                        return RemoveFromChannel_x64(nChannel,sName);
                    }        
                    else
                    return false;
#endif
    }

    public static void ClearChannel(int nChannel)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (ForceTubeVRPlugin != null)
        {
            ForceTubeVRPlugin.Call("ClearChannel",nChannel);
            SaveChannelJSon(); // now ... 
        }

#endif
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (IntPtr.Size == 8)
            { 
                ClearChannel_x64(nChannel);
            }        
#endif
    }

    public static void ClearAllChannel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (ForceTubeVRPlugin != null)
        {
            ForceTubeVRPlugin.Call("ClearAllChannel");
            SaveChannelJSon(); // now ... 
        }

#endif
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            if (IntPtr.Size == 8)
            { 
                ClearAllChannel_x64();
            }        
#endif
    }




    ///<summary>
    ///Only in Android system, launch the bluetooth settings activity, if you want to let users connect their ForceTubeVR in your game. 
    ///</summary>
    public static void OpenBluetoothSettings()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (ForceTubeVRPlugin != null)
            {
                ForceTubeVRPlugin.Call("openBluetoothSettings");
            }
#endif
    }

    public static void DisconnectAll()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (ForceTubeVRPlugin != null)
        {
            ForceTubeVRPlugin.Call("DisconnectAll");
        }
#endif

    }

    void OnDestroy()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		if (ForceTubeVRPlugin != null)
		{
			ForceTubeVRPlugin.Call("stop");
		}
#endif
    }
}