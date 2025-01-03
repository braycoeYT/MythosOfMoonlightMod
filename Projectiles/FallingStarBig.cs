﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;
using System.Collections.Generic;
using MythosOfMoonlight.Common.Globals;
using MythosOfMoonlight.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;
using MythosOfMoonlight.Common.Systems;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Linq.Expressions;
using MythosOfMoonlight.Gores;
using MythosOfMoonlight.Items.Weapons;
using MythosOfMoonlight.Items.Accessories;
using MythosOfMoonlight.NPCs.Enemies.Overworld.Asteroid;
using MythosOfMoonlight.Common.Graphics.MoMParticles;
using MythosOfMoonlight.Common.Graphics.MoMParticles.Types;

namespace MythosOfMoonlight.Projectiles
{
    public class FallingStarBig : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 400;
            Projectile.light = 1f;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        float opacity = 0f, scaleX = 0f;
        Texture2D texLine = null;
        float LengthToEndOfBeam = 3000f;
        float scaleMod = 1f, scaleModDir = 1f;
        bool canFade;
        float orangeInterpolant = 0f;
        Vector2 startPosition => Projectile.Center;
        Vector2 endPosition => Projectile.Center - Vector2.UnitY * LengthToEndOfBeam;

        void DrawGlowyBeam()
        {
            texLine ??= Request<Texture2D>("MythosOfMoonlight/Assets/Textures/Extra/GlowSuperSmall").Value;

            float distance = startPosition.Distance(endPosition);

            for (float i = 0; i < distance; i += 5f)
            {
                float progress = i / (float)distance;

                Vector2 drawPos = Vector2.Lerp(startPosition, endPosition, progress);

                Color outer = Color.Lerp(Color.Yellow, Color.LightYellow, 0.145f);

                Projectile.SimpleDrawProjectile(texLine, drawPos - Main.screenPosition, Color.Lerp(outer, Color.Lerp(Color.Orange, Color.LightYellow, 0.4f), orangeInterpolant) * opacity * (1f - progress) * 0.5f, true, 1f * new Vector2(1.5f * scaleX * (1.4f - progress) * scaleMod, 1.5f), PiOver2);
                Projectile.SimpleDrawProjectile(texLine, drawPos - Main.screenPosition, Color.Lerp(Color.Lerp(Color.Orange, Color.LightYellow, 0.4f), Color.LightYellow, 1f - orangeInterpolant) * opacity * (1f - progress) * 0.65f, true, 1f * new Vector2(0.5f * scaleX * (1.4f - progress) * scaleMod, 1f), PiOver2);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = TextureAssets.Extra[ExtrasID.FallingStar].Value;
            if (Projectile.ai[0] == 0)
            {
                Main.spriteBatch.Reload(BlendState.Additive);
                float alpha = MathHelper.Clamp(Projectile.velocity.Length(), 0, 1f);
                Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(3).RotatedBy(Projectile.rotation * 3) - Main.screenPosition, null, Color.CornflowerBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 2f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(3).RotatedBy(-Projectile.rotation * 3) - Main.screenPosition, null, Color.CornflowerBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 2f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Cyan * ((Projectile.ai[2] - 0.5f) * 0.5f * alpha), Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), Projectile.ai[2], SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Cyan * 0.25f * alpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 1.5f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(10).RotatedBy(Projectile.rotation) - Main.screenPosition, null, Color.DarkSlateBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 1f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(10).RotatedBy(-Projectile.rotation) - Main.screenPosition, null, Color.DarkSlateBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 1f, SpriteEffects.None, 0);
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }

            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D tex2 = TextureAssets.Projectile[Type].Value;

            Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White * ((MathF.Sin(Main.GlobalTimeWrappedHourly * 1.5f) + 1) / 2), Projectile.rotation, tex2.Size() / 2, 1f, SpriteEffects.None, 0);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Reload(BlendState.Additive);

            if (Projectile.velocity == Vector2.Zero && opacity > 0f) //opacity check because it still draws allat when faded, which is an unneeded waste of performance
                DrawGlowyBeam();

            for (int i = 0; i < 4; i++)
            {
                float angle = Helper.CircleDividedEqually(i, 4) + Main.GlobalTimeWrappedHourly * 3;
                Vector2 pos = Projectile.Center + new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 1.5f) * 6).RotatedBy(angle);
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.CornflowerBlue * 0.6f, Projectile.rotation, tex.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            for (int i = 0; i < 4; i++)
            {
                float angle = Helper.CircleDividedEqually(i, 4) - Main.GlobalTimeWrappedHourly * 3;
                Vector2 pos = Projectile.Center - new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 1.5f) * 6).RotatedBy(angle);
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.CornflowerBlue * 0.6f, Projectile.rotation, tex.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return true;
        }

        public override void AI()
        {
            if (Projectile.ai[1]++ % 40 == 0 && Projectile.ai[0] == 0)
                SoundEngine.PlaySound(SoundID.Item9.WithPitchOffset(Main.rand.NextFloat(-0.2f, -0.15f)), Projectile.Center);

            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 20000;

            if (Main.dayTime)
            {
                opacity = Lerp(opacity, 0f, 0.1f);
                scaleX = Lerp(scaleX, 0f, 0.071f);

                if (scaleX.CloseTo(0f, 0.015f))
                    Projectile.Kill();
            }

            else
            {
                if (Projectile.velocity == Vector2.Zero)
                {
                    if (canFade)
                    {
                        opacity = Lerp(opacity, 1f, 0.031f);

                        scaleX = Lerp(scaleX, 1f, 0.1f);

                        if (scaleX >= 0.98f)
                            scaleMod += 0.003f * scaleModDir;

                        if (scaleMod >= 1.1f || scaleMod <= 0.8f)
                            scaleModDir *= -1f;
                    }

                    if (Main.netMode != NetmodeID.Server)
                    {
                        foreach (Player player in Main.ActivePlayers)
                        {
                            if (player.active && !player.ghost && !player.dead)
                            {
                                if (player.Center.Distance(Projectile.Center) < 500f)
                                {
                                    opacity = Lerp(opacity, 0f, 0.018f);
                                    scaleX = Lerp(scaleX, 0f, 0.012f);
                                    orangeInterpolant = Lerp(orangeInterpolant, 1f, 0.2f);
                                    canFade = false;
                                }

                                else
                                {
                                    orangeInterpolant = Lerp(orangeInterpolant, 0f, 0.13f);
                                    canFade = true;
                                }
                            }
                        }
                    }

                    if (Main.rand.NextBool(3) && canFade)
                    {
                        Vector2 pos = Projectile.Center + new Vector2(Main.rand.NextFloat(-25f, 25f), Main.rand.NextFloat(-145f, 0f));
                        Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(1.5f, 4f) + Main.rand.NextVector2Circular(2f, 2f);

                        ParticleHandler.SpawnParticle(
                        new GlowyBall(pos, vel, Color.LightYellow, Color.Yellow, Main.rand.Next(45, 80), opacity * 0.45f, 0f, Main.rand.NextFloat(0.23f, 1.5f), Main.rand.NextFloat(0.3f, 1f), Vector2.One, Main.rand.NextFloat(0.97f, 1.003f), 0.00025f)
                        {
                            DrawWithBloom = true,
                            BloomColor = Color.Yellow,

                        });
                    }
                }
            }

            Projectile.ai[2] = MathHelper.Clamp(Projectile.ai[2] + 0.05f, 1f, 1.5f);
           
            if (Projectile.ai[2] > 1.49f)
                Projectile.ai[2] = 1f;

            Projectile.timeLeft = 10;
            Projectile.rotation += MathHelper.ToRadians(Math.Clamp(Projectile.velocity.Length(), 0, 5));
           
            if (Projectile.ai[0] == 1)
            {
                Projectile.velocity *= 0.75f;
                if (TRay.CastLength(Projectile.Center, Vector2.UnitY, 60) > 40)
                    Projectile.velocity.Y = 1f;
                else
                    Projectile.velocity.Y = 0;
            }

            if (Projectile.ai[0] == 0 && TRay.CastLength(Projectile.Center, Vector2.UnitY, 60) < 40)
            {
                Projectile.damage = 0;
                NPC.NewNPCDirect(Projectile.GetSource_FromThis(), Projectile.Center, ModContent.NPCType<FallingStar>(), ai0: Projectile.whoAmI);
                CameraSystem.ScreenShakeAmount = 6.5f;
                Projectile.velocity *= 0.35f;
                SoundEngine.PlaySound(new SoundStyle("MythosOfMoonlight/Assets/Sounds/bigassstar") { PitchVariance = 0.15f }, Projectile.Center);
                for (int num905 = 0; num905 < 10; num905++)
                {
                    Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
                    int num906 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), (int)Projectile.Size.X, (int)Projectile.Size.Y, 31, 0f, 0f, 0, default(Color), 2.5f);
                    Main.dust[num906].position = Projectile.position + Vector2.UnitX.RotatedByRandom(3.1415927410125732) * Projectile.Size.X / 2f;
                    Main.dust[num906].noGravity = true;
                    Dust dust2 = Main.dust[num906];
                    dust2.velocity *= 3f;
                }
                Gore.NewGorePerfect(null, Projectile.Center, Main.rand.NextVector2Circular(10, 10), GoreID.PirateShip10);
                for (int num899 = 0; num899 < 4; num899++)
                {
                    Gore.NewGorePerfect(null, Projectile.Center, Main.rand.NextVector2Circular(10, 10), GoreID.Smoke1);
                    Gore.NewGorePerfect(null, Projectile.Center, Main.rand.NextVector2Circular(10, 10), GoreID.Smoke2);
                    Gore.NewGorePerfect(null, Projectile.Center, Main.rand.NextVector2Circular(10, 10), GoreID.Smoke3);
                    int num900 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), (int)Projectile.Size.X, (int)Projectile.Size.Y, 31, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[num900].position = Projectile.position + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * Projectile.Size.X / 2f;
                }
                Color newColor7 = Color.CornflowerBlue;
                for (int num613 = 0; num613 < 15; num613++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 0.8f);
                }
                for (float num614 = 0f; num614 < 2; num614 += 0.125f)
                {
                    Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num614 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (9f + Main.rand.NextFloat() * 4f), 150, newColor7).noGravity = true;
                }
                for (float num615 = 0f; num615 < 2f; num615 += 0.25f)
                {
                    Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num615 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (6f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
                }
                Vector2 vector52 = new Vector2(Main.screenWidth, Main.screenHeight);
                if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + vector52 / 2f, vector52 + new Vector2(400f))))
                {
                    for (int num616 = 0; num616 < 10; num616++)
                    {
                        Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * Projectile.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                    }
                }
                Projectile.tileCollide = false;
                //Projectile.Center += new Vector2(0, Projectile.height / 3);
                Projectile.ai[0] = 1;
            }
            // just some homing code 
        }

        public override void OnKill(int timeLeft)
        {
            for (int num905 = 0; num905 < 10; num905++)
            {
                int num906 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), (int)Projectile.Size.X, (int)Projectile.Size.Y, 31, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[num906].position = Projectile.position + Vector2.UnitX.RotatedByRandom(3.1415927410125732) * Projectile.Size.X / 2f;
                Main.dust[num906].noGravity = true;
                Dust dust2 = Main.dust[num906];
                dust2.velocity *= 3f;
            }
            for (int num899 = 0; num899 < 4; num899++)
            {
                int num900 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), (int)Projectile.Size.X, (int)Projectile.Size.Y, 31, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num900].position = Projectile.position + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * Projectile.Size.X / 2f;
            }
            Color newColor7 = Color.CornflowerBlue;
            for (int num613 = 0; num613 < 15; num613++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 0.8f);
            }
            for (float num614 = 0f; num614 < 2; num614 += 0.125f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num614 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (9f + Main.rand.NextFloat() * 4f), 150, newColor7).noGravity = true;
            }
            for (float num615 = 0f; num615 < 2f; num615 += 0.25f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num615 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (6f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
            }
            Vector2 vector52 = new Vector2(Main.screenWidth, Main.screenHeight);
            if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + vector52 / 2f, vector52 + new Vector2(400f))))
            {
                for (int num616 = 0; num616 < 10; num616++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * Projectile.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }
            Item.NewItem(Projectile.GetSource_Loot(), Projectile.getRect(), ItemID.FallenStar, Stack: Main.rand.Next(10, 20));
            switch (Main.rand.Next(3))
            {
                case 0:
                    Item.NewItem(Projectile.GetSource_Loot(), Projectile.getRect(), ModContent.ItemType<StarBaton>());
                    break;
                case 1:
                    Item.NewItem(Projectile.GetSource_Loot(), Projectile.getRect(), ModContent.ItemType<WardingStar>());
                    break;
                case 2:
                    Item.NewItem(Projectile.GetSource_Loot(), Projectile.getRect(), ModContent.ItemType<StarBit>());
                    break;
            }

            if (Main.rand.NextBool(8))
            {
                NPC.NewNPCDirect(null, Projectile.Center - new Vector2(0, 1000), ModContent.NPCType<AsteroidWarden>());
            }

            if (canFade)
            {
                float distance = startPosition.Distance(endPosition);

                for (float i = 0; i < distance; i += Main.rand.NextFloat(5f, 8f))
                {
                    float progress = i / (float)distance;

                    Vector2 pos = Vector2.Lerp(startPosition, endPosition, progress);

                    Vector2 vel = Main.rand.NextVector2Circular(0.2f, 0.2f);

                    ParticleHandler.SpawnParticle(
                    new GlowyBall(pos, vel, Color.LightYellow, Color.Yellow, Main.rand.Next(45, 80), 0.8f, 0f, 1.5f, 0f, Vector2.One, Main.rand.NextFloat(0.97f, 1.003f), 0.00025f)
                    {
                        DrawWithBloom = true,
                        BloomColor = Color.Yellow,

                    });
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }

    public class FallingStar : ModNPC
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 50;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.defense = 30;
            NPC.lifeMax = 200;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
        }
        public override bool CheckActive()
        {
            return false;
        }
        List<int> GoreTypes = new List<int>()
        {
            ModContent.GoreType<StarG0>(),ModContent.GoreType<StarG1>(),ModContent.GoreType<StarG2>(),ModContent.GoreType<StarG3>(),ModContent.GoreType<StarG4>(),ModContent.GoreType<StarG5>(),ModContent.GoreType<StarG6>(),
        };
        public override bool CheckDead()
        {
            Projectile projectile = Main.projectile[(int)NPC.ai[0]];
            projectile.Kill();
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, NPC.Center);
            SoundEngine.PlaySound(SoundID.NPCHit3, NPC.Center);
            for (int i = 0; i < 10; i++)
                Gore.NewGore(default, NPC.Center, Main.rand.NextVector2Circular(10, 10), Main.rand.Next(GoreTypes));
            return true;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastHurt, NPC.Center);
            if (Main.rand.NextBool(3))
                Gore.NewGore(default, NPC.Center, Main.rand.NextVector2Circular(10, 10), Main.rand.Next(GoreTypes));
            Color newColor7 = Color.CornflowerBlue;
            if (Main.rand.NextBool(3))
            {


                for (int num613 = 0; num613 < 5; num613++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 58, 4, 4, 150, default, 0.8f);
                }
                for (float num614 = 0f; num614 < 1; num614 += 0.125f)
                {
                    Dust.NewDustPerfect(NPC.Center, 278, Vector2.UnitY.RotatedBy(num614 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, newColor7).noGravity = true;
                }
                for (float num615 = 0f; num615 < 0.75f; num615 += 0.25f)
                {
                    Dust.NewDustPerfect(NPC.Center, 278, Vector2.UnitY.RotatedBy(num615 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
                }

            }
        }
        public override void AI()
        {
            Projectile projectile = Main.projectile[(int)NPC.ai[0]];
            if (projectile == null || projectile.whoAmI != NPC.ai[0] || !projectile.active || projectile.type != ModContent.ProjectileType<FallingStarBig>()) { NPC.active = false; return; }

            NPC.timeLeft = 2;
            NPC.Center = projectile.Center;
        }
    }
}

