﻿using MythosOfMoonlight.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MythosOfMoonlight.Dusts
{
    internal class StretchyGlow : ModDust
    {
        public override string Texture => "MythosOFMoonlight/Assets/Textures/Extra/GlowSuperSmall";

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 30, 30);
            dust.active = true;
            dust.noGravity = true;
            //squish it if you havent 
            dust.customData ??= new Vector2(Main.rand.NextFloat(0.9f, 1.1f), Main.rand.NextFloat(0.9f, 1.1f));
        }

        public override bool Update(Dust dust)
        {
            dust.rotation = dust.velocity.ToRotation();

            dust.scale = Lerp(dust.scale, 0f, 0.1f);
            dust.alpha = (int)Lerp(dust.alpha, 255, 0.056f);

            dust.velocity = Vector2.Lerp(dust.velocity, Vector2.Zero, 0.05f);

            dust.position += dust.velocity;

            if (dust.scale <= 0 || dust.alpha >= 255 || dust.velocity == Vector2.Zero)
            {
                dust.active = false;
                return false;
            }

            return false;
        }


        public override bool PreDraw(Dust dust)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;

            Main.spriteBatch.Reload(BlendState.Additive);

            VFXManager.DrawCache.Add(() =>
            {
                Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, dust.frame, dust.color, dust.rotation, tex.Size() / 2f, dust.scale * (Vector2)dust.customData * 1.05f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, dust.frame, Color.Lerp(dust.color, Color.White, 0.3f), dust.rotation, tex.Size() / 2f, dust.scale * (Vector2)dust.customData * 0.35f, SpriteEffects.None, 0f);
            });

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, dust.frame, dust.color, dust.rotation, tex.Size() / 2f, dust.scale * (Vector2)dust.customData, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, dust.frame, Color.Lerp(dust.color, Color.White, 0.3f), dust.rotation, tex.Size() / 2f, dust.scale * (Vector2)dust.customData * 0.65f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
