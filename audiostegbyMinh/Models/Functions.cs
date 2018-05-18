using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace audiostegbyMinh.Models
{
    public class Functions
    {
        public static void Mp3ToWav(string mp3File, string outputFile)
        {
            using (Mp3FileReader reader = new Mp3FileReader(mp3File))
            {
                using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    WaveFileWriter.CreateWaveFile(outputFile, pcmStream);
                }
            }
        }
    }
}