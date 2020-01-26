﻿using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class FeedTheAnimals : BaseCustomChore
    {
        private IEnumerable<AnimalHouse> _animalHouses;
        private readonly bool _enableBarns;
        private readonly bool _enableCoops;

        public FeedTheAnimals(string choreName, IDictionary<string, string> config, IList<Translation> dialogue) : base(choreName, config, dialogue)
        {
            Config.TryGetValue("EnableBarns", out var enableBarns);
            Config.TryGetValue("EnableCoops", out var enableCoops);

            _enableBarns = (enableBarns == null) || Convert.ToBoolean(enableBarns);
            _enableCoops = (enableCoops == null) || Convert.ToBoolean(enableCoops);
        }

        public override bool CanDoIt(string name = null)
        {
            _animalHouses = Game1.getFarm().buildings
                .Where(building => building.daysOfConstructionLeft.Value <= 0)
                .Where(building => (_enableBarns && building is Barn) || (_enableCoops && building is Coop))
                .Select(building => building.indoors.Value)
                .OfType<AnimalHouse>();
            
            return _animalHouses.Any();
        }

        public override bool DoIt(string name = null)
        {
            foreach (var animalHouse in _animalHouses)
            {
                animalHouse.feedAllAnimals();
            }

            return true;
        }
    }
}
