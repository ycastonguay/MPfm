// Copyright � 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using Sessions.GenericControls.Services.Objects;
using Sessions.GenericControls.Controls.Items;

namespace Sessions.GenericControls.Helpers
{
    public static class ScrubbingSpeedHelper
    {
        public static List<ScrubbingSpeed> GetScrubbingSpeeds()
        {
            return GetScrubbingSpeeds(1);
        }

        public static List<ScrubbingSpeed> GetScrubbingSpeeds(float multiplier)
        {
            var scrubbingSpeeds = new List<ScrubbingSpeed>();
            scrubbingSpeeds.Add(new ScrubbingSpeed(1, 50 * multiplier, "High-speed scrubbing"));
            scrubbingSpeeds.Add(new ScrubbingSpeed(0.5f, 100 * multiplier, "Half-speed scrubbing"));
            scrubbingSpeeds.Add(new ScrubbingSpeed(0.25f, 150 * multiplier, "Quarter-speed scrubbing"));
            scrubbingSpeeds.Add(new ScrubbingSpeed(0.1f, 200 * multiplier, "Fine scrubbing"));
            return scrubbingSpeeds;
        }

        public static ScrubbingSpeed IdentifyScrubbingSpeed(float deltaY, IEnumerable<ScrubbingSpeed> scrubbingSpeeds)
        {
            foreach (var scrubbingSpeed in scrubbingSpeeds)
            {
                if (deltaY < scrubbingSpeed.DeltaRange)
                    return scrubbingSpeed;
            }

            return scrubbingSpeeds.ElementAt(scrubbingSpeeds.Count() - 1);
        }
    }
}
