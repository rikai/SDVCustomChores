﻿using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class RepairTheFences : BaseCustomChore
    {
        private IEnumerable<Fence> _fences;
        private readonly bool _enableFarm;
        private readonly bool _enableBuildings;
        private readonly bool _enableOutdoors;

        public RepairTheFences(string choreName, IDictionary<string, string> config, IEnumerable<Translation> dialogue) : base(choreName, config, dialogue)
        {
            Config.TryGetValue("EnableFarm", out var enableFarm);
            Config.TryGetValue("EnableBuildings", out var enableBuildings);
            Config.TryGetValue("EnableOutdoors", out var enableOutdoors);

            _enableFarm = string.IsNullOrWhiteSpace(enableFarm) || Convert.ToBoolean(enableFarm);
            _enableBuildings = string.IsNullOrWhiteSpace(enableBuildings) || Convert.ToBoolean(enableBuildings);
            _enableOutdoors = string.IsNullOrWhiteSpace(enableOutdoors) || Convert.ToBoolean(enableOutdoors);
        }

        public override bool CanDoIt(NPC spouse)
        {
            var locations =
                from location in Game1.locations
                where (_enableFarm && location.IsFarm) ||
                      (_enableOutdoors && location.IsOutdoors)
                select location;

            if (_enableBuildings)
                locations = locations.Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value);
            
            _fences = locations.SelectMany(location => location.objects.Values).OfType<Fence>();
            
            return _fences.Any();
        }

        public override bool DoIt(NPC spouse)
        {
            foreach (var fence in _fences)
            {
                fence.repair();
            }

            return true;
        }
    }
}