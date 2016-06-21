using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
	public AudioClip[] musicFiles;
	private int lastSongPlayed = -1;
	private static MusicManager _musicManagerInstance;

	public static MusicManager Instance 
	{
		get{ return _musicManagerInstance;}
	}
	// Use this for initialization
	void Start() 
	{
		playMusicOnTVAndGamePad ();

		audio.clip = musicFiles[GetSongIndex()];
		audio.Play();

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!audio.isPlaying) 
		{
			audio.clip = musicFiles[GetSongIndex()];
			audio.Play();
		}
	}

	void Awake()
	{
		if (_musicManagerInstance) 
		{
			DestroyImmediate(this.gameObject);
		} 
		else
		{
			DontDestroyOnLoad(this.gameObject);
			_musicManagerInstance = this;
		}
	}

	void playMusicOnTVAndGamePad()
	{
		WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.GamePad);
		WiiUAudio.EnableOutputForAudioSource(this.audio, WiiUAudioOutputDevice.TV);
	}

	int GetSongIndex()
	{
		int tempIndex;
		do 
		{
			tempIndex = Random.Range (0, musicFiles.Length - 1);
		} 
		while(lastSongPlayed == tempIndex);

		lastSongPlayed = tempIndex;
		return tempIndex;
	}
}
