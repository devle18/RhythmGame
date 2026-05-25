using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	// ==========================================================
	// Singleton
	// ==========================================================

	public static AudioManager Instance
	{
		get;
		private set;
	}

	// ==========================================================
	// SERIALIZABLE CLASSES
	// ==========================================================

	[System.Serializable]
	private class MusicSettings
	{
		[Header("Music Settings")]

		public AudioSource musicSource;
	}

	[System.Serializable]
	private class SFXSettings
	{
		[Header("SFX Settings")]

		public AudioSource sfxPrefab;

		public int poolSize = 10;
	}

	[System.Serializable]
	private class VolumeSettings
	{
		[Header("Volume Settings")]

		[Range(0f, 1f)]
		public float musicVolume = 1f;

		[Range(0f, 1f)]
		public float sfxVolume = 1f;
	}

	[System.Serializable]
	private class DebugSettings
	{
		[Header("Debug Settings")]

		public bool enableDebugLog = true;
	}

	// ==========================================================
	// INSPECTOR
	// ==========================================================

	[SerializeField]
	private MusicSettings musicSettings =
		new MusicSettings();

	[SerializeField]
	private SFXSettings sfxSettings =
		new SFXSettings();

	[SerializeField]
	private VolumeSettings volumeSettings =
		new VolumeSettings();

	[SerializeField]
	private DebugSettings debugSettings =
		new DebugSettings();

	// ==========================================================
	// Runtime
	// ==========================================================

	private Queue<AudioSource> sfxPool =
		new Queue<AudioSource>();

	// ==========================================================
	// UNITY
	// ==========================================================

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);

			return;
		}

		Instance = this;

		DontDestroyOnLoad(gameObject);

		CreatePool();
	}

	// ==========================================================
	// CREATE SFX POOL
	// ==========================================================

	private void CreatePool()
	{
		for (
			int i = 0;
			i < sfxSettings.poolSize;
			i++
		)
		{
			AudioSource source =
				Instantiate(
					sfxSettings.sfxPrefab,
					transform
				);

			source.gameObject
				.SetActive(false);

			sfxPool.Enqueue(source);
		}

		if (
			debugSettings
				.enableDebugLog
		)
		{
			Debug.Log(
				"SFX POOL CREATED"
			);
		}
	}

	// ==========================================================
	// PLAY MUSIC
	// ==========================================================

	public void PlayMusic()
	{
		if (
			musicSettings.musicSource
				.clip == null
		)
		{
			Debug.LogError(
				"No Music Clip"
			);

			return;
		}

		musicSettings.musicSource
			.volume =
			volumeSettings.musicVolume;

		musicSettings.musicSource
			.Play();

		if (
			debugSettings
				.enableDebugLog
		)
		{
			Debug.Log(
				"MUSIC PLAY"
			);
		}
	}

	// ==========================================================
	// STOP MUSIC
	// ==========================================================

	public void StopMusic()
	{
		musicSettings.musicSource
			.Stop();

		if (
			debugSettings
				.enableDebugLog
		)
		{
			Debug.Log(
				"MUSIC STOP"
			);
		}
	}

	// ==========================================================
	// PAUSE MUSIC
	// ==========================================================

	public void PauseMusic()
	{
		musicSettings.musicSource
			.Pause();

		if (
			debugSettings
				.enableDebugLog
		)
		{
			Debug.Log(
				"MUSIC PAUSE"
			);
		}
	}

	// ==========================================================
	// RESUME MUSIC
	// ==========================================================

	public void ResumeMusic()
	{
		musicSettings.musicSource
			.UnPause();

		if (
			debugSettings
				.enableDebugLog
		)
		{
			Debug.Log(
				"MUSIC RESUME"
			);
		}
	}

	// ==========================================================
	// PLAY SFX
	// ==========================================================

	public void PlaySFX(
		AudioClip clip
	)
	{
		AudioSource source =
			GetPooledSource();

		source.clip = clip;

		source.volume =
			volumeSettings.sfxVolume;

		source.gameObject
			.SetActive(true);

		source.Play();

		StartCoroutine(
			ReturnToPool(
				source,
				clip.length
			)
		);

		if (
			debugSettings
				.enableDebugLog
		)
		{
			Debug.Log(
				"SFX PLAY: "
				+ clip.name
			);
		}
	}

	// ==========================================================
	// GET POOLED SOURCE
	// ==========================================================

	private AudioSource GetPooledSource()
	{
		AudioSource source =
			sfxPool.Dequeue();

		sfxPool.Enqueue(source);

		return source;
	}

	// ==========================================================
	// RETURN TO POOL
	// ==========================================================

	private IEnumerator ReturnToPool(
		AudioSource source,
		float delay
	)
	{
		yield return
			new WaitForSecondsRealtime(
				delay
			);

		source.Stop();

		source.gameObject
			.SetActive(false);
	}

	// ==========================================================
	// GET MUSIC SOURCE
	// ==========================================================

	public AudioSource GetMusicSource()
	{
		return
			musicSettings.musicSource;
	}

	// ==========================================================
	// VOLUME
	// ==========================================================

	public void SetMusicVolume(
		float value
	)
	{
		volumeSettings.musicVolume =
			value;

		musicSettings.musicSource
			.volume =
			volumeSettings.musicVolume;
	}

	public void SetSFXVolume(
		float value
	)
	{
		volumeSettings.sfxVolume =
			value;
	}
}