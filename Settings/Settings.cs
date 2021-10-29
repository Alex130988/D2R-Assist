/**
 *   Copyright (C) 2021 okaygo
 *
 *   https://github.com/misterokaygo/D2RAssist/
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 **/

using D2RAssist.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;

// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace D2RAssist.Settings
{
    public class Rendering
    {
        private readonly Utils utils;

        public PointOfInterestRendering
            NextArea => utils.GetRenderingSettingsForPrefix("NextArea");

        public PointOfInterestRendering PreviousArea =>
            utils.GetRenderingSettingsForPrefix("PreviousArea");

        public PointOfInterestRendering Waypoint => utils.GetRenderingSettingsForPrefix("Waypoint");
        public PointOfInterestRendering Quest => utils.GetRenderingSettingsForPrefix("Quest");
        public PointOfInterestRendering Player => utils.GetRenderingSettingsForPrefix("Player");

        public PointOfInterestRendering SuperChest =>
            utils.GetRenderingSettingsForPrefix("SuperChest");

        public Rendering(Utils utils)
        {
            this.utils = utils;
        }
    }

    public class Map
    {
        public readonly Dictionary<int, Color?> MapColors = new Dictionary<int, Color?>();
        private readonly IConfiguration configuration;
        private readonly Utils utils;

        private float zoomLevel;
        private int size;

        public Map(IConfiguration configuration, Utils utils)
        {
            this.configuration = configuration;
            this.utils = utils;

            zoomLevel = Convert.ToSingle(configuration.Config["ZoomLevelDefault"]);
            size = Convert.ToInt16(configuration.Config["Size"]);
        }

        public void InitMapColors()
        {
            for (var i = -1; i < 600; i++)
            {
                LookupMapColor(i);
            }
        }

        public Color? LookupMapColor(int type)
        {
            var key = "MapColor[" + type + "]";

            if (!MapColors.ContainsKey(type))
            {
                var mapColorString = configuration.Config[key].ToString();
                if (!string.IsNullOrEmpty(mapColorString))
                {
                    MapColors[type] = utils.ParseColor(mapColorString);
                }
                else
                {
                    MapColors[type] = null;
                }
            }

            return MapColors[type];
        }

        public double Opacity => Convert.ToDouble(configuration.Config["Opacity"],
            System.Globalization.CultureInfo.InvariantCulture);

        public bool OverlayMode => Convert.ToBoolean(configuration.Config["OverlayMode"]);

        public bool AlwaysOnTop => Convert.ToBoolean(configuration.Config["AlwaysOnTop"]);

        public bool ToggleViaInGameMap =>
            Convert.ToBoolean(configuration.Config["ToggleViaInGameMap"]);

        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                if(value != size)
                {
                    size = value;
                }
            }
        }

        public MapPosition Position =>
            (MapPosition)Enum.Parse(typeof(MapPosition), configuration.Config["MapPosition"].ToString(), true);

        public int UpdateTime => Convert.ToInt16(configuration.Config["UpdateTime"]);
        public bool Rotate => Convert.ToBoolean(configuration.Config["Rotate"]);
        public char ToggleKey => Convert.ToChar(configuration.Config["ToggleKey"]);
        public char ZoomInKey => Convert.ToChar(configuration.Config["ZoomInKey"]);
        public char ZoomOutKey => Convert.ToChar(configuration.Config["ZoomOutKey"]);
        public float ZoomLevel
        {
            get
            {
                return zoomLevel;
            }
            set
            {
                if(value != zoomLevel)
                {
                    zoomLevel = value;
                }
            }
        }

        public Area[] PrefetchAreas =>
            utils.ParseCommaSeparatedAreasByName(configuration.Config["PrefetchAreas"].ToString());

        public Area[] HiddenAreas =>
            utils.ParseCommaSeparatedAreasByName(configuration.Config["HiddenAreas"].ToString());

        public bool ClearPrefetchedOnAreaChange =>
            Convert.ToBoolean(configuration.Config["ClearPrefetchedOnAreaChange"]);

    }

    public class Api
    {
        private readonly IConfiguration configuration;

        public Api(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Endpoint => configuration.Config["ApiEndpoint"].ToString();
    }
}
