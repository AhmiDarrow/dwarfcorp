using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DwarfCorp.Gui;
using System.Linq;

namespace DwarfCorp.Gui.Widgets
{
    public class EmployeePortrait : Widget
    {
        private Gui.Mesh SpriteMesh;
        public DwarfSprites.LayerStack Sprite;
        public AnimationPlayer AnimationPlayer;

        public override void Construct()
        {
            Root.RegisterForPostdraw(this);
            base.Construct();
        }

        public override void Layout()
        {
            base.Layout();
            SpriteMesh = Mesh.EmptyMesh();
            SpriteMesh.QuadPart()
                .Scale(Rect.Width, Rect.Height)
                .Translate(Rect.X, Rect.Y);
        }

        public override void PostDraw(GraphicsDevice device)
        {
            if (Hidden || Transparent)
                return;

            if (IsAnyParentHidden())
                return;

            if (Sprite == null)
                return;

            if (SpriteMesh == null)
                Layout();

            var texture = Sprite.GetCompositeTexture();
            if (texture != null)
            {
                var sheet = new SpriteSheet(texture, 48, 40);
                var frame = AnimationPlayer.GetCurrentAnimation().Frames[AnimationPlayer.CurrentFrame];
                SpriteMesh.EntireMeshAsPart().ResetQuadTexture().Texture(sheet.TileMatrix(frame.X, frame.Y));
                Root.DrawMesh(SpriteMesh, texture);
            }

            base.PostDraw(device);
        }
    }

}
