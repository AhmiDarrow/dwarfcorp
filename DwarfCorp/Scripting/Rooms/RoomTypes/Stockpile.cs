using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using DwarfCorp.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;

namespace DwarfCorp
{

    /// <summary>
    /// A stockpile is a kind of zone which contains items on top of it.
    /// </summary>
    [JsonObject(IsReference = true)]
    public class Stockpile : Room
    {
        [RoomFactory("Stockpile")]
        private static Room _factory(RoomData Data, Faction Faction, WorldManager World)
        {
            return new Stockpile(Data, Faction, World);
        }

        public Stockpile()
        {

        }

        protected Stockpile(RoomData Data, Faction Faction, WorldManager World) :
            base(Data, World, Faction)
        {
            Boxes = new List<Body>();
            Faction.Stockpiles.Add(this);
            ReplacementType = VoxelLibrary.GetVoxelType("Stockpile");
            this.Faction = Faction;
            BlacklistResources = new List<Resource.ResourceTags>()
            {
                Resource.ResourceTags.Corpse,
                Resource.ResourceTags.Money
            };
        }

        private static uint maxID = 0;
        public List<Body> Boxes { get; set; }
        public string BoxType = "Crate";
        public Vector3 BoxOffset = Vector3.Zero;

        public override string GetDescriptionString()
        {
            return ID;
        }

        // If this is empty, all resources are allowed if and only if whitelist is empty. Otherwise,
        // all but these resources are allowed.
        public List<Resource.ResourceTags> BlacklistResources = new List<Resource.ResourceTags>();
        // If this is empty, all resources are allowed if and only if blacklist is empty. Otherwise,
        // only these resources are allowed.
        public List<Resource.ResourceTags> WhitelistResources = new List<Resource.ResourceTags>(); 

        public static uint NextID()
        {
            maxID++;
            return maxID;
        }

        public bool IsAllowed(String type)
        {
            Resource resource = ResourceLibrary.GetResourceByName(type);
            if (WhitelistResources.Count == 0)
            {
                if (BlacklistResources.Count == 0)
                {
                    return true;
                }

                return !BlacklistResources.Any(tag => resource.Tags.Any(otherTag => otherTag == tag));
            }

            if (BlacklistResources.Count != 0) return true;
            return WhitelistResources.Count == 0 || WhitelistResources.Any(tag => resource.Tags.Any(otherTag => otherTag == tag));
        }

        public void KillBox(Body component)
        {
            ZoneBodies.Remove(component);
            EaseMotion deathMotion = new EaseMotion(0.8f, component.LocalTransform, component.LocalTransform.Translation + new Vector3(0, -1, 0));
            component.AnimationQueue.Add(deathMotion);
            deathMotion.OnComplete += component.Die;
            SoundManager.PlaySound(ContentPaths.Audio.whoosh, component.LocalTransform.Translation);
            Faction.World.ParticleManager.Trigger("puff", component.LocalTransform.Translation + new Vector3(0.5f, 0.5f, 0.5f), Color.White, 90);
        }

        public void CreateBox(Vector3 pos)
        {
            WorldManager.DoLazy(() =>
            {
                Vector3 startPos = pos + new Vector3(0.0f, -0.1f, 0.0f) + BoxOffset;
                Vector3 endPos = pos + new Vector3(0.0f, 0.9f, 0.0f) + BoxOffset;

                Body crate = EntityFactory.CreateEntity<Body>(BoxType, startPos);
                crate.AnimationQueue.Add(new EaseMotion(0.8f, crate.LocalTransform, endPos));
                Boxes.Add(crate);
                AddBody(crate, false);
                SoundManager.PlaySound(ContentPaths.Audio.whoosh, startPos);
                if (Faction != null)
                    Faction.World.ParticleManager.Trigger("puff", pos + new Vector3(0.5f, 1.5f, 0.5f), Color.White, 90);
            });
        }

        public void HandleBoxes()
        {
            if (Voxels == null || Boxes == null)
            {
                return;
            }

            bool anyDead = Boxes.Any(b => b.IsDead);
            if (anyDead)
            {
                ZoneBodies.RemoveAll(z => z.IsDead);
                Boxes.RemoveAll(c => c.IsDead);

                for (int i = 0; i < Boxes.Count; i++)
                {
                    Boxes[i].LocalPosition = new Vector3(0.5f, 1.5f, 0.5f) + Voxels[i].WorldPosition + VertexNoise.GetNoiseVectorFromRepeatingTexture(Voxels[i].WorldPosition);
                }
            }

            if (Voxels.Count == 0)
            {
                foreach(Body component in Boxes)
                {
                    KillBox(component);
                }
                Boxes.Clear();
            }

            int numBoxes = Math.Min(Math.Max(Resources.CurrentResourceCount / ResourcesPerVoxel, 1), Voxels.Count);

            if (Resources.CurrentResourceCount == 0)
                numBoxes = 0;

            if (Boxes.Count > numBoxes)
            {
                for (int i = Boxes.Count - 1; i >= numBoxes; i--)
                {
                    KillBox(Boxes[i]);
                    Boxes.RemoveAt(i);
                }
            }
            else if (Boxes.Count < numBoxes)
            {
                for (int i = Boxes.Count; i < numBoxes; i++)
                {
                    CreateBox(Voxels[i].WorldPosition + VertexNoise.GetNoiseVectorFromRepeatingTexture(Voxels[i].WorldPosition));
                }
            }
        }
        
        public override bool AddItem(Body component)
        {
            if (component.Tags.Count == 0)
            {
                return false;
            }

            var resourceType = component.Tags[0];
            if (!IsAllowed(resourceType))
            {
                return false;
            }

            bool worked =  base.AddItem(component);
            HandleBoxes();

            if (Boxes.Count > 0)
            {
                TossMotion toss = new TossMotion(1.0f, 2.5f, component.LocalTransform,
                    Boxes[Boxes.Count - 1].LocalTransform.Translation + new Vector3(0.5f, 0.5f, 0.5f));
                component.UpdateRate = 1;
                component.GetRoot().GetComponent<Physics>().CollideMode = Physics.CollisionMode.None;
                component.AnimationQueue.Add(toss);
                toss.OnComplete += component.Die;
            }
            else
            {
                component.Die();
            }
            Faction.RecomputeCachedResourceState();
            return worked;
        }

        public override void Destroy()
        {
            var box = GetBoundingBox();
            box.Min += Vector3.Up;
            box.Max += Vector3.Up;
            foreach(var resource in EntityFactory.CreateResourcePiles(Resources.Resources.Values, box))
            {

            }

            if (Faction != null)
            {
                Faction.Stockpiles.Remove(this);
            }
            base.Destroy();
        }

        public override void RecalculateMaxResources()
        {

            HandleBoxes();
            base.RecalculateMaxResources();
        }        

    }
}