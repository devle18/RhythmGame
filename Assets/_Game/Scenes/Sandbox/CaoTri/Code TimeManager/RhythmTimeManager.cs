using UnityEngine;
using UnityEngine.InputSystem;

public class RhythmTimeManager : MonoBehaviour
{
	// ==========================================================
	// Singleton
	// ==========================================================

	public static RhythmTimeManager Instance
	{
		get;
		private set;
	}

	// ==========================================================
	// SONG SETTINGS
	// ==========================================================

	[Header("Song Settings")]

	[SerializeField]
	private float bpm = 120f;

	[SerializeField]
	private float userOffsetMs = 0f;

	// ==========================================================
	// DEBUG
	// ==========================================================

	[Header("Debug")]

	[SerializeField]
	private bool enableDebugLog = true;

	// ==========================================================
	// DSP Timing
	// ==========================================================

	private double dspSongStartTime;

	private double pauseDSPTime;

	private bool wasPlaying;

	// ==========================================================
	// Runtime Values
	// ==========================================================

	public double SongPositionSeconds
	{
		get;
		private set;
	}

	public double SecondsPerBeat =>
		60.0 / bpm;

	public double SongPositionInBeats =>
		SongPositionSeconds /
		SecondsPerBeat;

	public double SongProgress =>
		SongPositionSeconds /
		AudioManager.Instance
			.GetMusicSource()
			.clip.length;

	public bool IsSongFinished =>
		SongProgress >= 1.0;

	// ==========================================================
	// UNITY
	// ==========================================================

	private void Awake()
	{
		// Singleton
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);

			return;
		}

		Instance = this;

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		// ==========================================================
		// AUTO START SONG
		// ==========================================================

		StartSong();
	}

	private void Update()
	{
		// ==========================================================
		// ESC = Pause / Resume
		// ==========================================================

		if (
			Keyboard.current.escapeKey
				.wasPressedThisFrame
		)
		{
			AudioSource source =
				AudioManager.Instance
					.GetMusicSource();

			if (source.isPlaying)
			{
				PauseGame();

				if (enableDebugLog)
				{
					Debug.Log("PAUSE");
				}
			}
			else
			{
				ResumeGame();

				if (enableDebugLog)
				{
					Debug.Log("RESUME");
				}
			}
		}

		// ==========================================================
		// HANDLE DSP RESYNC
		// ==========================================================

		HandlePauseResume();

		AudioSource musicSource =
			AudioManager.Instance
				.GetMusicSource();

		// Stop update if paused
		if (!musicSource.isPlaying)
			return;

		// ==========================================================
		// DSP TIME
		// ==========================================================

		SongPositionSeconds =
			(
				AudioSettings.dspTime
				- dspSongStartTime
			)
			+ (
				userOffsetMs / 1000.0
			);

		// ==========================================================
		// DEBUG
		// ==========================================================

		if (enableDebugLog)
		{
			Debug.Log(
				"Time: "
				+ SongPositionSeconds
					.ToString("F2")
				+ " | Beat: "
				+ SongPositionInBeats
					.ToString("F2")
				+ " | Progress: "
				+ (
					SongProgress * 100f
				).ToString("F0")
				+ "%"
			);
		}

		// ==========================================================
		// SONG FINISHED
		// ==========================================================

		if (IsSongFinished)
		{
			if (enableDebugLog)
			{
				Debug.Log(
					"SONG FINISHED"
				);
			}
		}
	}

	// ==========================================================
	// START SONG
	// ==========================================================

	public void StartSong()
	{
		dspSongStartTime =
			AudioSettings.dspTime;

		AudioManager.Instance.PlayMusic();

		wasPlaying = true;

		if (enableDebugLog)
		{
			Debug.Log("SONG START");
		}
	}

	// ==========================================================
	// SET BPM
	// ==========================================================

	public void SetBPM(float newBpm)
	{
		bpm = newBpm;

		if (enableDebugLog)
		{
			Debug.Log(
				"NEW BPM: " + bpm
			);
		}
	}

	// ==========================================================
	// HANDLE PAUSE / RESUME DSP SYNC
	// ==========================================================

	private void HandlePauseResume()
	{
		AudioSource music =
			AudioManager.Instance
				.GetMusicSource();

		bool isPlayingNow =
			music.isPlaying;

		// ==========================================================
		// PAUSE DETECTED
		// ==========================================================

		if (wasPlaying && !isPlayingNow)
		{
			pauseDSPTime =
				AudioSettings.dspTime;

			if (enableDebugLog)
			{
				Debug.Log(
					"DSP PAUSE DETECTED"
				);
			}
		}

		// ==========================================================
		// RESUME DETECTED
		// ==========================================================

		if (!wasPlaying && isPlayingNow)
		{
			double pausedDuration =
				AudioSettings.dspTime
				- pauseDSPTime;

			// Prevent desync
			dspSongStartTime +=
				pausedDuration;

			if (enableDebugLog)
			{
				Debug.Log(
					"DSP RESYNC COMPLETE"
				);
			}
		}

		wasPlaying = isPlayingNow;
	}

	// ==========================================================
	// PAUSE
	// ==========================================================

	public void PauseGame()
	{
		AudioManager.Instance
			.PauseMusic();

		Time.timeScale = 0f;
	}

	// ==========================================================
	// RESUME
	// ==========================================================

	public void ResumeGame()
	{
		AudioManager.Instance
			.ResumeMusic();

		Time.timeScale = 1f;
	}

	// ==========================================================
	// OFFSET
	// ==========================================================

	public void SetOffset(float offsetMs)
	{
		userOffsetMs = offsetMs;

		if (enableDebugLog)
		{
			Debug.Log(
				"OFFSET: "
				+ userOffsetMs
				+ " ms"
			);
		}
	}
}