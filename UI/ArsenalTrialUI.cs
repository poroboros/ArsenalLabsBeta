using CalamityMod.UI;
using ArsenalLabsBeta.Events.ArsenalTrial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.UI;

public class ArsenalTrialUI : InvasionProgressUI
{
    public override bool IsActive => ArsenalFactoryCombatTrial.TrialActive && Main.LocalPlayer.ArsenalLabs().ZoneArsenal;
    public override float CompletionRatio => 1f - ArsenalFactoryCombatTrial.ArsenalCompletionRatio;
    public override string InvasionName => "Arsenal Trial Wave: " + ArsenalFactoryCombatTrial.WaveIndex;
    public override Color InvasionBarColor => Color.LightCoral;
    public override Texture2D IconTexture =>
        (Texture2D)ModContent.Request<Texture2D>("ArsenalLabsBeta/Assets/UI/ArsenalIcon");
}
