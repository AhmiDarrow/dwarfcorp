using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DwarfCorp
{
    public class GetAnySingleMatchingResourceAct : CompoundCreatureAct
    {
        public List<Resource.ResourceTags> Tags;
        public String BlackboardEntry = "ResourcesStashed";

        public GetAnySingleMatchingResourceAct()
        {

        }

        public GetAnySingleMatchingResourceAct(CreatureAI agent, List<Resource.ResourceTags> Tags) :
            base(agent)
        {
            Name = "Get Resources";
            this.Tags = Tags;
        }

        public IEnumerable<Status> AlwaysTrue()
        {
            yield return Status.Success;
        }

        bool HasResources(CreatureAI agent, KeyValuePair<Zone, ResourceAmount> resource)
        {
            return resource.Key.Resources.HasResources(resource.Value);
        }

        public override void Initialize()
        {
            var hasAllResources = false;

            foreach (var tag in Tags)
                if (Creature.Inventory.HasResource(new Quantitiy<Resource.ResourceTags>(tag, 1)))
                    hasAllResources = true;

            if (!hasAllResources)
            {
                var location = Creature.World.GetFirstStockpileContainingResourceWithMatchingTag(Tags);
                if (!location.HasValue)
                {
                    Tree = null;
                    return;
                }
                else
                {
                    Tree = new Sequence(
                        new Domain(() => HasResources(Agent, location.Value),
                            new GoToZoneAct(Agent, location.Value.Key)),
                        new Condition(() => HasResources(Agent, location.Value)),
                        new StashResourcesAct(Agent, location.Value.Key, location.Value.Value),
                        new SetBlackboardData<List<ResourceAmount>>(Agent, BlackboardEntry, new List<ResourceAmount> { location.Value.Value }))
                        | (new Wrap(Agent.Creature.RestockAll) & false);
                }
            }
            else
            {
                // In this case the dwarf already has all the resources. We have to find the resources from the inventory.
                var resource = Tags.Select(t =>
                {
                    var matches = Creature.Inventory.GetResources(new Quantitiy<Resource.ResourceTags>(t));
                    if (matches.Count > 0)
                        return matches[0];
                    return null;
                }).FirstOrDefault(r => r != null);

                if (resource == null)
                {
                    Tree = null;
                    return;
                }
                else
                {
                    Tree = new SetBlackboardData<List<ResourceAmount>>(Agent, BlackboardEntry, new List<ResourceAmount> { resource });
                }
            }
          
            base.Initialize();
        }
    }

}