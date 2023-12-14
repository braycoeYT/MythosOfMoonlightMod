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

namespace MythosOfMoonlight.Projectiles
{

    public class FallingStarBig : ModProjectile
    {
        /*public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }*/

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

        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = TextureAssets.Extra[ExtrasID.FallingStar].Value;
            float alpha = MathHelper.Clamp(Projectile.velocity.Length(), 0, 1f);
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(3).RotatedBy(Projectile.rotation * 3) - Main.screenPosition, null, Color.CornflowerBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(3).RotatedBy(-Projectile.rotation * 3) - Main.screenPosition, null, Color.CornflowerBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Cyan * ((Projectile.ai[2] - 0.5f) * 0.5f * alpha), Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), Projectile.ai[2], SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Cyan * 0.25f * alpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 1.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(10).RotatedBy(Projectile.rotation) - Main.screenPosition, null, Color.DarkSlateBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(10).RotatedBy(-Projectile.rotation) - Main.screenPosition, null, Color.DarkSlateBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 1f, SpriteEffects.None, 0);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }
        public override void AI()
        {
            if (Main.dayTime) Projectile.Kill();
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
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
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
            NPC.lifeMax = 1000;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
        }
        public override bool CheckDead()
        {
            Projectile projectile = Main.projectile[(int)NPC.ai[0]];
            projectile.Kill();
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit)
        {
            for (int i = 0; i < 2; i++)
                Helper.SpawnGore(NPC.Center, "MythosOfMoonlight/StarG" + Main.rand.Next(7), vel: Main.rand.NextVector2Circular(4, 4));
        }
        public override void AI()
        {
            Main.NewText("pre check");
            Projectile projectile = Main.projectile[(int)NPC.ai[0]];
            if (projectile == null || projectile.whoAmI != NPC.ai[0] || !projectile.active || projectile.type != ModContent.ProjectileType<FallingStarBig>()) { NPC.active = false; return; }

            Main.NewText("post check");
            NPC.Center = projectile.Center;
        }
    }
}

