using ReLogic.Utilities;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace fearcell.Core
{
    public static class FearcellSounds
    {
        public static void UpdateLoopingSound(ref SlotId slot, SoundStyle style, float volume, float pitch = 0f, Vector2? position = null)
        {
            SoundEngine.TryGetActiveSound(slot, out var sound);

            if (volume > 0f)
            {
                if (sound == null)
                {
                    slot = SoundEngine.PlaySound(style with { Volume = volume, Pitch = pitch }, position);
                    return;
                }

                sound.Position = position;
                sound.Volume = volume;
            }
            else if (sound != null)
            {
                sound.Stop();

                slot = SlotId.Invalid;
            }
        }

        public static readonly SoundStyle UrielLine1 = new("fearcell/Sounds/Custom/UrielLine1") { Volume = 1.2f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
        public static readonly SoundStyle UrielLine2 = new("fearcell/Sounds/Custom/UrielLine2") { Volume = 1.2f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };

        public static readonly SoundStyle DialogueTick = new("fearcell/Sounds/Custom/dialoguetick") { Volume = 0.25f, PitchVariance = 1f, MaxInstances = 1000, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
        
        public static readonly SoundStyle RumbleSound = new("fearcell/Sounds/Custom/Rumble") { Volume = 1.2f, PitchVariance = 1f, MaxInstances = 1000, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };

        public static readonly SoundStyle PortalSound = new("fearcell/Sounds/Custom/PortalSound") { Volume = 1.2f, PitchVariance = 1f, MaxInstances = 1000, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };

        public static readonly SoundStyle KeyboardSound = new("fearcell/Sounds/Custom/ComputerStartup") { Volume = .5f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
        public static readonly SoundStyle VendingMachine = new("fearcell/Sounds/Custom/VendingMachine") { Volume = .8f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
        public static readonly SoundStyle Error = new("fearcell/Sounds/Custom/Error") { Volume = .8f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
    }
}
