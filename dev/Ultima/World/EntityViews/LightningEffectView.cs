﻿using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities.Effects;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;

namespace UltimaXNA.Ultima.World.EntityViews
{
    class LightningEffectView : AEntityView
    {
        LightningEffect Effect
        {
            get
            {
                return (LightningEffect)base.Entity;
            }
        }

        int m_DisplayItemID = -1;

        public LightningEffectView(LightningEffect effect)
            : base(effect)
        {

        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map, bool roofHideFlag)
        {
            CheckDefer(map, drawPosition);

            return DrawInternal(spriteBatch, drawPosition, mouseOverList, map, roofHideFlag);
        }

        public override bool DrawInternal(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOverList, Map map, bool roofHideFlag)
        {
            int displayItemdID = 0x4e20 + Effect.FramesActive;
            if (displayItemdID > 0x4e29)
                return false;

            if (displayItemdID != m_DisplayItemID)
            {
                m_DisplayItemID = displayItemdID;

                IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                DrawTexture = provider.GetUITexture(displayItemdID);

                Point offset = s_Offsets[m_DisplayItemID - 20000];
                DrawArea = new Rectangle(offset.X, DrawTexture.Height - 33 + (Entity.Z * 4) + offset.Y, DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickNothing;
                DrawFlip = false;
            }

            // Update hue vector.
            HueVector = Utility.GetHueVector(Entity.Hue);

            return base.Draw(spriteBatch, drawPosition, mouseOverList, map, roofHideFlag);
        }

        static Point[] s_Offsets = new Point[10]
            {
                new Point(48, 0),
                new Point(68, 0),
                new Point(92, 0),
                new Point(72, 0),
                new Point(48, 0),
                new Point(56, 0),
                new Point(76, 0),
                new Point(76, 0),
                new Point(92, 0),
                new Point(80, 0)
            };
    }
}
