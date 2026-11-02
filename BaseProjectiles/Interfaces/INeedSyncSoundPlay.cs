using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Projectiles.BaseProjectiles.Interfaces
{
    // I'm not sure if it's the right way to sync soundplay with lesser data sent - Lastprismer
    // Transform the holdout projectile on the non-owner client into a simple, signal-triggered sound player, eliminating any other state machine synchronization.
    // reviews required
    public interface INeedSyncSoundPlay
    {
        public EntityNetSoundManager SoundManager { get; set; }
    }

    public class EntityNetSoundManager
    {
        private readonly BitArray soundSignals;
        private bool hasSoundsToPlay;
        private readonly SoundStyle[] soundStyles;
        private readonly int syncByteBufferLength;
        private readonly bool additionalFlag;

        public EntityNetSoundManager(params SoundStyle[] sounds)
        {
            soundStyles = sounds;
            soundSignals = new BitArray(sounds.Length);
            syncByteBufferLength = (sounds.Length - 1) / 8 + 1;
            additionalFlag = sounds.Length >= 2;
        }

        public EntityNetSoundManager(bool useAdditionalFlag, params SoundStyle[] sounds)
        {
            soundStyles = sounds;
            soundSignals = new BitArray(soundSignals.Length);
            syncByteBufferLength = (soundSignals.Length - 1) / 8 + 1;
            additionalFlag = useAdditionalFlag;
        }

        public int BufferLength => syncByteBufferLength;

        public void SendSoundSignal(BinaryWriter writer)
        {
            writer.Write(GetSignals());
            if (additionalFlag)
                writer.Write(hasSoundsToPlay);
        }

        public void ReceiveSoundSignal(BinaryReader reader)
        {
            SetSignals(reader.ReadBytes(syncByteBufferLength));
            if (additionalFlag)
                hasSoundsToPlay = reader.ReadBoolean();
        }

        public void PlaySoundActively(int soundID, Vector2 position)
        {
            SoundEngine.PlaySound(soundStyles[soundID], position);
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                soundSignals[soundID] = true;
                if (additionalFlag)
                    hasSoundsToPlay = true;
            }
        }

        public void PlaySoundPassively(Vector2 position)
        {
            if (additionalFlag && !hasSoundsToPlay)
                return;
            for (int i = 0; i < soundSignals.Length; i++)
            {
                if (soundSignals[i])
                {
                    SoundEngine.PlaySound(soundStyles[i], position);
                    soundSignals[i] = false;
                }
            }
        }

        private byte[] GetSignals()
        {
            byte[] ret = new byte[syncByteBufferLength];
            soundSignals.CopyTo(ret, 0);
            for (int i = 0; i < soundSignals.Length; i++)
            {
                soundSignals[i] = false;
            }
            return ret;
        }

        private void SetSignals(byte[] bytes)
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(bytes.Length, syncByteBufferLength);
            for (int i = 0; i < bytes.Length; i++)
            {
                bool end = false;
                for (int j = 0; j < 8; j++)
                {
                    int bitIndex = i * 8 + j;
                    if (bitIndex >= soundSignals.Length)
                    {
                        end = true;
                        break;
                    }
                    bool bitValue = (bytes[i] & (1 << j)) != 0;
                    soundSignals.Set(bitIndex, bitValue);
                }
                if (end)
                    break;
            }
        }
    }
}
