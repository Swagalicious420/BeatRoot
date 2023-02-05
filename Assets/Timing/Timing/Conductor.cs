using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    //Song beats per minute
    //This is determined by the song you're trying to sync up to
    public float songBpm;

    //The number of seconds for each song beat
    public float secPerBeat;

    //Current song position, in seconds
    public float songPosition;

    //Current song position, in beats
    public float songPositionInBeats;

    public int songPositionInBeatsInt;
    private int lastBeat;

    //How many seconds have passed since the song started
    public float dspSongTime;

    //The offset to the first beat of the song in seconds
    public float firstBeatOffset;

    //the number of beats in each loop
    public float beatsPerLoop;

    //the total number of loops completed since the looping clip first started
    public int completedLoops = 0;

    //The current position of the song within the loop in beats.
    public float loopPositionInBeats;

    //The current relative position of the song within the loop measured between 0 and 1.
    public float loopPositionInAnalog;

    //Conductor instance
    public static Conductor instance;

    //an AudioSource attached to this GameObject that will play the music.
    public AudioSource musicSource;
    public AudioClip[] loops;

    public int currentClipIndex = 0;
    public bool playNextTrack = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //Load the AudioSource attached to the Conductor GameObject
        musicSource = GetComponent<AudioSource>();

        //Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;

        //Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        //Start the music
        musicSource.Play();
    }

    public void ResetMusic()
    {
        currentClipIndex = 0;
        playNextTrack = false;
        musicSource.clip = loops[0];
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //determine how many seconds since the song started
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);

        //determine how many beats since the song started
        songPositionInBeats = songPosition / secPerBeat;
        songPositionInBeatsInt = (int)Mathf.Floor(songPositionInBeats);

        //calculate the loop position
        if (songPositionInBeats >= (completedLoops + 1) * beatsPerLoop) completedLoops++;
        loopPositionInBeats = songPositionInBeats - completedLoops * beatsPerLoop;

        loopPositionInAnalog = loopPositionInBeats / beatsPerLoop;

        if (lastBeat != songPositionInBeatsInt)
        {
            lastBeat = songPositionInBeatsInt;

            if (playNextTrack && songPositionInBeatsInt % 4 == 0)
            {
                currentClipIndex = (currentClipIndex + 1) % loops.Length;

                musicSource.clip = loops[currentClipIndex];
                musicSource.Play();
                playNextTrack = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playNextTrack = true;
        }

    }

}
