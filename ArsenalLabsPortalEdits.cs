using System;
using System.Linq;
using CalamityMod;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ArsenalLabsBeta.Items.Weapons.ProtoPortalGun;
using ArsenalLabsBeta.Items.Tiles.PuzzleElements;

namespace ArsenalLabsBeta
{
    public class ArsenalLabsPortalEdits : GlobalProjectile
    {

        public override bool InstancePerEntity => true;

        public static SoundStyle portalAmbience => ArsenalLabsBeta.GetSound("Portal/PortalAmbiance").WithVolumeScale(.5f);
        public static SoundStyle[] portalCloseList => new SoundStyle[]
        {
            ArsenalLabsBeta.GetSound("Portal/PortalClose1").WithVolumeScale(.7f),
            ArsenalLabsBeta.GetSound("Portal/PortalClose2").WithVolumeScale(.7f)
        };
        public static SoundStyle[] portalEnterList => new SoundStyle[]
        {
            ArsenalLabsBeta.GetSound("Portal/PortalEnter1").WithVolumeScale(.7f),
            ArsenalLabsBeta.GetSound("Portal/PortalEnter2").WithVolumeScale(.7f)
        };
        public static SoundStyle[] portalFailList => new SoundStyle[]
        {
            ArsenalLabsBeta.GetSound("Portal/PortalFail1").WithVolumeScale(.7f),
            ArsenalLabsBeta.GetSound("Portal/PortalFail2").WithVolumeScale(.7f),
            ArsenalLabsBeta.GetSound("Portal/PortalFail3").WithVolumeScale(.7f),
        };

        public static SoundStyle[] portalOpenList => new SoundStyle[]
        {
            ArsenalLabsBeta.GetSound("Portal/PortalOpen1").WithVolumeScale(.7f),
            ArsenalLabsBeta.GetSound("Portal/PortalOpen2").WithVolumeScale(.7f),
            ArsenalLabsBeta.GetSound("Portal/PortalOpen3").WithVolumeScale(.7f),
        };

        public static SoundStyle[] portalShootList => new SoundStyle[]
        {
            ArsenalLabsBeta.GetSound("Portal/PortalGunShootBLUE").WithVolumeScale(.7f),
            ArsenalLabsBeta.GetSound("Portal/PortalGunShootRED").WithVolumeScale(.7f)
        };

        public static int[] boltTypes => new int[] { ProjectileID.PortalGunBolt, ModContent.ProjectileType<ProtoPortalShot>() };


        bool failed = false;


        public void EmanciGrid(Projectile Proj)
        {
            Tile t = Main.tile[Proj.Center.ToSafeTileCoordinates()];
            if ((Proj.type == ProjectileID.PortalGunBolt || Proj.type == ProjectileID.PortalGunGate || Proj.type == ModContent.ProjectileType<ProtoPortalShot>()) && t.TileType == ModContent.TileType<EmancipationGrid>())
            {
                Proj.Kill();
            }
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            bool ignoreTiles = projectile.type == ProjectileID.PortalGunBolt ? true : false;
            if (boltTypes.Contains(projectile.type) && ProtoPortalShot.ValidPortalPosition(projectile) == null)
            {
                failed = true;
            }

            return true;
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {

            if (boltTypes.Contains(projectile.type))
            {
                if (failed)
                {
                    SoundEngine.PlaySound(Main.rand.NextFromList(portalFailList), projectile.Center);
                }
                else
                {
                    SoundEngine.PlaySound(Main.rand.NextFromList(portalOpenList), projectile.Center);
                }

                if (activeAmbience != null)
                {

                    if (SoundEngine.TryGetActiveSound((SlotId)activeAmbience, out ActiveSound outSound))
                    {
                        outSound.Stop();
                    }

                }
            }

            if (projectile.type == ProjectileID.PortalGunGate)
            {
                SoundEngine.PlaySound(Main.rand.NextFromList(portalCloseList), projectile.Center);
            }
        }

        SlotId? activeAmbience = null;

        public override void PostAI(Projectile projectile)
        {
            if (boltTypes.Contains(projectile.type))
            {
                EmanciGrid(projectile);
            }

            if (projectile.type == ProjectileID.PortalGunGate)
            {



                if (activeAmbience == null)
                {
                    SoundStyle toPlay = portalAmbience;
                    toPlay.IsLooped = true;
                    activeAmbience = SoundEngine.PlaySound(toPlay, projectile.Center);
                }

                /*throughCooldown--;

                bool entered = false;

                int ai1 = (int)projectile.ai[1];

 
                
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile proj = Main.projectile[i];

                    if(proj.whoAmI != projectile.whoAmI && (int)proj.ai[1] == ai1+1)
                    {

                        foreach (Player player in Main.player)
                        {
                            if (proj.Distance(Main.player[0].Center) < 20 && throughCooldown < 0)
                            {
                                //SoundEngine.PlaySound(Main.rand.NextFromList(portalEnterList), player.Center);
                                flag1 = true;
                            }
                            if (projectile.Distance(Main.player[0].Center) < 20 && flag1)
                            {
                                flag1 = false;
                                throughCooldown = 20;
                                SoundEngine.PlaySound(Main.rand.NextFromList(portalEnterList), player.Center);
                            }
                        }
                            
                    }
                }*/


            }
        }


    }
}
