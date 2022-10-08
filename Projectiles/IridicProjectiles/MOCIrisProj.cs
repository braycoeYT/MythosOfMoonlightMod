﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using MythosOfMoonlight.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;

namespace MythosOfMoonlight.Projectiles.IridicProjectiles
{
    public class MOCIrisProj : ModProjectile
    {
        public override string Texture => "MythosOfMoonlight/Textures/Extra/blank";
        public float ExistingTime
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public float DustTimer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("MOC-Iris");
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 30;
            Projectile.netUpdate = true;
            Projectile.netUpdate2 = true;
            Projectile.netImportant = true;
            Projectile.ownerHitCheck = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            ExistingTime = 0;
        }
        public override void AI()
        {
            ExistingTime++;
            DustTimer++;
            Lighting.AddLight(Projectile.Center, .5f, .5f, .5f);
            foreach (Player player in Main.player)
            {
                if (player == Main.player[Projectile.owner])
                {
                    if (player == Main.LocalPlayer)
                    {
                        Projectile.timeLeft++;
                        player.direction = Main.MouseWorld.X >= player.Center.X ? 1 : -1;
                        player.heldProj = Projectile.whoAmI;
                        Projectile.Center = player.Center + Utils.SafeNormalize(Main.MouseWorld - player.Center, Vector2.UnitX);
                        Projectile.rotation = (Main.MouseWorld - player.Center).ToRotation();
                        if (ExistingTime > 215) player.channel = false;
                        if (!player.channel) Projectile.Kill();
                    }
                }
            }
            for (int i = 0; i <= 20; i += 5)
            {
                if (ExistingTime == 180 + i)
                {
                    Vector2 shoot = Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-.1f, .1f)) * 18f;
                    SoundEngine.PlaySound(SoundID.Item68, Projectile.Center + shoot * 2f);
                    Projectile star = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + shoot * 2f, shoot, ModContent.ProjectileType<IrisStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    star.DamageType = DamageClass.Magic;
                }
            }
            if (Main.netMode != NetmodeID.Server)
            {
                if (ExistingTime < 180)
                {
                    int DustCooldown = Math.Max((int)((190 - ExistingTime) / 10f), 2);
                    if (DustTimer >= DustCooldown)
                    {

                        DustTimer = 0;
                        for (int i = 1; i <= 3; i++)
                        {
                            Vector2 Center = Projectile.Center + Projectile.rotation.ToRotationVector2() * 36f;
                            Vector2 randPos = Main.rand.NextVector2CircularEdge(24, 24);
                            Dust dust = Dust.NewDustDirect(Center + randPos, 1, 1, ModContent.DustType<PurpurineDust>());
                            dust.noGravity = true;
                            dust.velocity = -randPos / 20f;
                        }
                    }
                }
            }
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("MythosOfMoonlight/Items/IridicSet/MOCIris").Value;
            Vector2 ori = new(38, 8);
            float rot = Projectile.rotation;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color color = Color.White;
            Main.EntitySpriteDraw(tex, pos, null, color, rot, ori, -1, Math.Abs(rot) >= MathHelper.PiOver2 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            return true;
        }
    }
}