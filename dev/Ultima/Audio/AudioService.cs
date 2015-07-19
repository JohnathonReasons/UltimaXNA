﻿/***************************************************************************
 *   AudioService.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.Core.Audio;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Audio
{
    public class AudioService
    {
        private readonly Dictionary<int, UOSound> m_Sounds = new Dictionary<int, UOSound>();
        private readonly Dictionary<int, UOMusic> m_Music = new Dictionary<int, UOMusic>();

        private UOMusic m_MusicCurrentlyPlaying = null;
        private XNAMP3 m_MusicCurrentlyPlayingMP3 = null;

        public void PlaySound(int soundIndex)
        {
            if (Settings.Audio.SoundOn)
            {
                UOSound sound;
                if (m_Sounds.TryGetValue(soundIndex, out sound))
                {
                    if (sound.Status == SoundState.Loaded)
                        sound.Play();
                }
                else
                {
                    sound = new UOSound();
                    m_Sounds.Add(soundIndex, sound);
                    string name;
                    byte[] data;
                    if (SoundData.TryGetSoundData(soundIndex, out data, out name))
                    {
                        sound.Name = name;
                        sound.WaveBuffer = data;
                        sound.Status = SoundState.Loaded;
                        sound.Play();
                    }
                }
            }
        }

        public void PlayMusic(int id)
        {
            if (Settings.Audio.MusicOn)
            {
                if (id < 0) // not a valid id, used to stop music.
                {
                    StopMusic();
                    Tracer.Error("Received unknown music id {0}", id);
                    return;
                }

                if (!m_Music.ContainsKey(id))
                {
                    string name;
                    bool loops;
                    if (MusicData.TryGetMusicData(id, out name, out loops))
                    {
                        m_Music.Add(id, new UOMusic(id, name, loops));
                    }
                    else
                    {
                        Tracer.Error("Received unknown music id {0}", id);
                        return;
                    }
                }

                UOMusic toPlay = m_Music[id];
                if (toPlay != m_MusicCurrentlyPlaying)
                {
                    // stop the current song
                    StopMusic();

                    try
                    {
                        m_MusicCurrentlyPlaying = toPlay;
                        m_MusicCurrentlyPlayingMP3 = new XNAMP3(toPlay.Path);
                    }
                    catch
                    {
                        Tracer.Error("Error opening mp3 file {0}", toPlay.Path);
                        return;
                    }

                    try
                    {
                        m_MusicCurrentlyPlayingMP3.Play(toPlay.DoLoop);
                    }
                    catch
                    {
                        Tracer.Error("Error playing mp3 file {0}", toPlay.Path);
                    }
                }
            }
        }

        public void StopMusic()
        {
            if (m_MusicCurrentlyPlaying != null)
            {
                m_MusicCurrentlyPlayingMP3.Stop();
                m_MusicCurrentlyPlayingMP3.Dispose();
                m_MusicCurrentlyPlayingMP3 = null;
                m_MusicCurrentlyPlaying = null;
            }
        }
    }
}
