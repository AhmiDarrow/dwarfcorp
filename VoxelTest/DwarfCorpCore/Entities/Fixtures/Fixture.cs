﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DwarfCorp.GameStates;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace DwarfCorp
{
    [JsonObject(IsReference = true)]
    public class Fixture : Body
    {
        public Sprite Sprite { get; set; }

        public Fixture()
        {
            
        }

        public Fixture(Vector3 position, SpriteSheet asset, Point frame, GameComponent parent) :
            base("Fixture", parent, Matrix.CreateTranslation(position), Vector3.One, Vector3.Zero, false)
        {
            Sprite = new Sprite(Manager, "Sprite", this, Matrix.Identity, TextureManager.GetTexture(asset.AssetName), false);
            Sprite.AddAnimation(new Animation(asset.GenerateFrame(frame)));
            AddToOctree = false;
            CollisionType = CollisionManager.CollisionType.Static;
        }
    }
}
