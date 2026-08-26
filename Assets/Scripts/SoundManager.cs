using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static void Play(AudioClip audioClip)
	{
    	//空のゲームオブジェクトgoを作成し、名前を"TempAudioSource"
    	GameObject go = new GameObject("TempAudioSource");

    	//AudioSourceをgoに追加して、AudioClipを指定し再生
    	AudioSource audioSource = go.AddComponent<AudioSource>();
    	audioSource.clip = audioClip;
    	audioSource.Play();

    	// //AutoDestroyをgoに追加して、AudioClipの長さと同じ時間経過後に自分を削除させる。
    	AutoDestroy autoDestroy = go.AddComponent<AutoDestroy>();
    	autoDestroy.destroyTime = audioClip.length;
	}
}
