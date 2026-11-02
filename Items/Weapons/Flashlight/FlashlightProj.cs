using ArsenalLabsBeta.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using ArsenalLabsBeta.WorldGeneration.Schematics.ArsenalFactory;

namespace ArsenalLabsBeta.Items.Flashlight;

public class FlashlightProj : FlashlightHoldOutProjectile
{
    private Texture2D glow;


    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.hide = true;
        Projectile.manualDirectionChange = true;
        Projectile.timeLeft = 20;
        Projectile.scale = 1.05f;

        Projectile.netImportant = true;

        glow = (Texture2D)ModContent.Request<Texture2D>(this.GetPath() + "_Glow");
    }

    private bool firstTime = true;

    private bool right = false;

    private bool left = false;

    private bool released = false;
    private bool releasedR = false;

    private int dClickcLeeway = 0;
    private int dClickrLeeewy = 0;

    private FlashlightConeFriendly light;

    public override void AI()
    {
        //Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RookOfVengeanceMFireballBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        var Player = Main.player[Projectile.owner];
        var channeling = Player.channel;
        var rotatedRelativePoint = Player.RotatedRelativePoint(Player.MountedCenter);


        if (Main.myPlayer == Projectile.owner)
        {
            if (!(Player.HeldItem.ModItem is WulfrumFlashlight) || Player.dead)
                Projectile.Kill();
            UpdateHeldProjDoVelocity(rotatedRelativePoint);
            //Main.NewText(Projectile.Center-Player.Center);

            if (!firstTime && released)
            {
                if (dClickcLeeway > 0)
                {
                    light.CycleColor();
                    var toPlay = ArsenalLabsBeta.GetSound("Item/Flashlight/FlashlightLensSwap").WithVolumeScale(0.5f);
                    toPlay.MaxInstances = 0;
                    light.Projectile.netUpdate = true;

                    SoundEngine.PlaySound(toPlay, Projectile.Center);
                }
                else
                {
                    dClickcLeeway = 30;
                }

                released = false;
            }

            if (releasedR)
            {
                if (dClickrLeeewy > 0)
                {
                    var toPlay = ArsenalLabsBeta.GetSound("Item/Flashlight/FlashlightOff").WithVolumeScale(0.5f);
                    toPlay.MaxInstances = 0;

                    SoundEngine.PlaySound(toPlay, Projectile.Center);
                    if (light != null)
                        light.Projectile.Kill();
                    Projectile.Kill();
                }
                else
                {
                    dClickrLeeewy = 30;
                }

                releasedR = false;
            }

            if (!Main.mouseLeft && left) released = true;

            if (!Main.mouseRight && right) releasedR = true;

            left = Main.mouseLeft;
            right = Main.mouseRight;
            dClickcLeeway--;
            dClickrLeeewy--;
            if (firstTime)
            {
                var toPlay = ArsenalLabsBeta.GetSound("Item/Flashlight/FlashlightOn").WithVolumeScale(0.5f);
                toPlay.MaxInstances = 0;
                var proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center,
                    Projectile.velocity, ModContent.ProjectileType<FlashlightConeFriendly>(), Projectile.damage,
                    Projectile.knockBack, Projectile.owner);

                SoundEngine.PlaySound(toPlay, Projectile.Center);

                light = Main.projectile[proj].ModProjectile as FlashlightConeFriendly;

                firstTime = false;
            }
        }

        Projectile.timeLeft = 10;
        Projectile.hide = false;
        UpdateHeldProj(rotatedRelativePoint, 10f, false);
    }

    public override void PostDraw(Color lightColor)
    {
        //SurfaceLab.DebugDrawBounds();


        var glowCol = light == null ? Color.White : light.drawColor;

        int? frames = Main.projFrames[Projectile.type] > 1 ? Main.projFrames[Projectile.type] : null;
        DrawHoldOutProjectile(glowCol, frames, glow);
    }

    public override void OnKill(int timeLeft)
    {
        Main.player[Projectile.owner].ArsenalLabs().hasFlashlight = false;
    }

    public override bool? CanCutTiles()
    {
        return false;
    }
}
